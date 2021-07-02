﻿using System;
using RedArrow.Framework.Extensions.Common;
using SnyderIS.sCore.Web.Mvc.JQGridHelpers;
using Trirand.Web.Mvc;

namespace IQI.Intuition.Web.Models.PatientCensus
{
    public class PatientCensusGrid : GridModel<PatientCensusInfo>
    {
        public PatientCensusGrid()
        { }

        public PatientCensusGrid(string dataUrl)
            : base (dataUrl, null) { }

        protected virtual Func<PatientCensusInfo, string> EditUrlFormatter { get; set; }

        protected override void ConfigureGrid(JQGrid grid)
        {
            grid.PagerSettings.NoRowsMessage = "No Patient Census match your search criteria";
            grid.AutoWidth = true;
            grid.ShrinkToFit = false;
            grid.Height = new System.Web.UI.WebControls.Unit(300);
            grid.ToolBarSettings.ShowSearchToolBar = true;
            grid.RenderingMode = RenderingMode.Optimized;
            grid.AppearanceSettings.HighlightRowsOnHover = true;
            grid.ClientSideEvents.RowSelect = "rowSelect";

            grid.AddColumns(

                ColumnFor(model => model.Year,
                    settings =>
                    {
                        settings.HeaderText = "Year";
                        settings.Width = 200;
                        settings.Sortable = false;
                    }),

               ColumnFor(model => model.Month,
                    settings =>
                    {
                        settings.HeaderText = "Month";
                        settings.Width = 200;
                        settings.Sortable = false;
                    }),


               ColumnFor(model => model.PatientDays,
                    settings =>
                    {
                        settings.HeaderText = "Patient Days";
                        settings.Width = 200;
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