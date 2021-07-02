using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SnyderIS.sCore.Exi.Implementation.Widget.FluentWizard;
using SnyderIS.sCore.Exi.Implementation.Widget;

namespace IQI.Intuition.Exi.Configuration.QIDashboard
{
    public class InfectionConfig
    {
        public static WidgetGroupBuilder Build(WizardBuilder src)
        {
            return src.BeginGroup()
                   .Named("Infections")
                   .WithDescription("Confirmed, HAI, Suspected Infections.")
                   .BeginWidget()
                       .Named("Active Confirmed Infections")
                       .WithDescription("Realtime view of active confirmed infections within the facility.")
                       .IncludeRenderer<Renderers.QIDashboard.Infections.ActiveConfirmedChart>()
                           .Named("Bar Chart")
                           .WithDescription("Chart displaying total infections in the facility by type.")
                           .EndRenderer()
                       .SetDataSource<DataSources.QIDashboard.Infections.RealtimeDataSource>()
                   .EndWidget();
        }
    }
}
