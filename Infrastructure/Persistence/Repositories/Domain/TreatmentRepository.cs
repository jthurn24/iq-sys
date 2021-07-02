using System;
using System.Collections.Generic;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Persistence;
using IQI.Intuition.Domain.Models;
using IQI.Intuition.Domain.Repositories;

namespace IQI.Intuition.Infrastructure.Persistence.Repositories.Domain
{
    public class TreatmentRepository : AbstractRepository<IDataContext>, ITreatmentRepository
    {
        public TreatmentRepository(IDataContext dataContext)
            : base(dataContext) { }

        #region ITreatmentRepository Members

        public IEnumerable<Treatment> AllTreatments
        {
            get { return DataContext.CreateQuery<Treatment>().FetchAll(); }
        }

        public IEnumerable<TreatmentType> AllTreatmentTypes
        {
            get { return DataContext.CreateQuery<TreatmentType>().FetchAll(); }
        }

        #endregion
    }
}
