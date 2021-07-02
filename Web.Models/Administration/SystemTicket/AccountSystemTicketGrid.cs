using System;
using RedArrow.Framework.Extensions.Common;
using SnyderIS.sCore.Web.Mvc.JQGridHelpers;
using Trirand.Web.Mvc;

namespace IQI.Intuition.Web.Models.Administration.SystemTicket
{
    public class AccountSystemTicketGrid : GridModel<SystemTicketGridItem>
    {
        public string UiIdentifier { get; private set; }

        public AccountSystemTicketGrid(string uiIdentifier)
        {
            UiIdentifier = string.Concat(this.GetType().Name, "-", uiIdentifier);
        }

        public AccountSystemTicketGrid(string dataUrl, string uiIdentifier)
            : base(dataUrl, null) 
        {
            UiIdentifier = string.Concat(this.GetType().Name, "-", uiIdentifier);
        }


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
                    settings.HeaderText = "User";
                    settings.Width = 150;
                })

            ,
            ColumnFor(model => model.Priority,
                settings =>
                {
                    settings.HeaderText = "Priority";
                    settings.Width = 50;
                })

            ,
            ColumnFor(model => model.Release,
                settings =>
                {
                    settings.HeaderText = "Release";
                    settings.Width = 50;
                })

            ,
            ColumnFor(model => model.SystemUser,
                settings =>
                {
                    settings.HeaderText = "Assigned";
                    settings.Width = 100;
                })

            ,
            ColumnFor(model => model.CreatedOn,
                settings =>
                {
                    settings.HeaderText = "Created";
                    settings.Width = 90;
                    settings.Searchable = false;
                })

            ,
            ColumnFor(model => model.ClosedOn,
                settings =>
                {
                    settings.HeaderText = "Closed";
                    settings.Width = 90;
                    settings.Searchable = false;
                })


            );

            grid.SortSettings.InitialSortColumn = grid.Columns[0].DataField;
        }
    }
}


