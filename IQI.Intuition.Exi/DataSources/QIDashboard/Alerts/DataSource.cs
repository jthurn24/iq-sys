using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SnyderIS.sCore.Exi.Interfaces.DataSource;
using SnyderIS.sCore.Exi.Implementation.DataSource;
using IQI.Intuition.Domain.Models;
using IQI.Intuition.Domain.Repositories;
using StructureMap;
using StructureMap.Query;
using IQI.Intuition.Reporting.Repositories;
using IQI.Intuition.Infrastructure.Services;
using IQI.Intuition.Exi.Models.QIDashboard.Alerts;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Extensions.Formatting;

namespace IQI.Intuition.Exi.DataSources.QIDashboard.Alerts
{
    public class DataSource : IDataSource<AlertView>
    {
        private IActionContext _Context;
        private IWarningRepository _WarningRepository;

        public DataSource(IActionContext context,
            IWarningRepository warningRepository)
        {
            _Context = context;
            _WarningRepository = warningRepository;
        }

        public IDataSourceResult<AlertView> GetResult(IDictionary<string, string> criteria)
        {
            var warnings = _WarningRepository.SearchFacility(_Context.CurrentFacility.Id, null, null, null, x => x.TriggeredOn, true, 1, 20).PageValues;

            warnings = warnings.Where(x => x.IsHiddenBy(_Context.CurrentUser.Id) == false);

            var results = new List<AlertView>();

            foreach (var warning in warnings)
            {
                var a = new AlertView();
                a.TriggeredOn = warning.TriggeredOn.FormatAsShortDate();
                a.Target = System.Enum.GetName(typeof(WarningTarget), warning.GetTarget());
                a.PatientName = warning.Patient != null ? warning.Patient.FullName : string.Empty;
                a.Recent = DateTime.Today.Subtract(warning.TriggeredOn).Days < 7;
                a.DescriptionText = warning.DescriptionText;
                a.Title = warning.Title;
                a.Id = warning.Id;
                results.Add(a);
            }

            var r = new DataSourceResult<AlertView>();
            r.Metrics = results;
            return r;
        }

        public string DefaultTitle(IDictionary<string, string> criteria)
        {
            return string.Concat("Recent Warnings");
        }

        IDataSourceResult IDataSource.GetResult(IDictionary<string, string> criteria)
        {
            return GetResult(criteria);
        }
    }
}
