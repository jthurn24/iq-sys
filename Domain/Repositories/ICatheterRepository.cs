using System;
using System.Linq.Expressions;
using RedArrow.Framework.Persistence;
using IQI.Intuition.Domain.Models;
using System.Collections.Generic;

namespace IQI.Intuition.Domain.Repositories
{
    public interface ICatheterRepository
    {
        CatheterEntry Get(int id);
        CatheterEntry Get(Guid guid);

        void Add(CatheterEntry entry);
        void Add(CatheterAssessment assessment);

        CatheterAssessment GetAssessment(int id);
        void Delete(CatheterAssessment assessment);

        IEnumerable<CatheterEntry> FindForFacility(Facility facility, IEnumerable<Guid> guids); 

        IPagedQueryResult<CatheterAssessment> FindAssessment(CatheterEntry entry,
                Expression<Func<CatheterAssessment, object>> sortByExpression,
                bool sortDescending, int page, int pageSize);

        IPagedQueryResult<CatheterEntry> Find(Facility facility, 
            string patientName, 
            string roomAndWingName,
            string startDate,
            string endDate,
            string diagnosis,
            Expression<Func<CatheterEntry, object>> sortByExpression,
            bool sortDescending, int page, int pageSize);

        IPagedQueryResult<CatheterEntry> Find(Patient patient,
            Expression<Func<CatheterEntry, object>> sortByExpression,
            bool sortDescending, int page, int pageSize);

        IEnumerable<CatheterEntry> FindForLineListing(Facility facility,
            int? wingId,
            DateTime? startDate,
            DateTime? endDate,
            bool includeDiscontinued);
    }
}
