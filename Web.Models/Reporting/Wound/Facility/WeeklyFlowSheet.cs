using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using IQI.Intuition.Domain.Models;
using RedArrow.Framework.Extensions.Formatting;
using RedArrow.Framework.Extensions.IO;
using RedArrow.Framework.Extensions.Common;
using IQI.Intuition.Reporting.Graphics;

namespace IQI.Intuition.Web.Models.Reporting.Wound.Facility
{
    public class WeeklyFlowSheet
    {
        public IList<Entry> Entries { get; set; }

        public void SetData(IEnumerable<Domain.Models.WoundReport> reports)
        {
            this.Entries = new List<Entry>();

            foreach (var report in reports)
            {
                var e = new Entry()
                {
                    WoundSite = report.Site.Name,
                    WoundType = report.WoundType.Name,
                    PatientName = report.Patient.FullName,

                };

                var a = report.Assessments.OrderByDescending(x => x.AssessmentDate).FirstOrDefault();

                if (a != null)
                {
                    e.LastAssessment = a.AssessmentDate.FormatAsShortDate();
                    e.Change = System.Enum.GetName(typeof(Domain.Enumerations.WoundProgress), a.Progress).SplitPascalCase();
                }

                this.Entries.Add(e);
            }

            this.Entries = this.Entries.OrderBy(x => x.PatientName).ToList();
        }

        public class Entry
        {
            public string PatientName { get; set; }
            public string WoundType { get; set; }
            public string WoundSite { get; set; }
            public string Change { get; set; }
            public string LastAssessment { get; set; }

        }
    }
}
