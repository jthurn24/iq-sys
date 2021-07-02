using System;
using RedArrow.Framework.Extensions.Common;
using SnyderIS.sCore.Web.Mvc.JQGridHelpers;
using Trirand.Web.Mvc;

namespace IQI.Intuition.Web.Models.Configuration.AccountUser
{
    public class AccountUserGrid : GridModel<AccountUserInfo>
    {
        public AccountUserGrid()
        { }

        public AccountUserGrid(string dataUrl)
            : base (dataUrl, null) { }

        protected virtual Func<AccountUserInfo, string> EditUrlFormatter { get; set; }

        protected override void ConfigureGrid(JQGrid grid)
        {
            grid.PagerSettings.NoRowsMessage = "No Account Users match your search criteria";
            grid.AutoWidth = true;
            grid.ShrinkToFit = false;
            grid.Height = new System.Web.UI.WebControls.Unit(300);
            grid.ToolBarSettings.ShowSearchToolBar = true;
            grid.RenderingMode = RenderingMode.Optimized;
            grid.AppearanceSettings.HighlightRowsOnHover = true;
            grid.ClientSideEvents.RowSelect = "rowSelect";

            grid.AddColumns(

                ColumnFor(model => model.Login,
                    settings =>
                    {
                        settings.HeaderText = "Login";
                        settings.Width = 200;
                        settings.Searchable = true;
                        settings.Sortable = true;
                    }),

               ColumnFor(model => model.FirstName,
                    settings =>
                    {
                        settings.HeaderText = "First";
                        settings.Width = 200;
                        settings.Searchable = true;
                        settings.Sortable = true;
                    }),


               ColumnFor(model => model.LastName,
                    settings =>
                    {
                        settings.HeaderText = "Last";
                        settings.Width = 200;
                        settings.Searchable = true;
                        settings.Sortable = true;
                    }),

                ColumnFor(model => model.EmailAddress,
                    settings =>
                    {
                        settings.HeaderText = "Email";
                        settings.Width = 200;
                        settings.Searchable = true;
                        settings.Sortable = true;
                    }),

                ColumnFor(model => model.IsActive,
                    settings =>
                    {
                        settings.HeaderText = "Active";
                        settings.Width = 50;
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
