using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IQI.Intuition.Web.Models.EmployeeInfection
{
    public class EmployeeInfectionInfo
    {
        public int Id { get; set; }
        public string EmployeeName { get; set; }
        public string InfectionTypeName { get; set; }
        public string Department { get; set; }
        public string NotifiedOn { get; set; }
        public string WellOn { get; set; }
    }
}
