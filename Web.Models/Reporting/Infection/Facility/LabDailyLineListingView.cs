using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using IQI.Intuition.Domain.Models;
using RedArrow.Framework.Extensions.Formatting;
using RedArrow.Framework.Extensions.IO;
using RedArrow.Framework.Extensions.Common;

namespace IQI.Intuition.Web.Models.Reporting.Infection.Facility
{
    public class LabDailyLineListingView
    {

        public IList<InfectionRow> Infections { get; set; }

        public void SetData(IEnumerable<InfectionVerification> infectionData)
        {
            this.Infections = new List<InfectionRow>();

            foreach (var infection in infectionData)
            {
                var row = new InfectionRow(infection);
                this.Infections.Add(row);
            }
        }



        public class InfectionRow
        {

            public string PatientName { get; set; }
            public string Date { get; set; }
            public string PointOfOrigin { get; set; }
            public string InfectionSite { get; set; }
            public string InfectionType { get; set; }

            public string Temp { get; set; }
            public IList<string> Antibiotics { get; set; }
            public string WBC { get; set; }
            public IList<string> LabResults { get; set; }
            public IList<string> IsolationTypes { get; set; }
            public string XrayResults { get; set; }
            public string Catheter { get; set; }
            public IList<string> Notes { get; set; }
            public string Id { get; set; }

            public InfectionRow(InfectionVerification infection)
            {
                Id = infection.Id.ToString();
                PatientName = infection.Patient.FullName;
                Date = infection.FirstNotedOn.FormatAs("MM/dd/yyyy");
                //IsolationTypes = infection.Precautions.Select(m => m.Name).ToList();
                InfectionType = infection.InfectionSite.Type.Name;
                InfectionSite = infection.InfectionSite.Name;
                Antibiotics = infection.Treatments.Select(x => x.TreatmentName).ToList();


                Notes = infection.InfectionNotes.OrderBy(X => X.CreatedAt).Select(x => x.Note).ToList();

                LabResults = new List<string>();

                foreach (var result in infection.LabResults)
                {
                    var resultDescriptionBuilder = new System.Text.StringBuilder();
                    resultDescriptionBuilder.Append(result.LabTestType.Name);
                    resultDescriptionBuilder.Append("<br>");
                    resultDescriptionBuilder.Append("Result: ");
                    resultDescriptionBuilder.Append(result.LabResult.Name);
                    resultDescriptionBuilder.Append("<br>");

                    foreach (var pathogen in result.ResultPathogens)
                    {
                        resultDescriptionBuilder.Append(pathogen.Pathogen.Name);
                        resultDescriptionBuilder.Append(" ");

                        if(pathogen.PathogenQuantity != null)
                        {
                            resultDescriptionBuilder.Append(pathogen.PathogenQuantity.Name);
                        }

                        resultDescriptionBuilder.Append("<br>");
                    }

                    LabResults.Add(resultDescriptionBuilder.ToString());
                    
                }

            }
        }

    }
    
}
