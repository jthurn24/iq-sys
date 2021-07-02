using System;
using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Mvc.ModelMapper;
using IQI.Intuition.Infrastructure.Services;
using IQI.Intuition.Reporting.Models.Cubes;
using IQI.Intuition.Reporting.Models.Dimensions;
using IQI.Intuition.Reporting.Repositories;
using IQI.Intuition.Web.Attributes;
using IQI.Intuition.Web.Models.Reporting.Infection.Facility;
using IQI.Intuition.Web.Models.Reporting.Incident.Facility;
using IQI.Intuition.Web.Models.Reporting.Psychotropic.Facility;
using IQI.Intuition.Web.Models.Reporting.Wound.Facility;
using IQI.Intuition.Web.Models.Reporting.Complaint.Facility;
using IQI.Intuition.Web.Models.Reporting.Catheter.Facility;
using IQI.Intuition.Web.Extensions;
using RedArrow.Framework.Extensions.Formatting;

namespace IQI.Intuition.Web.Areas.Reporting.Controllers
{
    [SupportsTokenAuthentication, PremiumOnly]
    public class FacilityController : Controller
    {
        private Facility _Facility;

        public FacilityController(
            IActionContext actionContext, 
            IModelMapper modelMapper,
            ICubeRepository cubeRepository,
            IDimensionRepository dimensionRepository)
        {
            ActionContext = actionContext.ThrowIfNullArgument("actionContext");
            ModelMapper = modelMapper.ThrowIfNullArgument("modelMapper");
            CubeRepository = cubeRepository;
            DimensionRepository = dimensionRepository;

            _Facility = DimensionRepository.GetFacility(ActionContext.CurrentFacility.Guid);
        }

        protected virtual IActionContext ActionContext { get; private set; }
        protected virtual IModelMapper ModelMapper { get; private set; }
        protected virtual ICubeRepository CubeRepository { get; private set; }
        protected virtual IDimensionRepository DimensionRepository { get; private set; }

        public ActionResult Home()
        {
            if(ActionContext.CurrentFacility.HasProduct(Domain.Enumerations.KnownProductType.InfectionTracking))
            {
                return RedirectToRoute(new { controller = "FacilityRealtime", area = "Reporting", action = "LineListingInfection" });
            }

            return RedirectToRoute(new { controller = "FacilityRealtime", area = "Reporting", action = "LineListingIncident" });
        }

        public ActionResult InfectionFloorMap(Web.Models.Reporting.Infection.FloorMapView model)
        {

            /* setup options */

            model.FloorMapOptions = DimensionRepository.GetFloorMapsForFacility(_Facility.Id)
                .Where(x => x.Active == true)
                .Select(m => new SelectListItem() { Text = m.Name, Value = m.Id.ToString() });

            if (model.FloorMap.HasValue == false)
            {
                model.FloorMap = Guid.Parse(model.FloorMapOptions.First().Value);
            }

            DateTime startDate;
            DateTime endDate;

            if (!DateTime.TryParse(model.StartDate, out startDate))
            {
                startDate = new DateTime(DateTime.Today.Year,DateTime.Today.Month,1);
            }

            if (!DateTime.TryParse(model.EndDate, out endDate))
            {
                endDate = startDate.AddMonths(1);
            }

            model.StartDate = startDate.ToString("MM/dd/yy");
            model.EndDate = endDate.ToString("MM/dd/yy");

            var displayModes = new List<SelectListItem>();
            displayModes.Add(new SelectListItem() { Text = "Show All", Value = "0" });
            displayModes.Add(new SelectListItem() { Text = "Show Confirmed Only", Value = "1" });
            displayModes.Add(new SelectListItem() { Text = "Show Nosocomial Only", Value = "2" });
            displayModes.Add(new SelectListItem() { Text = "Show Non-Nosocomial Only", Value = "3" });
            model.DisplayModeOptions = displayModes;


            /* load data and map */

            var data = CubeRepository.GetFloorMapRoomInfectionTypeByDateRange(
                model.FloorMap.Value, startDate, endDate);

            if (model.DisplayMode == 1)
            {
                data = data.Where(x => x.InfectionClassification.IsQualified).ToList();
            }
            else if (model.DisplayMode == 2)
            {
                data = data.Where(x => x.InfectionClassification.IsNosocomial && x.InfectionClassification.IsQualified).ToList();
            }
            else if (model.DisplayMode == 3)
            {
                data = data.Where(x => x.InfectionClassification.IsNosocomial == false && x.InfectionClassification.IsQualified).ToList();
            }

            var infectionTypes = data.Select(x => x.InfectionType).Distinct(x => x.Id);

            var floorMap = DimensionRepository.GetFloorMap(model.FloorMap.Value);
            var floorMapRooms = DimensionRepository.GetFloorMapRoomsForFloorMap(floorMap.Id);

            model.MapChart(data,floorMap.Id,floorMapRooms,infectionTypes);
 
            var selectedFloorMap = DimensionRepository.GetFloorMap(model.FloorMap.Value);

            this.ControllerContext.SetContentTitle("{0} - Infection Floor Map - {1} through {2} - {3} ",
                _Facility.Name, 
                startDate.ToString("MM/dd/yy"),
                endDate.ToString("MM/dd/yy"),
                selectedFloorMap.Name);

            return View(model);
        }

        public ActionResult IncidentFloorMap(Web.Models.Reporting.Incident.FloorMapView model)
        {

            /* setup options */

            model.FloorMapOptions = DimensionRepository.GetFloorMapsForFacility(_Facility.Id)
                .Where(x => x.Active == true)
                .Select(m => new SelectListItem() { Text = m.Name, Value = m.Id.ToString() });

            if (model.FloorMap.HasValue == false)
            {
                model.FloorMap = Guid.Parse(model.FloorMapOptions.First().Value);
            }

            DateTime startDate;
            DateTime endDate;

            if (!DateTime.TryParse(model.StartDate, out startDate))
            {
                startDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            }

            if (!DateTime.TryParse(model.EndDate, out endDate))
            {
                endDate = startDate.AddMonths(1);
            }

            model.StartDate = startDate.ToString("MM/dd/yy");
            model.EndDate = endDate.ToString("MM/dd/yy");



            /* load data and map */

            var data = CubeRepository.GetFloorMapRoomIncidentTypeByDateRange(model.FloorMap.Value, startDate, endDate);


            var incidentGroupTypes = data
                .Where(x => x.IncidentTypeGroups != null)
                .SelectMany(x => x.IncidentTypeGroups).Distinct(x => x.Id);

            var floorMap = DimensionRepository.GetFloorMap(model.FloorMap.Value);
            var floorMapRooms = DimensionRepository.GetFloorMapRoomsForFloorMap(floorMap.Id);

            model.MapChart(data, floorMap.Id, floorMapRooms, incidentGroupTypes);

            var selectedFloorMap = DimensionRepository.GetFloorMap(model.FloorMap.Value);

            this.ControllerContext.SetContentTitle("{0} - Incident Floor Map - {1} through {2} - {3} ",
                _Facility.Name,
                startDate.ToString("MM/dd/yy"),
                endDate.ToString("MM/dd/yy"),
                selectedFloorMap.Name);

            return View(model);
        }

        public ActionResult QuarterlyInfectionControlReport(QuarterlyInfectionControlView model)
        {
            if (model.Quarter == Guid.Empty)
            {
                return RedirectToAction("QuarterlyInfectionControlReport", new { Quarter = this.ControllerContext.GetDefaultQuarterId(DimensionRepository) });
            }
            else
            {
                this.ControllerContext.SetDefaultQuarterId(model.Quarter);
            }

            model.QuarterOptions = DimensionRepository.GetNonFutureQuarters()
                    .Select(m => new SelectListItem() { Value = m.Id.ToString(), Text = string.Concat("Quarter ",m.QuarterOfYear, " - ", m.Year)});

            var census = CubeRepository.GetFacilityMonthCensusByQuarter(_Facility.Id, model.Quarter);
            var quarter = DimensionRepository.GetQuarter(model.Quarter);
            var totals = CubeRepository.GetFacilityMonthInfectionTypeByQuarter(_Facility.Id, quarter.Id);
            var quarterMonths = DimensionRepository.GetQuarterMonths(model.Quarter);

            model.SetData(quarterMonths, census, totals);

            var selectedQuarter = DimensionRepository.GetAllQuarters().Where(x => x.Id == model.Quarter).FirstOrDefault();

            this.ControllerContext.SetContentTitle("{0} - Infection Control - {1} Q{2} ",
                _Facility.Name, selectedQuarter.Year,
                selectedQuarter.QuarterOfYear);

            return View(model);
        }

        public ActionResult QuarterlyInfectionByTypeReport(QuarterlyInfectionByTypeView model)
        {
            if (model.Quarter == Guid.Empty)
            {
                return RedirectToAction("QuarterlyInfectionByTypeReport", new { Quarter = this.ControllerContext.GetDefaultQuarterId(DimensionRepository) });
            }
            else
            {
                this.ControllerContext.SetDefaultQuarterId(model.Quarter);
            }

            model.QuarterOptions = DimensionRepository.GetNonFutureQuarters()
                    .Select(m => new SelectListItem() { Value = m.Id.ToString(), Text = string.Concat("Quarter ", m.QuarterOfYear, " - ", m.Year) });

            var quarter = DimensionRepository.GetQuarter(model.Quarter);
            var quarterMonths = DimensionRepository.GetQuarterMonths(model.Quarter);

            var totals = CubeRepository.GetFacilityMonthInfectionTypeByQuarter(_Facility.Id, quarter.Id);
            model.SetData(quarterMonths,totals);

            var selectedQuarter = DimensionRepository.GetAllQuarters().Where(x => x.Id == model.Quarter).FirstOrDefault();

            this.ControllerContext.SetContentTitle("{0} - Infections By Type - {1} Q{2} ",
                _Facility.Name, selectedQuarter.Year,
                selectedQuarter.QuarterOfYear);

            return View(model);
        }

        public ActionResult QuarterlyInfectionBySiteReport(QuarterlyInfectionBySiteView model)
        {
            if (model.Quarter == Guid.Empty)
            {
                return RedirectToAction(
                    "QuarterlyInfectionBySiteReport", 
                    new { 
                        Quarter = this.ControllerContext.GetDefaultQuarterId(DimensionRepository) ,
                        InfectionType = DimensionRepository.GetInfectionTypes().First().Id
                    });
            }
            else
            {
                this.ControllerContext.SetDefaultQuarterId(model.Quarter);
            }


            model.QuarterOptions = DimensionRepository.GetNonFutureQuarters()
                    .Select(m => new SelectListItem() { Value = m.Id.ToString(), Text = string.Concat("Quarter ", m.QuarterOfYear, " - ", m.Year) });

            model.InfectionTypeOptions = CubeRepository.GetFacilityInfectionSiteForFacility(_Facility.Id)
                    .Select(x => x.InfectionType)
                    .Distinct()
                    .Select(m => new SelectListItem() { Value = m.Id.ToString(), Text = m.Name });

            var quarter = DimensionRepository.GetQuarter(model.Quarter);
            var totals = CubeRepository.GetFacilityMonthInfectionSiteByQuarter(_Facility.Id, quarter.Id,model.InfectionType.Value);
            var quarterMonths = DimensionRepository.GetQuarterMonths(model.Quarter);

            model.SetData(quarterMonths, totals);

            var selectedQuarter = DimensionRepository.GetAllQuarters().Where(x => x.Id == model.Quarter).FirstOrDefault();

            var condition = DimensionRepository.GetInfectionTypes().Where(x => x.Id == model.InfectionType.Value).First();

            this.ControllerContext.SetContentTitle("{0} - {3} (Details) - {1} Q{2} ",
                _Facility.Name, selectedQuarter.Year,
                selectedQuarter.QuarterOfYear,
                condition.Name);

            return View(model);
        }

        public ActionResult QuarterlyInfectionByFloorReport(QuarterlyInfectionByFloorView model)
        {
            if (model.Quarter == Guid.Empty)
            {
                return RedirectToAction("QuarterlyInfectionByFloorReport", new { Quarter = this.ControllerContext.GetDefaultQuarterId(DimensionRepository) });
            }
            else
            {
                this.ControllerContext.SetDefaultQuarterId(model.Quarter);
            }

            model.QuarterOptions = DimensionRepository.GetNonFutureQuarters()
                    .Select(m => new SelectListItem() { Value = m.Id.ToString(), Text = string.Concat("Quarter ", m.QuarterOfYear, " - ", m.Year) });

            var quarter = DimensionRepository.GetQuarter(model.Quarter);
            var quarterMonths = DimensionRepository.GetQuarterMonths(model.Quarter);

            var totals = CubeRepository.GetFloorMonthInfectionTypeByQuarter(_Facility.Id, quarter.Id);
            model.SetData(quarterMonths, totals);

            var selectedQuarter = DimensionRepository.GetAllQuarters().Where(x => x.Id == model.Quarter).FirstOrDefault();

            this.ControllerContext.SetContentTitle("{0} - Infections By Floor - {1} Q{2} ",
                _Facility.Name, selectedQuarter.Year,
                selectedQuarter.QuarterOfYear);

            return View(model);
        }

        public ActionResult QuarterlyInfectionByWingReport(QuarterlyInfectionByWingView model)
        {
            if (model.Quarter == Guid.Empty)
            {
                return RedirectToAction("QuarterlyInfectionByWingReport", new { Quarter = this.ControllerContext.GetDefaultQuarterId(DimensionRepository) });
            }
            else
            {
                this.ControllerContext.SetDefaultQuarterId(model.Quarter);
            }

            model.QuarterOptions = DimensionRepository.GetNonFutureQuarters()
                    .Select(m => new SelectListItem() { Value = m.Id.ToString(), Text = string.Concat("Quarter ", m.QuarterOfYear, " - ", m.Year) });

            var quarter = DimensionRepository.GetQuarter(model.Quarter);
            var quarterMonths = DimensionRepository.GetQuarterMonths(model.Quarter);

            var totals = CubeRepository.GetWingMonthInfectionTypeByQuarter(_Facility.Id, quarter.Id);
            model.SetData(quarterMonths, totals);

            var selectedQuarter = DimensionRepository.GetAllQuarters().Where(x => x.Id == model.Quarter).FirstOrDefault();

            this.ControllerContext.SetContentTitle("{0} - Infections By Wing - {1} Q{2} ",
                _Facility.Name, selectedQuarter.Year,
                selectedQuarter.QuarterOfYear);

            return View(model);
        }

        public ActionResult QuarterlyInfectionAverageReport(QuarterlyInfectionAverageView model)
        {
            if (model.Quarter == Guid.Empty)
            {
                return RedirectToAction("QuarterlyInfectionAverageReport", new { Quarter = this.ControllerContext.GetDefaultQuarterId(DimensionRepository) });
            }
            else
            {
                this.ControllerContext.SetDefaultQuarterId(model.Quarter);
            }

            model.QuarterOptions = DimensionRepository.GetNonFutureQuarters()
                    .Select(m => new SelectListItem() { Value = m.Id.ToString(), Text = string.Concat("Quarter ", m.QuarterOfYear, " - ", m.Year) });

            var averageTypes = DimensionRepository.GetFacilityAverageTypesForFacility(_Facility.Id);
            model.AverageTypeOptions = averageTypes.Select(m => new SelectListItem() { Value = m.AverageType.Id.ToString(), Text = m.AverageType.Name });

            if(model.AverageType == Guid.Empty)
            {
                model.AverageType = averageTypes.First().AverageType.Id;
            }

            var quarter = DimensionRepository.GetQuarter(model.Quarter);
            var facilityTotals = CubeRepository.GetFacilityMonthInfectionTypeByQuarter(_Facility.Id, quarter.Id);
            var averageTotals = CubeRepository.GetAverageTypeMonthInfectionTypeByQuarter(model.AverageType, quarter.Id);

            
            var selectedQuarter = DimensionRepository.GetQuarter(model.Quarter);
            var selectedAverage = DimensionRepository.GetAverageType(model.AverageType);

            this.ControllerContext.SetContentTitle("{0} - Infection Averages ({3}) - {1} Q{2} ",
                _Facility.Name, selectedQuarter.Year,
                selectedQuarter.QuarterOfYear,
                selectedAverage.Name);

            var quarterMonths = DimensionRepository.GetQuarterMonths(quarter.Id);

            model.SetData(quarterMonths,facilityTotals,averageTotals, DimensionRepository.GetInfectionTypes());

            return View(model);
        }

        public ActionResult QuarterlyInfectionTrendReport(QuarterlyInfectionTrendView model)
        {
            if (model.Quarter == Guid.Empty)
            {
                return RedirectToAction("QuarterlyInfectionTrendReport", new { Quarter = this.ControllerContext.GetDefaultQuarterId(DimensionRepository) });
            }
            else
            {
                this.ControllerContext.SetDefaultQuarterId(model.Quarter);
            }

            var quarters = DimensionRepository.GetNonFutureQuarters();

            model.QuarterOptions = quarters
                    .Select(m => new SelectListItem() { Value = m.Id.ToString(), Text = string.Concat("Quarter ", m.QuarterOfYear, " - ", m.Year) });

            var selectedQuarter = DimensionRepository.GetAllQuarters().Where(x => x.Id == model.Quarter).FirstOrDefault();


            var selectedQuarters = quarters
                .Where( x =>
                    (x.Year == selectedQuarter.Year && x.QuarterOfYear <= selectedQuarter.QuarterOfYear)
                || (x.Year == (selectedQuarter.Year - 1) && x.QuarterOfYear >= selectedQuarter.QuarterOfYear));

            var selectedMonths = new List<Month>();

            foreach (var q in selectedQuarters)
            {
                var m = DimensionRepository.GetQuarterMonths(q.Id);
                selectedMonths.Add(m.Month1);
                selectedMonths.Add(m.Month2);
                selectedMonths.Add(m.Month3);
            }

            selectedMonths = selectedMonths.OrderBy(x => x.Year).ThenBy(x => x.MonthOfYear).ToList();

            
            IEnumerable<FacilityMonthInfectionSite.Entry> totals = new List<FacilityMonthInfectionSite.Entry>();

            foreach(var quarter in selectedQuarters)
            {
                totals = totals.Append(CubeRepository.GetFacilityMonthInfectionSiteByQuarter(_Facility.Id, quarter.Id, null).ToArray());
            }

            model.Months = selectedMonths;
            model.Month3 = selectedMonths.OrderByDescending(x => x.Year).ThenByDescending(x => x.MonthOfYear).First();
            model.Month2 = selectedMonths.OrderByDescending(x => x.Year).ThenByDescending(x => x.MonthOfYear).Skip(1).First();
            model.Month1 = selectedMonths.OrderByDescending(x => x.Year).ThenByDescending(x => x.MonthOfYear).Skip(2).First();

            model.SetData(selectedQuarter, totals);

            this.ControllerContext.SetContentTitle("{0} - Infection Trends - {1} Q{2} ",
                _Facility.Name, selectedQuarter.Year,
                selectedQuarter.QuarterOfYear);

            return View(model);
        }

        public ActionResult QuarterlyIncidentControlReport(QuarterlyIncidentControlView model)
        {
            if (model.Quarter == Guid.Empty)
            {
                return RedirectToAction("QuarterlyIncidentControlReport", new { Quarter = this.ControllerContext.GetDefaultQuarterId(DimensionRepository) });
            }
            else
            {
                this.ControllerContext.SetDefaultQuarterId(model.Quarter);
            }

            model.QuarterOptions = DimensionRepository.GetNonFutureQuarters()
                    .Select(m => new SelectListItem() { Value = m.Id.ToString(), Text = string.Concat("Quarter ", m.QuarterOfYear, " - ", m.Year) });

            var census = CubeRepository.GetFacilityMonthCensusByQuarter(_Facility.Id, model.Quarter);
            var quarter = DimensionRepository.GetQuarter(model.Quarter);
            var typeTotals = CubeRepository.GetFacilityMonthIncidentTypeGroupByQuarter(_Facility.Id, quarter.Id);
            var injuryTotals = CubeRepository.GetFacilityMonthIncidentInjuryByQuarter(_Facility.Id, quarter.Id);
            var injuryLevelTotals = CubeRepository.GetFacilityMonthIncidentInjuryLevelByQuarter(_Facility.Id, quarter.Id);
            var locationTotals = CubeRepository.GetFacilityMonthIncidentLocationByQuarter(_Facility.Id, quarter.Id);
            var dayOfWeekTotals = CubeRepository.GetFacilityMonthIncidentDayOfWeekByQuarter(_Facility.Id, quarter.Id);
            var hourOfDayTotals = CubeRepository.GetFacilityMonthIncidentHourOfDayByQuarter(_Facility.Id, quarter.Id);
            var floorTotals = CubeRepository.GetFloorMonthIncidentTypeGroupByQuarter(_Facility.Id, quarter.Id);
            var wingTotals = CubeRepository.GetWingMonthIncidentTypeGroupByQuarter(_Facility.Id, quarter.Id);
            var quarterMonths = DimensionRepository.GetQuarterMonths(model.Quarter);
            
            model.SetData(quarterMonths, census,
                typeTotals,
                injuryTotals,
                locationTotals,
                injuryLevelTotals,
                dayOfWeekTotals,
                hourOfDayTotals,
                floorTotals,
                wingTotals);

            var selectedQuarter = DimensionRepository.GetAllQuarters().Where(x => x.Id == model.Quarter).FirstOrDefault();

            this.ControllerContext.SetContentTitle("{0} - Incident Control - {1} Q{2} ",
                _Facility.Name, selectedQuarter.Year,
                selectedQuarter.QuarterOfYear);

            return View(model);
        }

        public ActionResult QuarterlyIncidentByTypeReport(QuarterlyIncidentByTypeView model)
        {
            if (model.Quarter == Guid.Empty)
            {
                return RedirectToAction("QuarterlyIncidentByTypeReport", new
                {
                    Quarter = this.ControllerContext.GetDefaultQuarterId(DimensionRepository),
                    IncidentTypeGroup = this.ControllerContext.GetDefaultIncidentGroupId(DimensionRepository)
                });
            }
            else
            {
                this.ControllerContext.SetDefaultQuarterId(model.Quarter);
                this.ControllerContext.SetDefaultIncidentGroupId(model.IncidentTypeGroup);
            }


            var quarter = DimensionRepository.GetQuarter(model.Quarter);
            var quarterMonths = DimensionRepository.GetQuarterMonths(model.Quarter);

            var totals = CubeRepository
                .GetFacilityMonthIncidentTypeByQuarter(_Facility.Id, quarter.Id)
                .Where( x => x.IncidentTypeGroup.Id == model.IncidentTypeGroup);

            model.QuarterOptions = DimensionRepository.GetNonFutureQuarters()
                    .Select(m => new SelectListItem() { Value = m.Id.ToString(), Text = string.Concat("Quarter ", m.QuarterOfYear, " - ", m.Year) });

            model.IncidentTypeGroupOptions = DimensionRepository.GetIncidentTypeGroups()
                    .Select(m => new SelectListItem() { Value = m.Id.ToString(), Text = m.Name });

            model.SetData(quarterMonths, totals);

            var group = DimensionRepository.GetIncidentTypeGroups().Where(x => x.Id == model.IncidentTypeGroup).First();

            this.ControllerContext.SetContentTitle("{0} - {3} (Details)  - {1} Q{2} ",
                _Facility.Name, quarter.Year,
                quarter.QuarterOfYear,
                group.Name);

            return View(model);
        }

        public ActionResult QuarterlyIncidentByInjuryReport(QuarterlyIncidentByInjuryView model)
        {
            if (model.Quarter == Guid.Empty)
            {
                return RedirectToAction("QuarterlyIncidentByInjuryReport", new
                {
                    Quarter = this.ControllerContext.GetDefaultQuarterId(DimensionRepository),
                    IncidentTypeGroup = this.ControllerContext.GetDefaultIncidentGroupId(DimensionRepository)
                });
            }
            else
            {
                this.ControllerContext.SetDefaultQuarterId(model.Quarter);
                this.ControllerContext.SetDefaultIncidentGroupId(model.IncidentTypeGroup);
            }


            var quarter = DimensionRepository.GetQuarter(model.Quarter);

            var totals = CubeRepository
                .GetFacilityMonthIncidentInjuryByQuarter(_Facility.Id, quarter.Id)
                .Where(x => x.IncidentTypeGroup.Id == model.IncidentTypeGroup);

            model.QuarterOptions = DimensionRepository.GetNonFutureQuarters()
                    .Select(m => new SelectListItem() { Value = m.Id.ToString(), Text = string.Concat("Quarter ", m.QuarterOfYear, " - ", m.Year) });

            model.IncidentTypeGroupOptions = DimensionRepository.GetIncidentTypeGroups()
                    .Select(m => new SelectListItem() { Value = m.Id.ToString(), Text = m.Name });

            var quarterMonths = DimensionRepository.GetQuarterMonths(model.Quarter);

            model.SetData(quarterMonths, totals);

            var group = DimensionRepository.GetIncidentTypeGroups().Where(x => x.Id == model.IncidentTypeGroup).First();

            this.ControllerContext.SetContentTitle("{0} - {3} (Injury)  - {1} Q{2} ",
                _Facility.Name, quarter.Year,
                quarter.QuarterOfYear,
                group.Name);

            return View(model);
        }

        public ActionResult QuarterlyIncidentByInjuryLevelReport(QuarterlyIncidentByInjuryLevelView model)
        {
            if (model.Quarter == Guid.Empty)
            {
                return RedirectToAction("QuarterlyIncidentByInjuryLevelReport", new
                {
                    Quarter = this.ControllerContext.GetDefaultQuarterId(DimensionRepository),
                    IncidentTypeGroup = this.ControllerContext.GetDefaultIncidentGroupId(DimensionRepository)
                });
            }
            else
            {
                this.ControllerContext.SetDefaultQuarterId(model.Quarter);
                this.ControllerContext.SetDefaultIncidentGroupId(model.IncidentTypeGroup);
            }


            var quarter = DimensionRepository.GetQuarter(model.Quarter);
            var quarterMonths = DimensionRepository.GetQuarterMonths(model.Quarter);

            var totals = CubeRepository
                .GetFacilityMonthIncidentInjuryLevelByQuarter(_Facility.Id, quarter.Id)
                .Where(x => x.IncidentTypeGroup.Id == model.IncidentTypeGroup);

            model.QuarterOptions = DimensionRepository.GetNonFutureQuarters()
                    .Select(m => new SelectListItem() { Value = m.Id.ToString(), Text = string.Concat("Quarter ", m.QuarterOfYear, " - ", m.Year) });

            model.IncidentTypeGroupOptions = DimensionRepository.GetIncidentTypeGroups()
                    .Select(m => new SelectListItem() { Value = m.Id.ToString(), Text = m.Name });

            model.SetData(quarterMonths, totals);

            var group = DimensionRepository.GetIncidentTypeGroups().Where(x => x.Id == model.IncidentTypeGroup).First();

            this.ControllerContext.SetContentTitle("{0} - {3} (Injury Severity)  - {1} Q{2} ",
                _Facility.Name, quarter.Year,
                quarter.QuarterOfYear,
                group.Name);

            return View(model);
        }

        public ActionResult QuarterlyIncidentByLocationReport(QuarterlyIncidentByLocationView model)
        {
            if (model.Quarter == Guid.Empty)
            {
                return RedirectToAction("QuarterlyIncidentByLocationReport", new
                {
                    Quarter = this.ControllerContext.GetDefaultQuarterId(DimensionRepository),
                    IncidentTypeGroup = this.ControllerContext.GetDefaultIncidentGroupId(DimensionRepository)
                });
            }
            else
            {
                this.ControllerContext.SetDefaultQuarterId(model.Quarter);
                this.ControllerContext.SetDefaultIncidentGroupId(model.IncidentTypeGroup);
            }


            var quarter = DimensionRepository.GetQuarter(model.Quarter);

            var totals = CubeRepository
                .GetFacilityMonthIncidentLocationByQuarter(_Facility.Id, quarter.Id)
                .Where(x => x.IncidentTypeGroup.Id == model.IncidentTypeGroup);

            model.QuarterOptions = DimensionRepository.GetNonFutureQuarters()
                    .Select(m => new SelectListItem() { Value = m.Id.ToString(), Text = string.Concat("Quarter ", m.QuarterOfYear, " - ", m.Year) });

            model.IncidentTypeGroupOptions = DimensionRepository.GetIncidentTypeGroups()
                    .Select(m => new SelectListItem() { Value = m.Id.ToString(), Text = m.Name });

            var quarterMonths = DimensionRepository.GetQuarterMonths(model.Quarter);

            model.SetData(quarterMonths, totals);

            var group = DimensionRepository.GetIncidentTypeGroups().Where(x => x.Id == model.IncidentTypeGroup).First();

            this.ControllerContext.SetContentTitle("{0} - {3} (Location)  - {1} Q{2} ",
                _Facility.Name, quarter.Year,
                quarter.QuarterOfYear,
                group.Name);

            return View(model);
        }

        public ActionResult QuarterlyIncidentByDayOfWeekReport(QuarterlyIncidentByDayOfWeekView model)
        {
            if (model.Quarter == Guid.Empty)
            {
                return RedirectToAction("QuarterlyIncidentByDayOfWeekReport", new
                {
                    Quarter = this.ControllerContext.GetDefaultQuarterId(DimensionRepository),
                    IncidentTypeGroup = this.ControllerContext.GetDefaultIncidentGroupId(DimensionRepository)
                });
            }
            else
            {
                this.ControllerContext.SetDefaultQuarterId(model.Quarter);
                this.ControllerContext.SetDefaultIncidentGroupId(model.IncidentTypeGroup);
            }


            var quarter = DimensionRepository.GetQuarter(model.Quarter);

            var totals = CubeRepository
                .GetFacilityMonthIncidentDayOfWeekByQuarter(_Facility.Id, quarter.Id)
                .Where(x => x.IncidentTypeGroup.Id == model.IncidentTypeGroup);

            model.QuarterOptions = DimensionRepository.GetNonFutureQuarters()
                    .Select(m => new SelectListItem() { Value = m.Id.ToString(), Text = string.Concat("Quarter ", m.QuarterOfYear, " - ", m.Year) });

            model.IncidentTypeGroupOptions = DimensionRepository.GetIncidentTypeGroups()
                    .Select(m => new SelectListItem() { Value = m.Id.ToString(), Text = m.Name });

            var quarterMonths = DimensionRepository.GetQuarterMonths(model.Quarter);

            model.SetData(quarterMonths, totals);

            var group = DimensionRepository.GetIncidentTypeGroups().Where(x => x.Id == model.IncidentTypeGroup).First();

            this.ControllerContext.SetContentTitle("{0} - {3} (Day Of Week)  - {1} Q{2} ",
                _Facility.Name, quarter.Year,
                quarter.QuarterOfYear,
                group.Name);

            return View(model);
        }

        public ActionResult QuarterlyIncidentByHourOfDayReport(QuarterlyIncidentByHourOfDayView model)
        {
            if (model.Quarter == Guid.Empty)
            {
                return RedirectToAction("QuarterlyIncidentByHourOfDayReport", new
                {
                    Quarter = this.ControllerContext.GetDefaultQuarterId(DimensionRepository),
                    IncidentTypeGroup = this.ControllerContext.GetDefaultIncidentGroupId(DimensionRepository)
                });
            }
            else
            {
                this.ControllerContext.SetDefaultQuarterId(model.Quarter);
                this.ControllerContext.SetDefaultIncidentGroupId(model.IncidentTypeGroup);
            }


            var quarter = DimensionRepository.GetQuarter(model.Quarter);

            var totals = CubeRepository
                .GetFacilityMonthIncidentHourOfDayByQuarter(_Facility.Id, quarter.Id)
                .Where(x => x.IncidentTypeGroup.Id == model.IncidentTypeGroup);

            model.QuarterOptions = DimensionRepository.GetNonFutureQuarters()
                    .Select(m => new SelectListItem() { Value = m.Id.ToString(), Text = string.Concat("Quarter ", m.QuarterOfYear, " - ", m.Year) });

            model.IncidentTypeGroupOptions = DimensionRepository.GetIncidentTypeGroups()
                    .Select(m => new SelectListItem() { Value = m.Id.ToString(), Text = m.Name });

            var quarterMonths = DimensionRepository.GetQuarterMonths(model.Quarter);

            model.SetData(quarterMonths, totals);

            var group = DimensionRepository.GetIncidentTypeGroups().Where(x => x.Id == model.IncidentTypeGroup).First();

            this.ControllerContext.SetContentTitle("{0} - {3} (Hour Of Day)  - {1} Q{2} ",
                _Facility.Name, quarter.Year,
                quarter.QuarterOfYear,
                group.Name);

            return View(model);
        }

        public ActionResult QuarterlyIncidentByWingReport(QuarterlyIncidentByWingView model)
        {
            if (model.Quarter == Guid.Empty)
            {
                return RedirectToAction("QuarterlyIncidentByWingReport", new
                {
                    Quarter = this.ControllerContext.GetDefaultQuarterId(DimensionRepository),
                    IncidentTypeGroup = this.ControllerContext.GetDefaultIncidentGroupId(DimensionRepository)
                });
            }
            else
            {
                this.ControllerContext.SetDefaultQuarterId(model.Quarter);
                this.ControllerContext.SetDefaultIncidentGroupId(model.IncidentTypeGroup);
            }


            var quarter = DimensionRepository.GetQuarter(model.Quarter);

            var totals = CubeRepository
                .GetWingMonthIncidentTypeGroupByQuarter(_Facility.Id, quarter.Id)
                .Where(x => x.IncidentTypeGroup.Id == model.IncidentTypeGroup);

            model.QuarterOptions = DimensionRepository.GetNonFutureQuarters()
                    .Select(m => new SelectListItem() { Value = m.Id.ToString(), Text = string.Concat("Quarter ", m.QuarterOfYear, " - ", m.Year) });

            model.IncidentTypeGroupOptions = DimensionRepository.GetIncidentTypeGroups()
                    .Select(m => new SelectListItem() { Value = m.Id.ToString(), Text = m.Name });

            var quarterMonths = DimensionRepository.GetQuarterMonths(model.Quarter);

            model.SetData(quarterMonths, totals);

            var group = DimensionRepository.GetIncidentTypeGroups().Where(x => x.Id == model.IncidentTypeGroup).First();

            this.ControllerContext.SetContentTitle("{0} - {3} (Wing)  - {1} Q{2} ",
                _Facility.Name, quarter.Year,
                quarter.QuarterOfYear,
                group.Name);

            return View(model);
        }

        public ActionResult QuarterlyIncidentByFloorReport(QuarterlyIncidentByFloorView model)
        {
            if (model.Quarter == Guid.Empty)
            {
                return RedirectToAction("QuarterlyIncidentByFloorReport", new
                {
                    Quarter = this.ControllerContext.GetDefaultQuarterId(DimensionRepository),
                    IncidentTypeGroup = this.ControllerContext.GetDefaultIncidentGroupId(DimensionRepository)
                });
            }
            else
            {
                this.ControllerContext.SetDefaultQuarterId(model.Quarter);
                this.ControllerContext.SetDefaultIncidentGroupId(model.IncidentTypeGroup);
            }


            var quarter = DimensionRepository.GetQuarter(model.Quarter);

            var totals = CubeRepository
                .GetFloorMonthIncidentTypeGroupByQuarter(_Facility.Id, quarter.Id)
                .Where(x => x.IncidentTypeGroup.Id == model.IncidentTypeGroup);

            model.QuarterOptions = DimensionRepository.GetNonFutureQuarters()
                    .Select(m => new SelectListItem() { Value = m.Id.ToString(), Text = string.Concat("Quarter ", m.QuarterOfYear, " - ", m.Year) });

            model.IncidentTypeGroupOptions = DimensionRepository.GetIncidentTypeGroups()
                    .Select(m => new SelectListItem() { Value = m.Id.ToString(), Text = m.Name });

            var quarterMonths = DimensionRepository.GetQuarterMonths(model.Quarter);

            model.SetData(quarterMonths, totals);

            var group = DimensionRepository.GetIncidentTypeGroups().Where(x => x.Id == model.IncidentTypeGroup).First();

            this.ControllerContext.SetContentTitle("{0} - {3} (Floor)  - {1} Q{2} ",
                _Facility.Name, quarter.Year,
                quarter.QuarterOfYear,
                group.Name);

            return View(model);
        }

        public ActionResult QuarterlyPsychotropicControlReport(QuarterlyPsychotropicControlView model)
        {
            if (model.Quarter == Guid.Empty)
            {
                return RedirectToAction("QuarterlyPsychotropicControlReport", new { Quarter = this.ControllerContext.GetDefaultQuarterId(DimensionRepository) });
            }
            else
            {
                this.ControllerContext.SetDefaultQuarterId(model.Quarter);
            }

            model.QuarterOptions = DimensionRepository.GetNonFutureQuarters()
                    .Select(m => new SelectListItem() { Value = m.Id.ToString(), Text = string.Concat("Quarter ", m.QuarterOfYear, " - ", m.Year) });


            var quarter = DimensionRepository.GetQuarter(model.Quarter);
            var drugTypes = DimensionRepository.GetAllPsychotropicDrugTypes();
            var data = CubeRepository.GetFacilityMonthPsychotropicDrugTypeByQuarter(_Facility.Id, model.Quarter);

            var quarterMonths = DimensionRepository.GetQuarterMonths(model.Quarter);

            model.SetData(quarterMonths, data, drugTypes);

            var selectedQuarter = DimensionRepository.GetAllQuarters().Where(x => x.Id == model.Quarter).FirstOrDefault();

            this.ControllerContext.SetContentTitle("{0} - Psychotropic Control - {1} Q{2} ",
                _Facility.Name, selectedQuarter.Year,
                selectedQuarter.QuarterOfYear);

            return View(model);
        }

        public ActionResult QuarterlyWoundControlReport(QuarterlyWoundControlView model)
        {
            var woundTypes = DimensionRepository.GetWoundTypes();

            if (model.Quarter == Guid.Empty)
            {
                var defaultWoundType = woundTypes.Where(x => x.Name == "Pressure Ulcer").First();

                return RedirectToAction("QuarterlyWoundControlReport", 
                    new { 
                    Quarter = this.ControllerContext.GetDefaultQuarterId(DimensionRepository),
                    WoundType = defaultWoundType.Id 
                    });
            }
            else
            {
                this.ControllerContext.SetDefaultQuarterId(model.Quarter);
            }

            model.QuarterOptions = DimensionRepository.GetNonFutureQuarters()
                    .Select(m => new SelectListItem() { Value = m.Id.ToString(), Text = string.Concat("Quarter ", m.QuarterOfYear, " - ", m.Year) });


            

            model.WoundTypeOptions = woundTypes
                    .Select(m => new SelectListItem() { Value = m.Id.ToString(), Text = m.Name });

            var census = CubeRepository.GetFacilityMonthCensusByQuarter(_Facility.Id, model.Quarter);
            var quarter = DimensionRepository.GetQuarter(model.Quarter);
            var classTotals = CubeRepository.GetFacilityMonthWoundClassificationByQuarter(_Facility.Id, quarter.Id, model.WoundType);
            var stageTotals = CubeRepository.GetFacilityMonthWoundStageByQuarter(_Facility.Id, quarter.Id, model.WoundType);
            var siteTotals = CubeRepository.GetFacilityMonthWoundSiteByQuarter(_Facility.Id, quarter.Id, model.WoundType);
            var allStages = DimensionRepository.GetWoundStages();

            var quarterMonths = DimensionRepository.GetQuarterMonths(model.Quarter);

            model.SetData(quarterMonths, census,
                classTotals,
                siteTotals,
                stageTotals,
                allStages);

            var selectedQuarter = DimensionRepository.GetAllQuarters().Where(x => x.Id == model.Quarter).FirstOrDefault();
            model.WoundTypeName = woundTypes.Where(x => x.Id == model.WoundType).First().Name;

            this.ControllerContext.SetContentTitle("{0} - Wound Control - {3} - {1} Q{2} ",
                _Facility.Name, selectedQuarter.Year,
                selectedQuarter.QuarterOfYear,
                model.WoundTypeName);

            return View(model);
        }

        public ActionResult QuarterlyComplaintControlReport(QuarterlyComplaintControlView model)
        {
            if (model.Quarter == Guid.Empty)
            {
                return RedirectToAction("QuarterlyComplaintControlReport", new { Quarter = this.ControllerContext.GetDefaultQuarterId(DimensionRepository) });
            }
            else
            {
                this.ControllerContext.SetDefaultQuarterId(model.Quarter);
            }

            model.QuarterOptions = DimensionRepository.GetNonFutureQuarters()
                    .Select(m => new SelectListItem() { Value = m.Id.ToString(), Text = string.Concat("Quarter ", m.QuarterOfYear, " - ", m.Year) });

            var census = CubeRepository.GetFacilityMonthCensusByQuarter(_Facility.Id, model.Quarter);
            var quarter = DimensionRepository.GetQuarter(model.Quarter);
            var totals = CubeRepository.GetFacilityMonthComplaintTypeByQuarter(_Facility.Id, quarter.Id);

            var quarterMonths = DimensionRepository.GetQuarterMonths(model.Quarter);


            model.SetData(quarterMonths, census,
                totals);

            var selectedQuarter = DimensionRepository.GetAllQuarters().Where(x => x.Id == model.Quarter).FirstOrDefault();

            this.ControllerContext.SetContentTitle("{0} - Complaint Control - {1} Q{2} ",
                _Facility.Name, selectedQuarter.Year,
                selectedQuarter.QuarterOfYear);

            return View(model);
        }

        public ActionResult QuarterlyComplaintByWingReport(QuarterlyComplaintByWingView model)
        {
            if (model.Quarter == Guid.Empty)
            {
                return RedirectToAction("QuarterlyComplaintByWingReport", new { Quarter = this.ControllerContext.GetDefaultQuarterId(DimensionRepository) });
            }
            else
            {
                this.ControllerContext.SetDefaultQuarterId(model.Quarter);
            }

            model.QuarterOptions = DimensionRepository.GetNonFutureQuarters()
                    .Select(m => new SelectListItem() { Value = m.Id.ToString(), Text = string.Concat("Quarter ", m.QuarterOfYear, " - ", m.Year) });

            var cTypes = DimensionRepository.GetComplaintTypes();

            if (model.ComplaintType.HasValue == false)
            {
                model.ComplaintType = cTypes.First().Id;
            }

            model.ComplaintTypeDescription = cTypes.Where(x => x.Id == model.ComplaintType).First().Name;

            model.ComplaintTypeOptions = cTypes
                .Select(m => new SelectListItem() { Value = m.Id.ToString(), Text = m.Name});

            var census = CubeRepository.GetFacilityMonthCensusByQuarter(_Facility.Id, model.Quarter);
            var quarter = DimensionRepository.GetQuarter(model.Quarter);
            var totals = CubeRepository.GetWingMonthComplaintTypeByQuarter(_Facility.Id, quarter.Id,model.ComplaintType.Value);

            var quarterMonths = DimensionRepository.GetQuarterMonths(model.Quarter);

            model.SetData(quarterMonths, census,
                totals);

            var selectedQuarter = DimensionRepository.GetAllQuarters().Where(x => x.Id == model.Quarter).FirstOrDefault();

            this.ControllerContext.SetContentTitle("{0} - {3} - {1} Q{2} ",
                _Facility.Name, selectedQuarter.Year,
                selectedQuarter.QuarterOfYear,
                model.ComplaintTypeDescription);

            return View(model);
        }

        public ActionResult QuarterlyComplaintByFloorReport(QuarterlyComplaintByFloorView model)
        {
            if (model.Quarter == Guid.Empty)
            {
                return RedirectToAction("QuarterlyComplaintByFloorReport", new { Quarter = this.ControllerContext.GetDefaultQuarterId(DimensionRepository) });
            }
            else
            {
                this.ControllerContext.SetDefaultQuarterId(model.Quarter);
            }

            model.QuarterOptions = DimensionRepository.GetNonFutureQuarters()
                    .Select(m => new SelectListItem() { Value = m.Id.ToString(), Text = string.Concat("Quarter ", m.QuarterOfYear, " - ", m.Year) });

            var cTypes = DimensionRepository.GetComplaintTypes();

            if (model.ComplaintType.HasValue == false)
            {
                model.ComplaintType = cTypes.First().Id;
            }

            model.ComplaintTypeDescription = cTypes.Where(x => x.Id == model.ComplaintType).First().Name;

            model.ComplaintTypeOptions = cTypes
                .Select(m => new SelectListItem() { Value = m.Id.ToString(), Text = m.Name });

            var census = CubeRepository.GetFacilityMonthCensusByQuarter(_Facility.Id, model.Quarter);
            var quarter = DimensionRepository.GetQuarter(model.Quarter);
            var totals = CubeRepository.GetFloorMonthComplaintTypeByQuarter(_Facility.Id, quarter.Id, model.ComplaintType.Value);

            var quarterMonths = DimensionRepository.GetQuarterMonths(model.Quarter);

            model.SetData(quarterMonths, census,
                totals);

            var selectedQuarter = DimensionRepository.GetAllQuarters().Where(x => x.Id == model.Quarter).FirstOrDefault();

            this.ControllerContext.SetContentTitle("{0} - {3} - {1} Q{2} ",
                _Facility.Name, selectedQuarter.Year,
                selectedQuarter.QuarterOfYear,
                model.ComplaintTypeDescription);

            return View(model);
        }

        public ActionResult QuarterlyComplaintTrendReport(QuarterlyComplaintTrendView model)
        {
            if (model.Quarter == Guid.Empty)
            {
                return RedirectToAction("QuarterlyComplaintTrendReport", new { Quarter = this.ControllerContext.GetDefaultQuarterId(DimensionRepository) });
            }
            else
            {
                this.ControllerContext.SetDefaultQuarterId(model.Quarter);
            }

            var quarters = DimensionRepository.GetNonFutureQuarters();

            model.QuarterOptions = quarters
                    .Select(m => new SelectListItem() { Value = m.Id.ToString(), Text = string.Concat("Quarter ", m.QuarterOfYear, " - ", m.Year) });

            var selectedQuarter = DimensionRepository.GetAllQuarters().Where(x => x.Id == model.Quarter).FirstOrDefault();


            var selectedQuarters = quarters
                .Where(x =>
                    (x.Year == selectedQuarter.Year && x.QuarterOfYear <= selectedQuarter.QuarterOfYear)
                || (x.Year == (selectedQuarter.Year - 1) && x.QuarterOfYear >= selectedQuarter.QuarterOfYear));

            var selectedMonths = new List<Month>();

            foreach (var q in selectedQuarters)
            {
                var m = DimensionRepository.GetQuarterMonths(q.Id);
                selectedMonths.Add(m.Month1);
                selectedMonths.Add(m.Month2);
                selectedMonths.Add(m.Month3);
            }

            selectedMonths = selectedMonths.OrderBy(x => x.Year).ThenBy(x => x.MonthOfYear).ToList();

            IEnumerable<FacilityMonthComplaintType.Entry> totals = new List<FacilityMonthComplaintType.Entry>();

            foreach (var quarter in selectedQuarters)
            {
                totals = totals.Append(CubeRepository.GetFacilityMonthComplaintTypeByQuarter(_Facility.Id, quarter.Id).ToArray());
            }

            model.Months = selectedMonths;

            model.SetData(selectedQuarter, totals, DimensionRepository.GetComplaintTypes());

            this.ControllerContext.SetContentTitle("{0} - Complaint Trends - {1} Q{2} ",
                _Facility.Name, selectedQuarter.Year,
                selectedQuarter.QuarterOfYear);

            return View(model);
        }

        public ActionResult QuarterlyCatheterControlReport(QuarterlyCatheterControlView model)
        {
            if (model.Quarter == Guid.Empty)
            {
                return RedirectToAction("QuarterlyCatheterControlReport", new { Quarter = this.ControllerContext.GetDefaultQuarterId(DimensionRepository) });
            }
            else
            {
                this.ControllerContext.SetDefaultQuarterId(model.Quarter);
            }

            model.QuarterOptions = DimensionRepository.GetNonFutureQuarters()
                    .Select(m => new SelectListItem() { Value = m.Id.ToString(), Text = string.Concat("Quarter ", m.QuarterOfYear, " - ", m.Year) });

            var census = CubeRepository.GetFacilityMonthCensusByQuarter(_Facility.Id, model.Quarter);
            var quarter = DimensionRepository.GetQuarter(model.Quarter);
            var totals = CubeRepository.GetFacilityMonthCatheterByQuarter(_Facility.Id, quarter.Id);

            var quarterMonths = DimensionRepository.GetQuarterMonths(model.Quarter);

            model.SetData(quarterMonths, census,
                totals);

            var selectedQuarter = DimensionRepository.GetAllQuarters().Where(x => x.Id == model.Quarter).FirstOrDefault();

            this.ControllerContext.SetContentTitle("{0} - Catheter Control - {1} Q{2} ",
                _Facility.Name, selectedQuarter.Year,
                selectedQuarter.QuarterOfYear);

            return View(model);
        }

        public ActionResult QuarterlyWoundByWingReport(QuarterlyWoundByWingView model)
        {
            if (model.Quarter == Guid.Empty)
            {
                return RedirectToAction("QuarterlyWoundByWingReport", new { Quarter = this.ControllerContext.GetDefaultQuarterId(DimensionRepository) });
            }
            else
            {
                this.ControllerContext.SetDefaultQuarterId(model.Quarter);
            }

            model.QuarterOptions = DimensionRepository.GetNonFutureQuarters()
                    .Select(m => new SelectListItem() { Value = m.Id.ToString(), Text = string.Concat("Quarter ", m.QuarterOfYear, " - ", m.Year) });

            var cTypes = DimensionRepository.GetWoundTypes();

            if (model.WoundType.HasValue == false)
            {
                model.WoundType = cTypes.First().Id;
            }

            model.WoundTypeDescription = cTypes.Where(x => x.Id == model.WoundType).First().Name;

            model.WoundTypeOptions = cTypes
                .Select(m => new SelectListItem() { Value = m.Id.ToString(), Text = m.Name });

            var census = CubeRepository.GetFacilityMonthCensusByQuarter(_Facility.Id, model.Quarter);
            var quarter = DimensionRepository.GetQuarter(model.Quarter);
            var totals = CubeRepository.GetWingMonthWoundTypeByQuarter(_Facility.Id, quarter.Id, model.WoundType.Value);

            var quarterMonths = DimensionRepository.GetQuarterMonths(model.Quarter);

            model.SetData(quarterMonths, census,
                totals);

            var selectedQuarter = DimensionRepository.GetAllQuarters().Where(x => x.Id == model.Quarter).FirstOrDefault();

            this.ControllerContext.SetContentTitle("{0} - {3} - {1} Q{2} ",
                _Facility.Name, selectedQuarter.Year,
                selectedQuarter.QuarterOfYear,
                model.WoundTypeDescription);

            return View(model);
        }

        public ActionResult QuarterlySCAUTIReport(QuarterlySCAUTIView model)
        {
            if (model.Quarter == Guid.Empty)
            {
                return RedirectToAction("QuarterlySCAUTIReport", new { Quarter = this.ControllerContext.GetDefaultQuarterId(DimensionRepository) });
            }
            else
            {
                this.ControllerContext.SetDefaultQuarterId(model.Quarter);
            }

            model.QuarterOptions = DimensionRepository.GetNonFutureQuarters()
                    .Select(m => new SelectListItem() { Value = m.Id.ToString(), Text = string.Concat("Quarter ", m.QuarterOfYear, " - ", m.Year) });

            var quarter = DimensionRepository.GetQuarter(model.Quarter);
            var totals = CubeRepository.GetFacilityMonthSCAUTIByQuarter(_Facility.Id, quarter.Id);

            var quarterMonths = DimensionRepository.GetQuarterMonths(model.Quarter);

            model.SetData(quarterMonths, totals);

            var selectedQuarter = DimensionRepository.GetAllQuarters().Where(x => x.Id == model.Quarter).FirstOrDefault();

            this.ControllerContext.SetContentTitle("{0} - SCAUTI - {1} Q{2} ",
                _Facility.Name, selectedQuarter.Year,
                selectedQuarter.QuarterOfYear);

            return View(model);
        }

    }
}
