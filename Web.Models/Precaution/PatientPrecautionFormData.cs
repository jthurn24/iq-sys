using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace IQI.Intuition.Web.Models.Precaution
{
    public class PatientPrecautionFormData
    {
        public IEnumerable<SelectListItem> Products;
        public IEnumerable<PatientPrecautionOption> Types;
        public int PatientId;
    }
}
