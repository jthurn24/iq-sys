using System;
using RedArrow.Framework.Extensions.Common;
using SnyderIS.sCore.Web.Mvc.JQGridHelpers;
using Trirand.Web.Mvc;

namespace IQI.Intuition.Web.Models.SupportCenter
{
    public class SystemTicketGrid : GridModel<SystemTicketGridItem>
    {
        public SystemTicketGrid()
        {
        }

        public SystemTicketGrid(string dataUrl)
            : base(dataUrl, null) { }


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
            grid.ClientSideEvents.RowSelect = "systemticketRowSelect";
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
            ColumnFor(model => model.SystemTicketType,
                settings =>
                {
                    settings.HeaderText = "Type";
                    settings.Width = 150;
                })

            ,
            ColumnFor(model => model.SystemTicketStatus,
                settings =>
                {
                    settings.HeaderText = "Status";
                    settings.Width = 80;
                    settings.Searchable = false;
                })

            ,

            ColumnFor(model => model.AccountUser,
                settings =>
                {
                    settings.HeaderText = "Requested By";
                    settings.Width = 100;
                }),

            ColumnFor(model => model.Details,
                settings =>
                {
                    settings.HeaderText = "Details";
                    settings.Width = 550;
                })

            );

            grid.SortSettings.InitialSortColumn = grid.Columns[0].DataField;
        }
    }
}


