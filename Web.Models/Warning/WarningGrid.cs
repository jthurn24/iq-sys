﻿using System;
using RedArrow.Framework.Extensions.Common;
using SnyderIS.sCore.Web.Mvc.JQGridHelpers;
using Trirand.Web.Mvc;

namespace IQI.Intuition.Web.Models.Warning
{
    public class WarningGrid : GridModel<WarningInfo>
    {
        public WarningGrid()
        { }

        public WarningGrid(string dataUrl)
            : base (dataUrl, null) { }

        protected virtual Func<WarningInfo, string> EditUrlFormatter { get; set; }

        protected override void ConfigureGrid(JQGrid grid)
        {
            grid.PagerSettings.NoRowsMessage = "No Warnings match your search criteria";
            grid.AutoWidth = true;
            grid.ShrinkToFit = false;
            grid.Height = new System.Web.UI.WebControls.Unit(300);
            grid.ToolBarSettings.ShowSearchToolBar = true;
            grid.RenderingMode = RenderingMode.Optimized;
            grid.AppearanceSettings.HighlightRowsOnHover = true;
            grid.ClientSideEvents.RowSelect = "rowSelect";
            grid.PagerSettings.PageSizeOptions = "[100,200,300]";
            grid.PagerSettings.PageSize = 100;

            grid.AddColumns(

                ColumnFor(model => model.TriggeredOn,
                    settings =>
                    {
                        settings.HeaderText = "On";
                        settings.Width = 100;
                    }),

               ColumnFor(model => model.Title,
                    settings =>
                    {
                        settings.HeaderText = "Details";
                        settings.Width = 400;
                    }),

               ColumnFor(model => model.PatientName,
                    settings =>
                    {
                        settings.HeaderText = "Patient";
                        settings.Width = 300;
                    }),

               ColumnFor(model => model.Target,
                    settings =>
                    {
                        settings.HeaderText = "Target";
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

            // On first display, sort the grid by the "Name" column
            grid.SortSettings.InitialSortColumn = grid.Columns[0].DataField;
        }
    }
}
