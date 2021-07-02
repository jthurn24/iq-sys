using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQI.Intuition.Domain.Models;
using System.Web.Mvc;

namespace IQI.Intuition.Web.Models.Reporting.CmsMatrix
{
    public class Matrix802View
    {
        public string CurrentFacilityName { get; set; }
        public IList<ColumnHeader> Headers { get; set; }
        public IList<Row> Rows { get; set; }
        public IList<Page> Pages { get; set; }
        public int? SelectedWing { get; set; }
        public IEnumerable<SelectListItem> WingOptions { get; set; }

        private const int ROWS_PER_PAGE = 23;


        public void SetData(
            IEnumerable<Domain.Models.CmsMatrixCategory> categories,
            IEnumerable<Domain.Models.Patient> patients,
            IEnumerable<Domain.Models.CmsMatrixEntry> entries
            )
        {

            Headers = new List<ColumnHeader>();
            Pages = new List<Page>();
            Rows = new List<Row>();

            int shadeCount = 1;
            bool shade = false;

            foreach (var category in categories)
            {

                if (shadeCount > 2)
                {
                    shadeCount = 1;
                    shade = shade ? false : true;
                }

                Headers.Add(new ColumnHeader()
                {
                    Category = category.Name,
                    Description = category.DescriptionText,
                    ColumnNumber = category.ColumnNumber.HasValue ? category.ColumnNumber.Value : 0,
                    ShowForDisplay = category.Editable.HasValue ? category.Editable.Value : false,
                    Shaded = shade
                });

                shadeCount++;
            }

            int rowCount = 0;
            int totalRowCount = 0;

            Page currentPage = new Page();
            currentPage.Rows = new List<Row>();
            Pages.Add(currentPage);
            int rowsPerPage = ROWS_PER_PAGE;


            foreach (var patient in patients)
            {
                rowCount++;
                totalRowCount++;

                if (Pages.Count() > 1)
                {
                    rowsPerPage = ROWS_PER_PAGE;
                }
                else
                {
                    rowsPerPage = ROWS_PER_PAGE - 4;
                }

                if (rowCount > rowsPerPage)
                {
                    currentPage = new Page();
                    currentPage.Rows = new List<Row>();
                    Pages.Add(currentPage);
                    rowCount = 0;
                }

                var r = new Row();
                r.Columns = new List<Column>();
                r.PatientName = patient.FullName;
                r.PatientNumber = totalRowCount;
                r.PatientId = patient.Id;
                r.Room = patient.Room.Name;
                Rows.Add(r);
                currentPage.Rows.Add(r);

                var patientEntries = entries.Where(x => x.Patient.Id == patient.Id).ToList();

                foreach (var category in categories)
                {
                    var c = new Column();
                    var e = patientEntries.Where(x => x.Category.Id == category.Id).FirstOrDefault();
                    c.Value = e != null ? e.SelectedOptions : string.Empty;
                    c.CategoryId = category.Id;
                    c.ColumnNumber = category.ColumnNumber.HasValue ? category.ColumnNumber.Value : 0;
                    c.ShowForDisplay = category.Editable.HasValue ? category.Editable.Value : false;
                    c.Shaded = Headers.Where(x => x.Category == category.Name).First().Shaded;
                    r.Columns.Add(c);
                }
            }

        }

        public class Page
        {
            public IList<Row> Rows { get; set; }
        }

        public class Row
        {
            public string PatientName { get; set; }
            public int? PatientId { get; set; }
            public IList<Column> Columns { get; set; }
            public int PatientNumber { get; set; }
            public string Room { get; set; }
        }

        public class Column
        {
            public string Value { get; set; }
            public int ColumnNumber { get; set; }
            public int? CategoryId { get; set; }
            public bool ShowForDisplay { get; set; }
            public bool Shaded { get; set; }
        }

        public class ColumnHeader
        {
            public string Category { get; set; }
            public string Description { get; set; }
            public int ColumnNumber { get; set; }
            public bool ShowForDisplay { get; set; }
            public bool Shaded { get; set; }
        }
    }
}
