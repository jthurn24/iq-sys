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
    public class PatientCensusRepository : AbstractRepository<IDataContext>, IPatientCensusRepository
    {
        public PatientCensusRepository(IDataContext dataContext)
            : base(dataContext) { }

        public void Add(PatientCensus source)
        {
            DataContext.TrackChanges(source);
        }

        public PatientCensus Get(int id)
        {
            return DataContext.Fetch<PatientCensus>(id);
        }

        public void Ensure(Facility facility, int years)
        {
            int targetYear = DateTime.Today.AddYears(0 - years).Year;
            var exisiting = DataContext.CreateQuery<PatientCensus>()
                .FilterBy(x => x.Year >= targetYear)
                .FilterBy(x => x.Facility.Id == facility.Id)
                .FetchAll();

            DateTime theDate = new DateTime(targetYear, 1, 1);

            while (theDate <= DateTime.Today)
            {
                if (exisiting.Where(x => x.Year == theDate.Year && x.Month == theDate.Month).Count() < 1)
                {
                    var census = new PatientCensus()
                    {
                         Facility = facility,
                         Month = theDate.Month,
                         Year = theDate.Year,
                         PatientDays = 0
                    };

                    Add(census);
                }

                theDate = theDate.AddMonths(1);
            }

        }

        public IPagedQueryResult<PatientCensus> Find(Facility facility, int? month, int? year, int page, int pageSize)
        {
            var query = DataContext.CreateQuery<PatientCensus>()
                .FilterBy(x => x.Facility.Id == facility.Id);

            if (month.HasValue)
            {
                query = query.FilterBy(x => x.Month == month.Value);
            }

            if (year.HasValue)
            {
                query = query.FilterBy(x => x.Year == year.Value);
            }

            return query.SortBy(x => x.Year).Descending().ThenSortBy(x => x.Month).Descending()
                .PageSize(pageSize)
                .FetchPage(page);
        }

        public IEnumerable<PatientCensus> Find(Facility facility, int? month, int? year)
        {
            var query = DataContext.CreateQuery<PatientCensus>()
                .FilterBy(x => x.Facility.Id == facility.Id);

            if (month.HasValue)
            {
                query = query.FilterBy(x => x.Month == month.Value);
            }

            if (year.HasValue)
            {
                query = query.FilterBy(x => x.Year == year.Value);
            }

            return query.SortBy(x => x.Year).Descending().ThenSortBy(x => x.Month).Descending().FetchAll();
        }

    }
}
