using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SnyderIS.sCore.Exi.Interfaces.Renderer;
using SnyderIS.sCore.Exi.Mvc.Renderers;
using System.Linq.Expressions;
using SnyderIS.sCore.Exi.Interfaces.DataSource;


namespace IQI.Intuition.Exi.Renderers.QIDashboard.FloorMap
{
    public class FloorMapOverview : MvcRenderer<Models.QICast.FloorMapView>
    {
        public override string Description
        {
            get { throw new NotImplementedException(); }
        }

        public override string Name
        {
            get { throw new NotImplementedException(); }
        }

        public override string RenderHtml(IDataSourceResult<Models.QICast.FloorMapView> model)
        {
            model.Metrics.First().FloorMap.ResizeMultipler = .5M;
            model.Metrics.First().FloorMap.DarkMode = false;
            return this.RenderPartialView("~/Views/Shared/Exi/QIDashboard/FloorMap/FloorMapOverview.cshtml", model.Metrics.First());
        }
    }
}
