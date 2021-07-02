using System;
using RedArrow.Framework.Extensions.Common;
using SnyderIS.sCore.Web.Mvc.JQGridHelpers;
using Trirand.Web.Mvc;

namespace IQI.Intuition.Web.Models.PsychotropicAdministration
{
    public class PsychotropicAdministrationGrid : GridModel<PsychotropicAdministrationInfo>
    {

        public PsychotropicAdministrationGrid()
            : base() { }

        public PsychotropicAdministrationGrid(string dataUrl)
            : base (dataUrl, null) { }

        protected virtual Func<PsychotropicAdministrationInfo, string> EditUrlFormatter { get; set; }

        protected override void ConfigureGrid(JQGrid grid)
        {
            grid.AutoWidth = true;
            grid.ShrinkToFit = false;
            grid.Height = new System.Web.UI.WebControls.Unit(300);
            grid.ToolBarSettings.ShowSearchToolBar = true;
            grid.RenderingMode = RenderingMode.Optimized;
            grid.ToolBarSettings.ShowSearchToolBar = true;
            grid.AppearanceSettings.HighlightRowsOnHover = true;
            grid.ClientSideEvents.RowSelect = "psychotropicRowSelect";

            grid.AddColumns(

                ColumnFor(model => model.PatientFullName,
                    settings =>
                    {
                        settings.HeaderText = "Name";
                        settings.Width = 100;
                        settings.Searchable = true;
                        settings.Sortable = true;
                    }),

                ColumnFor(model => model.PatientRoomWingName,
                    settings =>
                    {
                        settings.HeaderText = "Room/Wing";
                        settings.Width = 100;
                        settings.Searchable = true;
                        settings.Sortable = true;
                    }),

                ColumnFor(model => model.Name,
                    settings =>
                    {
                        settings.HeaderText = "Medication";
                        settings.Width = 100;
                        settings.Searchable = true;
                        settings.Sortable = true;
                    }),

                ColumnFor(model => model.DrugTypeName,
                    settings =>
                    {
                        settings.HeaderText = "Type";
                        settings.Searchable = false;
                        settings.Width = 100;
                        settings.Searchable = true;
                        settings.Sortable = true;
                    }),

                ColumnFor(model => model.CurrentDosage,
                    settings =>
                    {
                        settings.HeaderText = "Current Dosage";
                        settings.Width = 150;
                        settings.Searchable = false;
                        settings.Sortable = false;
                    }),

                ColumnFor(model => model.StartDate,
                    settings =>
                    {
                        settings.HeaderText = "Start Date";
                        settings.Width = 130;
                        settings.Searchable = false;
                        settings.Sortable = false;
                    }),
                ColumnFor(model => model.Active,
                    settings =>
                    {
                        settings.HeaderText = "Active";
                        settings.Width = 130;
                        settings.Searchable = false;
                        settings.Sortable = false;
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
