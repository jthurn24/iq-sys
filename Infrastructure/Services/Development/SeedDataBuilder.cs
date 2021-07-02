using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using RedArrow.Framework.Extensions.Common;
using IQI.Intuition.Domain.Repositories;
using IQI.Intuition.Domain.Models;
using RedArrow.Framework.Persistence;
using IQI.Intuition.Reporting.Repositories;
using Dimensions = IQI.Intuition.Reporting.Models.Dimensions;
using Cubes = IQI.Intuition.Reporting.Models.Cubes;
using SnyderIS.sCore.Console;
using SnyderIS.sCore.Persistence;

namespace IQI.Intuition.Infrastructure.Services.Development
{
    public class SeedDataBuilder :IConsoleService
    {
        private IDataContext DataContext;
        private IDimensionBuilderRepository DimensionBuilderRepository;
        private ICubeBuilderRepository CubeBuilderRepository;
        private IAccountRepository AccountRepository;
        private IFacilityRepository FacilityRepository;
        private IInfectionRepository InfectionRepository;
        private IPatientRepository PatientRepository;
        private IUnitOfWork UnitOfWork;

        public SeedDataBuilder(
            IDataContext dataContext,
            IAccountRepository accountRepository,
            IDimensionBuilderRepository dimensionBuilderRepository,
            ICubeBuilderRepository cubeBuilderRepository,
            IFacilityRepository facilityRepository,
            IPatientRepository patientRepository,
            IInfectionRepository infectionRepository,
            IUnitOfWork unitOfWork)
        {
            DataContext = dataContext;
            AccountRepository = accountRepository;
            DimensionBuilderRepository = dimensionBuilderRepository;
            CubeBuilderRepository = cubeBuilderRepository;
            FacilityRepository = facilityRepository;
            PatientRepository = patientRepository;
            InfectionRepository = infectionRepository;
            UnitOfWork = unitOfWork;
        }


        private Room RandomRoom(Facility facility)
        {
            return facility.Floors.Shuffle().Last()
                .Wings.Shuffle().First()
                .Rooms.Shuffle().Last();
        }

        private Patient RandomPatient()
        {
            return DataContext.CreateQuery<Patient>()
                .FilterBy(x => x.CurrentStatus == Domain.Enumerations.PatientStatus.Admitted)
                .FetchAll()
                .Shuffle()
                .Last();
        }

        public void Run(string[] args)
        {

            DateTime startDate = DateTime.Today.AddMonths(-3);
            DateTime curentDate = startDate;

            while (curentDate <= DateTime.Today)
            {
                
                var incidentTime = new DateTime(curentDate.Year, curentDate.Month, curentDate.Day,
                    (int)new Random().Next(24),
                    (int)new Random().Next(60),
                    0);

                var patient = DataContext.CreateQuery<Patient>()
                .FilterBy(x => x.CurrentStatus == Domain.Enumerations.PatientStatus.Admitted)
                .FetchAll()
                .Shuffle()
                .Last();

                var report = new IncidentReport(patient);
                report.Room = patient.Room;

                var injury = DataContext.CreateQuery<IncidentInjury>().FetchAll().Shuffle().First();
                var type = DataContext.CreateQuery<IncidentType>().FetchAll().Where(x => x.Name.Contains("Fall")).Shuffle().First();

                report.AssignTypes(new IncidentType[] { type });
                report.AssignInjuries(new IncidentInjury[] { injury });

                DataContext.TrackChanges(report);

                curentDate = curentDate.AddDays(2);
            }
        }
    }
}
