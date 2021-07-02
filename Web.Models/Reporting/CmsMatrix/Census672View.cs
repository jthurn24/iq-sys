using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQI.Intuition.Domain.Models;
using System.Web.Mvc;

namespace IQI.Intuition.Web.Models.Reporting.CmsMatrix
{
    public class Census672View
    {
        public int? SelectedWing { get; set; }
        public IEnumerable<SelectListItem> WingOptions { get; set; }

        private IEnumerable<Domain.Models.CmsMatrixEntry> Entries { get; set; }
        private IEnumerable<Domain.Models.CmsMatrixCategoryOption> Options { get; set; }

        public void SetData(
            IEnumerable<Domain.Models.CmsMatrixEntry> entries,
            IEnumerable<Domain.Models.CmsMatrixCategoryOption> options
            )
        {
            Entries = entries;
            Options = options;
        }

        public string GetTotalByMds3(string key)
        {
            var option = Options.Where(x => x.MDS3 == key).FirstOrDefault();

            if (option == null)
            {
                return "INVALID";
            }

            int count = Entries.Where(x => x.Category == option.Category && x.SelectedOptions.Split(',').Contains(option.OptionValue))
                .Where(x => x.Patient.CurrentStatus == Domain.Enumerations.PatientStatus.Admitted)
                .Count();

            return count.ToString();
                
        }
    }
}
