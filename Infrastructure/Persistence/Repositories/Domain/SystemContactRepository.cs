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
    public class SystemContactRepository: AbstractRepository<IDataContext>, ISystemContactRepository
    {
        public SystemContactRepository(IDataContext dataContext)
            : base(dataContext) { }


        public void Add(SystemContact src)
        {
            DataContext.TrackChanges(src);
        }

        public SystemContact Get(int id)
        {
            return DataContext.Fetch<SystemContact>(id);
        }

        public IPagedQueryResult<SystemContact> Find(
            int? accountId, 
            int? leadId, 
            string firstName, 
            string lastName, 
            string title, 
            string cell, 
            string direct, 
            string email, 
            Expression<Func<SystemContact, object>> sortByExpression, 
            bool sortDescending, 
            int page, 
            int pageSize)
        {
            var q = DataContext.CreateQuery<SystemContact>();

            if (accountId.HasValue)
            {
                q = q.FilterBy(x => x.Account.Id == accountId);
            }

            if (leadId.HasValue)
            {
                q = q.FilterBy(x => x.SystemLead.Id == leadId);
            }

            if(firstName.IsNotNullOrEmpty())
            {
                q = q.FilterBy(x => x.FirstName.Contains(firstName));
            }

            if (lastName.IsNotNullOrEmpty())
            {
                q = q.FilterBy(x => x.LastName.Contains(firstName));
            }

            if (title.IsNotNullOrEmpty())
            {
                q = q.FilterBy(x => x.Title.Contains(firstName));
            }

            if (cell.IsNotNullOrEmpty())
            {
                q = q.FilterBy(x => x.Cell.Contains(firstName));
            }

            if (direct.IsNotNullOrEmpty())
            {
                q = q.FilterBy(x => x.Direct.Contains(firstName));
            }

            if (email.IsNotNullOrEmpty())
            {
                q = q.FilterBy(x => x.Email.Contains(firstName));
            }

            return q.SortBy(sortByExpression)
                .DescendingWhen(sortDescending)
                .PageSize(pageSize)
                .FetchPage(page);
        }

    }
}
