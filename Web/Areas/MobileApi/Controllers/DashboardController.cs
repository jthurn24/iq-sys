using IQI.Intuition.Domain.Models;
using IQI.Intuition.Domain.Repositories;
using IQI.Intuition.Reporting.Graphics;
using IQI.Intuition.Web.Areas.MobileApi.Models;
using RedArrow.Framework.Mvc.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IQI.Intuition.Web.Areas.MobileApi.Controllers
{
    [AnonymousAccess]
    public class DashboardController : Controller
    {
        private IWarningRepository _WarningRepository { get; set; }
        private IInfectionRepository _InfectionRepository { get; set; }
        private IEmployeeInfectionRepository _EmployeeInfectionRepository { get; set; }
        private IIncidentRepository _IncidentRepository { get; set; }
        private IPatientCensusRepository _PatientCensusRepository { get; set; }
        private ISystemRepository _SystemRepository { get; set; }


        public DashboardController(
            IWarningRepository warningRepository,
            IInfectionRepository infectionRepository,
            IEmployeeInfectionRepository employeeInfectionRepository,
            IIncidentRepository incidentRepository,
            IPatientCensusRepository patientCensusRepository,
            ISystemRepository systemRepository,
            IResourceRepository resourceRepository)
        {
            _InfectionRepository = infectionRepository;
            _EmployeeInfectionRepository = employeeInfectionRepository;
            _WarningRepository = warningRepository;
            _IncidentRepository = incidentRepository;
            _PatientCensusRepository = patientCensusRepository;
            _SystemRepository = systemRepository;
        }

        public ActionResult Stats(string token, string facility)
        {
            var mobileToken = _SystemRepository.GetMobileToken(token);

            if (mobileToken == null) return Json(new { Success = false }, JsonRequestBehavior.AllowGet);

            var user = _SystemRepository.GetUserById(mobileToken.AccountUserId);

            var fc = user.Facilities.Where(x => x.Name == facility).FirstOrDefault();

            if (fc == null) return Json(new { Success = false }, JsonRequestBehavior.AllowGet);

            var patientInfections = _InfectionRepository.FindActiveFacility(fc);

            var counters = new List<Counter>();

            counters.Add(new Counter()
            {
                Count = patientInfections.Where(x => x.Classification == InfectionClassification.Admission || x.Classification == InfectionClassification.HealthCareAssociatedInfection || x.Classification == InfectionClassification.AdmissionHospitalDiagnosed).Count(),
                Description = "Confirmed Infections"
            });

            counters.Add(new Counter()
            {
                Count = patientInfections.Where(x => x.Classification == InfectionClassification.HealthCareAssociatedInfection).Count(),
                Description = "HAI Infections"
            });

            counters.Add(new Counter()
            {
                Count = patientInfections.Where(x => x.Classification == InfectionClassification.Admission || x.Classification == InfectionClassification.AdmissionHospitalDiagnosed).Count(),
                Description = "Admission Infections"
            });

            counters.Add(new Counter()
            {
                Count = patientInfections.Where(x => x.Classification == InfectionClassification.NoInfection).Count(),
                Description = "Suspected Infections"
            });


            /* Employee Infection Counters */

            if (user.HasPermission(Domain.Enumerations.KnownPermision.ViewAndEditEmployeeInfections))
            {
                var employeeInfections =
                _EmployeeInfectionRepository.FindForLineListing(fc, null, null, null)
                .Where(x => x.WellOn.HasValue == false && x.InfectionType != null);

                counters.Add(new Counter()
                {
                    Count = employeeInfections.Count(),
                    Description = "Employee Infections"
                }
                );
            }

            return Json(new { Success = false, Counters = counters  }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult InfectionGraph(string token, string facility)
        {
            var mobileToken = _SystemRepository.GetMobileToken(token);

            if (mobileToken == null) return Json(new { Success = false }, JsonRequestBehavior.AllowGet);

            var user = _SystemRepository.GetUserById(mobileToken.AccountUserId);

            var fc = user.Facilities.Where(x => x.Name == facility).FirstOrDefault();

            if (fc == null) return Json(new { Success = false }, JsonRequestBehavior.AllowGet);

            var chart = new ColumnChart();
            chart.MinimumYaxis = 10;
            var infections = _InfectionRepository.FindActiveFacility(fc);

            foreach (var type in infections.Select(x => x.InfectionSite.Type).Distinct())
            {
                chart.AddItem(
                    new ColumnChart.Item()
                    {
                        Category = type.ShortName,
                        Value = infections.Where(x => x.InfectionSite.Type == type
                             && (x.Classification == InfectionClassification.Admission || x.Classification == InfectionClassification.HealthCareAssociatedInfection)).Count(),
                        Color = System.Drawing.ColorTranslator.FromHtml(type.Color)
                    });
            }

            var stream = ColumnChart.GenerateImage(chart.SerializeDataForRender(), chart.SerializeOptionsForRender(), 450, 250);
            return File(stream, "image/jpeg");
        }

        public ActionResult Warnings(string token, string facility)
        {
            var mobileToken = _SystemRepository.GetMobileToken(token);

            if (mobileToken == null) return Json(new { Success = false }, JsonRequestBehavior.AllowGet);

            var user = _SystemRepository.GetUserById(mobileToken.AccountUserId);

            var fc = user.Facilities.Where(x => x.Name == facility).FirstOrDefault();

            if (fc == null) return Json(new { Success = false }, JsonRequestBehavior.AllowGet);

            var warnings = _WarningRepository.SearchFacility(fc.Id, null, null, null, x => x.TriggeredOn, true, 1, 20).PageValues;

            warnings = warnings.Where(x => x.IsHiddenBy(user.Id) == false);

            return Json(new { Success = false, Warnings = warnings.Select(x => new { Title = x.Title, Description = x.DescriptionText, ID = x.Id, On = x.TriggeredOn.ToShortDateString()  }) }, JsonRequestBehavior.AllowGet);
        }
    }
}