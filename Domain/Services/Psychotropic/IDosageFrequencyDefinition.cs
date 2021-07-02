using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IQI.Intuition.Domain.Services.Psychotropic
{
    public interface IDosageFrequencyDefinition
    {
        decimal GetDailyAverage(Domain.Models.PsychotropicDosageChange dosage);
        decimal GetTotal(Domain.Models.PsychotropicDosageChange dosage);
        decimal CalculateTotalScheduledAdministration(DateTime startDate, DateTime endDate, Domain.Models.PsychotropicDosageChange dosage);

        IEnumerable<DosageSegment> GetDefaultSegments();
        IEnumerable<DosageSegment> ReadSegments(string data);
        string WriteSegments(IEnumerable<DosageSegment> data);
        bool IndicatesActiveAdministration();
        
    }
}
