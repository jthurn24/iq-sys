using System;
using System.Collections.Generic;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using System.Text;
using IQI.Intuition.Reporting.Graphics;
using System.Web.Mvc;
using IQI.Intuition.Reporting.Models.Cubes;
using IQI.Intuition.Reporting.Models.Dimensions;
using System.Drawing;
using IQI.Intuition.Reporting.Containers;


namespace IQI.Intuition.Web.Models.Reporting.Infection.Facility
{
    public class QuarterlyInfectionTrendView
    {
        public Guid Quarter { get; set; }

        public IEnumerable<SelectListItem> QuarterOptions { get; set; }

        public Month Month1 { get; set; }
        public Month Month2 { get; set; }
        public Month Month3 { get; set; }
        public int Year1 { get; set; }
        public int Year2 { get; set; }
        
        public IList<Month> Months { get; set; }

        public IList<TrendChartGroup> ChartGroups { get; private set; }

        public void SetData(Quarter quarter,
            IEnumerable<FacilityMonthInfectionSite.Entry> totals)
        {
            ChartGroups = new List<TrendChartGroup>();

            if (totals.Select(x => x.Month.Year).Distinct().Count() < 2)
            {
                return;
            }

            var types = totals
                .Where(x => x.Month.Id == this.Month1.Id 
                    || x.Month.Id == this.Month2.Id 
                    || x.Month.Id == this.Month3.Id)
                    .Where(x => x.Total > 0)
                    .Select(x => x.InfectionType)
                    .Distinct(x => x.Id);
            
            var sites = totals.Where(x => x.Month.Id == this.Month1.Id || 
                x.Month.Id == this.Month2.Id || 
                x.Month.Id == this.Month3.Id)
                .Where(x => x.Total > 0)
                .Select(x => x.InfectionSite)
                .Distinct(x => x.Id);

            if (types.Count() < 1 || sites.Count() < 1)
            {
                return;
            }

            Year1 = totals.Select(x => x.Month.Year).Distinct().OrderBy(x => x).First();
            Year2 = totals.Select(x => x.Month.Year).Distinct().OrderBy(x => x).Skip(1).First();

            foreach (var type in types)
            {
                var group = new TrendChartGroup();
                group.Description = type.Name;
                this.ChartGroups.Add(group);
                group.Chart = new SeriesLineChart();
                group.Details = new List<TrendDetail>();

                foreach (var site in sites.Where(x => x.InfectionType.Id == type.Id))
                {
                    foreach (var month in this.Months)
                    {
                        var cube = totals.Where(x => x.Month.Id == month.Id && x.InfectionSite.Id == site.Id).FirstOrDefault();
                        double total = (double)(cube != null ? cube.Rate : 0);
                        group.Chart.AddItem(new SeriesLineChart.Item() { Category = string.Concat(month.Name, month.Year - 2000), Value = total, Series = site.Name });
                    }

                    var detail = new TrendDetail();
                    group.Details.Add(detail);

                    detail.Name = site.Name;

                    var month1Year1Cube = totals.Where(x => x.Month.MonthOfYear == this.Month1.MonthOfYear && x.Month.Year == this.Year1 && x.InfectionSite.Id == site.Id).FirstOrDefault();
                    var month1Year2Cube = totals.Where(x => x.Month.MonthOfYear == this.Month1.MonthOfYear && x.Month.Year == this.Year2 && x.InfectionSite.Id == site.Id).FirstOrDefault();

                    var month2Year1Cube = totals.Where(x => x.Month.MonthOfYear == this.Month2.MonthOfYear && x.Month.Year == this.Year1 && x.InfectionSite.Id == site.Id).FirstOrDefault();
                    var month2Year2Cube = totals.Where(x => x.Month.MonthOfYear == this.Month2.MonthOfYear && x.Month.Year == this.Year2 && x.InfectionSite.Id == site.Id).FirstOrDefault();

                    var month3Year1Cube = totals.Where(x => x.Month.MonthOfYear == this.Month3.MonthOfYear && x.Month.Year == this.Year1 && x.InfectionSite.Id == site.Id).FirstOrDefault();
                    var month3Year2Cube = totals.Where(x => x.Month.MonthOfYear == this.Month3.MonthOfYear && x.Month.Year == this.Year2 && x.InfectionSite.Id == site.Id).FirstOrDefault();

                    detail.Month1Year1Rate = month1Year1Cube != null ? month1Year1Cube.Rate : 0;
                    detail.Month1Year2Rate = month1Year2Cube != null ? month1Year2Cube.Rate : 0;
                    detail.Month1Change = detail.Month1Year2Rate - detail.Month1Year1Rate;
                    detail.Month1Variance = GetVariance(detail.Month1Change, detail.Month1Year1Rate, detail.Month1Year2Rate);


                    detail.Month2Year1Rate = month2Year1Cube != null ? month2Year1Cube.Rate : 0;
                    detail.Month2Year2Rate = month2Year2Cube != null ? month2Year2Cube.Rate : 0;
                    detail.Month2Change = detail.Month2Year2Rate - detail.Month2Year1Rate;
                    detail.Month2Variance = GetVariance(detail.Month2Change, detail.Month2Year1Rate, detail.Month2Year2Rate);

                    detail.Month3Year1Rate = month3Year1Cube != null ? month3Year1Cube.Rate : 0;
                    detail.Month3Year2Rate = month3Year2Cube != null ? month3Year2Cube.Rate : 0;
                    detail.Month3Change = detail.Month3Year2Rate - detail.Month3Year1Rate;
                    detail.Month3Variance = GetVariance(detail.Month3Change, detail.Month3Year1Rate, detail.Month3Year2Rate);
                }
            }

        }

        private decimal? GetVariance(decimal change, decimal year1, decimal year2)
        {
            if (change == 0)
            {
                return 0;
            }

            if (year1 == 0 || year2 == 0)
            {
                return null;
            }

            return ((change / year1) * 100);
        }

        public class TrendChartGroup
        {
            public string Description { get; set; }
            public SeriesLineChart Chart { get; set; }
            public IList<TrendDetail> Details { get; set; }
        }

        public class TrendDetail
        {
            public string Name { get; set; }
            public decimal Month1Year1Rate { get; set; }
            public decimal Month1Year2Rate { get; set; }
            public decimal Month1Change { get; set; }
            public decimal? Month1Variance { get; set; }

            public decimal Month2Year1Rate { get; set; }
            public decimal Month2Year2Rate { get; set; }
            public decimal Month2Change { get; set; }
            public decimal? Month2Variance { get; set; }

            public decimal Month3Year1Rate { get; set; }
            public decimal Month3Year2Rate { get; set; }
            public decimal Month3Change { get; set; }
            public decimal? Month3Variance { get; set; }


            public string Month1VarianceText
            {
                get
                {
                    if (this.Month1Variance.HasValue)
                    {
                        return string.Concat(this.Month1Variance.Value.ToString("#0.00"),"%");
                    }
                    else
                    {
                        return ">100%";
                    }
                }
            }

            public string Month2VarianceText
            {
                get
                {
                    if (this.Month2Variance.HasValue)
                    {
                        return string.Concat(this.Month2Variance.Value.ToString("#0.00"), "%");
                    }
                    else
                    {
                        return ">100%";
                    }
                }
            }

            public string Month3VarianceText
            {
                get
                {
                    if (this.Month3Variance.HasValue)
                    {
                        return string.Concat(this.Month3Variance.Value.ToString("#0.00"), "%");
                    }
                    else
                    {
                        return ">100%";
                    }
                }
            }
        }

    }
}
