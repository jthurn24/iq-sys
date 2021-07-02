using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IQI.Intuition.Web.Models.Drug
{
    public class SectionView
    {
        public string DrugName { get; set; }
        public IList<Section> Sections { get; set; }
    }
}
