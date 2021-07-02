using System;
using System.Linq.Expressions;
using RedArrow.Framework.Persistence;
using IQI.Intuition.Domain.Models;
using System.Collections.Generic;

namespace IQI.Intuition.Domain.Repositories
{
    public interface IPatientCensusRepository
    {
        PatientCensus Get(int id);
        void Add(PatientCensus source);
        void Ensure(Facility facility, int years);
        IPagedQueryResult<PatientCensus> Find(Facility facility,int? month, int? year, int page, int pageSize);
        IEnumerable<PatientCensus> Find(Facility facility, int? month, int? year);
    }
}
