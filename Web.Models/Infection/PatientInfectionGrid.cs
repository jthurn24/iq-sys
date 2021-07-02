using System;
using RedArrow.Framework.Extensions.Common;
using SnyderIS.sCore.Web.Mvc.JQGridHelpers;
using Trirand.Web.Mvc;
namespace IQI.Intuition.Web.Models.Infection
{
    public class PatientInfectionGrid : GridModel<InfectionInfo>
    {
        public PatientInfectionGrid(Func<InfectionInfo, string> editUrlFormatter)
        {
            EditUrlFormatter = editUrlFormatter;
        }

        public PatientInfectionGrid(string dataUrl)
            : base (dataUrl, null) { }

        protected virtual Func<InfectionInfo, string> EditUrlFormatter { get; set; }

        protected override void ConfigureGrid(JQGrid grid)
        {
            grid.AutoWidth = true;
            grid.ShrinkToFit = false;
            grid.Height = new System.Web.UI.WebControls.Unit(150);
            grid.ToolBarSettings.ShowSearchToolBar = true;
            grid.RenderingMode = RenderingMode.Optimized;
            grid.ToolBarSettings.ShowSearchToolBar = false;
            grid.AppearanceSettings.HighlightRowsOnHover = true;
            grid.ClientSideEvents.RowSelect = "infectionRowSelect";

            grid.AddColumns(

                ColumnFor(model => model.InfectionSiteTypeName, 
                    settings =>
                    {
                        settings.HeaderText = "Type";
                        settings.Width = 265;
                    }),

                ColumnFor(model => model.FirstNotedOn, 
                    settings => 
                    {
                        settings.HeaderText = "First Noted";
                        settings.Width = 100;
                        settings.DataFormatString = "{0:MM/dd/yyyy}";
                    }),

                ColumnFor(model => model.ReasonForEntry,
                    settings =>
                    {
                        settings.HeaderText = "Reason for Entry";
                        settings.Width = 300;
                    }),

                ColumnFor(model => model.ResolvedOn,
                    settings =>
                    {
                        settings.HeaderText = "Date Resolved";
                        settings.Width = 120;
                    }),

                ColumnFor(model => model.Id,
                    settings =>
                    {
                        settings.PrimaryKey = true;
                        settings.Visible = false;
                    })

            );

            grid.SortSettings.InitialSortColumn = grid.Columns[1].DataField;
            grid.SortSettings.InitialSortDirection = SortDirection.Desc;
        }
    }
}
