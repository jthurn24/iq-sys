using System;
using RedArrow.Framework.Extensions.Common;
using SnyderIS.sCore.Web.Mvc.JQGridHelpers;
using Trirand.Web.Mvc;

namespace IQI.Intuition.Web.Models.Administration.Facility
{
    public class FacilityGrid : GridModel<FacilityGridItem>
    {
        public FacilityGrid()
        {
        }

        public FacilityGrid(string dataUrl)
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
            grid.ClientSideEvents.RowSelect = "facilityRowSelect";
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
            ColumnFor(model => model.Guid,
                settings =>
                {
                    settings.HeaderText = "Guid";
                    settings.Width = 300;
                    settings.Searchable = false;
                })

            ,

            ColumnFor(model => model.Name,
                settings =>
                {
                    settings.HeaderText = "Name";
                    settings.Width = 250;
                })

            ,
            ColumnFor(model => model.SubDomain,
                settings =>
                {
                    settings.HeaderText = "Sub Domain";
                    settings.Width = 110;
                })

            ,
            ColumnFor(model => model.State,
                settings =>
                {
                    settings.HeaderText = "State";
                    settings.Width = 70;
                })

            ,
            ColumnFor(model => model.PatientCount,
                settings =>
                {
                    settings.HeaderText = "Patients";
                    settings.Width = 50;
                    settings.Searchable = false;
                })


            );

            grid.SortSettings.InitialSortColumn = grid.Columns[0].DataField;
        }
    }
}


