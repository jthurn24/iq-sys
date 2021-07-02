using System;
using System.Linq.Expressions;
using RedArrow.Framework.Persistence;
using IQI.Intuition.Domain.Models;
using System.Collections.Generic;

namespace IQI.Intuition.Domain.Repositories
{
    public interface ISystemSecureFileRepository
    {
        void Add(SystemSecureFile src);
        SystemSecureFile Get(int id);

        IPagedQueryResult<SystemSecureFile> Find(
            int? accountId,
            int? leadId,
            int? ticketId,
            DateTime? expiresAfter,
            string description,
            Expression<Func<SystemSecureFile, object>> sortByExpression,
            bool sortDescending, int page, int pageSize);

    }
}

