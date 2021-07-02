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
using IQI.Intuition.Exi.Models.QIDashboard.Wounds;


namespace IQI.Intuition.Exi.DataSources.QIDashboard.Wounds
{

    public class WoundStageDataSource :  IDataSource<MonthWoundStageTotal>
    {

        private IActionContext _ActionContext;
        private ICubeRepository _CubeRepository;

        public WoundStageDataSource(IActionContext actionContext,
         ICubeRepository cubeRepository)
        {
            _ActionContext = actionContext;
            _CubeRepository = cubeRepository;
        }

        public IDataSourceResult<MonthWoundStageTotal> GetResult(IDictionary<string, string> criteria)
        {
            var s = DateTime.Today.AddMonths(-1);
            var endDate = new DateTime(s.Year, s.Month, 1);
            var startDate = endDate.AddMonths(-24);

            var data = _CubeRepository.GetFacilityMonthWoundStageByRange(_ActionContext.CurrentFacility.Guid,
                startDate.Year, endDate.Year, startDate.Month, endDate.Month)
                .ToList();

            var r = new DataSourceResult<MonthWoundStageTotal>();

            r.Metrics = data.Select(x => new MonthWoundStageTotal()
            {
                Change = x.Change,
                Stage = x.Stage,
                Month = x.Month,
                Rate = x.Rate,
                Total = x.Total,
                CensusPatientDays = x.CensusPatientDays,
                Components = x.Components,
                ViewAction = x.ViewAction
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
