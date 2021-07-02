using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SnyderIS.sCore.Exi.Interfaces.Renderer;
using SnyderIS.sCore.Exi.Mvc.Renderers;
using System.Linq.Expressions;
using SnyderIS.sCore.Exi.Interfaces.DataSource;
using IQI.Intuition.Exi.Models.QIDashboard.Wounds;
using IQI.Intuition.Reporting.Graphics;
using System.Drawing;

namespace IQI.Intuition.Exi.Renderers.QIDashboard.Wounds
{
    public class StagePieRate12MonthView : MvcRenderer<MonthWoundStageTotal>
    {
        public override string Description
        {
            get { throw new NotImplementedException(); }
        }

        public override string Name
        {
            get { throw new NotImplementedException(); }
        }

        public override string RenderHtml(IDataSourceResult<MonthWoundStageTotal> data)
        {

            var chart = new PieChart();

            var months = data.Metrics.Select(x => x.Month)
                .Distinct()
                .OrderByDescending(x => x.Year)
                .ThenByDescending(x => x.MonthOfYear)
                .Take(12);

            int colorIndex = -1;


            foreach (var type in data.Metrics.Select(x => x.Stage).Distinct())
            {

                colorIndex++;

                var d = data.Metrics.Where(x => months.Contains(x.Month) && x.Stage == type);
                var c = d.Sum(x => x.CensusPatientDays);
                var r = Domain.Calculations.Rate1000(d.Sum(x => x.Total), c);

                if (d.Count() > 0 && d.Sum(x => x.Total) > 0)
                {
                    chart.AddItem(new PieChart.Item()
                    {
                         Color = PieChart.GetDefaultColor(colorIndex),
                         Label = type.Name,
                         Marker = Math.Round(r,2).ToString(),
                         Value = (double)r
                    });
                }
            }


            return this.RenderPartialView("~/Views/Shared/Exi/QIDashboard/Wounds/WoundPieView.cshtml", chart);

        }
    }
}
