using System;
using System.Web.Mvc;
using System.Collections.Generic;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Mvc.ModelMapper;
using RedArrow.Framework.Mvc.Security;
using IQI.Intuition.Infrastructure.Services;
using IQI.Intuition.Web.Models.Navigation;
using IQI.Intuition.Web.Attributes;

namespace IQI.Intuition.Web.Controllers
{
    [RequiresActiveAccount]
    public class NavigationController : Controller
    {
        public NavigationController(
            IActionContext actionContext,
            IAuthentication authentication,
            IModelMapper modelMapper)
        {
            ActionContext = actionContext.ThrowIfNullArgument("actionContext");
            Authentication = authentication.ThrowIfNullArgument("authentication");
            ModelMapper = modelMapper.ThrowIfNullArgument("modelMapper");
        }

        protected virtual IActionContext ActionContext { get; private set; }

        protected virtual IAuthentication Authentication { get; private set; }

        protected virtual IModelMapper ModelMapper { get; private set; }

        [AnonymousAccess]
        public ActionResult NavigationPanel()
        {
            var model = new List<NavigationItem>();

            if ((ActionContext.CurrentUser != null && ActionContext.CurrentUser.AgreementSignedOn.HasValue) 
                || (ActionContext.CurrentUser != null && ActionContext.CurrentUser.SystemUser ))
            {
                new NavigationItem(ActionContext)
                .SetText("Home")
                .SetLink(Url.Action("Index", new { controller = "Home", area = "" }))
                .Add("QI Dashboard",Url.Action("Index", new { controller = "Home", area = "" }))
                .Add("CMS Dashboard", Url.Action("Index", new { controller = "CmsMatrix", area = "" }),Domain.Enumerations.KnownProductType.CmsCompliance, Domain.Enumerations.KnownPermision.ManageCmsRecords)
                .Add("Support Center",Url.Action("Index", new { controller = "SupportCenter", area = "" }))
                .Add("Resource Library", Url.Action("Index", new { controller = "Resource", area = "" }))
                .Add("Drug Database", Url.Action("Index", new { controller = "Drug", area = "" }))
                .AddToList(model);

                new NavigationItem(ActionContext)
                .SetText("Patients")
                .SetLink("#")
                .Add("Add", Url.Action("Add", new { controller = "Patient", area = "" }))
                .Add("Search", Url.Action("List", new { controller = "Patient", area = "" }))
                .Add("Census", Url.Action("List", new { controller = "PatientCensus", area = "" }))
                .AddToList(model);

                new NavigationItem(ActionContext)
                .SetText("Tracking")
                .SetLink("#")
                .AddSeparator("Search", new UrlHelper(this.Request.RequestContext).Content("~/Content/images/nav-search.png"))
                .Add("--- Infections", Url.Action("List", new { controller = "Infection", area = "" }), Domain.Enumerations.KnownProductType.InfectionTracking)
                .Add("--- Incidents", Url.Action("List", new { controller = "Incident", area = "" }), Domain.Enumerations.KnownProductType.IncidentTracking)
                .Add("--- Complaints", Url.Action("List", new { controller = "Complaint", area = "" }), Domain.Enumerations.KnownProductType.ComplaintTracking, Domain.Enumerations.KnownPermision.ViewAndEditComplaints)
                .Add("--- Psychotropic", Url.Action("List", new { controller = "PsychoTropicAdministration", area = "" }), Domain.Enumerations.KnownProductType.PsychotropicTracking)
                .Add("--- Immunizations", Url.Action("List", new { controller = "Vaccine", area = "" }), Domain.Enumerations.KnownProductType.VaccineTracking)
                .Add("--- Wounds", Url.Action("List", new { controller = "Wound", area = "" }), Domain.Enumerations.KnownProductType.WoundTracking)
                .Add("--- Warnings/Alerts", Url.Action("List", new { controller = "Warning", area = "" }))
                .Add("--- Employee Infections", Url.Action("List", new { controller = "EmployeeInfection", area = "" }), Domain.Enumerations.KnownPermision.ViewAndEditEmployeeInfections)
                .Add("--- Catheters", Url.Action("List", new { controller = "Catheter", area = "" }), Domain.Enumerations.KnownProductType.InfectionTracking)
                .AddSeparator("Add", new UrlHelper(this.Request.RequestContext).Content("~/Content/images/nav-add.png"))
                .Add("--- Complaint", Url.Action("Add", new { controller = "Complaint", area = "" }), Domain.Enumerations.KnownProductType.ComplaintTracking)
                .Add("--- Employee Infection", Url.Action("Add", new { controller = "EmployeeInfection", area = "" }), Domain.Enumerations.KnownProductType.InfectionTracking)
                .AddToList(model);

                new NavigationItem(ActionContext)
                .SetText("Reports")
                .SetLink("#")
                .Add("Facility Reports", Url.Action("Home", new { controller = "Facility", area = "Reporting" }))
                .AddForMultiFacilityUser("Corporate Reports", Url.Action("Home", new { controller = "Account", area = "Reporting" }))
                .Add("Composite Report", Url.Action("ViewCompositeRequests", new { controller = "Export", area = "" }))
                .AddToList(model);

                new NavigationItem(ActionContext)
                .SetText("System")
                .SetLink("#")
                .RequireSubItems()
                .Add("Account Users", Url.Action("Index", new { controller = "AccountUser", area = "Configuration" }), Domain.Enumerations.KnownPermision.ManageUsers)
                .AddForSystemUser("Facility Floors", Url.Action("Index", new { controller = "Floor", area = "Configuration" }))
                .AddForSystemUser("Facility Floor Maps", Url.Action("FloorMapConfiguration", new { controller = "Configuration", area = "Reporting" }))
                .Add("Warning Rules", Url.Action("Index", new { controller = "WarningRule", area = "Configuration" }), Domain.Enumerations.KnownPermision.ViewWarningRules)
                .Add("QI Cast", Url.Action("Index", new { controller = "QICast", area = "Configuration" }), Domain.Enumerations.KnownPermision.ManageQICast)
                .AddToList(model);    

            }

            return PartialView(model);
        }

        public ActionResult FacilityReportQuickList()
        {
            var list = new Dictionary<string, List<SelectListItem>>();

            var lineListings = new List<SelectListItem>();
            list["Line Listings"] = lineListings;
            list["Floor Maps"] = new List<SelectListItem>();
            

            /* Infection product reports */

            if(ActionContext.CurrentFacility.HasProduct(Domain.Enumerations.KnownProductType.InfectionTracking))
            {
                list["Floor Maps"].Add(new SelectListItem() { Text = "Infection Floor Map History", Value = Url.Action("InfectionFloorMap", new { controller = "Facility", area = "Reporting" }) });

                list["Infection Statistics"] = new List<SelectListItem>()
                {
                    new SelectListItem() { Text="Quarterly Infections Overview" , Value = Url.Action("QuarterlyInfectionControlReport",new { controller = "Facility", area ="Reporting" })},
                    new SelectListItem() { Text="Quarterly Infections Trends" , Value = Url.Action("QuarterlyInfectionTrendReport",new { controller = "Facility", area ="Reporting" })},
                    new SelectListItem() { Text="Quarterly Infections By Type" , Value = Url.Action("QuarterlyInfectionByTypeReport",new { controller = "Facility", area ="Reporting" })},
                    new SelectListItem() { Text="Quarterly Infections By Condition" , Value = Url.Action("QuarterlyInfectionBySiteReport",new { controller = "Facility", area ="Reporting" })},
                    new SelectListItem() { Text="Quarterly Infections By Floor" , Value = Url.Action("QuarterlyInfectionByFloorReport",new { controller = "Facility", area ="Reporting" })},
                    new SelectListItem() { Text="Quarterly Infections By Wing" , Value = Url.Action("QuarterlyInfectionByWingReport",new { controller = "Facility", area ="Reporting" })},
                    new SelectListItem() { Text="Quarterly Infection Averages" , Value = Url.Action("QuarterlyInfectionAverageReport",new { controller = "Facility", area ="Reporting" })},
                    new SelectListItem() { Text="Quarterly CAUTI Overview" , Value = Url.Action("QuarterlySCAUTIReport",new { controller = "Facility", area ="Reporting" })}
                };

                list["Antibiotic Utilization / Lab Results"] = new List<SelectListItem>()
                    {
                        new SelectListItem() { Text="Multiple Antibiotics By MD" , Value = Url.Action("AntibioticUtilizationMultipleByMD",new { controller = "FacilityRealtime", area ="Reporting" })},
                        new SelectListItem() { Text="Path/Org Yearly Totals" , Value = Url.Action("OrganismsByYear",new { controller = "FacilityRealtime", area ="Reporting" })},
                        new SelectListItem() { Text="MD Antibiotic Statistics" , Value = Url.Action("AntibioticUtilizationByMDStatistics",new { controller = "FacilityRealtime", area ="Reporting" })}
                    };

                lineListings.Add(
                new SelectListItem()
                {
                    Text = "Patient Infection Line Listing",
                    Value = Url.Action("LineListingInfection",
                    new { Controller = "FacilityRealtime", area = "Reporting" })
                });



                list["Catheter Tracking"] = new List<SelectListItem>()
                {
                    new SelectListItem() { Text="Quarterly Catheters Overview" , Value = Url.Action("QuarterlyCatheterControlReport",new { controller = "Facility", area ="Reporting" })},
                };

                lineListings.Add(
                new SelectListItem()
                {
                    Text = "Catheter Line Listing",
                    Value = Url.Action("LineListingCatheter",
                    new { Controller = "FacilityRealtime", area = "Reporting" })
                });


                if (ActionContext.CurrentFacility.FacilityType == Domain.Enumerations.FacilityType.Hospital)
                {
                    lineListings.Add(
                    new SelectListItem()
                    {
                        Text = "Lab Daily Infection Line Listing",
                        Value = Url.Action("LabDailyLineListing",
                        new { Controller = "FacilityRealtime", area = "Reporting" })
                    });
                }

                if (ActionContext.CurrentUser.HasPermission(Domain.Enumerations.KnownPermision.ViewAndEditEmployeeInfections))
                {
                    lineListings.Add(
                        new SelectListItem()
                        {
                            Text = "Employee Infection Line Listing",
                            Value = Url.Action("LineListingEmployeeInfection",
                            new { Controller = "FacilityRealtime", area = "Reporting" })
                        });
                }

                if (ActionContext.CurrentFacility.HasProduct(Domain.Enumerations.KnownProductType.ComplaintTracking))
                {
                    if (ActionContext.CurrentUser.HasPermission(Domain.Enumerations.KnownPermision.ViewAndEditComplaints))
                    {
                        lineListings.Add(
                            new SelectListItem()
                            {
                                Text = "Complaint Line Listing",
                                Value = Url.Action("LineListingComplaint",
                                new { Controller = "FacilityRealtime", area = "Reporting" })
                            });


                        list["Complaint Statistics"] = new List<SelectListItem>()
                        {
                            new SelectListItem() { Text="Quarterly Complaints Overview" , Value = Url.Action("QuarterlyComplaintControlReport",new { controller = "Facility", area ="Reporting" })},
                            new SelectListItem() { Text="Quarterly Complaint Trends" , Value = Url.Action("QuarterlyComplaintTrendReport",new { controller = "Facility", area ="Reporting" })},
                            new SelectListItem() { Text="Quarterly Complaints By Wing" , Value = Url.Action("QuarterlyComplaintByWingReport",new { controller = "Facility", area ="Reporting" })},
                            new SelectListItem() { Text="Quarterly Complaints By Floor" , Value = Url.Action("QuarterlyComplaintByFloorReport",new { controller = "Facility", area ="Reporting" })}
                        };

                    }
                }

            }


            /* Incident product reports */
            if (ActionContext.CurrentFacility.HasProduct(Domain.Enumerations.KnownProductType.IncidentTracking))
            {
                list["Floor Maps"].Add(new SelectListItem() { Text = "Incident Floor Map History", Value = Url.Action("IncidentFloorMap", new { controller = "Facility", area = "Reporting" }) });

                list["Incident Statistics"] = new List<SelectListItem>()
                {
                    new SelectListItem() { Text="Quarterly Incident Overview" , Value = Url.Action("QuarterlyIncidentControlReport",new { controller = "Facility", area ="Reporting" })},
                    new SelectListItem() { Text="Quarterly Incidents By Type" , Value = Url.Action("QuarterlyIncidentByTypeReport",new { controller = "Facility", area ="Reporting" })},
                    new SelectListItem() { Text="Quarterly Incidents By Injury" , Value = Url.Action("QuarterlyIncidentByInjuryReport",new { controller = "Facility", area ="Reporting" })},
                    new SelectListItem() { Text="Quarterly Incidents By Injury Severity" , Value = Url.Action("QuarterlyIncidentByInjuryLevelReport",new { controller = "Facility", area ="Reporting" })},
                    new SelectListItem() { Text="Quarterly Incidents By Location" , Value = Url.Action("QuarterlyIncidentByLocationReport",new { controller = "Facility", area ="Reporting" })},
                    new SelectListItem() { Text="Quarterly Incidents By Day" , Value = Url.Action("QuarterlyIncidentByDayOfWeekReport",new { controller = "Facility", area ="Reporting" })},
                    new SelectListItem() { Text="Quarterly Incidents By Hour" , Value = Url.Action("QuarterlyIncidentByHourOfDayReport",new { controller = "Facility", area ="Reporting" })},
                    new SelectListItem() { Text="Quarterly Incidents By Wing" , Value = Url.Action("QuarterlyIncidentByWingReport",new { controller = "Facility", area ="Reporting" })},
                    new SelectListItem() { Text="Quarterly Incidents By Floor" , Value = Url.Action("QuarterlyIncidentByFloorReport",new { controller = "Facility", area ="Reporting" })},
                    new SelectListItem() { Text="Patient Repeat Falls" , Value = Url.Action("PatientRepeatFalls",new { controller = "FacilityRealtime", area ="Reporting" })},
                    new SelectListItem() { Text="Falls Witnessed/Unwitnessed" , Value = Url.Action("FallsByWitnessed",new { controller = "FacilityRealtime", area ="Reporting" })}
                };

                lineListings.Add(
                new SelectListItem()
                {
                    Text = "Incident Line Listing",
                    Value = Url.Action("LineListingIncident",
                    new { Controller = "FacilityRealtime", area = "Reporting" })
                });
            }

            /* Vaccine */

            if (ActionContext.CurrentFacility.HasProduct(Domain.Enumerations.KnownProductType.VaccineTracking))
            {
                lineListings.Add(
                new SelectListItem()
                {
                    Text = "Immunization Line Listing",
                    Value = Url.Action("LineListingVaccine",
                    new { Controller = "FacilityRealtime", area = "Reporting" })
                });
            }

            /* Wound product reports */

            if (ActionContext.CurrentFacility.HasProduct(Domain.Enumerations.KnownProductType.WoundTracking))
            {
                lineListings.Add(
                new SelectListItem()
                {
                    Text = "Wound Line Listing",
                    Value = Url.Action("LineListingWound",
                    new { Controller = "FacilityRealtime", area = "Reporting" })
                });


                list["Wound Statistics"] = new List<SelectListItem>()
                {
                    new SelectListItem() { Text="Quarterly Wound Overview" , Value = Url.Action("QuarterlyWoundControlReport",new { controller = "Facility", area ="Reporting" })},
                    new SelectListItem() { Text="Quarterly Wounds By Wing" , Value = Url.Action("QuarterlyWoundByWingReport",new { controller = "Facility", area ="Reporting" })},
                    new SelectListItem() { Text="Wound Flow Sheet" , Value = Url.Action("WoundFlowSheet",new { controller = "FacilityRealtime", area ="Reporting" })}
                };
                
            }

            /* Psycotropic product reports */
            if (ActionContext.CurrentFacility.HasProduct(Domain.Enumerations.KnownProductType.PsychotropicTracking))
            {
                lineListings.Add(
                new SelectListItem()
                {
                    Text = "Psychotropic Line Listing",
                    Value = Url.Action("LineListingPsychotropic",
                    new { Controller = "FacilityRealtime", area = "Reporting" })
                });

                list["Psychotropic Statistics"] = new List<SelectListItem>()
                {
                    new SelectListItem() { Text="Quarterly Psychotropic Overview" , Value = Url.Action("QuarterlyPsychotropicControlReport",new { controller = "Facility", area ="Reporting" })}
                };
            }

            /* CMS */

            if (ActionContext.CurrentFacility.HasProduct(Domain.Enumerations.KnownProductType.CmsCompliance))
            {
                if (ActionContext.CurrentUser.HasPermission(Domain.Enumerations.KnownPermision.ManageCmsRecords))
                {
                    list["CMS Compliance"] = new List<SelectListItem>();

                    list["CMS Compliance"].Add(
                        new SelectListItem()
                        {
                            Text = "CMS 802 Roster",
                            Value = Url.Action("Cms802",
                            new { Controller = "FacilityRealtime", area = "Reporting" })
                        });

                    list["CMS Compliance"].Add(
                        new SelectListItem()
                        {
                            Text = "CMS 672 Census",
                            Value = Url.Action("Cms672",
                            new { Controller = "FacilityRealtime", area = "Reporting" })
                        });


                    list["CMS Compliance"].Add(
                        new SelectListItem()
                        {
                            Text = "CMS Notes",
                            Value = Url.Action("CmsNotes",
                            new { Controller = "FacilityRealtime", area = "Reporting" })
                        });
                }
            }

          

            /* Misc */

            list["Miscellaneous"] = new List<SelectListItem>();

            list["Miscellaneous"].Add(
                        new SelectListItem()
                        {
                            Text = "Patient List",
                            Value = Url.Action("PatientList",
                            new { Controller = "FacilityRealtime", area = "Reporting" })
                        });

            /* WI Specific */
            if (ActionContext.CurrentFacility.State == "WI")
            {
                var wisconsinItems = new List<SelectListItem>();

                if(ActionContext.CurrentFacility.HasProduct(Domain.Enumerations.KnownProductType.InfectionTracking))
                {
                    if(ActionContext.CurrentUser.HasPermission(Domain.Enumerations.KnownPermision.ViewAndEditEmployeeInfections))
                    {
                        wisconsinItems.Add(
                        new SelectListItem()
                        {
                            Text = "DHS Staff Outbreak Case Log",
                            Value = Url.Action("DHSStaffOutbreakCaseLog",
                            new { Controller = "StateWisconsinRealtime", area = "Reporting" })
                        });
                    }
                }

                if(wisconsinItems.Count > 0)
                {
                    list["Wisconsin Specific"] = wisconsinItems;
                }
                
            }





            return PartialView("QuickList", list);
        }

        public ActionResult AccountReportQuickList()
        {
            var list = new Dictionary<string, List<SelectListItem>>();

            if (ActionContext.CurrentFacility.HasProduct(Domain.Enumerations.KnownProductType.InfectionTracking))
            {
                list["Quarterly Infection Control"] = new List<SelectListItem>()
                {
                    new SelectListItem() { Text="Quarterly Infections Overview" , Value = Url.Action("QuarterlyInfectionControlReport",new { controller = "Account", area ="Reporting" })},
                    new SelectListItem() { Text="Quarterly Infections By Facility" , Value = Url.Action("QuarterlyInfectionByFacilityReport",new { controller = "Account", area ="Reporting" })},
                    new SelectListItem() { Text="Quarterly Infection Averages" , Value = Url.Action("QuarterlyInfectionAverageReport",new { controller = "Account", area ="Reporting" })}
                };

            }

            return PartialView("QuickList", list);
        }

        public ActionResult PatientQuickSearch()
        {
            return PartialView();
        }

    }
}
