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
    public class CNAFallView
    {
        public IList<Entry> Entries { get; set; }
        public IEnumerable<SelectListItem> WingOptions { get; set; }
        public int? Wing { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public PieChart CNAChart { get; set;}

        public void SetData(IEnumerable<IncidentReport> reports)
        {
            Entries = new List<Entry>();

            var cnas = reports.Select(x => x.Employee2 != null ?  x.Employee2.FullName : string.Empty).Distinct();

            CNAChart = new PieChart();

            int colorIndex = -1;

            foreach (var c in cnas)
            {
                int cnaCount = 0;
                colorIndex++;

                if (c == string.Empty)
                {
                    cnaCount = reports.Where(x => x.Employee2 == null).Count();
                    double perc = (double)cnaCount / (double)reports.Count() * (double)100;

                    CNAChart.AddItem(new PieChart.Item()
                    {
                        Color = PieChart.GetDefaultColor(colorIndex),
                        Label = "No CNA Specified",
                        Value = perc,
                        Marker = String.Format("% {0}", Math.Round(perc, 2))
                    });

                    Entries.Add(new Entry()
                    {
                         CNA = "No CNA Specified",
                         Total = cnaCount
                    });
                }
                else
                {

                    cnaCount = reports.Where(x => x.Employee2 != null && x.Employee2.FullName == c).Count();
                    double perc = (double)cnaCount / (double)reports.Count() * (double)100;

                    CNAChart.AddItem(new PieChart.Item()
                    {
                        Color = PieChart.GetDefaultColor(colorIndex),
                        Label = c,
                        Value = perc,
                        Marker = String.Format("% {0}", Math.Round(perc, 2))
                    });

                    Entries.Add(new Entry()
                    {
                        CNA = c,
                        Total = cnaCount
                    });
                }
    
            }

            Entries = Entries.OrderByDescending(x => x.Total).ToList();




        }


        public class Entry
        {
            public string CNA { get; set; }
            public int Total { get; set; }
        }
    }
}
