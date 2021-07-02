using System;
using RedArrow.Framework.Extensions.Common;
using SnyderIS.sCore.Web.Mvc.JQGridHelpers;
using Trirand.Web.Mvc;

namespace IQI.Intuition.Web.Models.Administration.SystemContact
{
    public class SystemContactGrid : GridModel<SystemContactGridItem>
    {
        public SystemContactGrid()
        {
        }

        public SystemContactGrid(string dataUrl)
            : base (dataUrl, null) { }


        protected override void ConfigureGrid(JQGrid grid)
        {
            grid.PagerSettings.NoRowsMessage = string.Empty;
            grid.AutoWidth = true;
            grid.ShrinkToFit = false;
            grid.Height = new System.Web.UI.WebControls.Unit(200);
            grid.ToolBarSettings.ShowSearchToolBar = true;
            grid.RenderingMode = RenderingMode.Optimized;
            grid.PagerSettings.PageSize = 100;
            grid.PagerSettings.PageSizeOptions = "[100,200,300]";
            grid.ClientSideEvents.RowSelect = "systemcontactRowSelect";
            grid.AppearanceSettings.AlternateRowBackground = true;
            grid.AppearanceSettings.HighlightRowsOnHover = true;

			grid.AddColumns(

			ColumnFor(model => model.Id,
				settings =>
				{
					settings.PrimaryKey = true;
					settings.Visible = false;
				})
            ,

            ColumnFor(model => model.FirstName,
                settings =>
                {
                    settings.HeaderText = "FirstName";
                    settings.Width = 150;
                })

            ,
            ColumnFor(model => model.LastName,
                settings =>
                {
                    settings.HeaderText = "LastName";
                    settings.Width = 150;
                })

            ,
            ColumnFor(model => model.Title,
                settings =>
                {
                    settings.HeaderText = "Title";
                    settings.Width = 100;
                })

            ,
            ColumnFor(model => model.Cell,
                settings =>
                {
                    settings.HeaderText = "Cell";
                    settings.Width = 100;
                })

            ,
            ColumnFor(model => model.Direct,
                settings =>
                {
                    settings.HeaderText = "Direct";
                    settings.Width = 100;
                })

            ,
            ColumnFor(model => model.Email,
                settings =>
                {
                    settings.HeaderText = "Email";
                    settings.Width = 100;
                })


			);

            grid.SortSettings.InitialSortColumn = grid.Columns[0].DataField;
        }
    }
}


