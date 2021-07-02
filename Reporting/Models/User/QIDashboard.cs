using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IQI.Intuition.Reporting.Models.User
{
    public class QIDashboard : BaseReportingEntity
    {
        public IList<string> Dashboards { get; set; }
    }
}
