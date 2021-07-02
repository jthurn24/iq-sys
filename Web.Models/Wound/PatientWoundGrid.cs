using System;
using RedArrow.Framework.Extensions.Common;
using SnyderIS.sCore.Web.Mvc.JQGridHelpers;
using Trirand.Web.Mvc;

namespace IQI.Intuition.Web.Models.Wound
{
    public class PatientWoundGrid : GridModel<WoundInfo>
    {


        public PatientWoundGrid()
            : base() { }

        public PatientWoundGrid(string dataUrl)
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
            grid.ClientSideEvents.RowSelect = "WoundRowSelect";

            grid.AddColumns(

               ColumnFor(model => model.TypeName,
                    settings =>
                    {
                        settings.HeaderText = "Type";
                        settings.Width = 100;
                    }),

                ColumnFor(model => model.FirstNoted,
                    settings =>
                    {
                        settings.HeaderText = "First Noted";
                        settings.Width = 100;
                    }),

                ColumnFor(model => model.StageName,
                    settings =>
                    {
                        settings.HeaderText = "Current Stage";
                        settings.Width = 100;
                    }),

                ColumnFor(model => model.SiteName,
                    settings =>
                    {
                        settings.HeaderText = "Site";
                        settings.Width = 150;
                    }),

                ColumnFor(model => model.ResolvedOn,
                    settings =>
                    {
                        settings.HeaderText = "Resolved";
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
