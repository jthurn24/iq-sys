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

namespace IQI.Intuition.Infrastructure.Services.Reporting.SynchronizationService.Psychotropic.CubeServices
{
    public class FacilityMonthPsychotropicDrugType : AbstractMonthlyFacilityCubeService
    {

        private Cubes.FacilityMonthPsychotropicDrugType _Cube;
        private IEnumerable<Facts.PsychotropicAdministration> _Facts;
 

        protected override void Init(DataDimensions changes)
        {

            _Cube = GetQueryable<Cubes.FacilityMonthPsychotropicDrugType>()
                .Where(x => x.Facility.Id== changes.Facility.Id)
                .FirstOrDefault();

            if (_Cube == null)
            {
                _Cube = new Cubes.FacilityMonthPsychotropicDrugType();
                _Cube.Entries = new List<Cubes.FacilityMonthPsychotropicDrugType.Entry>();
                _Cube.Facility = changes.Facility;
                _Cube.Account = changes.Facility.Account;
            }

            _Facts = GetQueryable<Facts.PsychotropicAdministration>()
                .Where(x => x.Facility.Id == changes.Facility.Id
                && (x.Deleted == null || x.Deleted == false)).ToList();

        }

        protected override void ProcessDay(DataDimensions changes,
            DateTime currentDate,
            DateTime priorDate,
            Dimensions.Month currentMonth,
            Dimensions.Month priorMonth,
            Cubes.FacilityMonthCensus currentMonthCensus,
            Cubes.FacilityMonthCensus priorMonthCensus,
            int currentPatientDays,
            int priorPatientDays)
        {
            foreach (var drugType in changes.PsychotropicDrugTypes)
            {

  
                var prevDataCount = _Facts.ToList()
                    .Where(x =>
                        (x.AdministrationMonths
                            .Where(xx => xx.Month.MonthOfYear == priorMonth.MonthOfYear 
                                && xx.Month.Year == priorMonth.Year).Count() > 0)
                        && x.DrugType.Name == drugType.Name
                        )
                        .Count();


                var prevRate = Domain.Calculations.Rate1000(prevDataCount, priorPatientDays);

                var currentData = _Facts.ToList()
                    .Where(x =>
                        (x.AdministrationMonths
                            .Where(xx => xx.Month.MonthOfYear == currentMonth.MonthOfYear
                                && xx.Month.Year == currentMonth.Year).Count() > 0)
                        && x.DrugType.Name == drugType.Name
                        );

                var currentDataCount = currentData.Count();

                Cubes.FacilityMonthPsychotropicDrugType.Entry cube = _Cube.Entries
                    .Where(x => x.Month.MonthOfYear == currentMonth.MonthOfYear && x.Month.Year == currentMonth.Year
                        && x.DrugType.Name == drugType.Name)
                    .FirstOrDefault();

                if (cube == null)
                {
                    cube = new Cubes.FacilityMonthPsychotropicDrugType.Entry();
                    cube.Month = currentMonth;
                    cube.DrugType = drugType;
                    cube.Id = Guid.NewGuid();
                    _Cube.Entries.Add(cube);
                }

                /* Find all admins of this type and eval */

                int increaseCount = 0;
                int decreaseCount = 0;
                int activeCount = 0;
                int priorActiveCount = 0;

                foreach (var admin in currentData)
                {
                    /* get current and prior avg dosage */
                    var currentDosage = admin.AdministrationMonths.Where(x => x.Month.MonthOfYear == currentMonth.MonthOfYear && x.Month.Year == currentMonth.Year).FirstOrDefault();
                    var priorDosage = admin.AdministrationMonths.Where(x => x.Month.MonthOfYear == priorMonth.MonthOfYear && x.Month.Year == priorMonth.Year).FirstOrDefault();

                    decimal currentAvg = currentDosage != null && currentDosage.TotalDosage > 0 ? (currentDosage.TotalDosage.Value / (decimal)currentDosage.TotalDays.Value) : 0;
                    decimal priorAvg = priorDosage != null && priorDosage.TotalDosage > 0 ? (priorDosage.TotalDosage.Value / (decimal)priorDosage.TotalDays.Value) : 0;

                    if (currentAvg > priorAvg)
                    {
                        increaseCount++;
                    }
                    else if (currentAvg < priorAvg)
                    {
                        decreaseCount++;
                    }

                    if (currentDosage != null && currentDosage.TotalDosage > 0)
                    {
                        activeCount++;
                    }

                    if (priorDosage != null && priorDosage.TotalDosage > 0)
                    {
                        priorActiveCount++;
                    }

                }


                /* tally up the totals and build stats for this month */

                cube.IncreaseCount = increaseCount;
                cube.DecreaseCount = decreaseCount;
                cube.ActiveCount = activeCount;

                cube.ActiveRate = Domain.Calculations.Rate1000(activeCount, currentPatientDays);

                cube.ActiveChange = (0 - priorActiveCount) + activeCount;
                cube.DosageChange = (0 - decreaseCount) + increaseCount;


            }

        }


        protected override void Completed()
        {
            Save(_Cube);
            base.Completed();
        }
    }
}
