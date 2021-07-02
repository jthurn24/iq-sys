﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SnyderIS.sCore.Exi.Interfaces.Renderer;
using SnyderIS.sCore.Exi.Mvc.Renderers;
using System.Linq.Expressions;
using SnyderIS.sCore.Exi.Interfaces.DataSource;

namespace IQI.Intuition.Exi.Renderers.QICast
{
    public class FloorMapOverviewLarge : MvcRenderer<Models.QICast.FloorMapView>
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
            model.Metrics.First().FloorMap.AutoRotate = true;
            return this.RenderPartialView("~/Views/Shared/Exi/QICast/FloorMap.cshtml", model.Metrics.First());
        }
    }
}
