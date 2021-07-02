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
    public class QuarterlyInfectionAverageView
    {
        public Guid Quarter { get; set; }
        public Guid AverageType { get; set; }

        public IEnumerable<SelectListItem> QuarterOptions { get; set; }
        public IEnumerable<SelectListItem> AverageTypeOptions { get; set; }

        public Month Month1 { get; set; }
        public Month Month2 { get; set; }
        public Month Month3 { get; set; }

        public NosocomialInfectionTable NosocomialInfections { get; private set; }

        public SeriesColumnChart Month1Chart { get; private set; }
        public SeriesColumnChart Month2Chart { get; private set; }
        public SeriesColumnChart Month3Chart { get; private set; }

        public string AdditionalDetails { get; set; }

        public void SetData(QuarterMonths quarter, 
            IEnumerable<FacilityMonthInfectionType.Entry> facilityData, 
            IEnumerable<AverageTypeMonthInfectionType> averageData,
            IEnumerable<InfectionType> infectionTypes)
        {
            /* Setup Basic Info */

            this.Month1 = quarter.Month1;
            this.Month2 = quarter.Month2;
            this.Month3 = quarter.Month3;

            Month1Chart = new SeriesColumnChart();
            Month2Chart = new SeriesColumnChart();
            Month3Chart = new SeriesColumnChart();


            /* Map Nosocomial Data */

            NosocomialInfections = new NosocomialInfectionTable();
            NosocomialInfections.Groups = new List<NosocomialInfectionGroup>();
            NosocomialInfections.Month1Total = new NosocomialInfectionStat();
            NosocomialInfections.Month2Total = new NosocomialInfectionStat();
            NosocomialInfections.Month3Total = new NosocomialInfectionStat();

            double month1FacilityTotal = 0;
            double month1AverageTotal = 0;
            double month2FacilityTotal = 0;
            double month2AverageTotal = 0;
            double month3FacilityTotal = 0;
            double month3AverageTotal = 0;

            /* Use only infection types that have acutal data for this quarter */

            var filterTypes = infectionTypes.Where(x =>
                (facilityData.Where(xx => xx.Rate > 0 || xx.Change > 0).Select(xx => xx.InfectionType.Id).Contains(x.Id)) ||
                (averageData.Where(xx => xx.Rate > 0 ).Select(xx => xx.InfectionType.Id).Contains(x.Id))
                );

            foreach (var infectionType in filterTypes)
            {
                var group = new NosocomialInfectionGroup()
                {
                    InfectionType = infectionType,
                    Month1Total = new NosocomialInfectionStat(),
                    Month2Total = new NosocomialInfectionStat(),
                    Month3Total = new NosocomialInfectionStat()
                };

                NosocomialInfections.Groups.Add(group);


                /* MOnth 1 */

                group.Month1Total.FacilityRate =
                    facilityData.Where(m => m.Month == Month1
                        && m.InfectionType == infectionType).Sum(m => m.Rate);

                if (averageData.Count() > 0)
                {
                    group.Month1Total.AverageRate = averageData.Where(m => m.Month == Month1
                            && m.InfectionType == infectionType).Sum(m => m.Rate);
                }

                Month1Chart.AddItem(new SeriesColumnChart.Item()
                {
                    Category = infectionType.ShortName,
                    Series = "Facility",
                    Value = Convert.ToDouble(group.Month1Total.FacilityRate)
                });


                Month1Chart.AddItem(new SeriesColumnChart.Item()
                {
                    Category = infectionType.ShortName,
                    Series = "Average",
                    Value = Convert.ToDouble(group.Month1Total.AverageRate)
                });

                month1FacilityTotal += Convert.ToDouble(group.Month1Total.FacilityRate);
                month1AverageTotal += Convert.ToDouble(group.Month1Total.AverageRate);
    
                /* Month 2 */

                group.Month2Total.FacilityRate =
                    facilityData.Where(m => m.Month == Month2
                        && m.InfectionType == infectionType).Sum(m => m.Rate);

                if (averageData.Count() > 0)
                {
                    group.Month2Total.AverageRate = averageData.Where(m => m.Month == Month2
                            && m.InfectionType == infectionType).Sum(m => m.Rate);
                }

                Month2Chart.AddItem(new SeriesColumnChart.Item()
                {
                    Category = infectionType.ShortName,
                    Series = "Facility",
                    Value = Convert.ToDouble(group.Month2Total.FacilityRate)
                });

                Month2Chart.AddItem(new SeriesColumnChart.Item()
                {
                    Category = infectionType.ShortName,
                    Series = "Average",
                    Value = Convert.ToDouble(group.Month2Total.AverageRate)
                });

                month2FacilityTotal += Convert.ToDouble(group.Month2Total.FacilityRate);
                month2AverageTotal += Convert.ToDouble(group.Month2Total.AverageRate);

                /* Month 3 */

                group.Month3Total.FacilityRate =
                    facilityData.Where(m => m.Month == Month3
                        && m.InfectionType == infectionType).Sum(m => m.Rate);

                if (averageData.Count() > 0)
                {
                    group.Month3Total.AverageRate = averageData.Where(m => m.Month == Month3
                            && m.InfectionType == infectionType).Sum(m => m.Rate);
                }

                Month3Chart.AddItem(new SeriesColumnChart.Item()
                {
                    Category = infectionType.ShortName,
                    Series = "Facility",
                    Value = Convert.ToDouble(group.Month3Total.FacilityRate)
                });

                Month3Chart.AddItem(new SeriesColumnChart.Item()
                {
                    Category = infectionType.ShortName,
                    Series = "Average",
                    Value = Convert.ToDouble(group.Month3Total.AverageRate)
                });

                month3FacilityTotal += Convert.ToDouble(group.Month3Total.FacilityRate);
                month3AverageTotal += Convert.ToDouble(group.Month3Total.AverageRate);

            }


            Month3Chart.AddItem(new SeriesColumnChart.Item()
            {
                Category = "Total",
                Series = "Facility",
                Value = month3FacilityTotal
            });

            Month3Chart.AddItem(new SeriesColumnChart.Item()
            {
                Category = "Total",
                Series = "Average",
                Value = month3AverageTotal
            });

            Month2Chart.AddItem(new SeriesColumnChart.Item()
            {
                Category = "Total",
                Series = "Facility",
                Value = month2FacilityTotal
            });

            Month2Chart.AddItem(new SeriesColumnChart.Item()
            {
                Category = "Total",
                Series = "Average",
                Value = month2AverageTotal
            });

            Month1Chart.AddItem(new SeriesColumnChart.Item()
            {
                Category = "Total",
                Series = "Facility",
                Value = month1FacilityTotal
            });

            Month1Chart.AddItem(new SeriesColumnChart.Item()
            {
                Category = "Total",
                Series = "Average",
                Value = month1AverageTotal
            });

            NosocomialInfections.Month1Total.FacilityRate = facilityData.Where(m => m.Month == Month1).Sum(m => m.Rate);

            if (averageData.Count() > 0)
            {
                NosocomialInfections.Month1Total.AverageRate = averageData.Where(m => m.Month == Month1).Sum(m => m.Rate);
            }

            NosocomialInfections.Month2Total.FacilityRate = facilityData.Where(m => m.Month == Month2).Sum(m => m.Rate);

            if (averageData.Count() > 0)
            {
                NosocomialInfections.Month2Total.AverageRate = averageData.Where(m => m.Month == Month2).Sum(m => m.Rate);
            }

            NosocomialInfections.Month3Total.FacilityRate = facilityData.Where(m => m.Month == Month3).Sum(m => m.Rate);

            if (averageData.Count() > 0)
            {
                NosocomialInfections.Month3Total.AverageRate = averageData.Where(m => m.Month == Month3).Sum(m => m.Rate);
            }
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
        }

        public class NosocomialInfectionStat
        {
            public decimal AverageRate { get; set; }
            public decimal FacilityRate { get; set; }
        }


    }
}
