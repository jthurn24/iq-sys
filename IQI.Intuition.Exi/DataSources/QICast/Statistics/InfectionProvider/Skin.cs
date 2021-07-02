using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQI.Intuition.Exi.DataSources.QICast.Statistics.Interfaces;
using IQI.Intuition.Reporting.Repositories;
using IQI.Intuition.Reporting.Models;


namespace IQI.Intuition.Exi.DataSources.QICast.Statistics.InfectionProvider
{
    public class Skin : BaseReportingProvider
    {

        public Skin(ICubeRepository c, IDimensionRepository d)
            : base(c, d) { }

        public override Models.QICast.Statistic GetResult(Guid facilityGuid)
        {
            var facility = DimensionRepository.GetFacility(facilityGuid);
            var priorMonth = DimensionRepository.GetMonth(DateTime.Today.AddMonths(-1));
            var currentMonth = DimensionRepository.GetMonth(DateTime.Today);
            var result = new Models.QICast.Statistic();

            var priorCube = CubeRepository.GetFacilityMonthInfectionType(facility.Id, priorMonth.Id)
                .Where(x => x.InfectionType.ShortName == "Skin, Soft Tissue and Mucosal")
                .FirstOrDefault();

            var currentCube = CubeRepository.GetFacilityMonthInfectionType(facility.Id, currentMonth.Id)
                .Where(x => x.InfectionType.ShortName == "Skin, Soft Tissue and Mucosal")
                .FirstOrDefault();

            var currentTotal = currentCube != null ? currentCube.Total : 0;
            decimal currentRate = 0;
            var currentPatientDays = GetProjectedPatientDays(facility.Id);

            if (currentTotal > 0 &&currentPatientDays > 0)
            {
                currentRate = Domain.Calculations.Rate1000(currentTotal, currentPatientDays);
            }

            var priorRate = priorCube != null ? priorCube.Rate : 0;
            var change = currentRate - priorRate;

            result.Description = string.Concat("Skin/ST/M <br> P:", Math.Round(priorRate, 3), "<br> C:", Math.Round(currentRate, 3));

            if (change < 0)
            {
                result.Badge = Models.QICast.Statistic.BadgeType.Down;
                result.Label = Math.Round(change, 3).ToString();
            }
            else if (change > 0)
            {
                result.Badge = Models.QICast.Statistic.BadgeType.Up;
                result.Label = string.Concat("+", Math.Round(change, 3));
            }
            else
            {
                result.Badge = Models.QICast.Statistic.BadgeType.Normal;
                result.Label = "-";
            }


            result.ProductType = Domain.Enumerations.KnownProductType.InfectionTracking;

            return result;
        }
    }
}
