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
    public class QuarterlySCAUTIView
    {

        public Guid Quarter { get; set; }

        public IEnumerable<SelectListItem> QuarterOptions { get; set; }

        public Month Month1 { get; set; }
        public Month Month2 { get; set; }
        public Month Month3 { get; set; }

        public SeriesLineChart SCaUtiRateChart { get; private set; }
        public SCaUtiTable SCaUtis { get; private set; }

        public QuarterlySCAUTIView()
        {
            SCaUtiRateChart = new Intuition.Reporting.Graphics.SeriesLineChart();
            SCaUtis = new SCaUtiTable();
        }

        public void SetData(QuarterMonths quarter,
            IEnumerable<FacilityMonthSCAUTI.Entry> totals)
        {

            /* Setup Basic Info */


            this.Month1 = quarter.Month1;
            this.Month2 = quarter.Month2;
            this.Month3 = quarter.Month3;



            /* Map Nosocomial Data */

            SCaUtis = new SCaUtiTable();
            SCaUtis.Groups = new List<SCaUtiGroup>();
            SCaUtis.Groups.Add(new SCaUtiGroup());
            SCaUtis.Groups.First().Month1Total = new SCaUtiStat();
            SCaUtis.Groups.First().Month2Total = new SCaUtiStat();
            SCaUtis.Groups.First().Month3Total = new SCaUtiStat();

            foreach (var total in totals.OrderBy(x => x.Month.Year).OrderBy(x => x.Month.MonthOfYear))
            {

                /* Add item to table */
                SCaUtiStat stat;
                SCaUtiGroup group = SCaUtis.Groups.First();


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
                stat.Components = total.Components;
                stat.ViewAction = total.ViewAction;

                group.Total += total.Total;
                group.DeviceDays += total.DeviceDays;
                
                
            }


            SCaUtiRateChart.AddItem(new Intuition.Reporting.Graphics.SeriesLineChart.Item()
            {
                Series = "SCAUTI RATE",
                Category = this.Month1.Name,
                Value = (double)SCaUtis.Groups.First().Month1Total.Rate
            });

            SCaUtiRateChart.AddItem(new Intuition.Reporting.Graphics.SeriesLineChart.Item()
            {
                Series = "SCAUTI RATE",
                Category = this.Month2.Name,
                Value = (double)SCaUtis.Groups.First().Month2Total.Rate
            });

            SCaUtiRateChart.AddItem(new Intuition.Reporting.Graphics.SeriesLineChart.Item()
            {
                Series = "SCAUTI RATE",
                Category = this.Month3.Name,
                Value = (double)SCaUtis.Groups.First().Month3Total.Rate
            });


        }


 

        public class SCaUtiTable
        {
            public List<SCaUtiGroup> Groups { get; set; }
        }

        public class SCaUtiGroup
        {
            public InfectionType InfectionType { get; set; }
            public SCaUtiStat Month1Total { get; set; }
            public SCaUtiStat Month2Total { get; set; }
            public SCaUtiStat Month3Total { get; set; }

            public int Total { get; set; }
            public int DeviceDays { get; set; }

            public decimal Rate
            {
                get
                {
                    return Domain.Calculations.Rate1000(this.Total, this.DeviceDays);
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

        public class SCaUtiStat : AnnotatedEntry
        {
            public int Count { get; set; }
            public decimal Rate { get; set; }
            public decimal Change { get; set; }
            public int DeviceDays { get; set; }
        }

    }
}
