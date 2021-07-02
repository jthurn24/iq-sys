using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using IQI.Intuition.Domain;
using RedArrow.Framework.Extensions.Formatting;
using RedArrow.Framework.Extensions.IO;

namespace IQI.Intuition.Web.Models.Reporting.StateOfWisconsin
{
    public class DHSStaffOutbreakCaseLogView
    {
        public int? InfectionType { get; set; }
        public IEnumerable<SelectListItem> InfectionTypeOptions { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public IList<InfectionRow> Infections { get; set; }


        public void SetData(IEnumerable<Domain.Models.EmployeeInfection> infectionData)
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
            public string ID { get; set; }

            public string Number { get; set; }

            public string Name { get; set; }

            public string Age { get; set; }

            public string Gender { get; set; }

            public string Assignment { get; set; }

            public string OnSet { get; set; }

            public string Well { get; set; }

            public string N { get; set; }

            public string V { get; set; }

            public string D { get; set; }

            public string AC { get; set; }

            public string FE { get; set; }

            public string CH { get; set; }

            public string RetWork { get; set; }

            public string Hospital { get; set; }

            public string LabResults { get; set; }


            public InfectionRow(Domain.Models.EmployeeInfection infection)
            {
                this.ID = infection.Id.ToString();
                var Department = System.Enum.GetName(typeof(Domain.Enumerations.EmployeeDepartment), infection.Department).SplitPascalCase();
                var Wing = infection.Wing != null ? infection.Wing.Name : string.Empty;
                var Floor = infection.Wing != null ? infection.Wing.Floor.Name : string.Empty;
                var Notes = infection.Notes;

                this.Name = infection.FullName;
                this.Age = infection.DateOfBirth.HasValue ? Math.Floor(DateTime.Today.Subtract(infection.DateOfBirth.Value).TotalDays / 365).ToString() : string.Empty;
                this.Gender = System.Enum.GetName(typeof(Domain.Enumerations.Gender), infection.Gender).SplitPascalCase(); ;
                this.Assignment = string.Concat(Wing, " / ", Floor, " / ", Department);
                this.OnSet = infection.FirstSymptomOn.FormatAs("MM/dd/yy HH:mm");
                this.Well = infection.WellOn.FormatAsMinimalDate();
                this.LabResults = infection.LabResults;
                this.Hospital = infection.MDInstructions;

                if (infection.ReturnToWorkOn.HasValue)
                {
                    this.RetWork = infection.ReturnToWorkOn.FormatAsMinimalDate();
                }

                if (infection.InfectionSymptoms.Select(x => x.Name.ToLower()).Contains("nausea"))
                {
                    this.N = "Yes";
                }

                if (infection.InfectionSymptoms.Select(x => x.Name.ToLower()).Contains("vomiting"))
                {
                    this.V = "Yes";
                }

                if (infection.InfectionSymptoms.Select(x => x.Name.ToLower()).Contains("abdominal pain"))
                {
                    this.AC = "Yes";
                }

                if (infection.InfectionSymptoms.Select(x => x.Name.ToLower()).Contains("cramps"))
                {
                    this.AC = "Yes";
                }

                if (infection.InfectionSymptoms.Select(x => x.Name.ToLower()).Contains("fever"))
                {
                    this.FE = "Yes";
                }

                if (infection.InfectionSymptoms.Select(x => x.Name.ToLower()).Contains("chills"))
                {
                    this.CH = "Yes";
                }

                if (infection.InfectionSymptoms.Select(x => x.Name.ToLower()).Contains("diarrhea"))
                {
                    this.D = "Yes";
                }

            }
        }
    }
}
