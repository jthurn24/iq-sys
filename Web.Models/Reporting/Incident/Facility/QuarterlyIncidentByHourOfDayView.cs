using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQI.Intuition.Reporting.Graphics;
using IQI.Intuition.Reporting.Tables;
using System.Web.Mvc;
using IQI.Intuition.Reporting.Models.Cubes;
using IQI.Intuition.Reporting.Models.Dimensions;
using System.Drawing;
using IQI.Intuition.Reporting.Containers;

namespace IQI.Intuition.Web.Models.Reporting.Incident.Facility
{
    public class QuarterlyIncidentByHourOfDayView
    {

        public Guid Quarter { get; set; }
        public Guid IncidentTypeGroup { get; set; }

        public IEnumerable<SelectListItem> QuarterOptions { get; set; }
        public IEnumerable<SelectListItem> IncidentTypeGroupOptions { get; set; }

        public Month Month1 { get; set; }
        public Month Month2 { get; set; }
        public Month Month3 { get; set; }

        public PieChart Month1Chart { get; private set; }
        public PieChart Month2Chart { get; private set; }
        public PieChart Month3Chart { get; private set; }
        public PieChart TotalChart { get; private set; }

        public QuarterlyStatTable<int> IncidentHourOfDayView { get; private set; }


        public void SetData(QuarterMonths quarter,
            IEnumerable<FacilityMonthIncidentHourOfDay.Entry> totals)
        {
            totals = totals.Where(x => x.Total > 0 || x.Change != 0);

            var hourGroups = new Dictionary<int, string>();
            hourGroups.Add(0,"2400-0200");
            hourGroups.Add(1,"2400-0200");
            hourGroups.Add(2, "0200-0400");
            hourGroups.Add(3, "0200-0400");
            hourGroups.Add(4, "0400-0600");
            hourGroups.Add(5, "0400-0600");
            hourGroups.Add(6, "0600-0800");
            hourGroups.Add(7, "0600-0800");
            hourGroups.Add(8, "0800-1000");
            hourGroups.Add(9, "0800-1000");
            hourGroups.Add(10, "1000-1200");
            hourGroups.Add(11, "1000-1200");
            hourGroups.Add(12, "1200-1400");
            hourGroups.Add(13, "1200-1400");
            hourGroups.Add(14, "1400-1600");
            hourGroups.Add(15, "1400-1600");
            hourGroups.Add(16, "1600-1800");
            hourGroups.Add(17, "1600-1800");
            hourGroups.Add(18, "1800-2000");
            hourGroups.Add(19, "1800-2000");
            hourGroups.Add(20, "2000-2200");
            hourGroups.Add(21, "2000-2200");
            hourGroups.Add(22, "2200-2400");
            hourGroups.Add(23, "2200-2400");

            var colorGroups = new Dictionary<int,string>();
            colorGroups.Add(0,"2400-0200");
            colorGroups.Add(1, "0200-0400");
            colorGroups.Add(2, "0400-0600");
            colorGroups.Add(3, "0600-0800");
            colorGroups.Add(4, "0800-1000");
            colorGroups.Add(5, "1000-1200");
            colorGroups.Add(6, "1200-1400");
            colorGroups.Add(7, "1400-1600");
            colorGroups.Add(8, "1600-1800");
            colorGroups.Add(9, "1800-2000");
            colorGroups.Add(10, "2000-2200");
            colorGroups.Add(11, "2200-2400");

            /* Setup Basic Info */


            this.Month1 = quarter.Month1;
            this.Month2 = quarter.Month2;
            this.Month3 = quarter.Month3;


            IncidentHourOfDayView = new QuarterlyStatTable<int>();
            IncidentHourOfDayView.Month1 = this.Month1;
            IncidentHourOfDayView.Month2 = this.Month2;
            IncidentHourOfDayView.Month3 = this.Month3;
            IncidentHourOfDayView.CategoryDescription = "Hour";
            IncidentHourOfDayView.CountDescription = "Inc";

            QuarterlyStatTable<int>
                .LoadTable<int, FacilityMonthIncidentHourOfDay.Entry>(
                IncidentHourOfDayView,
                totals,
                x => x.Total,
                x => x.Month,
                x => hourGroups[x.HourOfDay.Value],
                x => x.Change,
                x => x.Rate);

            this.Month1Chart = new PieChart();
            LoadChart(this.Month1Chart,
                totals.Where(x => x.Month == this.Month1).ToList(),
                hourGroups,colorGroups);

            this.Month2Chart = new PieChart();
            LoadChart(this.Month2Chart,
                totals.Where(x => x.Month == this.Month2).ToList(),
                hourGroups,colorGroups);

            this.Month3Chart = new PieChart();
            LoadChart(this.Month3Chart,
                totals.Where(x => x.Month == this.Month3).ToList(),
                hourGroups,colorGroups);

            this.TotalChart = new PieChart();
            LoadChart(this.TotalChart,
                totals,
                hourGroups,
                colorGroups);

        }

        private void LoadChart(PieChart chart,
            IEnumerable<FacilityMonthIncidentHourOfDay.Entry> data,
            Dictionary<int, string> hourGroups,
            Dictionary<int, string> colorGroups)
        {
            var sections = data.Select(x => hourGroups[x.HourOfDay.Value]).Distinct();
            int colorIndex = 0;

            foreach (var section in sections)
            {
                var total = data.Sum(x => x.Total);

                var matchCount = data.Where(x => hourGroups[x.HourOfDay.Value] == section).Sum(x => x.Total);

                double perc = (Convert.ToDouble(matchCount) / Convert.ToDouble(total) * 100);

                chart.AddItem(new PieChart.Item()
                {
                    Label = section,
                    Marker = perc > 0 ? String.Format("{0:F2}%", perc) : string.Empty,
                    Value = matchCount,
                    Color = PieChart.GetDefaultColor(colorGroups.Where(x => x.Value == section).First().Key)
                });

                colorIndex ++;
            }
        }
 
    }
}
