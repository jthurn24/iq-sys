using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQI.Intuition.Domain.Models;
using RedArrow.Framework.Extensions.Formatting;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Extensions.IO;
using System.Web.Mvc;

namespace IQI.Intuition.Web.Models.Reporting.Infection.Facility
{
    public class AntibioticUtilizationByMDStatistics
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int? InfectionTypeId { get; set; }
        public IEnumerable<SelectListItem> InfectionTypeOptions { get; set; }
        public IList<Entry> Entries { get; set; }
        public string Name { get; set; }

        public void SetData(IEnumerable<InfectionVerification> verifications)
        {
            Entries = new List<Entry>();

            

            var treatments = verifications.SelectMany(x => x.Treatments).Where(x => x.TreatmentType.IsAntibiotic);

            var mdNames = treatments.Select(x => x.MDName)
                .Distinct()
                .Where(x => x != string.Empty)
                .OrderBy(x => x)
                .ToList();

            if (Name.IsNotNullOrEmpty())
            {
                mdNames = mdNames.Where(x => x.ToLower().Contains(Name.ToLower())).ToList();
            }


            foreach(var mdName in mdNames)
            {
                var entry = new Entry();
                entry.Antibiotics = new List<Antibiotic>();
                entry.PhysicianName = mdName;
                Entries.Add(entry);

                var mdTreatements = treatments.Where(x => x.MDName == mdName);
                var mdAntibiotics = mdTreatements.Select(x => x.TreatmentName).Distinct().OrderBy(x => x);

                foreach(var mdAntibiotic in mdAntibiotics)
                {
                    var aEntry = new Antibiotic();
                    aEntry.Name = mdAntibiotic;
                    entry.Antibiotics.Add(aEntry);

                    var antibioticTreatements = mdTreatements.Where(x => x.TreatmentName == mdAntibiotic);
                    aEntry.Count = antibioticTreatements.Select(x => x.InfectionVerification).Distinct().Count();
                    
                    var noInfectionCount = antibioticTreatements.Select(x => x.InfectionVerification).Distinct().Where(x => x.Classification == InfectionClassification.NoInfection).Count();
                    var admissionCount = antibioticTreatements.Select(x => x.InfectionVerification).Distinct().Where(x => x.Classification == InfectionClassification.AdmissionHospitalDiagnosed || x.Classification == InfectionClassification.Admission).Count();
                    var haiCount = antibioticTreatements.Select(x => x.InfectionVerification).Distinct().Where(x => x.Classification == InfectionClassification.AdmissionHospitalDiagnosed || x.Classification == InfectionClassification.Admission).Count(); 

                    if(noInfectionCount > 0) aEntry.PercentageNoInfection = ((decimal)noInfectionCount / (decimal)aEntry.Count ) * (decimal)100;
                    if(admissionCount > 0) aEntry.PercentageAdmission = ((decimal)admissionCount / (decimal)aEntry.Count ) * (decimal)100;
                    if(haiCount > 0) aEntry.PercentageHAI = ((decimal)haiCount / (decimal)aEntry.Count ) * (decimal)100;
                }

            }
        }

        public class Entry
        {
            public string PhysicianName { get; set; }
            public IList<Antibiotic> Antibiotics { get; set; }
        }

        public class Antibiotic
        {
            public string Name { get; set; }
            public int Count { get; set; }
            public decimal PercentageNoInfection { get; set; }
            public decimal PercentageAdmission { get; set; }
            public decimal PercentageHAI { get; set; }
        }
    }
}
