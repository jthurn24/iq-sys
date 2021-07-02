using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IQI.Intuition.Web.Models.Administration.Utilization
{
    public class FacilityStats
    {
        public string FacilityName { get; set; }
        public string AccountName { get; set; }

        public List<string> ModulesThisMonth { get; set; }
        public List<string> ModulesLast12Months { get; set; }
        public int ModulesChange { get; set; }



        public int TotalLogins { get; set; }
        public IList<string> TopLogins { get; set; }
    }
}
