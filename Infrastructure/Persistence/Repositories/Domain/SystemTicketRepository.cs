using System;
using System.Collections.Generic;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Persistence;
using IQI.Intuition.Domain.Models;
using IQI.Intuition.Domain;
using IQI.Intuition.Domain.Repositories;


namespace IQI.Intuition.Infrastructure.Persistence.Repositories.Domain
{
    public class SystemTicketRepository : AbstractRepository<IDataContext>, ISystemTicketRepository
    {
        public SystemTicketRepository(IDataContext dataContext)
            : base(dataContext) { }

        public void Add(SystemTicket src)
        {
            DataContext.TrackChanges(src);
        }

        public SystemTicket Get(int id)
        {
            return DataContext.Fetch<SystemTicket>(id);
        }

        public IPagedQueryResult<SystemTicket> Find(
            int? accountId,
            string accountName,
            string accountUser, 
            string ticketType, 
            string details, 
            int? priority, 
            string systemUser, 
            string release,
            Enumerations.SystemTicketSearchMode searchMode,
            System.Linq.Expressions.Expression<Func<SystemTicket, object>> sortByExpression, 
            bool sortDescending, 
            int page, 
            int pageSize)
        {
            var q = DataContext.CreateQuery<SystemTicket>();

            if (accountId.HasValue)
            {
                q = q.FilterBy(x => x.Account.Id == accountId.Value);
            }

            if (accountName.IsNotNullOrEmpty())
            {
                q = q.FilterBy(x => x.Account.Name.Contains(accountName));
            }

            if (accountUser.IsNotNullOrEmpty())
            {
                q = q.FilterBy(x => x.AccountUser.Login.Contains(accountUser));
            }

            if (ticketType.IsNotNullOrEmpty())
            {
                q = q.FilterBy(x => x.SystemTicketType.Name.Contains(ticketType));
            }

            if (details.IsNotNullOrEmpty())
            {
                q = q.FilterBy(x => x.Details.Contains(details));
            }

            if (priority.HasValue)
            {
                q = q.FilterBy(x => x.Priority >= priority.Value);
            }

            if (systemUser.IsNotNullOrEmpty())
            {
                q = q.FilterBy(x => x.SystemUser.Login.Contains(systemUser));
            }

            if (release.IsNotNullOrEmpty())
            {
                q = q.FilterBy(x => x.Release.Contains(release));
            }

            if (searchMode ==  Enumerations.SystemTicketSearchMode.ClosedOnly)
            {
                q = q.FilterBy(x => x.Status == Enumerations.SystemTicketStatus.Closed);
            }

            if (searchMode == Enumerations.SystemTicketSearchMode.OpenOnly)
            {
                q = q.FilterBy(x => x.Status != Enumerations.SystemTicketStatus.Closed);
            }

            return q.SortBy(sortByExpression)
                .DescendingWhen(sortDescending)
                .PageSize(pageSize)
                .FetchPage(page);
        }

        public IEnumerable<SystemTicket> Find(
            Enumerations.SystemTicketSearchMode mode,
            Enumerations.KnownSystemTicketType type
            )
        {
            var q =  DataContext.CreateQuery<SystemTicket>()
                .FilterBy(x => x.SystemTicketType.Id == (int)type);

            if (mode == Enumerations.SystemTicketSearchMode.ClosedOnly)
            {
                q = q.FilterBy(x => x.Status == Enumerations.SystemTicketStatus.Closed);
            }

            if (mode == Enumerations.SystemTicketSearchMode.OpenOnly)
            {
                q = q.FilterBy(x => x.Status != Enumerations.SystemTicketStatus.Closed);
            }

            return q.FetchAll();
        }

        public IEnumerable<SystemTicket> FindClosedIn(int month, int year)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            return DataContext.CreateQuery<SystemTicket>()
                .FilterBy(x => x.ClosedOn >= startDate && x.ClosedOn <= endDate)
                .FetchAll();
        }

        public IEnumerable<SystemTicketType> AllTicketTypes
        {
            get {

                return DataContext.CreateQuery<SystemTicketType>().FetchAll();

            }
        }

    }
}
