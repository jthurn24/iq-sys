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
    public class CatheterRespository : AbstractRepository<IDataContext>, ICatheterRepository
    {
        public CatheterRespository(IDataContext dataContext)
            : base(dataContext) { }


        public void Add(CatheterEntry entry)
        {
            DataContext.TrackChanges(entry);
        }

        public void Add(CatheterAssessment assessment)
        {
            DataContext.TrackChanges(assessment);
        }

        public CatheterEntry Get(int id)
        {
            return DataContext.Fetch<CatheterEntry>(id);
        }

        public CatheterEntry Get(Guid guid)
        {
            return DataContext.CreateQuery<CatheterEntry>()
                .FilterBy(x => x.Guid == guid)
                .FetchAll().FirstOrDefault();
        }

        public CatheterAssessment GetAssessment(int id)
        {
            return DataContext.Fetch<CatheterAssessment>(id);
        }

        public void Delete(CatheterAssessment assessment)
        {
            DataContext.Delete(assessment);
        }

        public IPagedQueryResult<CatheterEntry> Find(Patient patient,
            Expression<Func<CatheterEntry, object>> sortByExpression, 
            bool sortDescending, int page, int pageSize)
        {
            return DataContext.CreateQuery<CatheterEntry>()
                .FilterBy(x => x.Patient == patient)
                .FilterBy(x => x.Deleted == null || x.Deleted == false)
                .SortBy(sortByExpression)
                .DescendingWhen(sortDescending)
                .PageSize(pageSize)
                .FetchPage(page);
        }

        public IEnumerable<CatheterEntry> FindForFacility(Facility facility, IEnumerable<Guid> guids)
        {
            return DataContext.CreateQuery<CatheterEntry>()
                .FilterBy(x => guids.ToList().Contains(x.Guid))
                .FilterBy(x => x.Deleted == null || x.Deleted == false)
                .FetchAll();
        }

        public IEnumerable<CatheterEntry> FindForLineListing(Facility facility, int? wingId, DateTime? startDate, DateTime? endDate, bool includeDiscontinued)
        {
            var query = DataContext.CreateQuery<CatheterEntry>()
               .FilterBy(i => i.Room.Wing.Floor.Facility == facility)
               .FilterBy(i => i.Deleted == null || i.Deleted == false);

            if (wingId.HasValue)
            {
                query = query.FilterBy(x => x.Room.Wing.Id == wingId.Value);
            }

            if (startDate.HasValue)
            {
                query = query.FilterBy(x => x.DiscontinuedOn == null || x.DiscontinuedOn >= startDate || x.StartedOn >= startDate);
            }

            if (endDate.HasValue)
            {
                query = query.FilterBy(x => (x.DiscontinuedOn == null && x.StartedOn <= endDate.Value) || x.DiscontinuedOn <= endDate.Value || x.StartedOn <= endDate.Value);
            }

            if (includeDiscontinued == false)
            {
                query = query.FilterBy(x => x.DiscontinuedOn == null);
            }

            return query.FetchAll();

        }


        public IPagedQueryResult<CatheterEntry> Find(Facility facility, 
            string patientName, 
            string roomAndWingName,
            string startDate,
            string endDate,
            string diagnosis,
            Expression<Func<CatheterEntry, object>> sortByExpression,
            bool sortDescending, int page, int pageSize)
        {
            var query = DataContext.CreateQuery<CatheterEntry>()
               .FilterBy(i => i.Room.Wing.Floor.Facility == facility)
               .FilterBy(i => i.Deleted == null || i.Deleted == false);

            if (roomAndWingName.IsNotNullOrWhiteSpace())
            {
                query = query.FilterBy(i => (i.Room.Name + " " + i.Room.Wing.Name).Contains(roomAndWingName));
            }

            if (diagnosis.IsNotNullOrWhiteSpace())
            {
                query = query.FilterBy(x => x.Diagnosis.Contains(diagnosis));
            }



            if (startDate.IsNotNullOrWhiteSpace())
            {
                DateTime sdate;

                if (DateTime.TryParse(startDate, out sdate))
                {
                    query = query.FilterBy(x => x.StartedOn.Value == sdate);
                }
            }

            if (endDate.IsNotNullOrWhiteSpace())
            {
                DateTime dDate;

                if (DateTime.TryParse(endDate, out dDate))
                {
                    query = query.FilterBy(x => x.DiscontinuedOn != null && x.DiscontinuedOn == dDate);
                }
            }


            var results = query.FetchAll();

            if (patientName.IsNotNullOrWhiteSpace())
            {
                results = results.Where(x => (x.Patient.GetFirstName() + " " + x.Patient.GetLastName()).ToLower().Contains(patientName.ToLower()));
            }


            if (sortDescending)
            {
                results = results.AsQueryable().OrderByDescending(sortByExpression);
            }
            else
            {
                results = results.AsQueryable().OrderBy(sortByExpression);
            }

            var pager = new RedArrow.Framework.Persistence.PagedQueryResult<CatheterEntry>();
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

        public IPagedQueryResult<CatheterAssessment> FindAssessment(CatheterEntry entry,
                        Expression<Func<CatheterAssessment, object>> sortByExpression,
                        bool sortDescending, int page, int pageSize)
        {
            var query = DataContext.CreateQuery<CatheterAssessment>()
                .FilterBy(x => x.CatheterEntry.Id == entry.Id);

            return query.SortBy(sortByExpression)
                .DescendingWhen(sortDescending)
                .PageSize(pageSize)
                .FetchPage(page);
        }

    }
}
