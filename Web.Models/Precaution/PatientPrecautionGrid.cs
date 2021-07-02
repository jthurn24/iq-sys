using System;
using RedArrow.Framework.Extensions.Common;
using SnyderIS.sCore.Web.Mvc.JQGridHelpers;
using Trirand.Web.Mvc;

namespace IQI.Intuition.Web.Models.Precaution
{
    public class PatientPrecautionGrid : GridModel<PatientPrecautionInfo>
    {
        public PatientPrecautionGrid()
        { }

        public PatientPrecautionGrid(string dataUrl)
            : base (dataUrl, null) { }

        protected virtual Func<PatientPrecautionInfo, string> EditUrlFormatter { get; set; }

        protected override void ConfigureGrid(JQGrid grid)
        {
            grid.PagerSettings.NoRowsMessage = "No Patient Precautions match your search criteria";
            grid.AutoWidth = true;
            grid.ShrinkToFit = false;
            grid.Height = new System.Web.UI.WebControls.Unit(150);
            grid.ToolBarSettings.ShowSearchToolBar = true;
            grid.RenderingMode = RenderingMode.Optimized;
            grid.ToolBarSettings.ShowSearchToolBar = false;
            grid.AppearanceSettings.HighlightRowsOnHover = true;
            grid.ClientSideEvents.RowSelect = "precautionRowSelect";
            grid.ClientSideEvents.AfterAjaxRequest = "precautionLoadComplete";

            grid.AddColumns(

                ColumnFor(model => model.PrecautionTypeName,
                    settings =>
                    {
                        settings.HeaderText = "Type";
                        settings.Width = 200;
                        settings.Sortable = true;
                    }),

               ColumnFor(model => model.StartDate,
                    settings =>
                    {
                        settings.HeaderText = "Start Date";
                        settings.Width = 200;
                        settings.Sortable = true;
                        settings.Searchable = false;
                        settings.DataFormatString = "{0:MM/dd/yyyy}";
                    }),


               ColumnFor(model => model.EndDate,
                    settings =>
                    {
                        settings.HeaderText = "End Date";
                        settings.Width = 200;
                        settings.Searchable = false;
                        settings.Sortable = true;
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
            grid.SortSettings.InitialSortColumn = grid.Columns[1].DataField;
            grid.SortSettings.InitialSortDirection = SortDirection.Desc;
        }
    }
}
