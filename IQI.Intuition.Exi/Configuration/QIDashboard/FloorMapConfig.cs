using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SnyderIS.sCore.Exi.Implementation.Widget.FluentWizard;
using SnyderIS.sCore.Exi.Implementation.Widget;
using IQI.Intuition.Reporting.Repositories;
using IQI.Intuition.Infrastructure.Services;

namespace IQI.Intuition.Exi.Configuration.QIDashboard
{
    public class FloorMapConfig
    {
        public static WidgetGroupBuilder Build(WizardBuilder src, 
            IDimensionRepository dimensionRepository,
            IActionContext context)
        {
            var floorMapList = dimensionRepository.GetFloorMapsForFacility(context.CurrentFacility.Guid);

            return src.BeginGroup()
                   .Named("Floor Maps")
                   .WithDescription("Floor Maps")
                   .BeginWidget()
                       .Named("Facility Overview")
                       .WithDescription("A visual overview of events occuring within the facility. ")
                       .IncludeRenderer<Renderers.QIDashboard.FloorMap.FloorMapOverview>()
                           .Named("Floormap Overview")
                           .WithDescription("A visual overview of events occuring within the facility. ")
                           .EndRenderer()
                       .SetDataSource<DataSources.QICast.FloorMap.DataSource>()
                       .BeginOption()
                        .WithKey(DataSources.QICast.FloorMap.DataSource.FLOORMAP_GUID_KEY)
                        .WithLabel("Floor Map Guid")
                        .WithSelectList(floorMapList.Select(x => new KeyValuePair<string, string>(x.Id.ToString(), x.Name)))
                        .Required()
                       .EndOption()
                   .EndWidget();
        }
    }
}
