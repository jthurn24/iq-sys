using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SnyderIS.sCore.Exi.Implementation.Widget.FluentWizard;
using SnyderIS.sCore.Exi.Implementation.Widget;

namespace IQI.Intuition.Exi.Configuration.QIDashboard
{
    public static class AuditConfig
    {

        public static WidgetGroupBuilder Build(WizardBuilder src)
        {
            return src.BeginGroup()
                   .Named("System Audit")
                   .WithDescription("View recent changes made by staff members.")
                   .BeginWidget()
                       .Named("Last 3 Days")
                       .WithDescription("List of changes over the course of the last 3 days.")
                       .IncludeRenderer<Renderers.QIDashboard.Audits.RecentEntriesList>()
                           .Named("List")
                           .WithDescription("List of changes over the course of the last 3 days.")
                           .EndRenderer()
                       .SetDataSource<DataSources.QIDashboard.Audits.DataSource>()
                   .EndWidget();
        }
    }
}
