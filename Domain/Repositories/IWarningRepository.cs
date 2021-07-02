using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using RedArrow.Framework.Persistence;
using IQI.Intuition.Domain.Models;


namespace IQI.Intuition.Domain.Repositories
{
    public interface IWarningRepository
    {
        Warning Get(int id);

        void Add(WarningRule rule);

        IPagedQueryResult<Warning> SearchFacility(
            int facilityId,
            string triggeredOn,
            string title,
            string patientName,
            Expression<Func<Warning, object>> sortByExpression, 
            bool sortDescending, 
            int page, 
            int pageSize);

        IEnumerable<WarningRule> GetForFacility(int id);
        IEnumerable<WarningRuleDefault> GetDefaults();
    }
}
