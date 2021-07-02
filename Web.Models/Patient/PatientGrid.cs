using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Mvc.Extensions;
using SnyderIS.sCore.Web.Mvc.JQGridHelpers;
using Trirand.Web.Mvc;

namespace IQI.Intuition.Web.Models.Patient
{
    public class PatientGrid : GridModel<PatientInfo>
    {
        public PatientGrid(Func<PatientInfo, string> editUrlFormatter)
        {
            EditUrlFormatter = editUrlFormatter;
        }

        public PatientGrid(string dataUrl)
            : base (dataUrl, null) { }

        protected virtual Func<PatientInfo, string> EditUrlFormatter { get; set; }

        protected override void ConfigureGrid(JQGrid grid)
        {
            grid.PagerSettings.NoRowsMessage = "No patients match your search criteria";
            grid.AutoWidth = true;
            grid.ShrinkToFit = false;
            grid.Height = new System.Web.UI.WebControls.Unit(300);
            grid.ToolBarSettings.ShowSearchToolBar = true;
            grid.RenderingMode = RenderingMode.Optimized;
            grid.PagerSettings.PageSizeOptions = "[100,200,300]";
            grid.PagerSettings.PageSize = 100;

            grid.AddColumns(


                ColumnFor(model => model.LastName,
                    settings =>
                    {
                        settings.HeaderText = "Last Name";
                        settings.Width = 200;
                    }),

                ColumnFor(model => model.FirstName, 
                    settings => 
                    { 
                        settings.HeaderText = "First Name";
                        settings.Width = 180;
                    }),

                ColumnFor(model => model.BirthDate, 
                    settings => 
                    {
                        settings.HeaderText = "Date of Birth";
                        settings.Width = 100;
                        settings.DataFormatString = "{0:MM/dd/yyyy}";
                    }),

                ColumnFor(model => model.RoomWingName, 
                    settings => 
                    {
                        settings.HeaderText = "Wing";
                        settings.Width = 150;
                    }),

                ColumnFor(model => model.RoomName, 
                    settings => 
                    {
                        settings.HeaderText = "Room";
                        settings.Width = 150;
                    }),

                ColumnFor(model => model.Status, 
                    settings => 
                    {
                        settings.Width = 120;
                        settings.Searchable = false;
                    }),

                ColumnFor(model => model.Guid,
                    settings =>
                    {
                        settings.PrimaryKey = true;
                        settings.Visible = false;
                    })
            );

            // On first display, sort the grid by the "Name" column
            grid.SortSettings.InitialSortColumn = grid.Columns[0].DataField;
            grid.AppearanceSettings.HighlightRowsOnHover = true;
            grid.ClientSideEvents.RowSelect = "rowSelect";
        }
    }
}
