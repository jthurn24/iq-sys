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
    public class AntibioticUtilizationMultipleByMD
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int? InfectionTypeId { get; set; }
        public IEnumerable<SelectListItem> InfectionTypeOptions { get; set; }
        public IList<Entry> Entries { get; set; }
        public string Name { get; set; }

        public void SetData(IEnumerable<InfectionTreatment> treatments)
        {

            var qualifiedInfections = treatments.Select(x => x.InfectionVerification)
                .Where(x => x.Treatments.Where(xx => xx.TreatmentType.IsAntibiotic == true)
                .Select(xx => xx.TreatmentName.ToLower())
                .Distinct()
                .Count() > 1);

            var mdNames = treatments
                .Where(x => qualifiedInfections.Contains(x.InfectionVerification))
                .Select(x => x.MDName)
                .Distinct();

            if (Name.IsNotNullOrEmpty())
            {
                mdNames = mdNames.Where(x => x.ToLower().Contains(Name.ToLower()));
            }


            Entries = new List<Entry>();

            foreach (var name in mdNames)
            {

                var mdInfections = qualifiedInfections
                    .Where(x => x.Treatments.Select(xx => xx.MDName.ToLower().Trim())
                        .Contains(name.ToLower().Trim()))
                        .Distinct();

                var entry = new Entry();
                Entries.Add(entry);
                entry.Infections = new List<Infection>();
                entry.PhysicianName = name.ToUpper();
                entry.Infections = new List<Infection>();
                entry.Total = mdInfections.Count();


                foreach (var infectionInfo in mdInfections.OrderBy(x => x.FirstNotedOn))
                {
                    var i = new Infection();
                    i.InfectionType = infectionInfo.InfectionSite.Type.Name;
                    i.InitialAdministrationDate =
                        infectionInfo.Treatments
                        .Where(x => x.TreatmentType.IsAntibiotic && x.AdministeredOn.HasValue)
                        .OrderBy(x => x.AdministeredOn)
                        .Select(x => x.AdministeredOn.Value)
                        .First()
                        .FormatAsShortDate();

                    i.PatientName = infectionInfo.Patient.FullName;
                    i.InfectionId = infectionInfo.Id;
                    i.TreatmentNames = infectionInfo.Treatments.Where(x => x.TreatmentType.IsAntibiotic).Select(x => x.TreatmentName).Distinct();
                    entry.Infections.Add(i);

                }
                
            }


        }

        public class Entry
        {
            public string PhysicianName { get; set; }
            public int Total { get; set; }
            public IList<Infection> Infections { get; set; }
        }

        public class Infection
        {
            public string PatientName { get; set; }
            public string InfectionType { get; set; }
            public int InfectionId { get; set; }
            public IEnumerable<string> TreatmentNames { get; set; }
            public string InitialAdministrationDate { get; set; }
        }
    }
}
