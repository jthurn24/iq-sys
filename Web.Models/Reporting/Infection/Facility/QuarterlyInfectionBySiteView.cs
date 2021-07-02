using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQI.Intuition.Reporting.Graphics;
using System.Web.Mvc;
using IQI.Intuition.Reporting.Models.Cubes;
using IQI.Intuition.Reporting.Models.Dimensions;
using System.Drawing;
using IQI.Intuition.Reporting.Tables;
using IQI.Intuition.Reporting.Containers;

namespace IQI.Intuition.Web.Models.Reporting.Infection.Facility
{
    public class QuarterlyInfectionBySiteView
    {
        public Guid Quarter { get; set; }

        public IEnumerable<SelectListItem> QuarterOptions { get; set; }

        public Guid? InfectionType { get; set; }
        public IEnumerable<SelectListItem> InfectionTypeOptions { get; set; }

        public Month Month1 { get; set; }
        public Month Month2 { get; set; }
        public Month Month3 { get; set; }

        public PieChart Month1Chart { get; private set; }
        public PieChart Month2Chart { get; private set; }
        public PieChart Month3Chart { get; private set; }
        public PieChart TotalChart { get; private set; }

        public QuarterlyInfectionTable<InfectionSite> InfectionSiteView { get; private set; }

        public void SetData(QuarterMonths quarter, IEnumerable<FacilityMonthInfectionSite.Entry> data)
        {
            /* Setup Basic Info */


            this.Month1 = quarter.Month1;
            this.Month2 = quarter.Month2;
            this.Month3 = quarter.Month3;

            Month1Chart = new PieChart();
            Month2Chart = new PieChart();
            Month3Chart = new PieChart();
            TotalChart = new PieChart();

            InfectionSiteView = new QuarterlyInfectionTable<InfectionSite>();
            InfectionSiteView.Month1 = this.Month1;
            InfectionSiteView.Month2 = this.Month2;
            InfectionSiteView.Month3 = this.Month3;
            InfectionSiteView.CategoryDescription = "Type";
            InfectionSiteView.CountDescription = "Inf";

            QuarterlyInfectionTable<InfectionSite>.LoadTable(
                InfectionSiteView,
                data,
                x => x.Total,
                x => x.Month,
                x => x.InfectionSite.Name,
                x => x.Change,
                x => x.Rate,
                x => x.NonNosoTotal);

            var totalData = new Dictionary<InfectionSite, int>();

            IEnumerable<FacilityMonthInfectionSite.Entry> month1Total = 
                data.Where(m => m.Month == Month1).ToList();

            var month1Data = new Dictionary<InfectionSite, int>();
            ApplyTotals(month1Total, month1Data);
            ApplyTotals(month1Total, totalData);

            IEnumerable<FacilityMonthInfectionSite.Entry> month2Total =
                data.Where(m => m.Month == Month2).ToList();

            var month2Data = new Dictionary<InfectionSite, int>();
            ApplyTotals(month2Total, month2Data);
            ApplyTotals(month2Total, totalData);

            IEnumerable<FacilityMonthInfectionSite.Entry> month3Total =
                data.Where(m => m.Month == Month3).ToList();

            var month3Data = new Dictionary<InfectionSite, int>();
            ApplyTotals(month3Total, month3Data);
            ApplyTotals(month3Total, totalData);


            FillChart(Month1Chart, month1Data);
            FillChart(Month2Chart, month2Data);
            FillChart(Month3Chart, month3Data);
            FillChart(TotalChart, totalData);
        }


        private void FillChart(PieChart chart, Dictionary<InfectionSite, int> totals)
        {
            var totalCount = totals.Select(m => m.Value).Sum();

            int colorIndex = 0;

            foreach (var total in totals.OrderBy(x => x.Key.Name))
            {
                double perc = (Convert.ToDouble(total.Value) / Convert.ToDouble(totalCount) * 100);

                chart.AddItem(new PieChart.Item()
                {
                     Label= total.Key.Name,
                     Marker = perc > 0 ? String.Format("{0:F2}%", perc) : string.Empty,
                     Value = total.Value,
                     Color = PieChart.GetDefaultColor(colorIndex)
                });

                colorIndex++;
            }
        }

        private void ApplyTotals(IEnumerable<FacilityMonthInfectionSite.Entry> data, Dictionary<InfectionSite, int> dest)
        {
            foreach (var d in data)
            {
                if (dest.ContainsKey(d.InfectionSite))
                {
                    dest[d.InfectionSite] = dest[d.InfectionSite] + d.Total;
                }
                else
                {
                    dest[d.InfectionSite] = d.Total;
                }
            }
        }

    }
}
