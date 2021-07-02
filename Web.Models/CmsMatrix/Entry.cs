using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IQI.Intuition.Web.Models.CmsMatrix
{
    public class Entry
    {
        public int? PatientId { get; set; }
        public int? CategoryId { get; set; }
        public string PatientName { get; set; }
        public string CategoryName { get; set; }
        public string CategoryDescription { get; set; }
        public IList<Option> Options { get; set; }

        public class Option
        {
            public string OptionValue { get; set; }
            public string Description { get; set; }
            public bool Selected { get; set; }
        }
    }
}
