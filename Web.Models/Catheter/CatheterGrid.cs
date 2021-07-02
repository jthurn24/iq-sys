﻿using System;
using RedArrow.Framework.Extensions.Common;
using SnyderIS.sCore.Web.Mvc.JQGridHelpers;
using Trirand.Web.Mvc;

namespace IQI.Intuition.Web.Models.Catheter
{
    public class CatheterGrid : GridModel<CatheterInfo>
    {
        public CatheterGrid()
        { 
            
        }

        public CatheterGrid(string dataUrl)
            : base (dataUrl, null) { }

        protected virtual Func<CatheterInfo, string> EditUrlFormatter { get; set; }

        protected override void ConfigureGrid(JQGrid grid)
        {
            grid.PagerSettings.NoRowsMessage = "No Catheters match your search criteria";
            grid.AutoWidth = true;
            grid.ShrinkToFit = false; 
            grid.Height = new System.Web.UI.WebControls.Unit(300);
            grid.ToolBarSettings.ShowSearchToolBar = true;
            grid.RenderingMode = RenderingMode.Optimized;
            grid.AppearanceSettings.HighlightRowsOnHover = true;
            grid.ClientSideEvents.RowSelect = "catheterRowSelect";
            grid.PagerSettings.PageSizeOptions = "[100,200,300]";
            grid.PagerSettings.PageSize = 100;

            grid.AddColumns(

                ColumnFor(model => model.PatientFullName, 
                    settings => 
                    { 
                        settings.HeaderText = "Name";
                        settings.Width = 180;
                    }),

                 ColumnFor(model => model.RoomAndWingName,
                    settings =>
                    {
                        settings.HeaderText = "Room/Wing";
                        settings.Width = 180;
                    }),

                ColumnFor(model => model.Diagnosis, 
                    settings =>
                    {
                        settings.HeaderText = "Diagnosis";
                        settings.Width = 300;
                        settings.Sortable = false;
                    }),


                ColumnFor(model => model.StartedOn, 
                    settings => 
                    {
                        settings.HeaderText = "Started On";
                        settings.Width = 90;
                        settings.DataFormatString = "{0:MM/dd/yyyy}";
                    }),

                ColumnFor(model => model.DiscontinuedOn,
                    settings =>
                    {
                        settings.HeaderText = "Disc On";
                        settings.Width = 90;
                        settings.DataFormatString = "{0:MM/dd/yyyy}";
                    }),

                ColumnFor(model => model.Id,
                    settings =>
                    {
                        settings.PrimaryKey = true;
                        settings.Visible = false;
                    })
            );

            // On first display, sort the grid by the "Name" column
            grid.SortSettings.InitialSortColumn = grid.Columns[3].DataField;

        }
    }
}
