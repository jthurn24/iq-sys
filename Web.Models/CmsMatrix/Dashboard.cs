using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQI.Intuition.Domain.Models;
using System.Web.Mvc;

namespace IQI.Intuition.Web.Models.CmsMatrix
{
    public class Dashboard
    {
        public string CurrentFacilityName { get; set; }
        public IList<ColumnHeader> Headers { get; set; }
        public IList<Row> Rows { get; set; }
        public int? SelectedWing { get; set; }
        public IEnumerable<SelectListItem> WingOptions { get; set; }
        public int? SelectedMatrixType { get; set; }
        public IEnumerable<SelectListItem> MatrixTypeOptions { get; set; }
        public int? SortBy { get; set; }
        public IEnumerable<SelectListItem> SortByOptions { get; set; }

        public void SetData(
            IEnumerable<Domain.Models.CmsMatrixCategory> categories,
            IEnumerable<Domain.Models.Patient> patients,
            IEnumerable<Domain.Models.CmsMatrixEntry> entries,
            IEnumerable<Domain.Models.CmsNote> notes
            )
        {

            Headers = new List<ColumnHeader>();
            Rows = new List<Row>();

            foreach (var category in categories)
            {
                Headers.Add(new ColumnHeader()
                {
                     Category = category.Name,
                     Description = category.DescriptionText,
                     ColumnNumber = category.ColumnNumber.Value
                });
            }

            if (categories.Count() < 24)
            {
                for (int i = 1; i <= (24 - categories.Count()); i++)
                {
                    Headers.Add(new ColumnHeader()
                    {
                        Category = " ",
                        Description = string.Empty,
                        ColumnNumber = Headers.Count() + 1
                    });
                }
            }

            int rowCount = 0;

            foreach(var patient in patients)
            {
                rowCount++;

                var r = new Row();
                r.Columns = new List<Column>();
                r.PatientName = patient.FullName;
                r.PatientNumber = rowCount;
                r.PatientId = patient.Id;
                r.Room = patient.Room.Name;

                Rows.Add(r);

                var note = notes.Where(x => x.Patient.Id == patient.Id).FirstOrDefault();

                if(note != null && note.NoteText != null && note.NoteText.Trim() != string.Empty)
                {
                    r.HasNote = true;
                }
                else{
                    r.HasNote = false;
                }

                var patientEntries = entries.Where(x => x.Patient.Id == patient.Id).ToList();

                foreach (var category in categories)
                {
                    var c = new Column();
                    var e = patientEntries.Where(x => x.Category.Id == category.Id).FirstOrDefault();
                    c.Value = e != null ? e.SelectedOptions : string.Empty;
                    c.CategoryId = category.Id;
                    c.ColumnNumber = category.ColumnNumber.Value;
                    c.IsPlaceHolder = false;
                    r.Columns.Add(c);
                }

                if (categories.Count() < 24)
                {
                    for (int i = 1; i <= (24 - categories.Count()); i++)
                    {
                        var c = new Column();
                        c.Value = string.Empty;
                        c.ColumnNumber = r.Columns.Count() + 1;
                        c.IsPlaceHolder = true;
                        r.Columns.Add(c);
                    }
                }


            }

        }

        public class Row
        {
           public string PatientName { get; set; }
           public int? PatientId { get; set; }
           public IList<Column> Columns { get; set; }
           public int PatientNumber { get; set; }
           public bool HasNote { get; set; }
           public string Room { get; set; }
        }

        public class Column
        {
            public string Value { get; set; }
            public int ColumnNumber { get; set; }
            public int? CategoryId { get; set; }
            public bool IsPlaceHolder { get; set; }
        }

        public class ColumnHeader
        {
            public string Category { get; set; }
            public string Description { get; set; }
            public int ColumnNumber { get; set; }
        }
    }
}
