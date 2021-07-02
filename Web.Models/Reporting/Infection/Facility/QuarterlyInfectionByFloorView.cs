using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQI.Intuition.Reporting.Graphics;
using System.Web.Mvc;
using IQI.Intuition.Reporting.Models.Cubes;
using IQI.Intuition.Reporting.Models.Dimensions;
using System.Drawing;
using IQI.Intuition.Reporting.Containers;

namespace IQI.Intuition.Web.Models.Reporting.Infection.Facility
{
    public class QuarterlyInfectionByFloorView
    {
        public Guid Quarter { get; set; }

        public IEnumerable<SelectListItem> QuarterOptions { get; set; }

        public Month Month1 { get; set; }
        public Month Month2 { get; set; }
        public Month Month3 { get; set; }

        public PieChart Month1Chart { get; private set; }
        public PieChart Month2Chart { get; private set; }
        public PieChart Month3Chart { get; private set; }
        public PieChart TotalChart { get; private set; }

        public void SetData(QuarterMonths quarter, IEnumerable<FloorMonthInfectionType.Entry> data)
        {
            /* Setup Basic Info */


            this.Month1 = quarter.Month1;
            this.Month2 = quarter.Month2;
            this.Month3 = quarter.Month3;

            Month1Chart = new PieChart();
            Month2Chart = new PieChart();
            Month3Chart = new PieChart();
            TotalChart = new PieChart();


            var totalData = new Dictionary<Floor, int>();

            IEnumerable<FloorMonthInfectionType.Entry> month1Total =
                data.Where(m => m.Month == Month1).ToList();

            var month1Data = new Dictionary<Floor, int>();
            ApplyTotals(month1Total, month1Data);
            ApplyTotals(month1Total, totalData);

            IEnumerable<FloorMonthInfectionType.Entry> month2Total =
                data.Where(m => m.Month == Month2).ToList();

            var month2Data = new Dictionary<Floor, int>();
            ApplyTotals(month2Total, month2Data);
            ApplyTotals(month2Total, totalData);

            IEnumerable<FloorMonthInfectionType.Entry> month3Total =
                data.Where(m => m.Month == Month3).ToList();

            var month3Data = new Dictionary<Floor, int>();
            ApplyTotals(month3Total, month3Data);
            ApplyTotals(month3Total, totalData);


            FillChart(Month1Chart, month1Data);
            FillChart(Month2Chart, month2Data);
            FillChart(Month3Chart, month3Data);
            FillChart(TotalChart, totalData);
        }


        private void FillChart(PieChart chart, Dictionary<Floor, int> totals)
        {
            var totalCount = totals.Select(m => m.Value).Sum();
            int index = 0;

            foreach (var total in totals)
            {
                double perc = (Convert.ToDouble(total.Value) / Convert.ToDouble(totalCount) * 100);

                chart.AddItem(new PieChart.Item()
                {
                    Label = total.Key.Name,
                    Marker = perc > 0 ? String.Format("{0:F2}%", perc) : string.Empty,
                    Value = total.Value,
                    Color = Intuition.Reporting.Graphics.PieChart.GetDefaultColor(index)
                });

                index++;
            }
        }

        private void ApplyTotals(IEnumerable<FloorMonthInfectionType.Entry> data, Dictionary<Floor, int> dest)
        {
            foreach (var d in data)
            {
                if (dest.ContainsKey(d.Floor))
                {
                    dest[d.Floor] = dest[d.Floor] + d.Total;
                }
                else
                {
                    dest[d.Floor] = d.Total;
                }
            }
        }
    }
}
