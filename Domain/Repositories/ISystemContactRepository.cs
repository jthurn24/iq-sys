using System;
using System.Linq.Expressions;
using RedArrow.Framework.Persistence;
using IQI.Intuition.Domain.Models;
using System.Collections.Generic;

namespace IQI.Intuition.Domain.Repositories
{
    public interface ISystemContactRepository
    {
        void Add(SystemContact src);
        SystemContact Get(int id);

        IPagedQueryResult<SystemContact> Find(
            int? accountId,
            int? leadId,
            string firstName,
            string lastName,
            string title,
            string cell,
            string direct,
            string email,
            Expression<Func<SystemContact, object>> sortByExpression,
            bool sortDescending, int page, int pageSize);

    }
}

