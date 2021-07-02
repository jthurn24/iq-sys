using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQI.Intuition.Domain.Models;
using IQI.Intuition.Domain.Services.Psychotropic;

namespace IQI.Intuition.Infrastructure.Services.BusinessLogic.Psychotropic
{
    public class AdministrationCalculator
    {
        public decimal Calculate(DateTime startDate,
            DateTime endDate,
            IEnumerable<PsychotropicDosageChange> changes,
            IEnumerable<PsychotropicAdministrationPRN> prns,
            IEnumerable<PsychotropicFrequency> frequencies
            )
        {
            decimal total = 0;

            DateTime currentDate = startDate;


            while (currentDate <= endDate)
            {
                if (currentDate <= DateTime.Today)
                {

                    /* Begin by finding the applicable change to the current date */
                    var currentChange = changes.OrderBy(x => x.StartDate).Where(x => x.StartDate <= currentDate).LastOrDefault();

                    if (currentChange != null)
                    {
                        /* Do this in a way that supports both stateful and stateless data context */
                        var frequency = frequencies.Where(x => x.Id == currentChange.Frequency.Id).First();
                        total = total + frequency.GetFrequencyDefinition().CalculateTotalScheduledAdministration(currentDate, currentDate, currentChange);
                    }

                    /* Next apply prns for this date */

                    foreach (var prn in prns.Where(x => x.GivenOn == currentDate))
                    {
                        total = total + prn.Dosage.Value;
                    }
                }

                currentDate = currentDate.AddDays(1);
            }

            return total;
        }
    }
}
