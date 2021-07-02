using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQI.Intuition.Domain.Repositories;
using IQI.Intuition.Domain.Models;
using Dimensions = IQI.Intuition.Reporting.Models.Dimensions;
using Cubes = IQI.Intuition.Reporting.Models.Cubes;
using Facts = IQI.Intuition.Reporting.Models.Facts;
using IQI.Intuition.Reporting.Repositories;
using RedArrow.Framework.Persistence;
using RedArrow.Framework.Logging;
using SnyderIS.sCore.Persistence;

namespace IQI.Intuition.Infrastructure.Services.Reporting.IntegrityService.Incident.CubeServices
{
    public class FacilityMonthIncidentHourOfDay : IScanService
    {
        private IDimensionBuilderRepository _DimensionBuilderRepository;
        private ICubeBuilderRepository _CubeBuilderRepository;
        private IStatelessDataContext _DataContext;
        private IFactBuilderRepository _FactBuilderRespository;
        private ConsoleLogWrapper _Log;
        private IDocumentStore _Store;

        public FacilityMonthIncidentHourOfDay(
                IDimensionBuilderRepository dimensionBuilderRepository,
                ICubeBuilderRepository cubeBuilderRepository,
                IStatelessDataContext dataContext,
                IFactBuilderRepository factBuilderRespository,
                ConsoleLogWrapper log,
                IDocumentStore store
            )
        {
            _DimensionBuilderRepository = dimensionBuilderRepository;
            _CubeBuilderRepository = cubeBuilderRepository;
            _FactBuilderRespository = factBuilderRespository;
            _DataContext = dataContext;
            _Log = log;
            _Store = store;
        }

        public void Run(
            Domain.Models.Facility dFacility, 
            IQI.Intuition.Reporting.Models.Dimensions.Facility rFacility,
            IList<VarianceDetails> variances,
            int scanDays)
        {
            var scanDate = DateTime.Today.AddDays(0 - scanDays);

            var cubes = _Store.GetQueryable<Cubes.FacilityMonthIncidentHourOfDay>()
            .Where(x => x.Facility.Id == rFacility.Id)
            .First()
            .Entries
            .Where(x => x.Month.Year >= scanDate.Year)
            .ToList();

            int cubeCounter = 0;

            foreach (var cube in cubes)
            {
                cubeCounter++;

                _Log.SetStatus(string.Format("Inspecting cube {0} of {1}", cubeCounter, cubes.Count()));

                var typeGroup = cube.IncidentTypeGroup;
                var month = cube.Month;
                var startDate = new DateTime(month.Year,month.MonthOfYear,1);
                var endDate = startDate.AddMonths(1);
                var types = _DataContext.CreateQuery<Domain.Models.IncidentType>()
                .FilterBy(x => x.GroupName == typeGroup.Name).FetchAll();

                var incidents = new List<IncidentReport>();

                foreach (var type in types)
                {
                    incidents.AddRange(_DataContext.CreateQuery<IncidentReport>()
                    .FilterBy(x => x.Patient.Room.Wing.Floor.Facility.Id == dFacility.Id)
                    .FilterBy(x => x.Deleted == null || x.Deleted == false)
                    .FilterBy(x => (x.OccurredOn.HasValue == false && x.DiscoveredOn >= startDate && x.DiscoveredOn < endDate) || (x.OccurredOn >= startDate && x.OccurredOn < endDate))
                    .FilterBy(x => x.Patient.Deleted == null || x.Patient.Deleted == false)
                    .FilterBy(x => x.IncidentTypes.Contains(type))
                    .FetchAll());
                }

                incidents = incidents.Where(x => (x.OccurredOn.HasValue == false && x.DiscoveredOn.Value.Hour == cube.HourOfDay.Value)
                    || (x.OccurredOn.HasValue &&  x.OccurredOn.Value.Hour == cube.HourOfDay.Value)).ToList();

                var count = incidents
                    .Select(x => x.Id).Distinct()
                    .Count();

                if(count != cube.Total)
                {
                    var v = new VarianceDetails();
                    v.SetReportingEntity(cube);
                    v.AddDomainEntities(incidents);
                    v.ReportingValue = cube.Total;
                    v.DomainValue = count;
                    variances.Add(v);
                }
                
                    
            }

            _Log.ClearStatus();
        }
    }
}
