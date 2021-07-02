using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IQI.Intuition.Infrastructure.Services.Reporting.IntegrityService
{
    public interface IScanService
    {
        void Run(
            Domain.Models.Facility dfacility,
            IQI.Intuition.Reporting.Models.Dimensions.Facility rFacility,
            IList<VarianceDetails> variances,
            int scanDays);

    }
}
