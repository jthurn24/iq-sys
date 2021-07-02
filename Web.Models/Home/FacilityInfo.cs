using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace IQI.Intuition.Web.Models.Home
{
    public class FacilityInfo
    {
        public string AccountName { get; set; }

        public string SubDomain { get; set; }

        public string Protocol { get; set; }

        public IEnumerable<SelectListItem> FacilityOptions { get; set; }
    }
}
