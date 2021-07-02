using System;
using RedArrow.Framework.Extensions.Common;
using SnyderIS.sCore.Web.Mvc.JQGridHelpers;
using Trirand.Web.Mvc;

namespace IQI.Intuition.Web.Models.Complaint
{
    public class ComplaintGrid : GridModel<ComplaintInfo>
    {
        public ComplaintGrid()
        { }

        public ComplaintGrid(string dataUrl)
            : base (dataUrl, null) { }

        protected virtual Func<ComplaintInfo, string> EditUrlFormatter { get; set; }

        protected override void ConfigureGrid(JQGrid grid)
        {
            grid.PagerSettings.NoRowsMessage = "No Complaints match your search criteria";
            grid.AutoWidth = true;
            grid.ShrinkToFit = false;
            grid.Height = new System.Web.UI.WebControls.Unit(300);
            grid.ToolBarSettings.ShowSearchToolBar = true;
            grid.RenderingMode = RenderingMode.Optimized;
            grid.AppearanceSettings.HighlightRowsOnHover = true;
            grid.ClientSideEvents.RowSelect = "rowSelect";


            grid.AddColumns(

                ColumnFor(model => model.EmployeeName,
                    settings =>
                    {
                        settings.HeaderText = "Employee";
                        settings.Width = 170;
                        settings.Sortable = false;
                    }),

               ColumnFor(model => model.PatientName,
                    settings =>
                    {
                        settings.HeaderText = "Patient";
                        settings.Width = 170;
                        settings.Sortable = false;
                    }),

               ColumnFor(model => model.Wing,
                    settings =>
                    {
                        settings.HeaderText = "Wing";
                        settings.Width = 150;
                    }),

               ColumnFor(model => model.ComplaintTypeName,
                    settings =>
                    {
                        settings.HeaderText = "Type";
                        settings.Width = 200;
                    }),

                ColumnFor(model => model.DateReported,
                    settings =>
                    {
                        settings.HeaderText = "Reported";
                        settings.Width = 150;
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
        }
    }
}
