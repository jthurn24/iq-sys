using System;
using System.Collections.Generic;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Persistence;
using IQI.Intuition.Domain.Models;
using IQI.Intuition.Domain.Repositories;

namespace IQI.Intuition.Infrastructure.Persistence.Repositories.Domain
{
    public class LabResultRepository : AbstractRepository<IDataContext>, ILabResultRepository
    {
        public LabResultRepository(IDataContext dataContext)
            : base(dataContext) { }

        public IEnumerable<LabTestType> AllTestTypes
        {
            get
            {
                return DataContext.CreateQuery<LabTestType>().FetchAll();
            }
        }

        public IEnumerable<LabResult> AllResults
        {
            get
            {
                return DataContext.CreateQuery<LabResult>().FetchAll();
            }
        }

        public IEnumerable<Pathogen> AllPathogens
        {
            get
            {
                return DataContext.CreateQuery<Pathogen>().FetchAll();
            }
        }

        public IEnumerable<PathogenQuantity> AllPathogenQuantities
        {
            get
            {
                return DataContext.CreateQuery<PathogenQuantity>().FetchAll();
            }
        }

        public IEnumerable<InfectionLabResultPathogen> FindPathogens(int facilityId, DateTime startdate, DateTime enddate, int? wingId)
        {
            var q = DataContext.CreateQuery<InfectionLabResultPathogen>()
                .FilterBy(x =>
                    x.InfectionLabResult.InfectionVerification.Patient != null
                    && x.InfectionLabResult.InfectionVerification.Room.Wing.Floor.Facility.Id == facilityId
                    && x.InfectionLabResult.CompletedOn.HasValue
                    && x.InfectionLabResult.CompletedOn >= startdate
                    && x.InfectionLabResult.CompletedOn <= enddate);

            if (wingId.HasValue)
            {
                q = q.FilterBy(x => x.InfectionLabResult.InfectionVerification.Room.Wing.Id == wingId.Value);
            }

            return q.FetchAll();
        }

    }
}
