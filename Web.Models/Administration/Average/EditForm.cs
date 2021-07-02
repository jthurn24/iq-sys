using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace IQI.Intuition.Web.Models.Administration.Average
{
    public class EditForm
    {
        public IList<SelectListItem> AverageOptions { get; set; }
        public IList<SelectListItem> FacilityAverageOptions { get; set; }
        public Guid AverageType { get; set; }
    }
}

