using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IQI.Intuition.Reporting.Models.User
{
    public class QINote : BaseReportingEntity
    {
        public string Content { get; set; }
        public byte[] Image { get; set; }
        public Guid FacilityId { get; set; }
    }
}
