using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SnyderIS.sCore.Exi.Implementation.Widget.FluentWizard;
using SnyderIS.sCore.Exi.Implementation.Widget;

namespace IQI.Intuition.Exi.Configuration.QIDashboard
{
    public static class WoundConfig
    {

        public static WidgetGroupBuilder Build(WizardBuilder src)
        {
            return src.BeginGroup()
                   .Named("Wounds")
                   .WithDescription("Pressure Ulcers, Surgical Ulcers, and other types of Wounds.")
                   .BeginWidget()
                       .Named("Facility Overview")
                       .WithDescription("List of changes over the course of the last 3 days.")
                       .IncludeRenderer<Renderers.QIDashboard.Wounds.FacilityPressureUlcerOverview>()
                           .Named("List")
                           .WithDescription("List of changes over the course of the last 3 days.")
                           .EndRenderer()
                       .SetDataSource<DataSources.NullDataSource>()
                       .BeginOption()
                       .Hidden()
                       .WithKey("Title")
                       .WithDefaultValue("Facility Wound Overview")
                       .EndOption()
                   .EndWidget()
            .BeginWidget()
                       .Named("Stage Statistics")
                       .WithDescription("Wound stage statistics.")
                       .IncludeRenderer<Renderers.QIDashboard.Wounds.StagePieRate12MonthView>()
                           .Named("Year Rate Breakdown")
                           .WithDescription("Displays 12 months worth of injuries and breaks them down by type of injury. Based on rate calculation of inc/1000PD")
                           .EndRenderer()
                       .IncludeRenderer<Renderers.QIDashboard.Wounds.StageStat3MonthView>()
                           .Named("3 Month Statistics")
                           .WithDescription("Displays 3 months worth of wound stage stats in a table based format. ")
                           .EndRenderer()
                       .SetDataSource<DataSources.QIDashboard.Wounds.WoundStageDataSource>()
                   .EndWidget();
        }
    }
}
