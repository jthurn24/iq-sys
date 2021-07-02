using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SnyderIS.sCore.Exi.Implementation.Widget.FluentWizard;
using IQI.Intuition.Infrastructure.Services;
using IQI.Intuition.Reporting.Repositories;

namespace IQI.Intuition.Exi.Configuration.QICast
{
    public class WizardBuilder : SnyderIS.sCore.Exi.Implementation.Widget.FluentWizard.WizardBuilder
    {

        public WizardBuilder(IActionContext context,
            IDimensionRepository dimensionRepository)
        {
            var floorMapList = dimensionRepository.GetFloorMapsForFacility(context.CurrentFacility.Guid);

            this.BeginGroup()
                   .Named("Visual Floor Maps")
                   .WithDescription("Components displaying physical location of past and ongoing events within the facility. ")
                   .BeginWidget()
                       .Named("Facility Overview")
                       .WithDescription("Overview displaying wounds, infections and incidents visually on a floormap.")
                       .IncludeRenderer<Renderers.QICast.FloorMapOverview>()
                           .Named("Floormap Full Overview - Small")
                           .WithDescription("Small version of a selected floormap.")
                           .EndRenderer()
                       .IncludeRenderer<Renderers.QICast.FloorMapOverviewLarge>()
                           .Named("Floormap Full Overview - Larger")
                           .WithDescription("Larger version of a selected floormap")
                           .EndRenderer()
                       .SetDataSource<DataSources.QICast.FloorMap.DataSource>()
                       .BeginOption()
                        .WithKey(DataSources.QICast.FloorMap.DataSource.FLOORMAP_GUID_KEY)
                        .WithLabel("Select a floormap to display")
                        .WithSelectList(floorMapList.Select(x => new KeyValuePair<string,string>(x.Id.ToString(),x.Name)))
                        .Required()
                       .EndOption()
                   .EndWidget();

            this.BeginGroup()
               .Named("QA Statistics")
               .WithDescription("Components displaying specific quality assurance statistics.")
               .BeginWidget()
                   .Named("Facility Overview")
                   .WithDescription("Statistics calculated based on the faclity as a whole.")
                   .IncludeRenderer<Renderers.QICast.Statistic>()
                       .Named("Small Format")
                       .WithDescription("Small statistic format.")
                       .EndRenderer()
                   .SetDataSource<DataSources.QICast.Statistics.DataSource>()
                   .BeginOption()
                   .WithKey(DataSources.QICast.Statistics.DataSource.STAT_PROVIDER_TYPE_NAME)
                   .WithLabel("Statistic Source")
                   .WithSelectList(DataSources.QICast.Statistics.DataSource.GetAvailableProviders())
                   .EndOption()
                   .BeginOption()
                        .WithKey(DataSources.QICast.Statistics.DataSource.FACILITY_GUID_KEY)
                        .Hidden()
                        .WithDefaultValue(context.CurrentFacility.Guid.ToString())
                        .Required()
                    .EndOption()
               .EndWidget();


            this.BeginGroup()
               .Named("Warnings")
               .WithDescription("Components displaying facility warnings and alerts.")
               .BeginWidget()
                   .Named("Facility Overview")
                   .WithDescription("Alerts and warnings generated for the facility as a whole.")
                   .IncludeRenderer<Renderers.QICast.WarningCarousel>()
                       .Named("Warning Carousel")
                       .WithDescription("Rotating visual which displays recent alerts and warnings.")
                       .EndRenderer()
                   .SetDataSource<DataSources.QICast.WarningDataSource>()
                   .BeginOption()
                        .WithKey(DataSources.QICast.WarningDataSource.FACILITY_GUID_KEY)
                        .Hidden()
                        .WithDefaultValue(context.CurrentFacility.Guid.ToString())
                        .Required()
                    .EndOption()
               .EndWidget();

            this.BeginGroup()
               .Named("Family And Resident Concerns")
               .WithDescription("Components displaying resident and family concerns.")
               .BeginWidget()
                   .Named("Facility Overview")
                   .WithDescription("Concerns generated for the facility as a whole.")
                   .IncludeRenderer<Renderers.QICast.ComplaintGraph>()
                       .Named("Bar Chart")
                       .WithDescription("Visual representation of common concerns via bar graph")
                       .EndRenderer()
                   .SetDataSource<DataSources.QICast.Complaints.DataSource>()
                   .BeginOption()
                        .WithKey(DataSources.QICast.Complaints.DataSource.FACILITY_GUID_KEY)
                        .Hidden()
                        .WithDefaultValue(context.CurrentFacility.Guid.ToString())
                        .Required()
                    .EndOption()
                    .BeginOption()
                        .WithKey(DataSources.QICast.Complaints.DataSource.MONTHS_KEY)
                        .WithDefaultValue(6)
                        .WithSelectList(new KeyValuePair<string,string>[] {
                            new KeyValuePair<string,string>("3","3 Months"),
                            new KeyValuePair<string,string>("6","6 Months"),
                            new KeyValuePair<string,string>("12","1 Year"),
                            new KeyValuePair<string,string>("24","2 Years"),
                            new KeyValuePair<string,string>("36","3 Years"),
                            new KeyValuePair<string,string>("48","4 Years"),
                            new KeyValuePair<string,string>("60","5 Years"),
                            new KeyValuePair<string,string>("72","6 Years"),
                        })
                        .Required()
                    .EndOption()
               .EndWidget();


            this.BeginGroup()
               .Named("Custom Non-QI Items")
               .WithDescription("Add a unique items to your dashboad. ")
               .BeginWidget()
                   .Named("Special Note")
                   .WithDescription("Add a special message to staff. Example: Compliment staff on a job well done.")
                   .IncludeRenderer<Renderers.QICast.NoteView>()
                       .Named("Sticky Note")
                       .WithDescription("Display a message (and optionally a picture) using a sticky note.")
                       .EndRenderer()
                   .SetDataSource<DataSources.QICast.Note.DataSource>()
                   .BeginOption()
                        .WithKey(DataSources.QICast.Note.DataSource.FACILITY_GUID_KEY)
                        .Hidden()
                        .WithDefaultValue(context.CurrentFacility.Guid.ToString())
                        .Required()
                    .EndOption()
                    .BeginOption()
                        .WithKey(DataSources.QICast.Note.DataSource.BG_STYLE_KEY)
                        .WithDefaultValue(6)
                        .WithSelectList(new KeyValuePair<string, string>[] {
                            new KeyValuePair<string,string>("bg-emerald","Emerald"),
                            new KeyValuePair<string,string>("bg-teal","Teal"),
                            new KeyValuePair<string,string>("bg-yellow","Yellow"),
                            new KeyValuePair<string,string>("bg-cobalt","Cobalt"),
                            new KeyValuePair<string,string>("bg-brown","Brown"),
                            new KeyValuePair<string,string>("bg-pink","Pink"),
                            new KeyValuePair<string,string>("bg-lightGreen","Light Green"),
                            new KeyValuePair<string,string>("bg-darkMagenta","Dark Magenta")
                        })
                        .Required()
                    .EndOption()
               .EndWidget();
        }
    }
}
