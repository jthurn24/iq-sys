using System;
using System.Linq.Expressions;
using RedArrow.Framework.Persistence;
using IQI.Intuition.Domain.Models;
using System.Collections.Generic;

namespace IQI.Intuition.Domain.Repositories
{
    public interface ISystemLeadRepository
    {
        void Add(SystemLead src);
        SystemLead Get(int id);

        IPagedQueryResult<SystemLead> Find(
            string name,
            string status,
            string state,
            string city,
            string beds,
            string facilityType,
            string lastContactedOn,
            string multiHome,
            int? assignedTo,
            Expression<Func<SystemLead, object>> sortByExpression,
            bool sortDescending, int page, int pageSize);

        IEnumerable<SystemLead> FindActive();


    }
}
