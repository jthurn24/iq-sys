using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SnyderIS.sCore.Exi.Interfaces.Renderer;
using SnyderIS.sCore.Exi.Mvc.Renderers;
using System.Linq.Expressions;
using SnyderIS.sCore.Exi.Interfaces.DataSource;
using IQI.Intuition.Exi.Models.QIDashboard.Infections;
using IQI.Intuition.Reporting.Graphics;

namespace IQI.Intuition.Exi.Renderers.QIDashboard.Infections
{
    public class ActiveConfirmedChart : MvcRenderer<InfectionTotal>
    {
        public override string Description
        {
            get { throw new NotImplementedException(); }
        }

        public override string Name
        {
            get { throw new NotImplementedException(); }
        }

        public override string RenderHtml(IDataSourceResult<InfectionTotal> data)
        {
            var chart = new ColumnChart();

            foreach (var d in data.Metrics)
            {
                chart.AddItem(
                    new ColumnChart.Item()
                    {
                        Category = d.Name,
                        Value = d.Total,
                        Color = d.Color
                    });
            }

            return this.RenderPartialView("~/Views/Shared/Exi/QICommon/Infections/ActiveConfirmedChart.cshtml", chart);
        }
    }
}
