using System;
using RedArrow.Framework.Extensions.Common;
using SnyderIS.sCore.Web.Mvc.JQGridHelpers;
using Trirand.Web.Mvc;

namespace IQI.Intuition.Web.Models.Incident
{
    public class IncidentGrid : GridModel<IncidentInfo>
    {
        public IncidentGrid(Func<IncidentInfo, string> editUrlFormatter)
        { 
            EditUrlFormatter = editUrlFormatter;
        }

        public IncidentGrid(string dataUrl)
            : base (dataUrl, null) { }

        protected virtual Func<IncidentInfo, string> EditUrlFormatter { get; set; }

        protected override void ConfigureGrid(JQGrid grid)
        {
            grid.PagerSettings.NoRowsMessage = "No Incidents match your search criteria";
            grid.AutoWidth = true;
            grid.ShrinkToFit = false; 
            grid.Height = new System.Web.UI.WebControls.Unit(300);
            grid.ToolBarSettings.ShowSearchToolBar = true;
            grid.RenderingMode = RenderingMode.Optimized;
            grid.AppearanceSettings.HighlightRowsOnHover = true;
            grid.ClientSideEvents.RowSelect = "incidentRowSelect";
            grid.PagerSettings.PageSizeOptions = "[100,200,300]";
            grid.PagerSettings.PageSize = 100;

            grid.AddColumns(

                ColumnFor(model => model.PatientFullName, 
                    settings => 
                    { 
                        settings.HeaderText = "Name";
                        settings.Width = 180;
                    }),

                 ColumnFor(model => model.RoomAndWingName,
                    settings =>
                    {
                        settings.HeaderText = "Room/Wing";
                        settings.Width = 180;
                    }),

                ColumnFor(model => model.IncidentTypes, 
                    settings =>
                    {
                        settings.HeaderText = "Type";
                        settings.Width = 250;
                        settings.Sortable = false;
                    }),

                ColumnFor(model => model.InjuryLevel,
                    settings =>
                    {
                        settings.HeaderText = "Injury";
                        settings.Searchable = true;
                        settings.Width = 100;
                    }),

                ColumnFor(model => model.DiscoveredOn, 
                    settings => 
                    {
                        settings.HeaderText = "Discovered On";
                        settings.Width = 90;
                        settings.DataFormatString = "{0:MM/dd/yyyy}";
                    }),

                ColumnFor(model => model.OccurredOn,
                    settings =>
                    {
                        settings.HeaderText = "Occurred On";
                        settings.Width = 90;
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
            grid.SortSettings.InitialSortColumn = grid.Columns[0].DataField;
            grid.Columns[3].AddEnumSearchDropDown(typeof(Domain.Enumerations.InjuryLevel));
        }
    }
}
