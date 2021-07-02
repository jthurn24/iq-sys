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
    public class QuarterlyComplaintControlView
    {
        public Guid Quarter { get; set; }

        public IEnumerable<SelectListItem> QuarterOptions { get; set; }

        public Month Month1 { get; set; }
        public Month Month2 { get; set; }
        public Month Month3 { get; set; }

        public SeriesColumnChart ComplaintChart { get; private set; }
        public QuarterlyStatTable<ComplaintType> ComplaintView { get; private set; }
        public CensusTable Census { get; private set; }

        public QuarterlyComplaintControlView()
        {
            ComplaintChart = new Intuition.Reporting.Graphics.SeriesColumnChart();
        }

        public void SetData(QuarterMonths quarter,
            IEnumerable<FacilityMonthCensus> census,
            IEnumerable<FacilityMonthComplaintType.Entry> totals)
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


            ComplaintView = new QuarterlyStatTable<ComplaintType>();
            ComplaintView.CategoryDescription = "Type";
            ComplaintView.CountDescription = "Cmp";
            ComplaintView.Month1 = this.Month1;
            ComplaintView.Month2 = this.Month2;
            ComplaintView.Month3 = this.Month3;


            QuarterlyStatTable<ComplaintType>.LoadTable(
                ComplaintView,
                totals,
                x => x.Total,
                x => x.Month,
                x => x.ComplaintType.Name,
                x => x.Change,
                x => x.Rate);

            ComplaintChart.AddItem(new Intuition.Reporting.Graphics.SeriesColumnChart.Item()
            {
                Category = "Total",
                Series = this.Month1.Name,
                Value = Convert.ToDouble(ComplaintView.Month1Total.Rate)
            });

            ComplaintChart.AddItem(new Intuition.Reporting.Graphics.SeriesColumnChart.Item()
            {
                Category = "Total",
                Series = this.Month2.Name,
                Value = Convert.ToDouble(ComplaintView.Month2Total.Rate)
            });

            ComplaintChart.AddItem(new Intuition.Reporting.Graphics.SeriesColumnChart.Item()
            {
                Category = "Total",
                Series = this.Month3.Name,
                Value = Convert.ToDouble(ComplaintView.Month3Total.Rate)
            });
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

    
}
