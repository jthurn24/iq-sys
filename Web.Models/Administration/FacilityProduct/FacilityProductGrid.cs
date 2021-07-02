using System;
using RedArrow.Framework.Extensions.Common;
using SnyderIS.sCore.Web.Mvc.JQGridHelpers;
using Trirand.Web.Mvc;

namespace IQI.Intuition.Web.Models.Administration.FacilityProduct
{
    public class FacilityProductGrid : GridModel<FacilityProductGridItem>
    {
        public FacilityProductGrid()
        {
        }

        public FacilityProductGrid(string dataUrl)
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
            grid.ClientSideEvents.RowSelect = "facilityproductRowSelect";
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

                ColumnFor(model => model.Fee,
                    settings =>
                    {
                        settings.HeaderText = "Fee";
                        settings.Width = 80;
                    })

                ,
                ColumnFor(model => model.FeeType,
                    settings =>
                    {
                        settings.HeaderText = "Fee Type";
                        settings.Width = 200;
                    })

                ,
                ColumnFor(model => model.SystemProduct,
                    settings =>
                    {
                        settings.HeaderText = "Product";
                        settings.Width = 400;
                    })

                ,
                ColumnFor(model => model.StartOn,
                    settings =>
                    {
                        settings.HeaderText = "Start";
                        settings.Width = 100;
                    })

			);

            grid.SortSettings.InitialSortColumn = grid.Columns[0].DataField;
            grid.Columns[2].AddEnumSearchDropDown(typeof(Domain.Enumerations.ProductFeeType));
        }
    }
}


