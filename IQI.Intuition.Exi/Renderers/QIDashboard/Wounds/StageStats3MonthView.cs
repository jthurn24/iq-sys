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
    public class StageStat3MonthView : MvcRenderer<MonthWoundStageTotal>
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

            var table = new Reporting.Tables.QuarterlyStatTable<MonthWoundStageTotal>();

            var months = data.Metrics.Select(x => x.Month)
            .Distinct()
            .OrderByDescending(x => x.Year)
            .ThenByDescending(x => x.MonthOfYear)
            .Take(3);

            table.Month1 = months.Skip(2).First();
            table.Month2 = months.Skip(1).First();
            table.Month3 = months.First();

            var d = data.Metrics.Where(x=> months.Contains(x.Month));

            Reporting.Tables.QuarterlyStatTable<MonthWoundStageTotal>.LoadTable(
                table, d,
                 x => x.Total,
                 x => x.Month,
                 x => x.Stage.Name,
                 x => x.Change,
                 x => x.Rate);


            return this.RenderPartialView("~/Views/Shared/Exi/QIDashboard/Wounds/WoundStageStatsView.cshtml", table);

        }
    }
}
