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

namespace IQI.Intuition.Infrastructure.Services.Reporting.SynchronizationService.Psychotropic.FactServices
{
    public class PsychotropicAdministration : AbstractFactService
    {
        public PsychotropicAdministration(
            IDimensionBuilderRepository db,
            IDimensionRepository d,
            ICubeBuilderRepository cb,
            IStatelessDataContext dc,
            IFactBuilderRepository fb,
            ILog log,
            IDocumentStore ds) : base(db,d,cb,dc,fb,log,ds)
        {

        }

        public void Run(
            IList<Domain.Models.PsychotropicAdministration> dAdmins,
            Domain.Models.Facility dFacility,
            DataDimensions dimensions)
        {

            var dAccount = _DataContext.Fetch<Domain.Models.Account>(dFacility.Account.Id);

            foreach (var dAdmin in dAdmins)
            {

                var dChanges = _DataContext.CreateQuery<PsychotropicDosageChange>()
                    .FilterBy(x => x.Administration.Id == dAdmin.Id)
                    .FetchAll()
                    .OrderBy(x => x.StartDate);

                var dPrns = _DataContext.CreateQuery<PsychotropicAdministrationPRN>()
                    .FilterBy(x => x.Administration.Id == dAdmin.Id)
                    .FetchAll()
                    .OrderBy(x => x.GivenOn);


                _Log.Info(string.Format("Syncing psyc admin: {0}  facility: {1}", dAdmin.Guid, dFacility.Name));


                var dDrugType = _DataContext.Fetch<PsychotropicDrugType>(dAdmin.DrugType.Id);

                var account = _DimensionBuilderRepository.GetOrCreateAccount(dAccount.Guid, dAccount.Name);
                var facility = _DimensionBuilderRepository.GetOrCreateFacility(dFacility.Guid);
                var drugType = _DimensionBuilderRepository.GetOrCreatePsychotropicDrugType(dDrugType.Name);
                var drugName = _DimensionBuilderRepository.GetOrCreatePsychotropicDrugName(dAdmin.Name);

                var record = _FactBuilderRespository.GetOrCreatePsychotropicAdministration(dAdmin.Guid);

                /* Track initial dimensions */
                TrackDimensionChanges(dimensions,record);


                /* Apply changes */

                record.Account = account;
                record.Facility = facility;
                record.DrugName = drugName;
                record.DrugType = drugType;
                record.Deleted = dAdmin.Deleted;


                /* Get start date */

                if (dChanges.Count() > 0)
                {
                    record.StartDate = dChanges.First().StartDate;

                    var last = dChanges.Last();
                    var freq = _DataContext.Fetch<PsychotropicFrequency>(last.Frequency.Id);

                    if (freq.GetFrequencyDefinition().IndicatesActiveAdministration() == false)
                    {
                        record.EndDate = last.StartDate;
                    }
                    else
                    {
                        record.EndDate = null;
                    }

                }
                else
                {
                    record.StartDate = null;
                    record.EndDate = null;
                }



                /* Track final dimensions */
                TrackDimensionChanges(dimensions, record);
               


                /* calc monthly records */

                if (record.StartDate.HasValue)
                {
                    record.AdministrationMonths = new List<Facts.PsychotropicAdministrationMonth>();

                    var currentDate = record.StartDate.Value;
                    var endDate = record.EndDate.HasValue ? record.EndDate.Value : DateTime.Today;
                    var calcService = new Infrastructure.Services.BusinessLogic.Psychotropic.AdministrationCalculator();

                    var currentMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
                    var endMonth = new DateTime(endDate.Year, endDate.Month, 1);

                    var frequencies = _DataContext.CreateQuery<PsychotropicFrequency>().FetchAll();

                    while (currentMonth <= endMonth)
                    {
                        var days = currentMonth.AddMonths(1).AddDays(-1).Day;

                        if (currentMonth.AddMonths(1) > DateTime.Today)
                        {
                            days = DateTime.Today.Day;
                        }

                        if (currentMonth.Year == record.StartDate.Value.Year && currentMonth.Month == record.StartDate.Value.Month)
                        {
                            days = days - (record.StartDate.Value.Day -1);

                            if (days < 0)
                            {
                                days = 0;
                            }
                       
                        }

                        var totalDosage = calcService.Calculate(currentMonth,
                            currentMonth.AddMonths(1).AddDays(-1),
                            dChanges,
                            dPrns,
                            frequencies);

                        var monthRecord = new Facts.PsychotropicAdministrationMonth();
                        monthRecord.Month = _DimensionBuilderRepository.GetOrCreateMonth(currentMonth.Month, currentMonth.Year);
                        monthRecord.TotalDays = days;
                        monthRecord.TotalDosage = totalDosage;
                        record.AdministrationMonths.Add(monthRecord);

                        currentMonth = currentMonth.AddMonths(1);
                    }

                }

                Save<Facts.PsychotropicAdministration>(record);

            }
        }



        private void TrackDimensionChanges(DataDimensions dimensions,
            Facts.PsychotropicAdministration record
            )
        {

            if (record.DrugType != null)
            {
                if (dimensions.PsychotropicDrugTypes.Count(x => x.Name == record.DrugType.Name) < 1)
                {
                    dimensions.PsychotropicDrugTypes.Add(record.DrugType);
                }
            }

            if (dimensions.StartDate.HasValue == false || dimensions.StartDate.Value > record.StartDate)
            {
                dimensions.StartDate = record.StartDate;
            }

        }
    }
}
