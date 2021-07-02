using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SnyderIS.sCore.Exi.Interfaces.Renderer;
using SnyderIS.sCore.Exi.Mvc.Renderers;
using System.Linq.Expressions;
using SnyderIS.sCore.Exi.Interfaces.DataSource;


namespace IQI.Intuition.Exi.Renderers.QICast
{
    public class Statistic : MvcRenderer<Exi.Models.QICast.Statistic>
    {
        public override string RenderHtml(IDataSourceResult<Exi.Models.QICast.Statistic> data)
        {
            var statistic = data.Metrics.First();
            return this.RenderPartialView("~/Views/Shared/Exi/QICast/Statistic.cshtml", statistic);
        }

        public override string Description
        {
            get { throw new NotImplementedException(); }
        }

        public override string Name
        {
            get { throw new NotImplementedException(); }
        }
    }
}
