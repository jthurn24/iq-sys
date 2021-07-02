using System;
using System.Linq.Expressions;
using RedArrow.Framework.Persistence;
using IQI.Intuition.Domain.Models;
using System.Collections.Generic;

namespace IQI.Intuition.Domain.Repositories
{
    public interface IAccountRepository
    {
        void Add(Account account);

        Account Get(int id);

        Account Get(Guid guid);

        IEnumerable<Account> GetAll();

        IPagedQueryResult<Account> Find(
            string name,
            Expression<Func<Account, object>> sortByExpression,
            bool sortDescending, int page, int pageSize);


    }
}