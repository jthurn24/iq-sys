using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQI.Intuition.Reporting.Graphics;
using System.Web.Mvc;
using IQI.Intuition.Reporting.Models.Cubes;
using IQI.Intuition.Reporting.Models.Dimensions;
using System.Drawing;
using IQI.Intuition.Reporting.Funnels;
using IQI.Intuition.Reporting.Containers;

namespace IQI.Intuition.Web.Models.Reporting.Infection.Account
{
    public class QuarterlyInfectionControlView
    {

        public Guid Quarter { get; set; }

        public IEnumerable<SelectListItem> QuarterOptions { get; set; }

        public Month Month1 { get; set; }
        public Month Month2 { get; set; }
        public Month Month3 { get; set; }

        public SeriesColumnChart NosocomialInfectionRateChart { get; private set; }
        public NosocomialInfectionTable NosocomialInfections { get; private set; }
        public CensusTable Census { get; private set; }

        public IList<Guid> SelectedFacilities { get; set; }
        public IEnumerable<SelectListItem> FacilityOptions { get; set; }

        public QuarterlyInfectionControlView()
        {
            NosocomialInfectionRateChart = new Intuition.Reporting.Graphics.SeriesColumnChart();
            NosocomialInfections = new NosocomialInfectionTable();
        }

        public void SetData(QuarterMonths quarter,
            IEnumerable<FacilityMonthCensus> census,
            IEnumerable<FacilityMonthInfectionType.Entry> totals)
        {

            /* Scrub infection types that have no value for the 3 months
            (Eliminates old infection types in new reports, and new infection types in old reports */

            foreach (var type in totals.Select(x => x.InfectionType).Distinct())
            {
                if (totals.Where(x => x.InfectionType == type).Sum(x => x.NonNosoTotal + x.Total) < 1)
                {
                    totals = totals.Where(x => x.InfectionType != type);
                }
            }

            /* Setup Basic Info */


            this.Month1 = quarter.Month1;
            this.Month2 = quarter.Month2;
            this.Month3 = quarter.Month3;

            IDictionary<Guid, Color> monthColors = new Dictionary<Guid, Color>();
            monthColors.Add(this.Month1.Id, Intuition.Reporting.Graphics.ColumnChart.GetDefaultColor(0));
            monthColors.Add(this.Month2.Id, Intuition.Reporting.Graphics.ColumnChart.GetDefaultColor(1));
            monthColors.Add(this.Month3.Id, Intuition.Reporting.Graphics.ColumnChart.GetDefaultColor(2));

            /* Map Cenus Info */
            Census = new CensusTable();

            Census.Month1 = new CensusStat()
            {
                AverageCensus = census.Where(m => m.Month.Id == this.Month1.Id).Sum(x => x.Average),
                TimePeriod = census.Where(m => m.Month.Id == this.Month1.Id).Sum(m => m.TotalDays),
                PatientDays = census.Where(m => m.Month.Id == this.Month1.Id).Sum(m => m.TotalPatientDays)
            };


            Census.Month2 = new CensusStat()
            {
                AverageCensus = census.Where(m => m.Month.Id == this.Month2.Id).Sum(m => m.Average),
                TimePeriod = census.Where(m => m.Month.Id == this.Month2.Id).Sum(m => m.TotalDays),
                PatientDays = census.Where(m => m.Month.Id == this.Month2.Id).Sum(m => m.TotalPatientDays)
            };

            Census.Month3 = new CensusStat()
            {
                AverageCensus = census.Where(m => m.Month.Id == this.Month3.Id).Sum(m => m.Average),
                TimePeriod = census.Where(m => m.Month.Id == this.Month3.Id).Sum(m => m.TotalDays),
                PatientDays = census.Where(m => m.Month.Id == this.Month3.Id).Sum(m => m.TotalPatientDays)
            };


            /* Funnel Virtual Cube */
            var groupTotals = totals.CombineFacilityGroup(census).Where(x => x.Month == this.Month1 || x.Month == this.Month2 || x.Month == this.Month3);


            /* Map Nosocomial Data */

            NosocomialInfections = new NosocomialInfectionTable();
            NosocomialInfections.Groups = new List<NosocomialInfectionGroup>();
            NosocomialInfections.Month1Total = new NosocomialInfectionStat();
            NosocomialInfections.Month2Total = new NosocomialInfectionStat();
            NosocomialInfections.Month3Total = new NosocomialInfectionStat();

            foreach (var type in groupTotals.Select(x => x.InfectionType).Distinct().OrderBy(x => x.SortOrder))
            {
                /* Add type to chart */

                var month1Total = groupTotals.Where(x => x.Month == Month1 && x.InfectionType == type).FirstOrDefault();

                if(month1Total != null)
                {
                    NosocomialInfectionRateChart.AddItem(new Intuition.Reporting.Graphics.SeriesColumnChart.Item()
                    {
                        Category = type.ShortName,
                        Series = Month1.Name,
                        Value = Convert.ToDouble(month1Total.Rate),
                    });
                }

                var month2Total = groupTotals.Where(x => x.Month == Month2 && x.InfectionType == type).FirstOrDefault();

                if (month2Total != null)
                {
                    NosocomialInfectionRateChart.AddItem(new Intuition.Reporting.Graphics.SeriesColumnChart.Item()
                    {
                        Category = type.ShortName,
                        Series = Month2.Name,
                        Value = Convert.ToDouble(month2Total.Rate),
                    });
                }

                var month3Total = groupTotals.Where(x => x.Month == Month3 && x.InfectionType == type).FirstOrDefault();

                if (month3Total != null)
                {
                    NosocomialInfectionRateChart.AddItem(new Intuition.Reporting.Graphics.SeriesColumnChart.Item()
                    {
                        Category = type.ShortName,
                        Series = Month3.Name,
                        Value = Convert.ToDouble(month3Total.Rate),
                    });
                }

            }


            /* Add type to table */
            NosocomialInfectionGroup group;
            NosocomialInfectionStat stat;

            foreach (var total in groupTotals.OrderBy(x => x.InfectionType.SortOrder))
            {

                if (NosocomialInfections.Groups.Where(m => m.InfectionType == total.InfectionType).Count() < 1)
                {
                    NosocomialInfections.Groups.Add(
                        new NosocomialInfectionGroup()
                        {
                            InfectionType = total.InfectionType,
                            Month1Total = new NosocomialInfectionStat(),
                            Month2Total = new NosocomialInfectionStat(),
                            Month3Total = new NosocomialInfectionStat()
                        });
                }

                group = NosocomialInfections.Groups.Where(m => m.InfectionType == total.InfectionType).First();

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
                stat.NonNosoCount = total.NonNosoTotal;

                /* add to totals */

                NosocomialInfectionStat groupedStat;

                if (total.Month == this.Month1)
                {
                    groupedStat = NosocomialInfections.Month1Total;
                }
                else if (total.Month == this.Month2)
                {
                    groupedStat = NosocomialInfections.Month2Total;
                }
                else
                {
                    groupedStat = NosocomialInfections.Month3Total;
                }

                groupedStat.Count += total.Total;
                groupedStat.Change += total.Change;
                groupedStat.Rate += total.Rate;
                groupedStat.NonNosoCount += total.NonNosoTotal;
            }


            NosocomialInfectionRateChart.AddItem(new Intuition.Reporting.Graphics.SeriesColumnChart.Item()
            {
                Category = "Total",
                Series = this.Month1.Name,
                Value = Convert.ToDouble(NosocomialInfections.Month1Total.Rate)
            });

            NosocomialInfectionRateChart.AddItem(new Intuition.Reporting.Graphics.SeriesColumnChart.Item()
            {
                Category = "Total",
                Series = this.Month2.Name,
                Value = Convert.ToDouble(NosocomialInfections.Month2Total.Rate)
            });

            NosocomialInfectionRateChart.AddItem(new Intuition.Reporting.Graphics.SeriesColumnChart.Item()
            {
                Category = "Total",
                Series = this.Month3.Name,
                Value = Convert.ToDouble(NosocomialInfections.Month3Total.Rate)
            });


        }


        public class CensusTable
        {
            public CensusStat Month1 { get; set; }
            public CensusStat Month2 { get; set; }
            public CensusStat Month3 { get; set; }
        }

        public class CensusStat
        {
            public decimal AverageCensus { get; set; }
            public int TimePeriod { get; set; }
            public int PatientDays { get; set; }
        }

        public class NosocomialInfectionTable
        {
            public List<NosocomialInfectionGroup> Groups { get; set; }
            public NosocomialInfectionStat Month1Total { get; set; }
            public NosocomialInfectionStat Month2Total { get; set; }
            public NosocomialInfectionStat Month3Total { get; set; }
        }

        public class NosocomialInfectionGroup
        {
            public InfectionType InfectionType { get; set; }
            public NosocomialInfectionStat Month1Total { get; set; }
            public NosocomialInfectionStat Month2Total { get; set; }
            public NosocomialInfectionStat Month3Total { get; set; }

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

        public class NosocomialInfectionStat
        {
            public int Count { get; set; }
            public decimal Rate { get; set; }
            public decimal Change { get; set; }
            public int NonNosoCount { get; set; }
        }

    }
}
