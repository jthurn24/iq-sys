﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SnyderIS.sCore.Exi.Interfaces.Renderer;
using SnyderIS.sCore.Exi.Mvc.Renderers;
using System.Linq.Expressions;
using SnyderIS.sCore.Exi.Interfaces.DataSource;
using IQI.Intuition.Exi.Models.QIDashboard.Infections;
using IQI.Intuition.Reporting.Graphics;

namespace IQI.Intuition.Exi.Renderers.QIDashboard.Wounds
{
    public class FacilityPressureUlcerOverview : MvcRenderer<string>
    {
        public override string Description
        {
            get { throw new NotImplementedException(); }
        }

        public override string Name
        {
            get { throw new NotImplementedException(); }
        }

        public override string RenderHtml(IDataSourceResult<string> data)
        {
            return this.RenderPartialView("~/Views/Shared/Exi/QIDashboard/Wounds/FacilityPressureUlcerOverview.cshtml", null);
        }
    }
}