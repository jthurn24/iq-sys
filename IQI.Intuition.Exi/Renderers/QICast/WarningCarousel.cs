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
    public class WarningCarousel : MvcRenderer<Exi.Models.QICast.Warning>
    {
        public override string RenderHtml(IDataSourceResult<Exi.Models.QICast.Warning> data)
        {
            return this.RenderPartialView("~/Views/Shared/Exi/QICast/WarningCarousel.cshtml", data.Metrics);
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
