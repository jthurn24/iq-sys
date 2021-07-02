using System;
using RedArrow.Framework.Extensions.Common;
using SnyderIS.sCore.Web.Mvc.JQGridHelpers;
using Trirand.Web.Mvc;

namespace IQI.Intuition.Web.Models.PsychotropicDosageChange
{
    public class AdministrationPsychotropicDosageGrid : GridModel<PsychotropicDosageChangeInfo>
    {

        public AdministrationPsychotropicDosageGrid()
            : base() { }

        public AdministrationPsychotropicDosageGrid(string dataUrl)
            : base (dataUrl, null) { }


        protected override void ConfigureGrid(JQGrid grid)
        {
            grid.AutoWidth = true;
            grid.ShrinkToFit = false;
            grid.Height = new System.Web.UI.WebControls.Unit(150);
            grid.ToolBarSettings.ShowSearchToolBar = true;
            grid.RenderingMode = RenderingMode.Optimized;
            grid.ToolBarSettings.ShowSearchToolBar = false;
            grid.AppearanceSettings.HighlightRowsOnHover = true;
            grid.ClientSideEvents.RowSelect = "psychotropicRowSelect";

            grid.AddColumns(

                ColumnFor(model => model.StartDate,
                    settings =>
                    {
                        settings.HeaderText = "Date";
                        settings.Width = 90;
                        settings.Searchable = false;
                    }),

                ColumnFor(model => model.Dosage,
                    settings =>
                    {
                        settings.HeaderText = "Dosage";
                        settings.Width = 100;
                        settings.Searchable = false;
                    }),

                ColumnFor(model => model.Frequency,
                    settings =>
                    {
                        settings.HeaderText = "Frequency";
                        settings.Searchable = false;
                        settings.Width = 150;
                        settings.Searchable = false;
                    }),

                ColumnFor(model => model.Change,
                    settings =>
                    {
                        settings.HeaderText = "Change";
                        settings.Width = 150;
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


            grid.SortSettings.InitialSortColumn = grid.Columns[0].DataField;
            grid.SortSettings.InitialSortDirection = SortDirection.Desc;

        }
    }
}
