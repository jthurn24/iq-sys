using System;
using System.Linq;
using RedArrow.Framework.Mvc.ModelMapper.Mapping;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Extensions.Formatting;
using IQI.Intuition.Domain.Models;
using IQI.Intuition.Infrastructure.Services;
using IQI.Intuition.Domain.Repositories;
using System.Collections.Generic;
using RedArrow.Framework.Mvc.ModelMapper;
using IQI.Intuition.Web.Models.Warning;
using IQI.Intuition.Reporting.Graphics;

namespace IQI.Intuition.Web.Models.Home
{
    public class DashboardMap : ReadOnlyModelMap<Dashboard, IActionContext>
    {
        private IWarningRepository _WarningRepository { get; set; }
        private IInfectionRepository _InfectionRepository { get; set; }
        private IEmployeeInfectionRepository _EmployeeInfectionRepository { get; set; }
        private IIncidentRepository _IncidentRepository { get; set; }
        private System.Web.HttpContextBase _HttpContext { get; set; }
        private IPatientCensusRepository _PatientCensusRepository { get; set; }
        private ISystemRepository _SystemRepository { get; set; }
        private IResourceRepository _ResourceRepository { get; set; }

        private IModelMapper _ModelMapper { get; set; }

        public DashboardMap(
            IWarningRepository warningRepository,
            IInfectionRepository infectionRepository,
            IEmployeeInfectionRepository employeeInfectionRepository,
            IIncidentRepository incidentRepository,
            IModelMapper modelMapper,
            System.Web.HttpContextBase httpContext,
            IPatientCensusRepository patientCensusRepository,
            ISystemRepository systemRepository,
            IResourceRepository resourceRepository)
        {
            _InfectionRepository = infectionRepository;
            _EmployeeInfectionRepository = employeeInfectionRepository;
            _WarningRepository = warningRepository;
            _IncidentRepository = incidentRepository;
            _ModelMapper = modelMapper;
            _HttpContext = httpContext;
            _PatientCensusRepository = patientCensusRepository;
            _SystemRepository = systemRepository;
            _ResourceRepository = resourceRepository;

            AutoConfigure();

            ForProperty(model => model.RecentWarnings)
                .Read(GetWarnings);

            ForProperty(model => model.UserLastSignedIn)
                .Read(actionContext => FormatUserLastSignedIn(actionContext.CurrentUser));

            ForProperty(model => model.InfectionChart)
                .Read(GetInfectionChart);

            ForProperty(model => model.IncidentTypeChart)
                .Read(GetIncidentTypeChart);

            ForProperty(model => model.IncidentInjuryChart)
                .Read(GetIncidentInjuryChart);

            ForProperty(model => model.Resources)
                .Read(GetResources);

            ForProperty(model => model.Counters)
                .Read(GetCounters);

            ForProperty(model => model.Messages)
                .Read(GetMessages);
        }


        private string FormatUserLastSignedIn(AccountUser currentUser)
        {
            var userLastSignedInDate = (currentUser.PreviousSignInAt
                ?? (DateTime?)DateTime.Now).Value.Date;

            if (userLastSignedInDate != DateTime.Today)
            {
                return userLastSignedInDate.ToString("MMMM dd, yyyy");
            }

            return "earlier today";
        }

        private IEnumerable<WarningInfo> GetWarnings(IActionContext context)
        {
            IEnumerable<Domain.Models.Warning> warnings = new List<Domain.Models.Warning>();

            try
            {
                warnings = _WarningRepository.SearchFacility(context.CurrentFacility.Id, null, null, null, x => x.TriggeredOn, true, 1, 20).PageValues;

                warnings = warnings.Where(x => x.IsHiddenBy(context.CurrentUser.Id) == false);
            }
            catch(Exception ex)
            {

            }
            


            var results = new List<WarningInfo>();

            foreach (var warning in warnings)
            {
                results.Add(_ModelMapper.MapForReadOnly<WarningInfo>(warning));
            }

            return results;

        }

        private IEnumerable<Dashboard.Counter> GetCounters(IActionContext context)
        {
            var counters = new List<Dashboard.Counter>();

            /* Patient Infection Counters */

            var patientInfections = _InfectionRepository.FindActiveFacility(context.CurrentFacility);



            if(context.CurrentFacility.HasProduct(Domain.Enumerations.KnownProductType.InfectionTracking))
            {
                counters.Add(new Dashboard.Counter()
                {
                    Count = patientInfections.Where(x => x.Classification == InfectionClassification.Admission || x.Classification == InfectionClassification.HealthCareAssociatedInfection || x.Classification == InfectionClassification.AdmissionHospitalDiagnosed).Count(),
                     Description = "Active confirmed infections",
                    ToolTip = patientInfections.Where(x => x.Classification == InfectionClassification.Admission || x.Classification == InfectionClassification.HealthCareAssociatedInfection || x.Classification == InfectionClassification.AdmissionHospitalDiagnosed)
                    .Select(x => new InfectionToolTip(x, _HttpContext).ToString())
                    .ToJoinedString()
                }
                );

                counters.Add(new Dashboard.Counter()
                {
                    Count = patientInfections.Where(x => x.Classification == InfectionClassification.HealthCareAssociatedInfection).Count(),
                    Description = "Active <br>HAI<br> infections",
                    ToolTip = patientInfections.Where(x => x.Classification == InfectionClassification.HealthCareAssociatedInfection)
                    .Select(x => new InfectionToolTip(x, _HttpContext).ToString())
                    .ToJoinedString()
                }
                );

                counters.Add(new Dashboard.Counter()
                {
                    Count = patientInfections.Where(x => x.Classification == InfectionClassification.Admission || x.Classification == InfectionClassification.AdmissionHospitalDiagnosed).Count(),
                    Description = "Active <br>admission<br> infections",
                    ToolTip = patientInfections.Where(x => x.Classification == InfectionClassification.Admission || x.Classification == InfectionClassification.AdmissionHospitalDiagnosed)
                    .Select(x => new InfectionToolTip(x,_HttpContext).ToString() )
                    .ToJoinedString()

                }
                );

                counters.Add(new Dashboard.Counter()
                {
                    Count = patientInfections.Where(x => x.Classification == InfectionClassification.NoInfection).Count(),
                    Description = "Active <br>suspected<br> infections",
                    ToolTip = patientInfections.Where(x => x.Classification == InfectionClassification.NoInfection)
                    .Select(x => new InfectionToolTip(x, _HttpContext).ToString())
                    .ToJoinedString()
                }
                );


                /* Employee Infection Counters */

                if (context.CurrentUser.HasPermission(Domain.Enumerations.KnownPermision.ViewAndEditEmployeeInfections))
                {
                    var employeeInfections =
                    _EmployeeInfectionRepository.FindForLineListing(context.CurrentFacility, null, null, null)
                    .Where(x => x.WellOn.HasValue == false && x.InfectionType != null);

                    counters.Add(new Dashboard.Counter()
                    {
                        Count = employeeInfections.Count(),
                        Description = "Active <br>employee<br> infections",
                        ToolTip = employeeInfections
                        .Select(x => new EmployeeInfectionToolTip(x, _HttpContext).ToString())
                        .ToJoinedString()
                    }
                    );
                }

            }

            if(context.CurrentFacility.HasProduct(Domain.Enumerations.KnownProductType.IncidentTracking))
            {
                var recentIncidents = _IncidentRepository.FindForLineListing(context.CurrentFacility,null,null,null,null, DateTime.Today.AddDays(-30),null);

                counters.Add(new Dashboard.Counter()
                {
                    Count = recentIncidents.Count(),
                    Description = "Incidents<br> last 30 <br> days",
                    ToolTip = recentIncidents
                    .Select(x => new IncidentToolTip(x, _HttpContext).ToString())
                    .ToJoinedString()
                }
                );

                counters.Add(new Dashboard.Counter()
                    {
                        Count = recentIncidents.Where(x => x.IncidentTypes.Select(xx => xx.GroupName).Contains("Fall") ).Count(),
                        Description = "Falls<br> last 30 <br> days",
                        ToolTip = recentIncidents.Where(x => x.IncidentTypes.Select(xx => xx.GroupName).Contains("Fall"))
                        .Select(x => new IncidentToolTip(x, _HttpContext).ToString())
                        .ToJoinedString()
                    }
                );

                counters.Add(new Dashboard.Counter()
                    {
                        Count = recentIncidents.Where(x => x.IncidentTypes.Select(xx => xx.GroupName).Contains("Med Error")).Count(),
                        Description = "Med Errors<br> last 30 <br> days",
                        ToolTip = recentIncidents.Where(x => x.IncidentTypes.Select(xx => xx.GroupName).Contains("Med Error"))
                        .Select(x => new IncidentToolTip(x, _HttpContext).ToString())
                        .ToJoinedString()
                    }
                );
            }

            return counters;
        }



        private IEnumerable<string> GetMessages(IActionContext context)
        {
            var messages = new List<string>();
            var helper = new System.Web.Mvc.UrlHelper(_HttpContext.Request.RequestContext);

            var lastMonth = DateTime.Today.AddMonths(-1);
            var census = _PatientCensusRepository.Find(context.CurrentFacility, lastMonth.Month, lastMonth.Year).FirstOrDefault();

            if (census == null || census.PatientDays < 1)
            {
                messages.Add(string.Format("You have not yet entered patient census details for {0}. This information is require for calculation of QA metrics. <a href='{1}'>Click here</a> to enter your census value.",
                    lastMonth.ToString("MMMM"),
                    helper.Action("List","PatientCensus")));
            }

            var facility = context.CurrentFacility;
            
            if(facility.State == "WI" && facility.InfectionDefinition.Id != 3)
            {
                messages.Add(string.Format("Your facility may be interested in upgrading to our new hybrid criteria set based on both McGeers 2012 and the WI DHS guidelines for reporting acute respiratory illnesses. <a href='{0}'>Click here to learn more</a>.",
                    helper.Content("~/Content/Downloads/Wisconsin-ARI-Criteria.pdf")));
            }

            return messages;

        }

        private IEnumerable<Dashboard.Resource> GetResources(IActionContext context)
        {
            var resources = _ResourceRepository.GetRecent(10);
            var result = resources.Select(x => new Dashboard.Resource() { Added = x.CreatedOn.FormatAsShortDate(), id = x.Folder.Id, Name = x.Name, Path = x.Folder.Name });
            return result;
        }

        private ColumnChart GetInfectionChart(IActionContext context)
        {
            var chart = new ColumnChart();
            chart.MinimumYaxis = 10;
            var infections = _InfectionRepository.FindActiveFacility(context.CurrentFacility);

            foreach (var type in infections.Select(x => x.InfectionSite.Type).Distinct())
            {
                chart.AddItem(
                    new ColumnChart.Item() { 
                        Category = type.ShortName, 
                        Value= infections.Where(x => x.InfectionSite.Type == type 
                            && (x.Classification == InfectionClassification.Admission || x.Classification == InfectionClassification.HealthCareAssociatedInfection)).Count(),
                        Color = System.Drawing.ColorTranslator.FromHtml(type.Color)
                    });
            }

            return chart;
        }

        private ColumnChart GetIncidentTypeChart(IActionContext context)
        {

            var recentIncidents = _IncidentRepository.FindForLineListing(context.CurrentFacility, null, null, null, null, DateTime.Today.AddDays(-30), null);
            var groups = recentIncidents.SelectMany(xx => xx.IncidentTypes).Select(x => x.GroupName).Distinct();

            var chart = new ColumnChart();
            chart.MinimumYaxis = 10;
            int colorIndex = 0;

            foreach (var group in groups)
            {
                chart.AddItem(
                    new ColumnChart.Item()
                    {
                        Category = group,
                        Value = recentIncidents.Where(x => x.IncidentTypes.Select(xx => xx.GroupName).Contains(group) ).Count(),
                        Color = ColumnChart.GetDefaultColor(colorIndex)
                    });

                colorIndex++;
            }

            return chart;
        }

        private ColumnChart GetIncidentInjuryChart(IActionContext context)
        {

            var recentIncidents = _IncidentRepository.FindForLineListing(context.CurrentFacility, null,null, null, null, DateTime.Today.AddDays(-30), null);
            var injuries = recentIncidents.SelectMany(x => x.IncidentInjuries).Select(x => x.Name).Distinct();

            var chart = new ColumnChart();
            chart.MinimumYaxis = 10;
            int colorIndex = 0;

            foreach (var injury in injuries)
            {
                chart.AddItem(
                    new ColumnChart.Item()
                    {
                        Category = injury,
                        Value = recentIncidents.Where(x => x.IncidentInjuries.Select(xx => xx.Name).Contains(injury)).Count(),
                        Color = ColumnChart.GetDefaultColor(colorIndex)
                    });

                colorIndex++;
            }

            return chart;
        }

        private class InfectionToolTip
        {
            private InfectionVerification _Verification;
            private System.Web.HttpContextBase _HttpContext;

            public InfectionToolTip(InfectionVerification verification,
                System.Web.HttpContextBase httpContext)
            {
                _Verification = verification;
                _HttpContext = httpContext;
            }

            public override string ToString()
            {
                var helper = new System.Web.Mvc.UrlHelper(_HttpContext.Request.RequestContext);

                return string.Format("<div style=\\'padding-bottom:5px;padding-top:0px;\\'><a style=\\'color:white;text-decoration:none;\\' href=\\'{2}\\'>{0} ({1})</div>",
                    _Verification.Patient.FullName,
                    _Verification.InfectionSite.Type.Name,
                    helper.Action("View", "Infection", new { id = _Verification.Id })
                    );
            }
        }

        private class EmployeeInfectionToolTip
        {
            private Domain.Models.EmployeeInfection _EmployeeInfection;
            private System.Web.HttpContextBase _HttpContext;

            public EmployeeInfectionToolTip(Domain.Models.EmployeeInfection employeeInfection,
                System.Web.HttpContextBase httpContext)
            {
                _EmployeeInfection = employeeInfection;
                _HttpContext = httpContext;
            }

            public override string ToString()
            {
                var helper = new System.Web.Mvc.UrlHelper(_HttpContext.Request.RequestContext);

                return string.Format("<div style=\\'padding-bottom:5px;padding-top:0px;\\'><a style=\\'color:white;text-decoration:none;\\' href=\\'{2}\\'>{0} ({1})</div>",
                    _EmployeeInfection.FullName,
                    _EmployeeInfection.InfectionType.Name,
                    helper.Action("View", "EmployeeInfection", new { id = _EmployeeInfection.Id })
                    );
            }
        }

        private class IncidentToolTip
        {
            private Domain.Models.IncidentReport _Report;
            private System.Web.HttpContextBase _HttpContext;

            public IncidentToolTip(Domain.Models.IncidentReport report,
                System.Web.HttpContextBase httpContext)
            {
                _Report = report;
                _HttpContext = httpContext;
            }

            public override string ToString()
            {
                var helper = new System.Web.Mvc.UrlHelper(_HttpContext.Request.RequestContext);

                return string.Format("<div style=\\'padding-bottom:5px;padding-top:0px;\\'><a style=\\'color:white;text-decoration:none;\\' href=\\'{2}\\'>{0} ({1})</div>",
                    _Report.Patient.FullName,
                    _Report.IncidentTypes.Select(x => string.Concat(x.Name," ")).ToJoinedString(),
                    helper.Action("View", "Incident", new { id = _Report.Id })
                    );
            }
        }
    }
}
