﻿using System;
using RedArrow.Framework.Extensions.Common;
using SnyderIS.sCore.Web.Mvc.JQGridHelpers;
using Trirand.Web.Mvc;

namespace IQI.Intuition.Web.Models.WoundAssessment
{
    public class ReportAssessmentGrid : GridModel<AssessmentInfo>
    {


        public ReportAssessmentGrid()
            : base() { }

        public ReportAssessmentGrid(string dataUrl)
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
            grid.ClientSideEvents.RowSelect = "assessmentRowSelect";

            grid.AddColumns(

                ColumnFor(model => model.AssessmentDate,
                    settings =>
                    {
                        settings.HeaderText = "Assessment Date";
                        settings.Width = 150;
                    }),

                ColumnFor(model => model.StageName,
                    settings =>
                    {
                        settings.HeaderText = "Stage";
                        settings.Width = 150;
                    }),

                ColumnFor(model => model.RoomWingName,
                    settings =>
                    {
                        settings.HeaderText = "Room/Wing";
                        settings.Width = 200;
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