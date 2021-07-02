using System;
using System.Collections.Generic;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Persistence;
using IQI.Intuition.Domain.Models;
using IQI.Intuition.Domain.Repositories;
using System.Linq.Expressions;

namespace IQI.Intuition.Infrastructure.Persistence.Repositories.Domain
{
    public class AccountRepository : AbstractRepository<IDataContext>, IAccountRepository
    {
        public AccountRepository(IDataContext dataContext)
            : base(dataContext) { }

        public void Add(Account account)
        {
            DataContext.TrackChanges(account);
        }

        public Account Get(int id)
        {
            return DataContext.Fetch<Account>(id);
        }

        public Account Get(Guid guid)
        {
            return DataContext.CreateQuery<Account>()
                .FilterBy(account => account.Guid == guid)
                .FetchFirst();
        }


        public IEnumerable<Account> GetAll()
        {
            return DataContext.CreateQuery<Account>()
            .FetchAll();
        }


        public IPagedQueryResult<Account> Find(
            string name, 
            Expression<Func<Account, object>> sortByExpression, 
            bool sortDescending, 
            int page, 
            int pageSize)
        {

            var q = DataContext.CreateQuery<Account>();

            if (name.IsNotNullOrEmpty())
            {
                q = q.FilterBy(x => x.Name.Contains(name));
            }

            return q.SortBy(sortByExpression)
                .DescendingWhen(sortDescending)
                .PageSize(pageSize)
                .FetchPage(page);
        }

    }
}
