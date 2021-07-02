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
    public class QuarterlyWoundControlView
    {
        public Guid Quarter { get; set; }
        public IEnumerable<SelectListItem> QuarterOptions { get; set; }

        public Guid WoundType { get; set; }
        public IEnumerable<SelectListItem> WoundTypeOptions { get; set; }
        public string WoundTypeName { get; set; }


        public Month Month1 { get; set; }
        public Month Month2 { get; set; }
        public Month Month3 { get; set; }
        public CensusTable Census { get; private set; }

        public IList<WoundStatGroup> ClassificationStats { get; set; }
        public IList<WoundStatGroup> SiteStats { get; set; }
        public IList<WoundStatGroup> StageStats { get; set; }

        public BodyGraph Month1BodyGraph { get; set; }
        public BodyGraph Month2BodyGraph { get; set; }
        public BodyGraph Month3BodyGraph { get; set; }

        public void SetData(QuarterMonths quarter,
            IEnumerable<FacilityMonthCensus> census,
            IEnumerable<FacilityMonthWoundClassification.Entry> classificationTotals,
            IEnumerable<FacilityMonthWoundSite.Entry> siteTotals,
            IEnumerable<FacilityMonthWoundStage.Entry> stageTotals,
            IEnumerable<WoundStage> allStages)
        {

            /* Setup Basic Info */
            this.Month1 = quarter.Month1;
            this.Month2 = quarter.Month2;
            this.Month3 = quarter.Month3;


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

            /* Map Stage Data */

            StageStats = new List<WoundStatGroup>();

            foreach (var stage in allStages.OrderBy( x=> x.Rating))
            {
                var stageGroup = new WoundStatGroup();
                stageGroup.Name = stage.Name;
                StageStats.Add(stageGroup);

                var stageM1 = stageTotals.Where(x => x.Stage.Id == stage.Id && x.Month.Id == this.Month1.Id).FirstOrDefault();
                var stageM2 = stageTotals.Where(x => x.Stage.Id == stage.Id && x.Month.Id == this.Month2.Id).FirstOrDefault();
                var stageM3 = stageTotals.Where(x => x.Stage.Id == stage.Id && x.Month.Id == this.Month3.Id).FirstOrDefault();

                stageGroup.Month1Stat = stageM1 != null ?
                    new WoundStat() { Change = stageM1.Change, Count = stageM1.Total, PercentageOfPopulation = stageM1.PercentageOfPopulation, Rate = stageM1.Rate, Components = stageM1.Components, ViewAction = stageM1.ViewAction }
                    : new WoundStat() { Change = 0, Count = 0, PercentageOfPopulation = 0, Rate = 0 };

                stageGroup.Month2Stat = stageM2 != null ?
                    new WoundStat() { Change = stageM2.Change, Count = stageM2.Total, PercentageOfPopulation = stageM2.PercentageOfPopulation, Rate = stageM2.Rate, Components = stageM2.Components, ViewAction = stageM2.ViewAction }
                    : new WoundStat() { Change = 0, Count = 0, PercentageOfPopulation = 0, Rate = 0 };

                stageGroup.Month3Stat = stageM3 != null ?
                    new WoundStat() { Change = stageM3.Change, Count = stageM3.Total, PercentageOfPopulation = stageM3.PercentageOfPopulation, Rate = stageM3.Rate, Components = stageM3.Components, ViewAction = stageM3.ViewAction }
                    : new WoundStat() { Change = 0, Count = 0, PercentageOfPopulation = 0, Rate = 0 };

            }


            /* Map Classification Data */

            ClassificationStats = new List<WoundStatGroup>();

            foreach (var classification in classificationTotals.Select(x => x.Classification).Distinct())
            {
                var classificationGroup = new WoundStatGroup();
                classificationGroup.Name = classification.Name;
                ClassificationStats.Add(classificationGroup);

                var classM1 = classificationTotals.Where(x => x.Classification.Id == classification.Id && x.Month.Id == this.Month1.Id).FirstOrDefault();
                var classM2 = classificationTotals.Where(x => x.Classification.Id == classification.Id && x.Month.Id == this.Month2.Id).FirstOrDefault();
                var classM3 = classificationTotals.Where(x => x.Classification.Id == classification.Id && x.Month.Id == this.Month3.Id).FirstOrDefault();

                classificationGroup.Month1Stat = classM1 != null ?
                    new WoundStat() { Change = classM1.Change, Count = classM1.Total, PercentageOfPopulation = classM1.PercentageOfPopulation, Rate = classM1.Rate, Components = classM1.Components, ViewAction = classM1.ViewAction }
                    : new WoundStat() { Change = 0, Count = 0, PercentageOfPopulation = 0, Rate = 0 };

                classificationGroup.Month2Stat = classM2 != null ?
                    new WoundStat() { Change = classM2.Change, Count = classM2.Total, PercentageOfPopulation = classM2.PercentageOfPopulation, Rate = classM2.Rate, Components = classM2.Components, ViewAction = classM2.ViewAction }
                    : new WoundStat() { Change = 0, Count = 0, PercentageOfPopulation = 0, Rate = 0 };

                classificationGroup.Month3Stat = classM3 != null ?
                    new WoundStat() { Change = classM3.Change, Count = classM3.Total, PercentageOfPopulation = classM3.PercentageOfPopulation, Rate = classM3.Rate, Components = classM3.Components, ViewAction = classM3.ViewAction }
                    : new WoundStat() { Change = 0, Count = 0, PercentageOfPopulation = 0, Rate = 0 };

            }

            /* Map Stage Data */

            SiteStats = new List<WoundStatGroup>();

            Month1BodyGraph = new BodyGraph();
            Month2BodyGraph = new BodyGraph();
            Month3BodyGraph = new BodyGraph();

            foreach (var site in siteTotals.Select(x => x.Site).Distinct())
            {
                var siteGroup = new WoundStatGroup();
                siteGroup.Name = site.Name;


                var siteM1 = siteTotals.Where(x => x.Site.Id == site.Id && x.Month.Id == this.Month1.Id).FirstOrDefault();
                var siteM2 = siteTotals.Where(x => x.Site.Id == site.Id && x.Month.Id == this.Month2.Id).FirstOrDefault();
                var siteM3 = siteTotals.Where(x => x.Site.Id == site.Id && x.Month.Id == this.Month3.Id).FirstOrDefault();

                siteGroup.Month1Stat = siteM1 != null ?
                    new WoundStat() { Change = siteM1.Change, Count = siteM1.Total, PercentageOfPopulation = siteM1.PercentageOfPopulation, Rate = siteM1.Rate, Components = siteM1.Components, ViewAction = siteM1.ViewAction }
                    : new WoundStat() { Change = 0, Count = 0, PercentageOfPopulation = 0, Rate = 0 };

                siteGroup.Month2Stat = siteM2 != null ?
                    new WoundStat() { Change = siteM2.Change, Count = siteM2.Total, PercentageOfPopulation = siteM2.PercentageOfPopulation, Rate = siteM2.Rate, Components = siteM2.Components, ViewAction = siteM2.ViewAction }
                    : new WoundStat() { Change = 0, Count = 0, PercentageOfPopulation = 0, Rate = 0 };

                siteGroup.Month3Stat = siteM3 != null ?
                    new WoundStat() { Change = siteM3.Change, Count = siteM3.Total, PercentageOfPopulation = siteM3.PercentageOfPopulation, Rate = siteM3.Rate, Components = siteM3.Components, ViewAction = siteM3.ViewAction }
                    : new WoundStat() { Change = 0, Count = 0, PercentageOfPopulation = 0, Rate = 0 };


                if (siteGroup.Month1Stat.Count > 0 || siteGroup.Month2Stat.Count > 0 || siteGroup.Month3Stat.Count > 0
                    || siteGroup.Month1Stat.Change != 0 || siteGroup.Month2Stat.Change != 0 || siteGroup.Month3Stat.Change != 0)
                {
                    SiteStats.Add(siteGroup);
                }


                if (siteM1 != null)
                {
                    var m1Total = siteTotals.Where(x => x.Month.Id == this.Month1.Id).Sum(x => x.Total);
                    int m1Opacity = GetOpacity(siteM1.Total);


                    foreach (var r in site.GetRectangles())
                    {
                        Month1BodyGraph.Areas.Add(new BodyGraph.Area()
                        {
                            ShadingColor = Color.Red,
                            ShadingOpacity = m1Opacity,
                            BottomRightX = r.BottomRightX,
                            BottomRightY = r.BottomRightY,
                            TopLeftX = r.TopLeftX,
                            TopLeftY = r.TopLeftY
                        });
                    }


                }

  
                if (siteM2 != null)
                {
                    var m2Total = siteTotals.Where(x => x.Month.Id == this.Month2.Id).Sum(x => x.Total);
                    int m2Opacity = GetOpacity(siteM2.Total);

                    foreach (var r in site.GetRectangles())
                    {
                        Month2BodyGraph.Areas.Add(new BodyGraph.Area()
                        {
                            ShadingColor = Color.Red,
                            ShadingOpacity = m2Opacity,
                            BottomRightX = r.BottomRightX,
                            BottomRightY = r.BottomRightY,
                            TopLeftX = r.TopLeftX,
                            TopLeftY = r.TopLeftY
                        });
                    }
                }


                if (siteM3 != null)
                {
                    var m3Total = siteTotals.Where(x => x.Month.Id == this.Month3.Id).Sum(x => x.Total);
                    int m3Opacity = GetOpacity(siteM3.Total);

                    foreach (var r in site.GetRectangles())
                    {
                        Month3BodyGraph.Areas.Add(new BodyGraph.Area()
                        {
                            ShadingColor = Color.Red,
                            ShadingOpacity = m3Opacity,
                            BottomRightX = r.BottomRightX,
                            BottomRightY = r.BottomRightY,
                            TopLeftX = r.TopLeftX,
                            TopLeftY = r.TopLeftY
                        });
                    }
                }

            }


        }

        private int GetOpacity(int total)
        {
            if (total < 1)
            {
                return 0;
            }
            else if (total < 4)
            {
                return 15;
            }
            else if (total < 7)
            {
                return 30;
            }
            else if (total < 10)
            {
                return 45;
            }
            else if (total < 13)
            {
                return 60;
            }
            else if (total < 16)
            {
                return 75;
            }
            else if (total < 19)
            {
                return 75;
            }

            return 100;
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


        public class WoundStatGroup
        {
            public string Name { get; set; }
            public WoundStat Month1Stat { get; set; }
            public WoundStat Month2Stat { get; set; }
            public WoundStat Month3Stat { get; set; }
        }

        public class WoundStat : AnnotatedEntry
        {
            public int Count { get; set; }
            public decimal Rate { get; set; }
            public decimal Change { get; set; }
            public decimal PercentageOfPopulation { get; set; }
        }
    }
}
