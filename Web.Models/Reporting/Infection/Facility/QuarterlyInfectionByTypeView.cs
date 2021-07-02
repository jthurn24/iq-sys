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
    public class QuarterlyInfectionByTypeView
    {
        public Guid Quarter { get; set; }

        public IEnumerable<SelectListItem> QuarterOptions { get; set; }

        public Month Month1 { get; private set; }
        public Month Month2 { get; private set; }
        public Month Month3 { get; private set; }

        public PieChart Month1Chart { get; private set; }
        public PieChart Month2Chart { get; private set; }
        public PieChart Month3Chart { get; private set; }
        public PieChart TotalChart { get; private set; }

        public void SetData(QuarterMonths quarter,
            IEnumerable<FacilityMonthInfectionType.Entry> data)
        {
            /* Setup Basic Info */

            this.Month1 = quarter.Month1;
            this.Month2 = quarter.Month2;
            this.Month3 = quarter.Month3;

            Month1Chart = new PieChart();
            Month2Chart = new PieChart();
            Month3Chart = new PieChart();
            TotalChart = new PieChart();


            var totalData = new Dictionary<InfectionType, int>();

            IEnumerable<FacilityMonthInfectionType.Entry> month1Total = 
                data.Where(m => m.Month == Month1).ToList();

            var month1Data = new Dictionary<InfectionType, int>();
            ApplyTotals(month1Total, month1Data);
            ApplyTotals(month1Total, totalData);

            IEnumerable<FacilityMonthInfectionType.Entry> month2Total =
                data.Where(m => m.Month == Month2).ToList();

            var month2Data = new Dictionary<InfectionType, int>();
            ApplyTotals(month2Total, month2Data);
            ApplyTotals(month2Total, totalData);

            IEnumerable<FacilityMonthInfectionType.Entry> month3Total =
                data.Where(m => m.Month == Month3).ToList();

            var month3Data = new Dictionary<InfectionType, int>();
            ApplyTotals(month3Total, month3Data);
            ApplyTotals(month3Total, totalData);


            FillChart(Month1Chart, month1Data);
            FillChart(Month2Chart, month2Data);
            FillChart(Month3Chart, month3Data);
            FillChart(TotalChart, totalData);
        }


        private void FillChart(PieChart chart, Dictionary<InfectionType, int> totals)
        {
            var totalCount = totals.Select(m => m.Value).Sum();

            foreach (var total in totals.OrderBy(x => x.Key.SortOrder))
            {
                if (total.Value > 0)
                {
                    double perc = (Convert.ToDouble(total.Value) / Convert.ToDouble(totalCount) * 100);

                    chart.AddItem(new PieChart.Item()
                    {
                        Label = total.Key.Name,
                        Marker = perc > 0 ? String.Format("{0:F2}%", perc) : string.Empty,
                        Value = total.Value,
                        Color = System.Drawing.ColorTranslator.FromHtml(total.Key.Color)
                    });
                }
            }
        }

        private void ApplyTotals(IEnumerable<FacilityMonthInfectionType.Entry> data, Dictionary<InfectionType, int> dest)
        {
            foreach (var d in data.OrderBy(x => x.InfectionType.SortOrder))
            {
                if (dest.ContainsKey(d.InfectionType))
                {
                    dest[d.InfectionType] = dest[d.InfectionType] + d.Total;
                }
                else
                {
                    dest[d.InfectionType] = d.Total;
                }
            }
        }

    }
}
