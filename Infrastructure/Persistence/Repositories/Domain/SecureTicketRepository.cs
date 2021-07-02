using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Persistence;
using IQI.Intuition.Domain.Models;
using IQI.Intuition.Domain.Repositories;
using IQI.Intuition.Domain;

namespace IQI.Intuition.Infrastructure.Persistence.Repositories.Domain
{
    public class SecureTicketRepository : AbstractRepository<IDataContext>, ISystemSecureFileRepository
    {
        public SecureTicketRepository(IDataContext dataContext)
            : base(dataContext) { }
    
        public void  Add(SystemSecureFile src)
        {
            DataContext.TrackChanges(src);
        }

        public SystemSecureFile  Get(int id)
        {
            return DataContext.Fetch<SystemSecureFile>(id);
        }

        public IPagedQueryResult<SystemSecureFile>  Find(
            int? accountId, 
            int? leadId, 
            int? ticketId, 
            DateTime? expiresAfter,
            string description,
            Expression<Func<SystemSecureFile,object>> sortByExpression, bool sortDescending, int page, int pageSize)
        {
            var q = DataContext.CreateQuery<SystemSecureFile>();

            if (accountId.HasValue)
            {
                q = q.FilterBy(x => x.Account.Id == accountId.Value);
            }

            if (leadId.HasValue)
            {
                q = q.FilterBy(x => x.Lead.Id == leadId.Value);
            }

            if (ticketId.HasValue)
            {
                q = q.FilterBy(x => x.Ticket.Id == ticketId.Value);
            }

            if (expiresAfter.HasValue)
            {
                q = q.FilterBy(x => x.ExpiresOn < expiresAfter.Value);
            }

            if (description.IsNotNullOrEmpty())
            {
                q = q.FilterBy(x => x.Description.Contains(description));
            }

            return q.SortBy(sortByExpression)
                .DescendingWhen(sortDescending)
                .PageSize(pageSize)
                .FetchPage(page);
        }

    }
}
