using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQI.Intuition.Domain.Models;
using System.Web.Mvc;
using IQI.Intuition.Infrastructure.Services.BusinessLogic.Incident;
using RedArrow.Framework.Extensions.Formatting;
using IQI.Intuition.Reporting.Graphics;

namespace IQI.Intuition.Web.Models.Reporting.Incident.Facility
{
    public class WitnessedFallView
    {
        public IList<Entry> Entries { get; set; }
        public IEnumerable<SelectListItem> WingOptions { get; set; }
        public int? Wing { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public PieChart WitnessedChart { get; set;}

        public void SetData(IEnumerable<IncidentReport> reports)
        {
            Entries = new List<Entry>();
            WitnessedChart = new PieChart();

            var total = reports.Count();
            var w = reports.Where(x => x.IncidentWitnesses.Count() > 0).Count();
            var u = reports.Where(x => x.IncidentWitnesses.Count() < 1).Count();

            var wp = w > 0 ? ((double)w / (double)total) * (double)100 : (double)0;
            var up = u > 0 ? ((double)u / (double)total) * (double)100 : (double)0;

            Entries.Add(new Entry()
            {
                 Title = "Witnessed",
                 Total = w
            });

            WitnessedChart.AddItem(new PieChart.Item()
            {
                Color = PieChart.GetDefaultColor(0),
                Label = "Witnessed",
                Value = wp,
                Marker = String.Format("% {0}", Math.Round(wp, 2))
            });


            Entries.Add(new Entry()
            {
                Title = "Unwitnessed",
                Total = u
            });

            WitnessedChart.AddItem(new PieChart.Item()
            {
                Color = PieChart.GetDefaultColor(1),
                Label = "Unwitnessed",
                Value = up,
                Marker = String.Format("% {0}", Math.Round(up, 2))
            });
        }


        public class Entry
        {
            public string Title { get; set; }
            public int Total { get; set; }
        }
    }
}
