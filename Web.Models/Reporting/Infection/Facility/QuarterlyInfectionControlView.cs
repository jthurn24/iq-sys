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

namespace IQI.Intuition.Web.Models.Reporting.Infection.Facility
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

            foreach(var type in totals.Select(x => x.InfectionType).Distinct())
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

            IDictionary<Guid,Color> monthColors = new Dictionary<Guid,Color>();
            monthColors.Add(this.Month1.Id, Intuition.Reporting.Graphics.ColumnChart.GetDefaultColor(0));
            monthColors.Add(this.Month2.Id, Intuition.Reporting.Graphics.ColumnChart.GetDefaultColor(1));
            monthColors.Add(this.Month3.Id, Intuition.Reporting.Graphics.ColumnChart.GetDefaultColor(2));

            /* Map Cenus Info */
            Census = new CensusTable();

            Census.Month1 = new CensusStat()
            {
                AverageCensus = census.Where(m => m.Month.Id == this.Month1.Id).Select(m => m.Average).FirstOrDefault(),
                TimePeriod = census.Where(m => m.Month.Id == this.Month1.Id).Select(m => m.TotalDays).FirstOrDefault(),
                PatientDays = census.Where(m => m.Month.Id == this.Month1.Id).Select(m => m.TotalPatientDays).FirstOrDefault()
            };


            Census.Month2 = new CensusStat()
            {
                AverageCensus = census.Where(m => m.Month.Id == this.Month2.Id).Select(m => m.Average).FirstOrDefault(),
                TimePeriod = census.Where(m => m.Month.Id == this.Month2.Id).Select(m => m.TotalDays).FirstOrDefault(),
                PatientDays = census.Where(m => m.Month.Id == this.Month2.Id).Select(m => m.TotalPatientDays).FirstOrDefault()
            };

            Census.Month3 = new CensusStat()
            {
                AverageCensus = census.Where(m => m.Month.Id == this.Month3.Id).Select(m => m.Average).FirstOrDefault(),
                TimePeriod = census.Where(m => m.Month.Id == this.Month3.Id).Select(m => m.TotalDays).FirstOrDefault(),
                PatientDays = census.Where(m => m.Month.Id == this.Month3.Id).Select(m => m.TotalPatientDays).FirstOrDefault()
            };

            /* Map Nosocomial Data */

            NosocomialInfections = new NosocomialInfectionTable();
            NosocomialInfections.Groups = new List<NosocomialInfectionGroup>();
            NosocomialInfections.Month1Total = new NosocomialInfectionStat();
            NosocomialInfections.Month2Total = new NosocomialInfectionStat();
            NosocomialInfections.Month3Total = new NosocomialInfectionStat();


            foreach (var total in totals.OrderBy(x => x.InfectionType.SortOrder).ToList())
            {

                if (total.Rate > (decimal)0)
                {
                    /* Add item to chart */
                    NosocomialInfectionRateChart.AddItem(new Intuition.Reporting.Graphics.SeriesColumnChart.Item()
                    {
                        Category = total.Month.Name,
                        Series = total.InfectionType.ShortName,
                        Value = Convert.ToDouble(total.Rate),
                    });
                }

                /* Add item to table */
                NosocomialInfectionStat stat;
                NosocomialInfectionGroup group;

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

                stat.Components = total.Components;
                stat.ViewAction = total.ViewAction;



                var statCensus = census.Where(x => x.Month.Id == total.Month.Id).FirstOrDefault();

                if (statCensus != null && statCensus.TotalPatientDays > 0)
                {
                    group.Total += total.Total;
                    group.PatientDays += statCensus.TotalPatientDays;
                }
                
                
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


            foreach (var group in NosocomialInfections.Groups)
            {
                if (group.Rate > (decimal)0)
                {
                    NosocomialInfectionRateChart.AddItem(new Intuition.Reporting.Graphics.SeriesColumnChart.Item()
                    {
                        Series = group.InfectionType.ShortName,
                        Category = "Total",
                        Value = Convert.ToDouble(group.Rate)
                    });
                }
            }




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

        public class NosocomialInfectionStat : AnnotatedEntry
        {
            public int Count { get; set; }
            public decimal Rate { get; set; }
            public decimal Change { get; set; }
            public int NonNosoCount { get; set; }
        }

    }
}
