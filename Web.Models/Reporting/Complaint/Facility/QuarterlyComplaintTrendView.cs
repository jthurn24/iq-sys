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

namespace IQI.Intuition.Web.Models.Reporting.Complaint.Facility
{
    public class QuarterlyComplaintTrendView
    {
        public Guid Quarter { get; set; }

        public IEnumerable<SelectListItem> QuarterOptions { get; set; }

        public int Year { get; set; }

        public IList<Month> Months { get; set; }

        public IList<TrendChartGroup> ChartGroups { get; private set; }

        public void SetData(Quarter quarter,
            IEnumerable<FacilityMonthComplaintType.Entry> totals,
            IEnumerable<ComplaintType> types)
        {
            ChartGroups = new List<TrendChartGroup>();


            foreach (var type in types)
            {
                var group = new TrendChartGroup();
                group.Description = type.Name;
                this.ChartGroups.Add(group);
                group.Chart = new SeriesLineChart();

                foreach (var month in this.Months)
                {
                    var cube = totals.Where(x => x.Month.Id == month.Id && x.ComplaintType.Id == type.Id).FirstOrDefault();
                    double total = (double)(cube != null ? cube.Rate : 0);
                    group.Chart.AddItem(new SeriesLineChart.Item() { Category = string.Concat(month.Name, month.Year - 2000), Value = total, Series = type.Name });
                }
            }

        }

      
        public class TrendChartGroup
        {
            public string Description { get; set; }
            public SeriesLineChart Chart { get; set; }
        }

    }
}
