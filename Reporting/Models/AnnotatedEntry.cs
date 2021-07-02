using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IQI.Intuition.Reporting.Models
{
    public class AnnotatedEntry
    {
        public IEnumerable<Guid> Components { get; set; }
        public string ViewAction { get; set; }
        public Guid Id { get; set; }
    }
}
