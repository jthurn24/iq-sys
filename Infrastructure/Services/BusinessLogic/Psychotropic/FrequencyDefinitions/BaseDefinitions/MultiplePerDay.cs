using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQI.Intuition.Domain.Services.Psychotropic;

namespace IQI.Intuition.Infrastructure.Services.BusinessLogic.Psychotropic.FrequencyDefinitions.BaseDefinitions
{
    public class MultiplePerDay : BaseDefinition
    {

        private int _TimesPerDay = 0;

        public MultiplePerDay(int timesPerDay)
        {
            _TimesPerDay = timesPerDay;
        }

        public override decimal GetDailyAverage(Intuition.Domain.Models.PsychotropicDosageChange dosage)
        {
            return GetTotal(dosage);
        }

        public override decimal GetTotal(Domain.Models.PsychotropicDosageChange dosage)
        {
            decimal total = 0;

            var segments = ReadSegments(dosage.DosageSegments);

            if (segments != null)
            {
                foreach (var segment in segments)
                {
                    total = total + (segment.Dosage.HasValue ? segment.Dosage.Value : 0);
                }
            }

            return total;
        }

        public override IEnumerable<DosageSegment> GetDefaultSegments()
        {
            var segments = new List<DosageSegment>();

            for (int i = 1; i <= _TimesPerDay; i++)
            {
                segments.Add(new DosageSegment()
                {
                     Label = String.Concat("Time #",i),
                     Description = string.Empty,
                     DescriptionOptions = null,
                     ID = String.Concat("SEG_", i)
                });
            }

            return segments;
        }
    }
}
