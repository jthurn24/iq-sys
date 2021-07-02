using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SnyderIS.sCore.Exi.Interfaces.DataSource;
using SnyderIS.sCore.Exi.Implementation.DataSource;
using IQI.Intuition.Domain.Repositories;
using StructureMap;
using StructureMap.Query;
using IQI.Intuition.Reporting.Repositories;
using IQI.Intuition.Infrastructure.Services;
using IQI.Intuition.Exi.Models.QIDashboard.Alerts;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Extensions.Formatting;
using IQI.Intuition.Exi.Models.QIDashboard.Incidents;

namespace IQI.Intuition.Exi.DataSources.QIDashboard.Incidents
{
    public class InjuryDataSource : IDataSource<MonthlyInjuryTotal>
    {

        public const string INCIDENT_TYPE_GROUP = "IncidentTypeGroup";


        private IActionContext _ActionContext;
        private ICubeRepository _CubeRepository;

        public InjuryDataSource(IActionContext actionContext,
         ICubeRepository cubeRepository)
        {
            _ActionContext = actionContext;
            _CubeRepository = cubeRepository;
        }


        public IDataSourceResult<MonthlyInjuryTotal> GetResult(IDictionary<string, string> criteria)
        {
            var s = DateTime.Today.AddMonths(-1);
            var endDate = new DateTime(s.Year,s.Month,1);
            var startDate = endDate.AddMonths(-24);

            var data = _CubeRepository.GetFacilityMonthIncidentInjuryByRange(_ActionContext.CurrentFacility.Guid,
                startDate.Year, endDate.Year, startDate.Month, endDate.Month)
                .ToList();

            var census = _CubeRepository.GetFacilityMonthCensus(_ActionContext.CurrentFacility.Guid);

            if (criteria.ContainsKey(INCIDENT_TYPE_GROUP))
            {
                data = data
                    .Where(x => x.IncidentTypeGroup.Name == criteria[INCIDENT_TYPE_GROUP])
                    .ToList();
            }

            var r = new DataSourceResult<MonthlyInjuryTotal>();
            
            r.Metrics = data.Select(x => new MonthlyInjuryTotal()
            {
                 Change= x.Change,
                 Injury =x.IncidentInjury,
                 Month = x.Month,
                 Rate = x.Rate,
                 Total = x.Total,
                 CensusPatientDays = x.CensusPatientDays
            });

            return r;

        }

        public string DefaultTitle(IDictionary<string, string> criteria)
        {
            return "N/A";
        }

        IDataSourceResult IDataSource.GetResult(IDictionary<string, string> criteria)
        {
            return GetResult(criteria);
        }
    }
}
