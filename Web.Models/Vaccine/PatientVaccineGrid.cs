using System;
using RedArrow.Framework.Extensions.Common;
using SnyderIS.sCore.Web.Mvc.JQGridHelpers;
using Trirand.Web.Mvc;
namespace IQI.Intuition.Web.Models.Vaccine
{
    public class PatientVaccineGrid : GridModel<VaccineInfo>
    {
        public PatientVaccineGrid(Func<VaccineInfo, string> editUrlFormatter)
        {
            EditUrlFormatter = editUrlFormatter;
        }

        public PatientVaccineGrid(string dataUrl)
            : base (dataUrl, null) { }

        protected virtual Func<VaccineInfo, string> EditUrlFormatter { get; set; }

        protected override void ConfigureGrid(JQGrid grid)
        {
            grid.AutoWidth = true;
            grid.ShrinkToFit = false;
            grid.Height = new System.Web.UI.WebControls.Unit(150);
            grid.ToolBarSettings.ShowSearchToolBar = true;
            grid.RenderingMode = RenderingMode.Optimized;
            grid.ToolBarSettings.ShowSearchToolBar = false;
            grid.AppearanceSettings.HighlightRowsOnHover = true;
            grid.ClientSideEvents.RowSelect = "vaccineRowSelect";

            grid.AddColumns(

                ColumnFor(model => model.VaccineType,
                    settings =>
                    {
                        settings.HeaderText = "Type";
                        settings.Width = 500;
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

            grid.SortSettings.InitialSortColumn = grid.Columns[1].DataField;
            grid.SortSettings.InitialSortDirection = SortDirection.Desc;
        }
    }
}
