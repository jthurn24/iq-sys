using System;
using System.Linq.Expressions;
using RedArrow.Framework.Persistence;
using IQI.Intuition.Domain.Models;
using System.Collections.Generic;

namespace IQI.Intuition.Domain.Repositories
{
    public interface ISystemTicketRepository
    {
        void Add(SystemTicket src);
        SystemTicket Get(int id);

        IPagedQueryResult<SystemTicket> Find(
            int? accountId,
            string accountName,
            string accountUser,
            string ticketType,
            string details,
            int? priority,
            string systemUser,
            string release,
            Enumerations.SystemTicketSearchMode searchMode,
            Expression<Func<SystemTicket, object>> sortByExpression,
            bool sortDescending, int page, int pageSize);

        IEnumerable<SystemTicket> Find(
            Enumerations.SystemTicketSearchMode mode,
            Enumerations.KnownSystemTicketType type
            );

        IEnumerable<SystemTicket> FindClosedIn(int month, int year);


        IEnumerable<SystemTicketType> AllTicketTypes { get; }
    }
}
