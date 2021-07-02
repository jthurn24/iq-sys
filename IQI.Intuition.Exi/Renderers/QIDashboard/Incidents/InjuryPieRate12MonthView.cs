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
using System.Drawing;

namespace IQI.Intuition.Exi.Renderers.QIDashboard.Incidents
{
    public class InjuryPieRate12MonthView : MvcRenderer<MonthlyInjuryTotal>
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

            var chart = new PieChart();

            var months = data.Metrics.Select(x => x.Month)
                .Distinct()
                .OrderByDescending(x => x.Year)
                .ThenByDescending(x => x.MonthOfYear)
                .Take(12);

            foreach (var type in data.Metrics.Select(x => x.Injury).Distinct())
            {
                var d = data.Metrics.Where(x => months.Contains(x.Month) && x.Injury == type);
                var c = d.Sum(x => x.CensusPatientDays);
                var r = Domain.Calculations.Rate1000(d.Sum(x => x.Total), c);

                if (d.Count() > 0 && d.Sum(x => x.Total) > 0)
                {
                    chart.AddItem(new PieChart.Item()
                    {
                         Color = System.Drawing.ColorTranslator.FromHtml(type.Color),
                         Label = type.Name,
                         Marker = Math.Round(r,2).ToString(),
                         Value = (double)r
                    });
                }
            }


            return this.RenderPartialView("~/Views/Shared/Exi/QIDashboard/Incidents/InjuryPieView.cshtml", chart);

        }
    }
}
