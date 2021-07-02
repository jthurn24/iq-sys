using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQI.Intuition.Domain.Services.Psychotropic;
using System.Xml.Serialization;

namespace IQI.Intuition.Infrastructure.Services.BusinessLogic.Psychotropic.FrequencyDefinitions.BaseDefinitions
{
    public abstract class BaseDefinition :  IDosageFrequencyDefinition
    {

        abstract public decimal GetDailyAverage(Intuition.Domain.Models.PsychotropicDosageChange dosage);

        abstract public IEnumerable<DosageSegment> GetDefaultSegments();

        abstract public decimal GetTotal(Domain.Models.PsychotropicDosageChange dosage);

        public IEnumerable<DosageSegment> ReadSegments(string data)
        {
            if (data == null || data == string.Empty)
            {
                return null;
            }

            var reader = new System.IO.StringReader(data);
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(List<DosageSegment>));
            return (List<DosageSegment>)serializer.Deserialize(reader);
        }

        public string WriteSegments(IEnumerable<DosageSegment> data)
        {
            var writer = new System.IO.StringWriter();
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(List<DosageSegment>));
            serializer.Serialize(writer, data.ToList());
            return writer.ToString();
        }

        public bool IndicatesActiveAdministration()
        {
            return true;
        }

        public decimal CalculateTotalScheduledAdministration(DateTime startDate, DateTime endDate, Domain.Models.PsychotropicDosageChange dosage)
        {
            DateTime currentDate = startDate;
            decimal total = 0;
            
            while (currentDate <= endDate)
            {
                total = total + GetTotal(dosage);

                currentDate = currentDate.AddDays(1);
            }

            return total;
        }

    }
}
