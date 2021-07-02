using System;
using RedArrow.Framework.Extensions.Common;
using SnyderIS.sCore.Web.Mvc.JQGridHelpers;
using Trirand.Web.Mvc;

namespace IQI.Intuition.Web.Models.Administration.Account
{
    public class AccountGrid : GridModel<AccountGridItem>
    {
        public AccountGrid()
        {
        }

        public AccountGrid(string dataUrl)
            : base (dataUrl, null) { }


        protected override void ConfigureGrid(JQGrid grid)
        {
            grid.PagerSettings.NoRowsMessage = string.Empty;
            grid.AutoWidth = true;
            grid.ShrinkToFit = false;
            grid.Height = new System.Web.UI.WebControls.Unit(300);
            grid.ToolBarSettings.ShowSearchToolBar = true;
            grid.RenderingMode = RenderingMode.Optimized;
            grid.PagerSettings.PageSize = 100;
            grid.PagerSettings.PageSizeOptions = "[100,200,300]";
            grid.ClientSideEvents.RowSelect = "accountRowSelect";
            grid.AppearanceSettings.AlternateRowBackground = true;
            grid.AppearanceSettings.HighlightRowsOnHover = true;

			grid.AddColumns(

			ColumnFor(model => model.Id,
				settings =>
				{
                    settings.HeaderText = "ID";
					settings.PrimaryKey = true;
					settings.Visible = true;
                    settings.Searchable = false;
                    settings.Width = 50;
				})
            ,
            ColumnFor(model => model.Guid,
                settings =>
                {
                    settings.HeaderText = "Guid";
                    settings.Width = 250;
                    settings.Searchable = false;
                })

            ,
            ColumnFor(model => model.Name,
                settings =>
                {
                    settings.HeaderText = "Name";
                    settings.Width = 550;
                })

			);

            grid.SortSettings.InitialSortColumn = grid.Columns[0].DataField;
        }
    }
}


