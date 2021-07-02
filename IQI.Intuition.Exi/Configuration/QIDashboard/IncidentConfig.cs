using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SnyderIS.sCore.Exi.Implementation.Widget.FluentWizard;
using SnyderIS.sCore.Exi.Implementation.Widget;
using IQI.Intuition.Reporting.Repositories;

namespace IQI.Intuition.Exi.Configuration.QIDashboard
{
    public class IncidentConfig
    {
        public static WidgetGroupBuilder Build(WizardBuilder src, IDimensionRepository dimensionRepository)
        {

            var groups = dimensionRepository.GetIncidentTypeGroups()
                .Select(x => new KeyValuePair<string,string>(x.Name,x.Name));

            return src.BeginGroup()
                   .Named("Incidents")
                   .WithDescription("Falls, Med Errors, and other misc resident related incidents.")
                   .BeginWidget()
                       .Named("Injuries")
                       .WithDescription("View of incident related injury data.")
                       .IncludeRenderer<Renderers.QIDashboard.Incidents.InjuryTrendRateView>()
                           .Named("Year Rate Trend")
                           .WithDescription("Displays 2 years worth of trending data for incident related injuries. Based on rate calculation of inc/1000PD")
                           .EndRenderer()
                       .IncludeRenderer<Renderers.QIDashboard.Incidents.InjuryTrendTotalView>()
                           .Named("Year Total Trend")
                           .WithDescription("Displays 2 years worth of trending data for incident related injuries. Based on total injuries")
                           .EndRenderer()
                      .IncludeRenderer<Renderers.QIDashboard.Incidents.InjuryPieTotal12MonthView>()
                           .Named("Year Total Breakdown")
                           .WithDescription("Displays 12 months worth of injuries and breaks them down by type of injury. Based on total")
                           .EndRenderer()
                      .IncludeRenderer<Renderers.QIDashboard.Incidents.InjuryPieTotal3MonthView>()
                           .Named("3 Month Total Breakdown")
                           .WithDescription("Displays 3 months worth of injuries and breaks them down by type of injury. Based on total")
                           .EndRenderer()
                       .IncludeRenderer<Renderers.QIDashboard.Incidents.InjuryPieRate12MonthView>()
                           .Named("Year Rate Breakdown")
                           .WithDescription("Displays 12 months worth of injuries and breaks them down by type of injury. Based on rate calculation of inc/1000PD")
                           .EndRenderer()
                      .IncludeRenderer<Renderers.QIDashboard.Incidents.InjuryPieRate3MonthView>()
                           .Named("3 Month Rate Breakdown")
                           .WithDescription("Displays 3 months worth of injuries and breaks them down by type of injury. Based on rate calculation of inc/1000PD")
                           .EndRenderer()
                      .IncludeRenderer<Renderers.QIDashboard.Incidents.InjuryStat3MonthView>()
                           .Named("3 Month Statistics")
                           .WithDescription("Displays 3 months worth of injuries in a table based format. ")
                           .EndRenderer()
                       .SetDataSource<DataSources.QIDashboard.Incidents.InjuryDataSource>()
                       .BeginOption()
                       .WithKey(DataSources.QIDashboard.Incidents.InjuryDataSource.INCIDENT_TYPE_GROUP)
                       .WithSelectList(groups)
                       .WithLabel("Type Of Incident")
                       .EndOption()
                   .EndWidget();
        }
    }
}
