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
    public class InjuryStat3MonthView : MvcRenderer<MonthlyInjuryTotal>
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

            var table = new Reporting.Tables.QuarterlyStatTable<MonthlyInjuryTotal>();

            var months = data.Metrics.Select(x => x.Month)
            .Distinct()
            .OrderByDescending(x => x.Year)
            .ThenByDescending(x => x.MonthOfYear)
            .Take(3);

            table.Month1 = months.Skip(2).First();
            table.Month2 = months.Skip(1).First();
            table.Month3 = months.First();

            var d = data.Metrics.Where(x=> months.Contains(x.Month));

            Reporting.Tables.QuarterlyStatTable<MonthlyInjuryTotal>.LoadTable(
                table, d,
                 x => x.Total,
                 x => x.Month,
                 x => x.Injury.Name,
                 x => x.Change,
                 x => x.Rate);
            

            return this.RenderPartialView("~/Views/Shared/Exi/QIDashboard/Incidents/InjuryStatsView.cshtml", table);

        }
    }
}
