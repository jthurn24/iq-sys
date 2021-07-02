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
using IQI.Intuition.Reporting.Containers;

namespace IQI.Intuition.Infrastructure.Services.Reporting.SynchronizationService.PatientCensus.CubeServices
{
    public class FacilityMonthCensus 
    {
        private IDimensionBuilderRepository _DimensionBuilderRepository;
        private ICubeBuilderRepository _CubeBuilderRepository;
        private IStatelessDataContext _DataContext;
        private IFactBuilderRepository _FactBuilderRespository;
        private ILog _Log;
        private IDocumentStore _Store;

        public FacilityMonthCensus(
                IDimensionBuilderRepository dimensionBuilderRepository,
                ICubeBuilderRepository cubeBuilderRepository,
                IStatelessDataContext dataContext,
                IFactBuilderRepository factBuilderRespository,
                ILog log ,
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
            Dimensions.Facility facility,
            Dimensions.Month month
            )
        {

            _Log.Info(string.Format("Syncing cube FacilityMonthCensus for: {0} {1} facility: {2}", month.MonthOfYear, month.Year, facility));

            var cube = _CubeBuilderRepository.GetOrCreateCensus(facility, month);
            cube.Account = facility.Account;
            cube.Facility = facility;
            cube.Month = month;

            var domain = _DataContext.CreateQuery<Domain.Models.PatientCensus>()
                .FilterBy(x => x.Facility.Guid == facility.Id)
                .FilterBy(x => x.Month == month.MonthOfYear && x.Year == month.Year)
                .FetchAll().FirstOrDefault();

            if (domain != null)
            {
                cube.TotalDays = new DateTime(domain.Year, domain.Month, 1).AddMonths(1).AddDays(-1).Day;
                cube.TotalPatientDays = domain.PatientDays;

                if (cube.TotalPatientDays > 1)
                {
                    cube.Average = Convert.ToDecimal(cube.TotalPatientDays) / Convert.ToDecimal(cube.TotalDays);
                }
                else
                {
                    cube.Average = 0;
                }

                _Store.Save<Cubes.FacilityMonthCensus>(cube);
            }
        }

    }
}
