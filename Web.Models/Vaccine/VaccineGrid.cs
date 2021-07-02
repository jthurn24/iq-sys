using System;
using RedArrow.Framework.Extensions.Common;
using SnyderIS.sCore.Web.Mvc.JQGridHelpers;
using Trirand.Web.Mvc;

namespace IQI.Intuition.Web.Models.Vaccine
{
    public class VaccineGrid : GridModel<VaccineInfo>
    {
        public VaccineGrid(Func<VaccineInfo, string> editUrlFormatter)
        {
            EditUrlFormatter = editUrlFormatter;
        }

        public VaccineGrid(string dataUrl)
            : base (dataUrl, null) { }

        protected virtual Func<VaccineInfo, string> EditUrlFormatter { get; set; }

        protected override void ConfigureGrid(JQGrid grid)
        {
            grid.PagerSettings.NoRowsMessage = "No vaccines match your search criteria";
            grid.AutoWidth = true;
            grid.ShrinkToFit = false;
            grid.Height = new System.Web.UI.WebControls.Unit(300);
            grid.ToolBarSettings.ShowSearchToolBar = true;
            grid.RenderingMode = RenderingMode.Optimized;
            grid.AppearanceSettings.HighlightRowsOnHover = true;
            grid.ClientSideEvents.RowSelect = "vaccineRowSelect";
            grid.PagerSettings.PageSizeOptions = "[100,200,300]";
            grid.PagerSettings.PageSize = 100;

            grid.AddColumns(

                ColumnFor(model => model.PatientFullName, 
                    settings => 
                    { 
                        settings.HeaderText = "Name";
                        settings.Width = 150;
                    }),

                ColumnFor(model => model.RoomAndWingName,
                    settings =>
                    {
                        settings.HeaderText = "Room/Wing";
                        settings.Width = 110;
                    }),

                ColumnFor(model => model.VaccineType, 
                    settings =>
                    {
                        settings.HeaderText = "Type";
                        settings.Width = 400;
                    }),

                ColumnFor(model => model.AdministeredOn, 
                    settings => 
                    {
                        settings.HeaderText = "Administered";
                        settings.Width = 100;
                        settings.DataFormatString = "{0:MM/dd/yyyy}";
                    }),

                ColumnFor(model => model.RefusalReason,
                    settings =>
                    {
                        settings.HeaderText = "Refusal Reason";
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
