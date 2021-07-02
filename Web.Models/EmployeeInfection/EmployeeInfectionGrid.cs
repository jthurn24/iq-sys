using System;
using RedArrow.Framework.Extensions.Common;
using SnyderIS.sCore.Web.Mvc.JQGridHelpers;
using Trirand.Web.Mvc;

namespace IQI.Intuition.Web.Models.EmployeeInfection
{
    public class EmployeeInfectionGrid : GridModel<EmployeeInfectionInfo>
    {
        public EmployeeInfectionGrid()
        { }

        public EmployeeInfectionGrid(string dataUrl)
            : base (dataUrl, null) { }

        protected virtual Func<EmployeeInfectionInfo, string> EditUrlFormatter { get; set; }



        protected override void ConfigureGrid(JQGrid grid)
        {
            grid.PagerSettings.NoRowsMessage = "No employee infections match your search criteria";
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
                        settings.HeaderText = "Employee Name";
                        settings.Width = 200;
                    }),


               ColumnFor(model => model.InfectionTypeName,
                    settings =>
                    {
                        settings.HeaderText = "Infection Type";
                        settings.Width = 250;
                    }),

               ColumnFor(model => model.Department,
                    settings =>
                    {
                        settings.HeaderText = "Department";
                        settings.Width = 150;
                    }),

               ColumnFor(model => model.NotifiedOn,
                    settings =>
                    {
                        settings.HeaderText = "Notified On";
                        settings.Width = 100;
                    }),

                ColumnFor(model => model.WellOn,
                    settings =>
                    {
                        settings.HeaderText = "Well On";
                        settings.Width = 100;
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
