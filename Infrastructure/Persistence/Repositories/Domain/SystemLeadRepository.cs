using System;
using System.Collections.Generic;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Persistence;
using IQI.Intuition.Domain.Models;
using IQI.Intuition.Domain;
using IQI.Intuition.Domain.Repositories;
using System.Linq.Expressions;

namespace IQI.Intuition.Infrastructure.Persistence.Repositories.Domain
{
    public class SystemLeadRepository : AbstractRepository<IDataContext>, ISystemLeadRepository
    {
        public SystemLeadRepository(IDataContext dataContext)
            : base(dataContext) { }

        public void Add(SystemLead src)
        {
            DataContext.TrackChanges(src);
        }

        SystemLead ISystemLeadRepository.Get(int id)
        {
            return DataContext.Fetch<SystemLead>(id);
        }

        public IEnumerable<SystemLead> FindActive()
        {
            return DataContext.CreateQuery<SystemLead>()
                .FilterBy(x => x.Status != Enumerations.SystemLeadStatus.NotInterested
                    && x.Status != Enumerations.SystemLeadStatus.Converted)
                    .FetchAll();

        }

        public IPagedQueryResult<SystemLead> Find(
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
            bool sortDescending, int page, int pageSize)
        {
            var q = DataContext.CreateQuery<SystemLead>();

            if (name.IsNotNullOrEmpty())
            {
                q = q.FilterBy(x => x.Name.Contains(name));
            }

            if (status.IsNotNullOrEmpty())
            {
                q = q.FilterBy(x => x.Status == (Enumerations.SystemLeadStatus)System.Enum.Parse(typeof(Enumerations.SystemLeadStatus), status));
            }

            if (city.IsNotNullOrEmpty())
            {
                q = q.FilterBy(x => x.City.Contains(city));
            }

            if (state.IsNotNullOrEmpty())
            {
                q = q.FilterBy(x => x.State.Contains(state));
            }

            if (multiHome.IsNotNullOrEmpty())
            {
                q = q.FilterBy(x => x.MultiHome.Contains(multiHome));
            }

            if (beds.IsNotNullOrEmpty())
            {
                int bedCount = 0;
                Int32.TryParse(beds, out bedCount);
                q = q.FilterBy(x => x.Beds >= bedCount);
            }

            if (facilityType.IsNotNullOrEmpty())
            {
                q = q.FilterBy(x => x.FacilityType == (Enumerations.FacilityType)System.Enum.Parse(typeof(Enumerations.FacilityType), facilityType));
            }

            if(lastContactedOn.IsNotNullOrEmpty())
            {
                DateTime lcd;

                if(DateTime.TryParse(lastContactedOn,out lcd))
                {
                    q = q.FilterBy(x => x.LastContactedOn == lcd);
                }

            }

            if (assignedTo.HasValue)
            {
                q = q.FilterBy(x => x.SystemUser.Id == assignedTo.Value);
            }
            else
            {
                q = q.FilterBy(x => x.SystemUser == null);
            }

            return q.SortBy(sortByExpression)
                .DescendingWhen(sortDescending)
                .PageSize(pageSize)
                .FetchPage(page);
        }

    }
}
