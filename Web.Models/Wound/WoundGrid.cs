using System;
using RedArrow.Framework.Extensions.Common;
using SnyderIS.sCore.Web.Mvc.JQGridHelpers;
using Trirand.Web.Mvc;

namespace IQI.Intuition.Web.Models.Wound
{
    public class WoundGrid : GridModel<WoundInfo>
    {


        public WoundGrid()
            : base() { }

        public WoundGrid(string dataUrl)
            : base (dataUrl, null) { }


        protected override void ConfigureGrid(JQGrid grid)
        {
            grid.AutoWidth = true;
            grid.ShrinkToFit = false;
            grid.Height = new System.Web.UI.WebControls.Unit(300);
            grid.ToolBarSettings.ShowSearchToolBar = true;
            grid.RenderingMode = RenderingMode.Optimized;
            grid.ToolBarSettings.ShowSearchToolBar = true;
            grid.AppearanceSettings.HighlightRowsOnHover = true;
            grid.ClientSideEvents.RowSelect = "WoundRowSelect";

            grid.AddColumns(


               ColumnFor(model => model.PatientFullName,
                    settings =>
                    {
                        settings.HeaderText = "Name";
                        settings.Width = 200;
                        settings.Searchable = true;
                        settings.Sortable = true;
                    }),

                ColumnFor(model => model.RoomWingName,
                    settings =>
                    {
                        settings.HeaderText = "Room/Wing";
                        settings.Width = 100;
                        settings.Searchable = true;
                        settings.Sortable = true;
                    }),

               ColumnFor(model => model.TypeName,
                    settings =>
                    {
                        settings.HeaderText = "Type";
                        settings.Width = 100;
                        settings.Searchable = true;
                        settings.Sortable = true;
                    }),

                ColumnFor(model => model.FirstNoted,
                    settings =>
                    {
                        settings.HeaderText = "First Noted";
                        settings.Width = 100;
                        settings.Searchable = true;
                        settings.Sortable = true;
                    }),

                ColumnFor(model => model.StageName,
                    settings =>
                    {
                        settings.HeaderText = "Current Stage";
                        settings.Width = 100;
                        settings.Searchable = true;
                        settings.Sortable = true;
                    }),

                ColumnFor(model => model.SiteName,
                    settings =>
                    {
                        settings.HeaderText = "Site";
                        settings.Width = 150;
                        settings.Searchable = true;
                        settings.Sortable = true;
                    }),

                ColumnFor(model => model.ResolvedOn,
                    settings =>
                    {
                        settings.HeaderText = "Resolved";
                        settings.Width = 150;
                        settings.Searchable = true;
                        settings.Sortable = true;
                    }),

                ColumnFor(model => model.Id,
                    settings =>
                    {
                        settings.PrimaryKey = true;
                        settings.Visible = false;
                    })

            );

            grid.SortSettings.InitialSortColumn = grid.Columns[0].DataField;
        }
    }
}
