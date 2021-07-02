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

namespace IQI.Intuition.Infrastructure.Services.Reporting.IntegrityService.Infection.CubeServices
{
    public class FacilityMonthInfectionSite : IScanService
    {
        private IDimensionBuilderRepository _DimensionBuilderRepository;
        private ICubeBuilderRepository _CubeBuilderRepository;
        private IStatelessDataContext _DataContext;
        private IFactBuilderRepository _FactBuilderRespository;
        private ConsoleLogWrapper _Log;
        private IDocumentStore _Store;

        public FacilityMonthInfectionSite(
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
            Domain.Models.Facility dfacility, 
            IQI.Intuition.Reporting.Models.Dimensions.Facility rFacility,
            IList<VarianceDetails> variances,
            int scanDays)
        {
            var scanDate = DateTime.Today.AddDays(0 - scanDays);

            var root = _Store.GetQueryable<Cubes.FacilityMonthInfectionSite>()
            .Where(x => x.Facility.Id == rFacility.Id)
            .FirstOrDefault();

            var entries = root.Entries.Where(x => x.Month.Year >= scanDate.Year);

            int cubeCounter = 0;

            if(root != null)
            {

                var allInfections = _DataContext.CreateQuery<InfectionVerification>()
                    .FilterBy(x => x.Patient.Room.Wing.Floor.Facility.Id == dfacility.Id)
                    .FilterBy(x => x.Deleted == null || x.Deleted == false)
                    .FilterBy(x => x.Patient.Deleted == null || x.Patient.Deleted == false)
                    .FilterBy(x => x.Classification != InfectionClassification.NoInfection)
                    .FetchAll();

                var allInfectionSites = _DataContext.CreateQuery<InfectionSite>().FetchAll();

                foreach (var cube in entries)
                {
                    cubeCounter++;

                    _Log.SetStatus(string.Format("Inspecting cube {0} of {1}", cubeCounter, entries.Count()));

                    var month = cube.Month;
                    var startDate = new DateTime(month.Year, month.MonthOfYear, 1);
                    var endDate = startDate.AddMonths(1);

                    
                    var iSite = allInfectionSites.Where(x => x.Name == cube.InfectionSite.Name).First();

                    var infections = allInfections.Where(x => x.InfectionSite.Id == iSite.Id)
                                        .Where(x => x.FirstNotedOn >= startDate && x.FirstNotedOn < endDate);

                    if (infections.Count() != (cube.Total + cube.NonNosoTotal))
                    {
                        var v = new VarianceDetails();
                        v.SetReportingEntity(cube);
                        v.AddDomainEntities(infections);
                        v.ReportingValue = cube.Total + cube.NonNosoTotal;
                        v.DomainValue = infections.Count();
                        variances.Add(v);
                    }

                }
          
            }

            _Log.ClearStatus();
        }
    }
}
