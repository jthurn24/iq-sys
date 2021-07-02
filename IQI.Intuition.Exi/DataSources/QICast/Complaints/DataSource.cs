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

namespace IQI.Intuition.Exi.DataSources.QICast.Complaints
{
    public class DataSource : IDataSource<Models.QICast.ComplaintGraph>
    {


        private IContainer _Container;
        private ISystemRepository _SystemRepository;
        private IFacilityRepository _FacilityRepository;
        private IComplaintRepository _ComplaintRepository;

        public const string FACILITY_GUID_KEY = "FACILITY_GUID_KEY";
        public const string MONTHS_KEY = "COMPLAINT_MONTHS_KEY";

        public DataSource(ISystemRepository systemRepository,
            IContainer container,
            IFacilityRepository facilityRepository,
            IComplaintRepository complaintRepository)
        {
            _SystemRepository = systemRepository;
            _Container = container;
            _ComplaintRepository = complaintRepository;
            _FacilityRepository = facilityRepository;
        }

        public IDataSourceResult<Models.QICast.ComplaintGraph> GetResult(IDictionary<string, string> criteria)
        {
            var resultWrapper = new SnyderIS.sCore.Exi.Implementation.DataSource.DataSourceResult<Models.QICast.ComplaintGraph>();
            var result = new Models.QICast.ComplaintGraph();
            result.Chart = new Reporting.Graphics.ColumnChart();

            var facilityId = new Guid(criteria[FACILITY_GUID_KEY]);

            var facility = _FacilityRepository.Get(facilityId);

            var months = Convert.ToInt32(criteria[MONTHS_KEY]);

            var complaints = _ComplaintRepository.FindForLineListing(facility, null, DateTime.Today.AddMonths(0 - months), DateTime.Today, null, null);

            int colorIndex = 0;

            foreach (var key in complaints.Select(x => x.ComplaintType.Name).Distinct())
            {
                var total = complaints.Where(x => x.ComplaintType.Name == key).Count();
                result.Chart.AddItem(new Reporting.Graphics.ColumnChart.Item()
                {
                     Color = Reporting.Graphics.PieChart.GetDefaultColor(colorIndex),
                     Category = key,
                     Value = ((double)total / (double)complaints.Count()) * (double)100
                });

                colorIndex++;
            }


            resultWrapper.Metrics = new List<Models.QICast.ComplaintGraph>() { result };
            return resultWrapper;
        }


        public string DefaultTitle(IDictionary<string, string> criteria)
        {
            return string.Concat("Complaint Graph");
        }

        IDataSourceResult IDataSource.GetResult(IDictionary<string, string> criteria)
        {
            return GetResult(criteria);
        }


    }
}
