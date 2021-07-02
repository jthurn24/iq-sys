using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Persistence;
using IQI.Intuition.Domain.Models;
using IQI.Intuition.Domain.Repositories;

namespace IQI.Intuition.Infrastructure.Persistence.Repositories.Domain
{
    public class EmployeeInfectionRepository : AbstractRepository<IDataContext>, IEmployeeInfectionRepository
    {
        public EmployeeInfectionRepository(IDataContext dataContext)
            : base(dataContext) { }

        public void Add(EmployeeInfection infection)
        {
            DataContext.TrackChanges(infection);
        }

        public void Remove(EmployeeInfection infection)
        {
            DataContext.Delete(infection);
        }

        public IEnumerable<EmployeeInfection> FindForLineListing(Facility facility, 
            DateTime? startDate, 
            DateTime? endDate,
            int? type)
        {
            var q = DataContext.CreateQuery<EmployeeInfection>()
               .FilterBy(x => x.Facility.Id == facility.Id)
               .FilterBy(infection => infection.Deleted == null || infection.Deleted == false);

            if (startDate.HasValue)
            {
                q = q.FilterBy(x => x.NotifiedOn >= startDate || x.FirstSymptomOn >= startDate);
            }

            if (endDate.HasValue)
            {
                q = q.FilterBy(x => x.NotifiedOn <= endDate || x.FirstSymptomOn <= endDate);
            }

            if (type.HasValue)
            {
                q = q.FilterBy(x => x.InfectionType.Id == type.Value);
            }

            return q.SortBy(x => x.NotifiedOn).Descending().FetchAll();
        }

        public EmployeeInfection Get(int id)
        {
            return DataContext.Fetch<EmployeeInfection>(id);
        }

        public IEnumerable<EmployeeInfection> FindCreatedIn(int month, int year)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            return DataContext.CreateQuery<EmployeeInfection>()
                .FilterBy(x => x.CreatedAt >= startDate && x.CreatedAt <= endDate)
                .FetchAll();
        }

        public IPagedQueryResult<EmployeeInfection> Find(Facility facility, 
            string employeeName,
            int? infectionType,
            int? department,
            string notifiedOn,
            string wellOn,
            Expression<Func<EmployeeInfection, object>> sortByExpression,
            bool sortDescending, int page, int pageSize)
        {

            var query = DataContext.CreateQuery<EmployeeInfection>()
                         .FilterBy(infection => infection.Facility == facility)
                         .FilterBy(infection => infection.Deleted == null || infection.Deleted == false);


            if (infectionType.HasValue)
            {
                query = query.FilterBy(infection => infection.InfectionType.Id == infectionType.Value);
            }

            if (notifiedOn.IsNotNullOrEmpty())
            {
                DateTime notedDate;

                if (DateTime.TryParse(notifiedOn, out notedDate))
                {
                    query = query.FilterBy(infection => infection.NotifiedOn >= notedDate && infection.NotifiedOn < notedDate.AddDays(1));
                }
            }

            if (wellOn.IsNotNullOrEmpty())
            {
                DateTime wellDate;

                if (DateTime.TryParse(wellOn, out wellDate))
                {
                    query = query.FilterBy(infection => infection.WellOn >= wellDate && infection.WellOn < wellDate.AddDays(1));
                }
            }

            var results = query.FetchAll();

            if (employeeName.IsNotNullOrWhiteSpace())
            {
                results = results.Where(infection => (infection.FullName.ToLower().Contains(employeeName.ToLower())));
            }

            if (sortDescending)
            {
                results = results.AsQueryable().OrderByDescending(sortByExpression);
            }
            else
            {
                results = results.AsQueryable().OrderBy(sortByExpression);
            }


            var pager = new RedArrow.Framework.Persistence.PagedQueryResult<EmployeeInfection>();
            pager.PageSize = pageSize;
            pager.PageNumber = page;
            pager.TotalResults = results.Count();

            if (results.Count() > 1)
            {
                pager.TotalPages = (int)Math.Ceiling((double)pager.TotalResults / pager.PageSize);
            }
            else
            {
                pager.TotalPages = 0;
            }

            pager.PageValues = results.Skip((pager.PageNumber - 1) * pager.PageSize).Take(pager.PageSize);

            return pager;
        }

    }
}
