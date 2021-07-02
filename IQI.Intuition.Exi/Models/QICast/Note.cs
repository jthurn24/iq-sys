using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IQI.Intuition.Exi.Models.QICast
{
    public class Note
    {
        public Guid Id { get; set; }
        public String Content { get; set; }
        public bool HasImage { get; set; }
        public string BackgroundStyle { get; set; }
    }
}
