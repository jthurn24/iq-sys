using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQI.Intuition.Exi.DataSources.QICast.Statistics.Interfaces;
using IQI.Intuition.Reporting.Repositories;
using IQI.Intuition.Reporting.Models;

namespace IQI.Intuition.Exi.DataSources.QICast.Statistics
{
    public abstract class BaseReportingProvider : Interfaces.IStatProvider
    {
        protected ICubeRepository CubeRepository { get; private set; }
        protected IDimensionRepository DimensionRepository { get; private set; }


        public BaseReportingProvider(ICubeRepository c,
            IDimensionRepository d)
        {
            CubeRepository = c;
            DimensionRepository = d;
        }

        public abstract Models.QICast.Statistic GetResult(Guid facilityGuid);

        public int GetProjectedPatientDays(Guid facilityId)
        {
            var census = CubeRepository.GetFacilityMonthCensus(facilityId)
                .Where(x => x.TotalPatientDays != 0 && x.TotalDays != 0)
                .Where(x => x.Month.Year >= DateTime.Today.AddYears(-1).Year);

            if (census.Count() < 1)
            {
                return 0;
            }

            var totalDays = census.Sum(x => x.TotalDays);
            var totalPatientDays = census.Sum(x => x.TotalPatientDays);

            var dailyAvg = totalPatientDays / totalDays;

            return DateTime.Today.Day * dailyAvg;
        }


    }
}
