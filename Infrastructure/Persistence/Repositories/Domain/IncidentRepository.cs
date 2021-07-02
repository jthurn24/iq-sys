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
    public class IncidentRepository : AbstractRepository<IDataContext>, IIncidentRepository
    {
        public IncidentRepository(IDataContext dataContext)
            : base(dataContext) { }

        public void Add(IncidentReport incident)
        {
            DataContext.TrackChanges(incident);
        }

        public IncidentReport Get(int id)
        {
            return DataContext.Fetch<IncidentReport>(id);
        }

        public IncidentReport Get(Guid guid)
        {
            return DataContext.CreateQuery<IncidentReport>().FilterBy(x => x.Guid == guid).FetchAll().FirstOrDefault();
        }

        public void Delete(IncidentWitness value)
        {
            DataContext.Delete(value);
        }


        public IEnumerable<IncidentReport> FindForFacility(Facility facility, IEnumerable<Guid> guids)
        {
            var q = DataContext.CreateQuery<IncidentReport>()
            .FilterBy(x => x.Deleted == null || x.Deleted != true)
            .FilterBy(x => x.Room.Wing.Floor.Facility.Id == facility.Id)
            .FilterBy(x => guids.ToList().Contains(x.Guid));

            return q.FetchAll();
        }

        public IEnumerable<IncidentReport> FindForLineListing(Facility facility,
        int? wingId, int? floorId, IList<string> groups, IList<int> injuries, DateTime? startDate,DateTime? endDate)
        {
            var query = DataContext.CreateQuery<IncidentReport>()
                .FilterBy(i => i.Room.Wing.Floor.Facility.Id == facility.Id)
                .FilterBy(i => i.Deleted == null || i.Deleted == false);

            if (wingId.HasValue)
            {
                query = query.FilterBy(x => x.Room.Wing.Id == wingId);
            }

            if (floorId.HasValue)
            {
                query = query.FilterBy(x => x.Room.Wing.Floor.Id == floorId.Value);
            }


            if (startDate.HasValue)
            {
                query = query.FilterBy(x => x.OccurredOn >= startDate || x.DiscoveredOn >= startDate);
            }

            if (endDate.HasValue)
            {
                DateTime eDate = endDate.Value.AddDays(1);
                query = query.FilterBy(x => x.OccurredOn < eDate || x.DiscoveredOn < eDate);
            }

            var postFiltered = query.FetchAll();

            if(groups != null && groups.Count() > 0)
            {
                postFiltered = postFiltered.Where(x => x.IncidentTypes.Select(xx => xx.GroupName).ContainsAny(groups)).ToList();
            }

            if (injuries != null && injuries.Count() > 0)
            {
                postFiltered = postFiltered.Where(x => x.IncidentInjuries.Select(xx => xx.Id).ContainsAny(injuries)).ToList();
            }

            return postFiltered.OrderByDescending(x => x.OccurredOn.HasValue ? x.OccurredOn : x.DiscoveredOn);
        }

        public IPagedQueryResult<IncidentReport> Find(Facility facility, string patientName, string roomAndWingName, string type,
            string discoveredOn, string occurredOn, string injury, 
            Expression<Func<IncidentReport, object>> sortByExpression, bool sortDescending, int page, int pageSize)
        {
            var query = DataContext.CreateQuery<IncidentReport>()
               .FilterBy(i => i.Room.Wing.Floor.Facility == facility)
               .FilterBy(i => i.Deleted == null || i.Deleted == false);

            if (roomAndWingName.IsNotNullOrWhiteSpace())
            {
                query = query.FilterBy(i => (i.Room.Name + " " + i.Room.Wing.Name).Contains(roomAndWingName));
            }

            if (discoveredOn.IsNotNullOrWhiteSpace())
            {
                DateTime discoveredOndate;

                if (DateTime.TryParse(discoveredOn, out discoveredOndate))
                {
                    query = query.FilterBy(incident => incident.DiscoveredOn.Value == discoveredOndate);
                }
            }

            if (occurredOn.IsNotNullOrWhiteSpace())
            {
                DateTime occurredOndate;

                if (DateTime.TryParse(occurredOn, out occurredOndate))
                {
                    query = query.FilterBy(incident => incident.OccurredOn.Value == occurredOndate);
                }
            }

            if (injury.IsNotNullOrEmpty())
            {
                var value = (IQI.Intuition.Domain.Enumerations.InjuryLevel)(System.Enum.Parse(typeof(IQI.Intuition.Domain.Enumerations.InjuryLevel),injury));

                query = query.FilterBy(incident => incident.InjuryLevel == value);
            }

            var results = query.FetchAll();

            if (patientName.IsNotNullOrWhiteSpace())
            {
                results = results.Where(incident => (incident.Patient.GetFirstName() + " " + incident.Patient.GetLastName()).ToLower().Contains(patientName.ToLower()));
            }

            if (type.IsNotNullOrWhiteSpace())
            {
                results = results.Where(i => i.IncidentTypes.Select(x => x.Name).Contains(type));
            }

            if (sortDescending)
            {
                results = results.AsQueryable().OrderByDescending(sortByExpression);
            }
            else
            {
                results = results.AsQueryable().OrderBy(sortByExpression);
            }

            var pager = new RedArrow.Framework.Persistence.PagedQueryResult<IncidentReport>();
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

        public IPagedQueryResult<IncidentReport> Find(Patient patient, Expression<Func<IncidentReport, object>> sortByExpression, bool sortDescending, int page, int pageSize)
        {
            return DataContext.CreateQuery<IncidentReport>()
            .FilterBy(i => i.Patient == patient)
            .FilterBy(i => i.Deleted == null || i.Deleted == false)
            .SortBy(sortByExpression)
            .DescendingWhen(sortDescending)
            .PageSize(pageSize)
            .FetchPage(page);
        }

        public IEnumerable<IncidentReport> FindCreatedIn(int month, int year)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            return DataContext.CreateQuery<IncidentReport>()
                .FilterBy(x => x.CreatedAt >= startDate && x.CreatedAt <= endDate)
                .FetchAll();
        }

        public IEnumerable<IncidentLocation> AllLocations
        {
            get
            {
                return DataContext.CreateQuery<IncidentLocation>()
                    .SortBy(y => y.Name)
                    .FetchAll();
            }
        }

        public IEnumerable<IncidentType> AllTypes
        {
            get
            {
                return DataContext.CreateQuery<IncidentType>()
                    .SortBy(y => y.SortOrder)
                    .FetchAll();
            }
        }

        public IEnumerable<IncidentInjury> AllInjuries
        {
            get
            {
                return DataContext.CreateQuery<IncidentInjury>()
                    .SortBy(y => y.SortOrder)
                    .FetchAll();
            }
        }
    }
}
