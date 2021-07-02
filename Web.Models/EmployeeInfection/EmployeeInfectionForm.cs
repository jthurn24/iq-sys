using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQI.Intuition.Domain;
using System.Web.Mvc;

namespace IQI.Intuition.Web.Models.EmployeeInfection
{
    public class EmployeeInfectionForm
    {

        public EmployeeInfectionForm()
        {
            ClientData = new EmployeeInfectionFormClientData();
        }

        public int? Id { get; set; }

        public DateTime? NotifiedOn { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string DateOfBirth { get; set; }

        public Enumerations.Gender Gender { get; set; }

        public Enumerations.EmployeeShift LastShift { get; set; }

        public DateTime? LastWorkedOn { get; set; }

        public Enumerations.EmployeeDepartment Department { get; set; }

        public int? Wing { get; set; }

        public int? Floor { get; set; }

        public List<SelectListItem> FloorOptions { get; set; }

        public string Notes { get; set; }

        public DateTime? FirstSymptomOn { get; set; }
        public int? FirstSymptomHour { get; set; }
        public int? FirstSymptomMinutes { get; set; }

        public int? InfectionType { get; set; }

        public bool? SeenByMD { get; set; }

        public string MDInstructions { get; set; }

        public string LabResults { get; set; }

        public DateTime? ReturnToWorkOn { get; set; }

        public DateTime? LastSymptomOn { get; set; }
        public int? LastSymptomHour { get; set; }
        public int? LastSymptomMinutes { get; set; }

        public DateTime? WellOn { get; set; }

        public IEnumerable<int> InfectionSymptoms { get; set; }

        public EmployeeInfectionFormClientData ClientData { get; set; }

        public object ClientViewModel
        {
            get
            {
                return new
                {
                    Floors = ClientData.Floors,
                    Wings = ClientData.Wings,
                    Floor = Floor,
                    Wing = Wing
                };
            }
        }
    }
}
