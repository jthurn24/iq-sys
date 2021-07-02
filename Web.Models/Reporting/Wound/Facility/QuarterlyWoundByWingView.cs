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
using IQI.Intuition.Reporting.Models;

namespace IQI.Intuition.Web.Models.Reporting.Wound.Facility
{
    public class QuarterlyWoundByWingView
    {
        public Guid Quarter { get; set; }

        public IEnumerable<SelectListItem> QuarterOptions { get; set; }

        public Month Month1 { get; set; }
        public Month Month2 { get; set; }
        public Month Month3 { get; set; }

        public WingWoundTable Wounds { get; private set; }
        public IEnumerable<SelectListItem> WoundTypeOptions { get; set; }
        public Guid? WoundType { get; set; }
        public string WoundTypeDescription { get; set; }

        public PieChart Month1Chart { get; private set; }
        public PieChart Month2Chart { get; private set; }
        public PieChart Month3Chart { get; private set; }
        public PieChart TotalChart { get; private set; }

        public QuarterlyWoundByWingView()
        {
            Wounds = new WingWoundTable();
        }

        public void SetData(QuarterMonths quarter,
            IEnumerable<FacilityMonthCensus> census,
            IEnumerable<WingMonthWoundType.Entry> totals)
        {

            /* Setup Basic Info */

            this.Month1 = quarter.Month1;
            this.Month2 = quarter.Month2;
            this.Month3 = quarter.Month3;

            IDictionary<Guid, Color> monthColors = new Dictionary<Guid, Color>();
            monthColors.Add(this.Month1.Id, Intuition.Reporting.Graphics.ColumnChart.GetDefaultColor(0));
            monthColors.Add(this.Month2.Id, Intuition.Reporting.Graphics.ColumnChart.GetDefaultColor(1));
            monthColors.Add(this.Month3.Id, Intuition.Reporting.Graphics.ColumnChart.GetDefaultColor(2));



            Wounds = new WingWoundTable();
            Wounds.Groups = new List<WingWoundGroup>();
            Wounds.Month1Total = new WingWoundStat();
            Wounds.Month2Total = new WingWoundStat();
            Wounds.Month3Total = new WingWoundStat();


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


            foreach (var total in totals)
            {

               /* Add item to table */
                WingWoundStat stat;
                WingWoundGroup group;

                if (Wounds.Groups.Where(m => m.Wing == string.Concat(total.Wing.Floor.Name, "-", total.Wing.Name)).Count() < 1)
                {
                    Wounds.Groups.Add(
                        new WingWoundGroup()
                        {
                            Wing = string.Concat(total.Wing.Floor.Name, "-", total.Wing.Name),
                            Month1Total = new WingWoundStat(),
                            Month2Total = new WingWoundStat(),
                            Month3Total = new WingWoundStat()
                        });
                }

                group = Wounds.Groups.Where(m => m.Wing == string.Concat(total.Wing.Floor.Name, "-", total.Wing.Name)).First();

                if (total.Month == this.Month1)
                {
                    stat = group.Month1Total;
                }
                else if (total.Month == this.Month2)
                {
                    stat = group.Month2Total;
                }
                else
                {
                    stat = group.Month3Total;
                }

                stat.Count = total.Total;
                stat.Change = total.Change;
                stat.Rate = total.Rate;
                stat.ViewAction = total.ViewAction;
                stat.Components = total.Components;

                var statCensus = census.Where(x => x.Month.Id == total.Month.Id).FirstOrDefault();

                if (statCensus != null && statCensus.TotalPatientDays > 0)
                {
                    group.Total += total.Total;
                    group.PatientDays += statCensus.TotalPatientDays;
                }


                /* add to totals */

                WingWoundStat groupedStat;

                if (total.Month == this.Month1)
                {
                    groupedStat = Wounds.Month1Total;
                }
                else if (total.Month == this.Month2)
                {
                    groupedStat = Wounds.Month2Total;
                }
                else
                {
                    groupedStat = Wounds.Month3Total;
                }

                groupedStat.Count += total.Total;
                groupedStat.Change += total.Change;
                groupedStat.Rate += total.Rate;


            }

        }


        private void LoadChart(PieChart chart,
            IEnumerable<WingMonthWoundType.Entry> data)
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


        public class WingWoundTable
        {
            public List<WingWoundGroup> Groups { get; set; }
            public WingWoundStat Month1Total { get; set; }
            public WingWoundStat Month2Total { get; set; }
            public WingWoundStat Month3Total { get; set; }
        }

        public class WingWoundGroup
        {
            public string Wing { get; set; }
            public WingWoundStat Month1Total { get; set; }
            public WingWoundStat Month2Total { get; set; }
            public WingWoundStat Month3Total { get; set; }

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

        public class WingWoundStat : AnnotatedEntry
        {
            public int Count { get; set; }
            public decimal Rate { get; set; }
            public decimal Change { get; set; }
            public int NonNosoCount { get; set; }
        }
    
}
