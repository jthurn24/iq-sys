using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using IQI.Intuition.Domain;
using RedArrow.Framework.Extensions.Formatting;
using RedArrow.Framework.Extensions.IO;

namespace IQI.Intuition.Web.Models.Reporting.EmployeeInfection.Facility
{
    public class LineListingInfectionView
    {

        public int? InfectionType { get; set; }
        public IEnumerable<SelectListItem> InfectionTypeOptions { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public IList<InfectionRow> Infections { get; set; }


        public void SetData(IEnumerable<Domain.Models.EmployeeInfection> infectionData)
        {
            this.Infections = new List<InfectionRow>();

            foreach(var infection in infectionData)
            {
                var row = new InfectionRow(infection);
                this.Infections.Add(row);
            }
        }

        public class InfectionRow
        {
            public string ID { get; set; }

            public string FullName { get; set; }

            public string NotifiedOn { get; set; }

            public string DateOfBirth { get; set; }

            public string Gender { get; set; }

            public string LastShift { get; set; }

            public string LastWorkedOn { get; set; }

            public string Department { get; set; }

            public string Wing { get; set; }

            public string Floor { get; set; }

            public string Notes { get; set; }

            public string FirstSymptomOn { get; set; }

            public string InfectionType { get; set; }

            public string SeenByMD { get; set; }

            public string MDInstructions { get; set; }

            public string LabResults { get; set; }

            public string LastSymptomOn { get; set; }

            public string WellOn { get; set; }

            public List<string> Symptoms { get; set; }

            public InfectionRow(Domain.Models.EmployeeInfection infection)
            {
                this.ID = infection.Id.ToString();
                this.FullName = infection.FullName;
                this.NotifiedOn = infection.NotifiedOn.FormatAsMinimalDate();
                this.DateOfBirth = infection.DateOfBirth.FormatAsMinimalDate();
                this.Gender = System.Enum.GetName(typeof(Domain.Enumerations.Gender), infection.Gender).SplitPascalCase();
                this.LastShift = System.Enum.GetName(typeof(Domain.Enumerations.EmployeeShift), infection.LastShift).SplitPascalCase();
                this.LastWorkedOn = infection.LastWorkedOn.FormatAsMinimalDate();
                this.Department = System.Enum.GetName(typeof(Domain.Enumerations.EmployeeDepartment), infection.Department).SplitPascalCase();
                this.Wing = infection.Wing != null ? infection.Wing.Name : string.Empty;
                this.Floor = infection.Wing != null ? infection.Wing.Floor.Name : string.Empty;
                this.Notes = infection.Notes;
                this.FirstSymptomOn = infection.FirstSymptomOn.FormatAs("MM/dd/yy HH:mm");
                this.InfectionType = infection.InfectionType != null ? infection.InfectionType.Name : "Other (No Infection)";
                this.SeenByMD = infection.SeenByMD.FormatAsAnswer();
                this.MDInstructions = infection.MDInstructions;
                this.LabResults = infection.LabResults;
                this.LastSymptomOn = infection.LastSymptomOn.FormatAs("MM/dd/yy HH:mm");
                this.WellOn = infection.WellOn.FormatAsMinimalDate();
                

 

                this.Symptoms = new List<string>();

                foreach (var symptom in infection.InfectionSymptoms)
                {
                    this.Symptoms.Add(symptom.Name);
                }

            }
        }
    }
}
