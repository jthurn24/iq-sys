using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQI.Intuition.Domain.Models;
using System.Web.Mvc;
using RedArrow.Framework.Extensions.Formatting;

namespace IQI.Intuition.Web.Models.Reporting.CmsMatrix
{
    public class NotesView
    {
        public int? SelectedWing { get; set; }
        public IEnumerable<SelectListItem> WingOptions { get; set; }

        public IList<Note> Entries { get; set; }


        public void SetData(
            IEnumerable<Domain.Models.CmsNote> notes
            )
        {
            notes = notes.OrderBy(x => x.Patient.FullName);
            Entries = new List<Note>();

            foreach (var note in notes.Where(x => x.NoteText != null && x.NoteText.Trim() != string.Empty))
            {
                Entries.Add(new Note() { Name = note.Patient.FullName, NoteText = note.NoteText.Replace("\n", "<br>") });
            }

        }

        public class Note
        {
            public string Name { get; set; }
            public string NoteText { get; set; }
        }
    }
}
