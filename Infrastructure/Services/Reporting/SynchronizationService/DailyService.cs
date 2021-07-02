using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQI.Intuition.Reporting.Repositories;
using IQI.Intuition.Domain.Repositories;
using RedArrow.Framework.Persistence;
using RedArrow.Framework.Persistence.NHibernate;
using RedArrow.Framework.Logging;
using RedArrow.Framework.Extensions.Common;
using SnyderIS.sCore.Persistence;
using Dimensions = IQI.Intuition.Reporting.Models.Dimensions;
using IQI.Intuition.Infrastructure.Services.Reporting.SynchronizationService.Infection;
using IQI.Intuition.Reporting.Containers;
using RedArrow.Framework.Utilities;
using IQI.Intuition.Reporting.Models.User;

namespace IQI.Intuition.Infrastructure.Services.Reporting.SynchronizationService
{
    public class DailyService
    {
        private IDimensionBuilderRepository _DimensionBuilderRepository;
        private IDimensionRepository _DimensionRepository;
        private ICubeBuilderRepository _CubeBuilderRepository;
        private IStatelessDataContext _DataContext;
        private IFactBuilderRepository _FactBuilderRespository;
        private ILog _Log;
        private ISessionActivator _SessionActivator;
        private IDocumentStore _Store;

        public DailyService(
                IDimensionBuilderRepository dimensionBuilderRepository,
                IDimensionRepository dimensionRepository,
                ICubeBuilderRepository cubeBuilderRepository,
                IStatelessDataContext statelessDataContext,
                IStatelessDataContext dataContext,
                IUnitOfWork unitOfWork,
                IFactBuilderRepository factBuilderRespository,
                ILog log,
                ISessionActivator sessionActivator,
                IDocumentStore store
            )
        {
            _DimensionBuilderRepository = dimensionBuilderRepository;
            _CubeBuilderRepository = cubeBuilderRepository;
            _FactBuilderRespository = factBuilderRespository;
            _DimensionRepository = dimensionRepository;
            _DataContext = dataContext;
            _Log = log;
            _SessionActivator = sessionActivator;
            _Store = store;
        }

        protected void AddCubeSyncJob<T>(int priority,
        DataDimensions data,
        DateTime startScanDate,
        int facilityId) where T : AbstractService
        {
            var job = new CubeSyncJob();
            job.Id = GuidHelper.NewGuid();
            job.Dimensions = data;
            job.CreatedOn = DateTime.Now;
            job.Priority = priority;
            job.ServiceTypeName = typeof(T).AssemblyQualifiedName;
            job.ScanStartDate = startScanDate;
            job.FacilityId = facilityId;

            _Store.Insert(job);

            _Log.Info("Queued Cubesync Job {0} Pri: {1}", job.Id, job.Priority);
        }

        public void Run(string[] args)
        {

            foreach (var facility in _DataContext.CreateQuery<Domain.Models.Facility>()
                .FetchAll()
                .OrderBy(x => x.LastSynchronizedAt))
            {
                try
                {
                    Process(facility);
                }
                catch (Exception ex)
                {
                    _Log.Error(ex);
                }
            }
        }

        private void Process(Domain.Models.Facility facility)
        {




            var dimensions = new DataDimensions();
            dimensions.Facility = _DimensionBuilderRepository.GetOrCreateFacility(facility.Guid);

            dimensions.StartDate = DateTime.Today;
            dimensions.EndDate = DateTime.Today.AddDays(1).AddMinutes(-1);

            dimensions.InfectionClassifications.AddRange(
                _DimensionRepository.GetInfectionClassifications().ToList());

            dimensions.InfectionTypes.AddRange(
                _DimensionRepository.GetInfectionTypes().ToList());

            dimensions.InfectionSites.AddRange(
                _DimensionRepository.GetInfectionSites().ToList());

            dimensions.IncidentInjuries.AddRange(
                _DimensionRepository.GetIncidentInjuries().ToList());

            dimensions.IncidentInjuryLevels.AddRange(
                _DimensionRepository.GetIncidentInjuryLevels().ToList());

            dimensions.IncidentLocations.AddRange(
                _DimensionRepository.GetIncidentLocations().ToList());

            dimensions.IncidentTypeGroups.AddRange(
                _DimensionRepository.GetIncidentTypeGroups().ToList());

            dimensions.IncidentTypes.AddRange(
                _DimensionRepository.GetIncidentTypes().ToList());



            /* Sync infection floormaps */

           AddCubeSyncJob<Infection.CubeServices.FloorMapRoomInfectionType>(10, dimensions, DateTime.Today, facility.Id);


            /* For psychotropic meds we need to reprocess facts, since the current month needs to be updated */

            var psychService = new Psychotropic.FactServices.PsychotropicAdministration(
                _DimensionBuilderRepository,
                _DimensionRepository,
                _CubeBuilderRepository,
                _DataContext,
                _FactBuilderRespository,
                _Log,
                _Store);


            var admins = _DataContext.CreateQuery<Domain.Models.PsychotropicAdministration>()
                .FilterBy(x => x.Active == true && x.Patient.Room.Wing.Floor.Facility.Id == facility.Id)
                .FetchAll()
                .ToList();

            psychService.Run(admins, facility, dimensions);

            AddCubeSyncJob<Psychotropic.CubeServices.FacilityMonthPsychotropicDrugType>(10, dimensions, DateTime.Today, facility.Id);


            /* For catheter we just need to refresh current month to update device days */

            AddCubeSyncJob<Catheter.CubeServices.FacilityMonthCatheter>(10, dimensions, DateTime.Today, facility.Id);

            /* For SCAUTI need to refresh because its based on catheter days */

            AddCubeSyncJob<SymptomaticCUATI.CubeServices.FacilityMonthSCAUTI>(10, dimensions, DateTime.Today, facility.Id);


            _Log.Info("Finished setting up daily cube syncs for {0}", facility.Name);

        }



    }


}
