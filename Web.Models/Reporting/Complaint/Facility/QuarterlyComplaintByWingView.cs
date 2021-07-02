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

namespace IQI.Intuition.Web.Models.Reporting.Complaint.Facility
{
    public class QuarterlyComplaintByWingView
    {
        public Guid Quarter { get; set; }

        public IEnumerable<SelectListItem> QuarterOptions { get; set; }

        public Month Month1 { get; set; }
        public Month Month2 { get; set; }
        public Month Month3 { get; set; }

        public QuarterlyStatTable<Wing> ComplaintView { get; private set; }
        public IEnumerable<SelectListItem> ComplaintTypeOptions { get; set; }
        public Guid? ComplaintType { get; set; }
        public string ComplaintTypeDescription { get; set; }

        public PieChart Month1Chart { get; private set; }
        public PieChart Month2Chart { get; private set; }
        public PieChart Month3Chart { get; private set; }
        public PieChart TotalChart { get; private set; }

        public QuarterlyComplaintByWingView()
        {
        }

        public void SetData(QuarterMonths quarter,
            IEnumerable<FacilityMonthCensus> census,
            IEnumerable<WingMonthComplaintType.Entry> totals)
        {

            this.Month1 = quarter.Month1;
            this.Month2 = quarter.Month2;
            this.Month3 = quarter.Month3;

            IDictionary<Guid, Color> monthColors = new Dictionary<Guid, Color>();
            monthColors.Add(this.Month1.Id, Intuition.Reporting.Graphics.ColumnChart.GetDefaultColor(0));
            monthColors.Add(this.Month2.Id, Intuition.Reporting.Graphics.ColumnChart.GetDefaultColor(1));
            monthColors.Add(this.Month3.Id, Intuition.Reporting.Graphics.ColumnChart.GetDefaultColor(2));



            ComplaintView = new QuarterlyStatTable<Wing>();
            ComplaintView.CategoryDescription = "Wing";
            ComplaintView.CountDescription = "Cmp";
            ComplaintView.Month1 = this.Month1;
            ComplaintView.Month2 = this.Month2;
            ComplaintView.Month3 = this.Month3;

            QuarterlyStatTable<ComplaintType>.LoadTable(
             ComplaintView,
             totals,
             x => x.Total,
             x => x.Month,
             x => string.Concat(x.Wing.Floor.Name,"-",x.Wing.Name),
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
            IEnumerable<WingMonthComplaintType.Entry> data)
        {
            var sections = data.Select(x => x.Wing).Distinct().OrderBy(x => x.Floor.Name).ThenBy(x => x.Name);
            int colorIndex = 0;

            foreach (var section in sections)
            {
                var total = data.Sum(x => x.Total);
                var matchCount = data.Where(x => x.Wing == section).Sum(x => x.Total);

                double perc = (Convert.ToDouble(matchCount) / Convert.ToDouble(total) * 100);

                chart.AddItem(new PieChart.Item()
                {
                    Label = string.Concat(section.Floor.Name, " - ", section.Name),
                    Marker = perc > 0 ? String.Format("{0:F2}%", perc) : string.Empty,
                    Value = matchCount,
                    Color = PieChart.GetDefaultColor(colorIndex)
                });

                colorIndex++;
            }
        }
    }


        public class WingComplaintTable
        {
            public List<WingComplaintGroup> Groups { get; set; }
            public WingComplaintStat Month1Total { get; set; }
            public WingComplaintStat Month2Total { get; set; }
            public WingComplaintStat Month3Total { get; set; }
        }

        public class WingComplaintGroup
        {
            public string Wing { get; set; }
            public WingComplaintStat Month1Total { get; set; }
            public WingComplaintStat Month2Total { get; set; }
            public WingComplaintStat Month3Total { get; set; }

            public int Total { get; set; }
            public int PatientDays { get; set; }

            public decimal Rate
            {
                get
                {
                    return Domain.Calculations.Rate1000(this.Total, this.PatientDays);
                }
            }

            public int GroupCount
            {
                get
                {
                    int count = 0;

                    if (this.Month1Total != null)
                    {
                        count = count + this.Month1Total.Count;
                    }

                    if (this.Month2Total != null)
                    {
                        count = count + this.Month2Total.Count;
                    }

                    if (this.Month3Total != null)
                    {
                        count = count + this.Month3Total.Count;
                    }

                    return count;
                }
            }
        }

        public class WingComplaintStat
        {
            public int Count { get; set; }
            public decimal Rate { get; set; }
            public decimal Change { get; set; }
            public int NonNosoCount { get; set; }
        }
    
}
