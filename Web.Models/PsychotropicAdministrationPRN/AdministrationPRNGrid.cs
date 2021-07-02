using System;
using RedArrow.Framework.Extensions.Common;
using SnyderIS.sCore.Web.Mvc.JQGridHelpers;
using Trirand.Web.Mvc;

namespace IQI.Intuition.Web.Models.PsychotropicAdministrationPRN
{
    public class AdministrationPRNGrid : GridModel<PsychotropicAdministrationPRNInfo>
    {

        public AdministrationPRNGrid()
            : base() { }

        public AdministrationPRNGrid(string dataUrl)
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
            grid.ClientSideEvents.RowSelect = "psychotropicPrnRowSelect";

            grid.AddColumns(

                ColumnFor(model => model.GivenOn,
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
