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

namespace IQI.Intuition.Web.Models.Reporting.Catheter.Facility
{
    public class QuarterlyCatheterControlView
    {
        public Guid Quarter { get; set; }

        public IEnumerable<SelectListItem> QuarterOptions { get; set; }

        public Month Month1 { get; set; }
        public Month Month2 { get; set; }
        public Month Month3 { get; set; }

        public SeriesLineChart CatheterChart { get; private set; }
        public CatheterTable Catheters { get; private set; }
        public CensusTable Census { get; private set; }

        public QuarterlyCatheterControlView()
        {
            CatheterChart = new Intuition.Reporting.Graphics.SeriesLineChart();
            Catheters = new CatheterTable();
        }

        public void SetData(QuarterMonths quarter,
            IEnumerable<FacilityMonthCensus> census,
            IEnumerable<FacilityMonthCatheter.Entry> totals)
        {

  
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

            Catheters = new CatheterTable();
            Catheters.Groups = new List<CatheterGroup>();
            Catheters.Month1Total = new CatheterStat();
            Catheters.Month2Total = new CatheterStat();
            Catheters.Month3Total = new CatheterStat();


            foreach (var total in totals)
            {
                /* Add item to chart */
                CatheterChart.AddItem(new Intuition.Reporting.Graphics.SeriesLineChart.Item()
                {
                    Category = total.Month.Name,
                    Series = total.CatheterType.Name,
                    Value = Convert.ToDouble(total.UtilizationRatio),
                });

                /* Add item to table */
                CatheterStat stat;
                CatheterGroup group;

                if (Catheters.Groups.Where(m => m.CatheterType == total.CatheterType.Name).Count() < 1)
                {
                    Catheters.Groups.Add(
                        new CatheterGroup()
                        {
                            CatheterType = total.CatheterType.Name,
                            Month1Total = new CatheterStat(),
                            Month2Total = new CatheterStat(),
                            Month3Total = new CatheterStat()
                        });
                }

                group = Catheters.Groups.Where(m => m.CatheterType == total.CatheterType.Name).First();

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
                stat.DeviceDays = total.DeviceDays;
                stat.UtilizationRatio = total.UtilizationRatio;
                stat.Components = total.Components;
                stat.ViewAction = total.ViewAction;

                var statCensus = census.Where(x => x.Month.Id == total.Month.Id).FirstOrDefault();

                if (statCensus != null && statCensus.TotalPatientDays > 0)
                {
                    group.Total += total.Total;
                    group.PatientDays += statCensus.TotalPatientDays;
                }

                group.DeviceDays += total.DeviceDays;

                
                /* add to month totals */

                CatheterStat groupedStat;

                int groupedPatientDays = 0;

                if (total.Month == this.Month1)
                {
                    groupedStat = Catheters.Month1Total;
                    groupedPatientDays = Census.Month1.PatientDays; 
                }
                else if (total.Month == this.Month2)
                {
                    groupedStat = Catheters.Month2Total;
                    groupedPatientDays = Census.Month2.PatientDays; 
                }
                else
                {
                    groupedStat = Catheters.Month3Total;
                    groupedPatientDays = Census.Month3.PatientDays; 
                }

                groupedStat.Count += total.Total;
                groupedStat.Change += total.Change;
                groupedStat.Rate += total.Rate;
                groupedStat.DeviceDays += total.DeviceDays;

                if (groupedStat.DeviceDays > 0 && groupedPatientDays > 0)
                {
                    groupedStat.UtilizationRatio = (decimal)groupedStat.DeviceDays / (decimal)groupedPatientDays;
                }
                
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

        public class CatheterTable
        {
            public List<CatheterGroup> Groups { get; set; }
            public CatheterStat Month1Total { get; set; }
            public CatheterStat Month2Total { get; set; }
            public CatheterStat Month3Total { get; set; }
        }

        public class CatheterGroup
        {
            public string CatheterType { get; set; }
            public CatheterStat Month1Total { get; set; }
            public CatheterStat Month2Total { get; set; }
            public CatheterStat Month3Total { get; set; }

            public int Total { get; set; }
            public int PatientDays { get; set; }
            public int DeviceDays { get; set; }

            public decimal Rate
            {
                get
                {
                    return Domain.Calculations.Rate1000(this.Total, this.PatientDays);
                }
            }

            public decimal UtilizationRatio
            {
                get
                {
                    if(this.PatientDays > 0 && this.Total > 0)
                    {
                        return (decimal)this.DeviceDays /(decimal)this.PatientDays;
                    }

                    return 0;
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

        public class CatheterStat : AnnotatedEntry
        {
            public int Count { get; set; }
            public decimal Rate { get; set; }
            public decimal Change { get; set; }
            public int DeviceDays { get; set; }
            public decimal UtilizationRatio { get; set; }
        }
    
}
