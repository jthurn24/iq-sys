using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IQI.Intuition.Reporting.Containers;

namespace IQI.Intuition.Reporting.Models.User
{
    public class CubeSyncJob : BaseReportingEntity
    {
        public int Priority { get; set; }
        public DateTime CreatedOn { get; set; }
        public DataDimensions Dimensions { get; set; }
        public string ServiceTypeName { get; set; }
        public DateTime ScanStartDate { get; set; }
        public int FacilityId { get; set; }
    }
}
