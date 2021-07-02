using System;
using System.Linq.Expressions;
using RedArrow.Framework.Persistence;
using IQI.Intuition.Domain.Models;
using System.Collections.Generic;

namespace IQI.Intuition.Domain.Repositories
{
    public interface ILabResultRepository
    {

        IEnumerable<InfectionLabResultPathogen> FindPathogens(int facilityId, DateTime startdate, DateTime enddate, int? wingId);

        IEnumerable<LabTestType> AllTestTypes { get; }
        IEnumerable<LabResult> AllResults { get; }
        IEnumerable<Pathogen> AllPathogens { get; }
        IEnumerable<PathogenQuantity> AllPathogenQuantities { get;  }
    }
}
