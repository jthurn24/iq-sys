using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IQI.Intuition.Reporting.Models.User
{
    public class QICastCommand : BaseReportingEntity 
    {
        public string Command { get; set; }
        public Guid QiCastId { get; set; }
    }
}
