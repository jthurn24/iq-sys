using System;
using RedArrow.Framework.Extensions.Common;
using SnyderIS.sCore.Web.Mvc.JQGridHelpers;
using Trirand.Web.Mvc;
namespace IQI.Intuition.Web.Models.Catheter
{
    public class PatientCatheterGrid : GridModel<CatheterInfo>
    {
        public PatientCatheterGrid()
        {
            
        }

        public PatientCatheterGrid(string dataUrl)
            : base (dataUrl, null) { }

        protected virtual Func<CatheterInfo, string> EditUrlFormatter { get; set; }

        protected override void ConfigureGrid(JQGrid grid)
        {
            grid.AutoWidth = true;
            grid.ShrinkToFit = false;
            grid.Height = new System.Web.UI.WebControls.Unit(150);
            grid.ToolBarSettings.ShowSearchToolBar = true;
            grid.RenderingMode = RenderingMode.Optimized;
            grid.ToolBarSettings.ShowSearchToolBar = false;
            grid.AppearanceSettings.HighlightRowsOnHover = true;
            grid.ClientSideEvents.RowSelect = "catheterRowSelect";

            grid.AddColumns(

                ColumnFor(model => model.Type,
                    settings =>
                    {
                        settings.HeaderText = "Type";
                        settings.Width = 90;
                        settings.Sortable = false;
                    }),

               ColumnFor(model => model.Material,
                    settings =>
                    {
                        settings.HeaderText = "Material";
                        settings.Width = 90;
                        settings.Sortable = false;
                    }),

              ColumnFor(model => model.Reason,
                    settings =>
                    {
                        settings.HeaderText = "Reason";
                        settings.Width = 90;
                        settings.Sortable = false;
                    }),

                ColumnFor(model => model.StartedOn,
                    settings =>
                    {
                        settings.HeaderText = "Started On";
                        settings.Width = 90;
                        settings.DataFormatString = "{0:MM/dd/yyyy}";
                    }),

                ColumnFor(model => model.DiscontinuedOn,
                    settings =>
                    {
                        settings.HeaderText = "Disc On";
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
            grid.SortSettings.InitialSortColumn = grid.Columns[3].DataField;
            grid.SortSettings.InitialSortDirection = SortDirection.Desc;
        }
    }
}
