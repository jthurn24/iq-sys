using System;
using RedArrow.Framework.Extensions.Common;
using SnyderIS.sCore.Web.Mvc.JQGridHelpers;
using Trirand.Web.Mvc;

namespace IQI.Intuition.Web.Models.Administration.SystemSecureFile
{
    public class SystemSecureFileGrid : GridModel<SystemSecureFileGridItem>
    {

        public SystemSecureFileGrid(Func<SystemSecureFileGridItem, string> downloadUrlFormatter)
        {
            DownloadUrlFormatter = downloadUrlFormatter;
        }

        public SystemSecureFileGrid(string dataUrl)
            : base (dataUrl, null) {

        }

        protected virtual Func<SystemSecureFileGridItem, string> DownloadUrlFormatter { get; set; }

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
            grid.AppearanceSettings.AlternateRowBackground = true;
            grid.AppearanceSettings.HighlightRowsOnHover = true;

			grid.AddColumns(


            ColumnFor(model => model.Description,
                settings =>
                {
                    settings.HeaderText = "Description";
                    settings.Width = 450;
                    settings.Searchable = true;
                })

            ,
            ColumnFor(model => model.ExpiresOn,
                settings =>
                {
                    settings.HeaderText = "Expires";
                    settings.Width = 80;
                    settings.Searchable = false;
                })
            ,
            ColumnFor(model => model.CreatedOn,
                settings =>
                {
                    settings.HeaderText = "Created";
                    settings.Width = 80;
                    settings.Searchable = false;
                })
            ,
            ColumnFor(model => model.FileExtension,
                settings =>
                {
                    settings.HeaderText = "Extension";
                    settings.Width = 90;
                    settings.Searchable = false;
                })

			,

            ColumnFor(model => model.Id,
	            settings =>
	            {
		            settings.PrimaryKey = true;
		            settings.Visible = false;
	            })

            ,

            ColumnFor(@"DownloadLink",
                    model => @"<a href=""{0}"">Download</a>".FormatWith(DownloadUrlFormatter(model)),
                    settings =>
                    {
                        settings.HeaderText = " ";
                        settings.Width = 100;
                        settings.TextAlign = TextAlign.Center;
                        settings.HtmlEncode = false;
                        settings.Sortable = false;
                        settings.Searchable = false;
                    })
            );

            grid.SortSettings.InitialSortColumn = grid.Columns[0].DataField;
        }
    }
}


