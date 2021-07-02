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
    public class WarningRepository : AbstractRepository<IDataContext>, IWarningRepository
    {
        public WarningRepository(IDataContext dataContext)
            : base(dataContext) { }

        public Warning Get(int id)
        {
            return DataContext.Fetch<Warning>(id);
        }

        public void Add(WarningRule rule)
        {
            DataContext.TrackChanges(rule);
        }

        public IPagedQueryResult<Warning> SearchFacility(
            int facilityId,
            string triggeredOn,
            string title,
            string patientName,
            Expression<Func<Warning, object>> sortByExpression,
            bool sortDescending,
            int page,
            int pageSize)
        {

            int[] patientIDList = DataContext.CreateQuery<Patient>()
                .FilterBy(x => x.Room.Wing.Floor.Facility.Id == facilityId)
                .FetchAll().Select(x => x.Id).ToArray();

            var query = DataContext.CreateQuery<Warning>()
              .FilterBy(x => (x.Patient != null && x.Patient.Room.Wing.Floor.Facility.Id == facilityId) || x.Facility.Id == facilityId);

            if (triggeredOn.IsNotNullOrEmpty())
            {
                DateTime triggeredOnDate;

                if (DateTime.TryParse(triggeredOn, out triggeredOnDate))
                {
                    query = query.FilterBy(x => x.TriggeredOn >= triggeredOnDate && x.TriggeredOn < triggeredOnDate.AddDays(1));
                }
            }

            if (title.IsNotNullOrEmpty())
            {
                query = query.FilterBy(x => x.Title.Contains(title));
            }

            var results = query.FetchAll();

            if (patientName.IsNotNullOrEmpty())
            {
                results = results.Where(x => x.Patient != null && (x.Patient.GetFirstName().ToLower().Contains(patientName.ToLower()) || x.Patient.GetLastName().ToLower().Contains(patientName.ToLower())));
            }

            if (sortDescending)
            {
                results = results.AsQueryable().OrderByDescending(sortByExpression);
            }
            else
            {
                results = results.AsQueryable().OrderBy(sortByExpression);
            }

            var pager = new RedArrow.Framework.Persistence.PagedQueryResult<Warning>();
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

        public IEnumerable<WarningRule> GetForFacility(int id)
        {
            return DataContext.CreateQuery<WarningRule>()
                .FilterBy(x => x.Facility.Id == id)
                .FetchAll();
        }

        public IEnumerable<WarningRuleDefault> GetDefaults()
        {
            return DataContext.CreateQuery<WarningRuleDefault>().FetchAll();
        }

    }
}
