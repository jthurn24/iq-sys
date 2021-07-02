using System;
using System.Collections.Generic;
using IQI.Intuition.Web.Models.Warning;
using IQI.Intuition.Reporting.Graphics;


namespace IQI.Intuition.Web.Models.Home
{
    public class Dashboard
    {
        public string CurrentFacilityName { get; set; }

        public string UserLastSignedIn { get; set; }

        public IEnumerable<WarningInfo> RecentWarnings { get; set; }

        public ColumnChart InfectionChart { get; set; }

        public ColumnChart IncidentTypeChart { get; set; }

        public ColumnChart IncidentInjuryChart { get; set; }

        public IEnumerable<Counter> Counters { get; set; }

        public IEnumerable<string> Messages { get; set; }

        public IEnumerable<Resource> Resources { get; set; }
  
        public class Counter
        {
            public int Count { get; set;}
            public string Description { get; set; } 
            public string ToolTip { get; set; }
        }


        public class Resource
        {
            public string Name { get; set; }
            public string Added { get; set; }
            public int id { get; set; }
            public string Path { get; set; }
        }
    }
}
