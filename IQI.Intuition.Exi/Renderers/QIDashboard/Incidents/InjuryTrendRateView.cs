using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SnyderIS.sCore.Exi.Interfaces.Renderer;
using SnyderIS.sCore.Exi.Mvc.Renderers;
using System.Linq.Expressions;
using SnyderIS.sCore.Exi.Interfaces.DataSource;
using IQI.Intuition.Exi.Models.QIDashboard.Incidents;
using IQI.Intuition.Reporting.Graphics;
using IQI.Intuition.Reporting.Models.Dimensions;

namespace IQI.Intuition.Exi.Renderers.QIDashboard.Incidents
{
    public class InjuryTrendRateView : MvcRenderer<MonthlyInjuryTotal>
    {
        public override string Description
        {
            get { throw new NotImplementedException(); }
        }

        public override string Name
        {
            get { throw new NotImplementedException(); }
        }

        public override string RenderHtml(IDataSourceResult<MonthlyInjuryTotal> data)
        {

            var chart = new SeriesLineChart();
            chart.ColorOffset = 2;

            var months = data.Metrics.Select(x => x.Month)
                .Distinct()
                .OrderByDescending(x => x.Year)
                .ThenByDescending(x => x.MonthOfYear);


            var series1Months = months.Take(12).Reverse();
            var series2Months = months.Skip(12).Take(12).Reverse();

            foreach (var m in series1Months)
            {
                var mData = data.Metrics.Where(x => x.Month == m);

                if (mData.Count() > 0)
                {
                    var mCensus = mData.Max(x => x.CensusPatientDays);
                    var mTotal = mData.Sum(x => x.Total);
                    var mRate = Domain.Calculations.Rate1000(mTotal, mCensus);

                    chart.AddItem(new SeriesLineChart.Item()
                    {
                        Category = string.Concat(m.Name),
                        Series = BuildDescription(series1Months),
                        Value = (double)mRate
                    });
                }
            }


            if (series2Months.Count() > 1)
            {

                foreach (var m in series2Months)
                {
                    var mData = data.Metrics.Where(x => x.Month == m);

                    if (mData.Count() > 0)
                    {
                        var mCensus = mData.Max(x => x.CensusPatientDays);
                        var mTotal = mData.Sum(x => x.Total);
                        var mRate = Domain.Calculations.Rate1000(mTotal, mCensus);

                        chart.AddItem(new SeriesLineChart.Item()
                        {
                            Category = string.Concat(m.Name),
                            Series = BuildDescription(series2Months),
                            Value = (double)mRate
                        });
                    }
                }
            }


            return this.RenderPartialView("~/Views/Shared/Exi/QIDashboard/Incidents/InjuryTrendView.cshtml", chart);

        }

        private string BuildDescription(IEnumerable<Month> months)
        {
            if (months.Max(x => x.Year) == months.Max(x => x.Year))
            {
                return months.Max(x => x.Year).ToString();
            }

            return String.Concat(months.Min(x => x.Year), "-", months.Max(x => x.Year));

        }
    }
}
