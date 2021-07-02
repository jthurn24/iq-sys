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
using IQI.Intuition.Web.Models.Reporting.Infection;
using IQI.Intuition.Web.Models.Reporting.Infection.Account;
using IQI.Intuition.Web.Extensions;
using RedArrow.Framework.Extensions.Formatting;

namespace IQI.Intuition.Web.Areas.Reporting.Controllers
{
    [SupportsTokenAuthentication]
    public class AccountController : Controller
    {
        private Account _Account;
        private IEnumerable<Facility> _Facilities;

        public AccountController(
            IActionContext actionContext, 
            IModelMapper modelMapper,
            ICubeRepository cubeRepository,
            IDimensionRepository dimensionRepository)
        {
            ActionContext = actionContext.ThrowIfNullArgument("actionContext");
            ModelMapper = modelMapper.ThrowIfNullArgument("modelMapper");
            CubeRepository = cubeRepository;
            DimensionRepository = dimensionRepository;

            _Account = DimensionRepository.GetAccount(actionContext.CurrentAccount.Guid);

            if (ActionContext.CurrentUser == null || ActionContext.CurrentUser.SystemUser)
            {
                _Facilities = DimensionRepository.GetFacilities(actionContext.CurrentAccount.Facilities.Select(x => x.Guid))
                    .ToList();
            }
            else
            {
                _Facilities = DimensionRepository.GetFacilities(actionContext.CurrentUser.Facilities.Select(x => x.Guid))
                    .ToList();
            }

        }

        protected virtual IActionContext ActionContext { get; private set; }
        protected virtual IModelMapper ModelMapper { get; private set; }
        protected virtual ICubeRepository CubeRepository { get; private set; }
        protected virtual IDimensionRepository DimensionRepository { get; private set; }

        public ActionResult Home()
        {
            if (ActionContext.CurrentFacility.HasProduct(Domain.Enumerations.KnownProductType.InfectionTracking))
            {
                return RedirectToRoute(new { controller = "Account", area = "Reporting", action = "QuarterlyInfectionControlReport" });
            }

            /* TODO -- We need to handel this better. Eventually we may have a client who doesn;t have infection control module */

            return RedirectToRoute(new { controller = "Facility", area = "Reporting", action = "Home" });
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
                    .Select(m => new SelectListItem() { Value = m.Id.ToString(), Text = string.Concat("Quarter ", m.QuarterOfYear, " - ", m.Year) });

            model.FacilityOptions = _Facilities.Select(x => new SelectListItem() { Value = x.Id.ToString(), Text = x.Name });

            if (model.SelectedFacilities == null || model.SelectedFacilities.Count() < 1)
            {
                model.SelectedFacilities = _Facilities.Select(x => x.Id).ToList();
            }

            /* Load data for the selected quarter and the prior month (so that change rates can be calculated properly) */
            
            var quarter = DimensionRepository.GetQuarter(model.Quarter);
            var quarterMonths = DimensionRepository.GetQuarterMonths(model.Quarter);

            var priorMonth = DimensionRepository.GetMonth(new DateTime(quarterMonths.Month1.Year, quarterMonths.Month1.MonthOfYear, 1).AddMonths(-1));

            var census = CubeRepository.GetFacilityMonthCensusByQuarter(model.SelectedFacilities, model.Quarter);
            var totals = CubeRepository.GetFacilityMonthInfectionTypeByQuarter(model.SelectedFacilities, model.Quarter);

            if(priorMonth != null)
            {
                census = CubeRepository.GetFacilityMonthCensus(model.SelectedFacilities, priorMonth.Id).Concat(census);
                totals = CubeRepository.GetFacilityMonthInfectionType(model.SelectedFacilities, priorMonth.Id).Concat(totals);
            }

            model.SetData(quarterMonths, census, totals);

            var selectedQuarter = DimensionRepository.GetAllQuarters().Where(x => x.Id == model.Quarter).FirstOrDefault();

            this.ControllerContext.SetContentTitle("{0} - Infection Control - Group Q{1} ",
                selectedQuarter.Year,
                selectedQuarter.QuarterOfYear);

            return View(model);
        }

        public ActionResult QuarterlyInfectionByFacilityReport(QuarterlyInfectionByFacilityView model)
        {
            if (model.Quarter == Guid.Empty)
            {
                return RedirectToAction("QuarterlyInfectionByFacilityReport", new { Quarter = this.ControllerContext.GetDefaultQuarterId(DimensionRepository) });
            }
            else
            {
                this.ControllerContext.SetDefaultQuarterId(model.Quarter);
            }

            model.QuarterOptions = DimensionRepository.GetNonFutureQuarters()
                    .Select(m => new SelectListItem() { Value = m.Id.ToString(), Text = string.Concat("Quarter ", m.QuarterOfYear, " - ", m.Year) });

            model.FacilityOptions = _Facilities.Select(x => new SelectListItem() { Value = x.Id.ToString(), Text = x.Name });

            if (model.SelectedFacilities == null || model.SelectedFacilities.Count() < 1)
            {
                model.SelectedFacilities = _Facilities.Select(x => x.Id).ToList();
            }


            var quarter = DimensionRepository.GetQuarter(model.Quarter);
            var quarterMonths = DimensionRepository.GetQuarterMonths(model.Quarter);

            var totals = CubeRepository.GetRootFacilityMonthInfectionTypeByQuarter(model.SelectedFacilities, model.Quarter);

            model.SetData(quarterMonths, totals);

            var selectedQuarter = DimensionRepository.GetAllQuarters().Where(x => x.Id == model.Quarter).FirstOrDefault();

            string typeDescription = String.Format("(by {0})",System.Enum.GetName(typeof(Domain.Enumerations.InfectionMetric),model.Metric).SplitPascalCase());

            model.MetricOptions = new List<SelectListItem>() 
            {
                new SelectListItem() { Text = 
                    System.Enum.GetName(typeof(Domain.Enumerations.InfectionMetric),Domain.Enumerations.InfectionMetric.NosocomialTotal).SplitPascalCase(),
                    Value = ((int)Domain.Enumerations.InfectionMetric.NosocomialTotal).ToString()} ,

                new SelectListItem() { Text = 
                    System.Enum.GetName(typeof(Domain.Enumerations.InfectionMetric),Domain.Enumerations.InfectionMetric.Rate).SplitPascalCase(),
                    Value = ((int)Domain.Enumerations.InfectionMetric.Rate).ToString()}
            };

            this.ControllerContext.SetContentTitle("{0} - Infection By Facility - Group Q{2} {1}",
                selectedQuarter.Year,
                typeDescription,
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

            var averageTypes = DimensionRepository.GetFacilityAverageTypesForAccount(_Account.Id);
            model.AverageTypeOptions = averageTypes.Select(m => new SelectListItem() { Value = m.AverageType.Id.ToString(), Text = m.AverageType.Name });

            if (model.AverageType == Guid.Empty)
            {
                model.AverageType = averageTypes.First().AverageType.Id;
            }

            model.FacilityOptions = _Facilities.Select(x => new SelectListItem() { Value = x.Id.ToString(), Text = x.Name });

            if (model.SelectedFacilities == null || model.SelectedFacilities.Count() < 1)
            {
                model.SelectedFacilities = _Facilities.Select(x => x.Id).ToList();
            }

            /* Load data for the selected quarter and the prior month (so that change rates can be calculated properly) */

            var quarter = DimensionRepository.GetQuarter(model.Quarter);
            var quarterMonths = DimensionRepository.GetQuarterMonths(model.Quarter);

            var priorMonth = DimensionRepository.GetMonth(new DateTime(quarterMonths.Month1.Year, quarterMonths.Month1.MonthOfYear, 1).AddMonths(-1));

            var census = CubeRepository.GetFacilityMonthCensusByQuarter(model.SelectedFacilities, model.Quarter);
            var totals = CubeRepository.GetFacilityMonthInfectionTypeByQuarter(model.SelectedFacilities, model.Quarter);

            if (priorMonth != null)
            {
                census = CubeRepository.GetFacilityMonthCensus(model.SelectedFacilities, priorMonth.Id).Concat(census);
                totals = CubeRepository.GetFacilityMonthInfectionType(model.SelectedFacilities, priorMonth.Id).Concat(totals);
            }

            var averageTotals = CubeRepository.GetAverageTypeMonthInfectionTypeByQuarter(model.AverageType, quarter.Id);


            var selectedQuarter = DimensionRepository.GetQuarter(model.Quarter);
            var selectedAverage = DimensionRepository.GetAverageType(model.AverageType);

            this.ControllerContext.SetContentTitle("Infection Averages ({2}) - {0} Q{1} ",
                selectedQuarter.Year,
                selectedQuarter.QuarterOfYear,
                selectedAverage.Name);

            model.SetData(quarterMonths, totals, averageTotals, census);

            return View(model);
        }
    }
}
