using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQI.Intuition.Reporting.Graphics;
using System.Web.Mvc;
using IQI.Intuition.Reporting.Models.Cubes;
using IQI.Intuition.Reporting.Models.Dimensions;
using Dimensions = IQI.Intuition.Reporting.Models.Dimensions;
using System.Drawing;
using IQI.Intuition.Reporting.Containers;

namespace IQI.Intuition.Web.Models.Reporting.Infection.Account
{
    public class QuarterlyInfectionByFacilityView
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

        public IList<Guid> SelectedFacilities { get; set; }
        public IEnumerable<SelectListItem> FacilityOptions { get; set; }
        public Domain.Enumerations.InfectionMetric Metric { get; set; }
        public IEnumerable<SelectListItem> MetricOptions { get; set; }


        public void SetData(QuarterMonths quarter, IEnumerable<FacilityMonthInfectionType> data)
        {
            /* Setup Basic Info */

            this.Month1 = quarter.Month1;
            this.Month2 = quarter.Month2;
            this.Month3 = quarter.Month3;

            Month1Chart = new PieChart();
            Month2Chart = new PieChart();
            Month3Chart = new PieChart();
            TotalChart = new PieChart();


            var totalData = new Dictionary<Dimensions.Facility, decimal>();

            


            var month1Data = new Dictionary<Dimensions.Facility, decimal>();
            ApplyTotals(data, month1Data, this.Month1);
            ApplyTotals(data, totalData, this.Month1);


            var month2Data = new Dictionary<Dimensions.Facility, decimal>();
            ApplyTotals(data, month2Data, this.Month2);
            ApplyTotals(data, totalData, this.Month2);

            var month3Data = new Dictionary<Dimensions.Facility, decimal>();
            ApplyTotals(data, month3Data, this.Month3);
            ApplyTotals(data, totalData, this.Month3);

            
            FillChart(Month1Chart, month1Data);
            FillChart(Month2Chart, month2Data);
            FillChart(Month3Chart, month3Data);
            FillChart(TotalChart, totalData);

            
        }


        private void FillChart(PieChart chart, Dictionary<Dimensions.Facility, decimal> totals)
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
                    Value = (double)total.Value,
                    Color = Intuition.Reporting.Graphics.PieChart.GetDefaultColor(index)
                });

                index++;
            }
        }

        private void ApplyTotals(IEnumerable<FacilityMonthInfectionType> data, 
            Dictionary<Dimensions.Facility, decimal> dest,
            Dimensions.Month month)
        {
            
            foreach (var d in data.OrderBy(x => x.Facility.Name))
            {
                foreach (var e in d.Entries.Where(x => x.Month.Id == month.Id))
                {
                    if (dest.ContainsKey(d.Facility))
                    {
                        if (Metric == Domain.Enumerations.InfectionMetric.Rate)
                        {
                            dest[d.Facility] = dest[d.Facility] + e.Rate;
                        }
                        else
                        {
                            dest[d.Facility] = dest[d.Facility] + e.Total;
                        }
                    }
                    else
                    {
                        if (Metric == Domain.Enumerations.InfectionMetric.Rate)
                        {
                            dest[d.Facility] = e.Rate;
                        }
                        else
                        {
                            dest[d.Facility] = e.Total;
                        }
                    }
                }
            }
 
        }
    }
}
