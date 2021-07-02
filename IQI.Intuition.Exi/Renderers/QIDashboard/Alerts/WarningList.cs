using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SnyderIS.sCore.Exi.Interfaces.Renderer;
using SnyderIS.sCore.Exi.Mvc.Renderers;
using System.Linq.Expressions;
using SnyderIS.sCore.Exi.Interfaces.DataSource;

namespace IQI.Intuition.Exi.Renderers.QIDashboard.Alerts
{
    public class WarningList : MvcRenderer<Models.QIDashboard.Alerts.AlertView>
    {
        public override string Description
        {
            get { throw new NotImplementedException(); }
        }

        public override string Name
        {
            get { throw new NotImplementedException(); }
        }

        public override string RenderHtml(IDataSourceResult<Models.QIDashboard.Alerts.AlertView> data)
        {
            return this.RenderPartialView("~/Views/Shared/Exi/QIDashboard/Alerts/AlertView.cshtml", data.Metrics);
        }
    }
}
