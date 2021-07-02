using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SnyderIS.sCore.Exi.Interfaces.DataSource;
using SnyderIS.sCore.Exi.Implementation.DataSource;
using IQI.Intuition.Exi.Models.QICast;
using IQI.Intuition.Domain.Repositories;
using StructureMap;
using StructureMap.Query;
using System.Web.Mvc;

namespace IQI.Intuition.Exi.DataSources.QICast
{
    public class WarningDataSource : IDataSource<Warning>
    {

        private IWarningRepository _WarningRepository { get; set; }
        private IFacilityRepository _FacilityRepository { get; set; }

        public const string FACILITY_GUID_KEY = "FACILITY_GUID_KEY";

        public WarningDataSource(IWarningRepository warningRepository,
        IFacilityRepository facilityRepository)
        {
            _WarningRepository = warningRepository;
            _FacilityRepository = facilityRepository;
        }

        public IDataSourceResult<Warning> GetResult(IDictionary<string, string> criteria)
        {
            var result = new DataSourceResult<Warning>();
            var metrics = new List<Warning>();

            var facility = _FacilityRepository.Get(new Guid(criteria[FACILITY_GUID_KEY]));
            var warnings = _WarningRepository
                .SearchFacility(facility.Id, null, null, null, x => x.TriggeredOn, true, 1, 10)
                .PageValues
                .Where(x => x.Patient == null);

            foreach (var w in warnings)
            {
                metrics.Add(new Warning()
                {
                     Title = w.Title,
                     ProductType = Domain.Enumerations.KnownProductType.InfectionTracking
                });
            }

            result.Metrics = metrics;
            return result;
        }

        public string DefaultTitle(IDictionary<string, string> criteria)
        {
            return string.Concat("Warnings");
        }

        IDataSourceResult IDataSource.GetResult(IDictionary<string, string> criteria)
        {
            return GetResult(criteria);
        }
    }
}
