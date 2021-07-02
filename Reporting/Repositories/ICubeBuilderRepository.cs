using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQI.Intuition.Reporting.Models.Cubes;
using IQI.Intuition.Reporting.Models.Dimensions;

namespace IQI.Intuition.Reporting.Repositories
{
    public interface ICubeBuilderRepository 
    {
        FacilityMonthCensus GetOrCreateCensus(Facility facility, Month month);
    }
}
