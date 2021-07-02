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
using IQI.Intuition.Exi.Models.QIDashboard.Infections;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Extensions.Formatting;

namespace IQI.Intuition.Exi.DataSources.QIDashboard.Infections
{
    public class RealtimeDataSource : IDataSource<InfectionTotal>
    {

        private IActionContext _ActionContext;
        private IInfectionRepository _InfectionRepository;

        public RealtimeDataSource(IActionContext actionContext,
         IInfectionRepository infectionRepository)
        {
            _ActionContext = actionContext;
            _InfectionRepository = infectionRepository;
        }

        public IDataSourceResult<InfectionTotal> GetResult(IDictionary<string, string> criteria)
        {
            var infections = _InfectionRepository
                .FindActiveFacility(_ActionContext.CurrentFacility)
                .Where(x => x.Classification == IQI.Intuition.Domain.Models.InfectionClassification.Admission 
                || x.Classification == IQI.Intuition.Domain.Models.InfectionClassification.HealthCareAssociatedInfection);

            var results = new List<InfectionTotal>();

            foreach (var type in infections.Select(x => x.InfectionSite.Type).Distinct())
            {

                results.Add(new InfectionTotal()
                {
                     Name = type.ShortName,
                     Total =  infections.Where(x => x.InfectionSite.Type == type).Count(),
                     Color = System.Drawing.ColorTranslator.FromHtml(type.Color)
                });

            }

            var r = new DataSourceResult<InfectionTotal>();
            r.Metrics = results;
            return r;
        }

        public string DefaultTitle(IDictionary<string, string> criteria)
        {
            return "Active Infections (Confirmed)";
        }

        IDataSourceResult IDataSource.GetResult(IDictionary<string, string> criteria)
        {
            return GetResult(criteria);
        }
    }
}
