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
    public class QuarterlyIncidentByFloorView
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

        public QuarterlyStatTable<Floor> IncidentFloorView { get; private set; }


        public void SetData(QuarterMonths quarter,
            IEnumerable<FloorMonthIncidentTypeGroup.Entry> totals)
        {


            this.Month1 = quarter.Month1;
            this.Month2 = quarter.Month2;
            this.Month3 = quarter.Month3;


            IncidentFloorView = new QuarterlyStatTable<Floor>();
            IncidentFloorView.Month1 = this.Month1;
            IncidentFloorView.Month2 = this.Month2;
            IncidentFloorView.Month3 = this.Month3;
            IncidentFloorView.CategoryDescription = "Floor";
            IncidentFloorView.CountDescription = "Inc";

            QuarterlyStatTable<Floor>
                .LoadTable<Floor, FloorMonthIncidentTypeGroup.Entry>(
                IncidentFloorView,
                totals,
                x => x.Total,
                x => x.Month,
                x => x.Floor.Name,
                x => x.Change,
                x => x.Rate);

            this.Month1Chart = new PieChart();
            LoadChart(this.Month1Chart,
                totals.Where(x => x.Month == this.Month1).ToList());

            this.Month2Chart = new PieChart();
            LoadChart(this.Month2Chart,
                totals.Where(x => x.Month == this.Month2).ToList());

            this.Month3Chart = new PieChart();
            LoadChart(this.Month3Chart,
                totals.Where(x => x.Month == this.Month3).ToList());

            this.TotalChart = new PieChart();
            LoadChart(this.TotalChart,
                totals);

        }

        private void LoadChart(PieChart chart,
            IEnumerable<FloorMonthIncidentTypeGroup.Entry> data)
        {
            var sections = data.Select(x => x.Floor).Distinct().OrderBy(x => x.Name);
            int colorIndex = 0;

            foreach (var section in sections)
            {
                var total = data.Sum(x => x.Total);
                var matchCount = data.Where(x => x.Floor == section).Sum(x => x.Total);

                double perc = (Convert.ToDouble(matchCount) / Convert.ToDouble(total) * 100);

                chart.AddItem(new PieChart.Item()
                {
                    Label = section.Name,
                    Marker = perc > 0 ? String.Format("{0:F2}%", perc) : string.Empty,
                    Value = matchCount,
                    Color = PieChart.GetDefaultColor(colorIndex)
                });

                colorIndex ++;
            }
        }
 
    }
}
