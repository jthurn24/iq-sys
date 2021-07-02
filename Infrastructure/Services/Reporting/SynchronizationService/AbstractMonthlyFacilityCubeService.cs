using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dimensions = IQI.Intuition.Reporting.Models.Dimensions;
using Cubes = IQI.Intuition.Reporting.Models.Cubes;
using IQI.Intuition.Reporting.Containers;

namespace IQI.Intuition.Infrastructure.Services.Reporting.SynchronizationService
{
    public abstract class AbstractMonthlyFacilityCubeService : AbstractService 
    {
        protected abstract void Init(DataDimensions changes);

        protected abstract void ProcessDay(
            DataDimensions changes,
            DateTime currentDate,
            DateTime priorDate,
            Dimensions.Month currentMonth,
            Dimensions.Month priorMonth,
            Cubes.FacilityMonthCensus currentMonthCensus,
            Cubes.FacilityMonthCensus priorMonthCensus,
            int currentPatientDays,
            int priorPatientDays);

        protected override void Run(DataDimensions changes)
        {
            _Log.Info(string.Format("Syncing cube {0} starting: {1} facility: {2}", this.GetType().Name, changes.StartDate, changes.Facility.Name));

            Init(changes);

            var facilityCensus = GetQueryable<Cubes.FacilityMonthCensus>()
            .Where(x => x.Facility.Id == changes.Facility.Id)
            .ToList();

            var startDate = changes.StartDate.Value;
            var currentDate = new DateTime(startDate.Year, startDate.Month, 1);

            while (currentDate <= DateTime.Today)
            {

                var currentMonth = _DimensionBuilderRepository.GetOrCreateMonth(currentDate.Month, currentDate.Year);

                /* Setup previous month */

                var pMonth = new DateTime(currentMonth.Year, currentMonth.MonthOfYear, 1).AddMonths(-1);

                var priorMonth = _DimensionBuilderRepository
                    .GetOrCreateMonth(
                        pMonth.Month,
                        pMonth.Year);

                /* Locate census data. We pull this form another cube right now. The census cube should probably be a fact table given we are refrenceing it this way */
                var currentMonthCensus = facilityCensus
                    .Where(x => x.Month.MonthOfYear == currentMonth.MonthOfYear && x.Month.Year == currentMonth.Year)
                    .FirstOrDefault();

                int currentPatientDays = 0;

                if (currentMonthCensus != null)
                {
                    currentPatientDays = currentMonthCensus.TotalPatientDays;
                }

                /* Locate cenus data for the previous month */

                var priorMonthCensus = facilityCensus.Where(x => x.Month.MonthOfYear == priorMonth.MonthOfYear
                    && x.Month.Year == priorMonth.Year).FirstOrDefault();

                int priorPatientDays = 0;

                if (priorMonthCensus != null)
                {
                    priorPatientDays = priorMonthCensus.TotalPatientDays;
                }

                ProcessDay(
                    changes,
                    currentDate,
                    currentDate.AddMonths(-1),
                    currentMonth,
                    priorMonth,
                    currentMonthCensus,
                    priorMonthCensus,
                    currentPatientDays,
                    priorPatientDays);
                

                currentDate = currentDate.AddMonths(1);
            }

            Completed();
        }

        protected virtual void Completed()
        {

        }
    }
}
