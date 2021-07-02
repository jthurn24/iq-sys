using System;
using RedArrow.Framework.Extensions.Common;
using SnyderIS.sCore.Web.Mvc.JQGridHelpers;
using Trirand.Web.Mvc;
namespace IQI.Intuition.Web.Models.Incident
{
    public class PatientIncidentGrid : GridModel<IncidentInfo>
    {
        public PatientIncidentGrid(Func<IncidentInfo, string> editUrlFormatter)
        {
            EditUrlFormatter = editUrlFormatter;
        }

        public PatientIncidentGrid(string dataUrl)
            : base (dataUrl, null) { }

        protected virtual Func<IncidentInfo, string> EditUrlFormatter { get; set; }

        protected override void ConfigureGrid(JQGrid grid)
        {
            grid.AutoWidth = true;
            grid.ShrinkToFit = false;
            grid.Height = new System.Web.UI.WebControls.Unit(150);
            grid.ToolBarSettings.ShowSearchToolBar = true;
            grid.RenderingMode = RenderingMode.Optimized;
            grid.ToolBarSettings.ShowSearchToolBar = false;
            grid.AppearanceSettings.HighlightRowsOnHover = true;
            grid.ClientSideEvents.RowSelect = "incidentRowSelect";

            grid.AddColumns(

                ColumnFor(model => model.IncidentTypes,
                    settings =>
                    {
                        settings.HeaderText = "Type";
                        settings.Width = 300;
                    }),

                ColumnFor(model => model.InjuryLevel,
                    settings =>
                    {
                        settings.HeaderText = "Injury Severity";
                        settings.Searchable = false;
                        settings.Width = 200;
                    }),

                ColumnFor(model => model.DiscoveredOn,
                    settings =>
                    {
                        settings.HeaderText = "Discovered On";
                        settings.Width = 150;
                        settings.DataFormatString = "{0:MM/dd/yyyy}";
                    }),

                ColumnFor(model => model.OccurredOn,
                    settings =>
                    {
                        settings.HeaderText = "Occurred On";
                        settings.Width = 150;
                        settings.DataFormatString = "{0:MM/dd/yyyy}";
                    }),

                ColumnFor(model => model.Id,
                    settings =>
                    {
                        settings.PrimaryKey = true;
                        settings.Visible = false;
                    })

            );

            // On first display, sort the grid by the "Name" column
            grid.SortSettings.InitialSortColumn = grid.Columns[2].DataField;
            grid.SortSettings.InitialSortDirection = SortDirection.Desc;
        }
    }
}
