using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IQI.Intuition.Exi.DataSources.QICast.Statistics.Interfaces
{
    public interface IStatProvider
    {
        Models.QICast.Statistic GetResult(Guid facilityGuid);
    }
}
