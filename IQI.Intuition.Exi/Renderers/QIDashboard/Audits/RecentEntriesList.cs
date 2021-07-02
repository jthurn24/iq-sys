using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SnyderIS.sCore.Exi.Interfaces.Renderer;
using SnyderIS.sCore.Exi.Mvc.Renderers;
using System.Linq.Expressions;
using SnyderIS.sCore.Exi.Interfaces.DataSource;

namespace IQI.Intuition.Exi.Renderers.QIDashboard.Audits
{
    public class RecentEntriesList : MvcRenderer<IQI.Intuition.Web.Models.Audit.AuditSummaryEntry>
    {
        public override string Description
        {
            get { throw new NotImplementedException(); }
        }

        public override string Name
        {
            get { throw new NotImplementedException(); }
        }

        public override string RenderHtml(IDataSourceResult<IQI.Intuition.Web.Models.Audit.AuditSummaryEntry> data)
        {
            return this.RenderPartialView("~/Views/Shared/Exi/QIDashboard/Audits/RecentEntriesList.cshtml", data.Metrics);
        }
    }
}
