using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQI.Intuition.Domain.Services.Psychotropic;
using System.Xml.Serialization;

namespace IQI.Intuition.Infrastructure.Services.BusinessLogic.Psychotropic.FrequencyDefinitions
{
    public class Discontinue : IDosageFrequencyDefinition
    {

        public decimal GetDailyAverage(Domain.Models.PsychotropicDosageChange dosage)
        {
            return 0;
        }

        public decimal GetTotal(Domain.Models.PsychotropicDosageChange dosage)
        {
            return 0;
        }

        public decimal CalculateTotalScheduledAdministration(DateTime startDate, DateTime endDate, Domain.Models.PsychotropicDosageChange dosage)
        {
            return 0;
        }

        public IEnumerable<DosageSegment> GetDefaultSegments()
        {
            return new List<DosageSegment>();
        }

        public IEnumerable<DosageSegment> ReadSegments(string data)
        {
            return new List<DosageSegment>();
        }

        public string WriteSegments(IEnumerable<DosageSegment> data)
        {
            return string.Empty;
        }

        public bool IndicatesActiveAdministration()
        {
            return false;
        }

    }
}
