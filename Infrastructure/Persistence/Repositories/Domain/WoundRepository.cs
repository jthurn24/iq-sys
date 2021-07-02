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
    public class WoundRepository : AbstractRepository<IDataContext>, IWoundRepository
    {
        public WoundRepository(IDataContext dataContext)
            : base(dataContext) { }

        public void Add(WoundReport report)
        {
            DataContext.TrackChanges(report);
        }

        public void Add(WoundAssessment assessment)
        {
            DataContext.TrackChanges(assessment);
        }

        public WoundReport GetReport(int id)
        {
            return DataContext.Fetch<WoundReport>(id);
        }

        public WoundReport GetReport(Guid guid)
        {
            return DataContext.CreateQuery<WoundReport>()
                .FilterBy(x => x.Guid == guid)
                .FetchAll()
                .FirstOrDefault(); ;
        }

        public WoundAssessment GetAssessment(int id)
        {
            return DataContext.Fetch<WoundAssessment>(id);
        }

        public void Delete(WoundAssessment assessment)
        {
            DataContext.Delete(assessment);
        }

        public IEnumerable<WoundReport> FindReportsForFacility(Facility facility, IEnumerable<Guid> guids)
        {
            var q = DataContext.CreateQuery<WoundReport>()
            .FilterBy(x => x.Deleted == null || x.Deleted != true)
            .FilterBy(x => x.Room.Wing.Floor.Facility.Id == facility.Id)
            .FilterBy(x => guids.ToList().Contains(x.Guid));

            return q.FetchAll();
        }

        public IPagedQueryResult<WoundReport> Find(Patient patient, 
            Expression<Func<WoundReport, object>> sortByExpression, bool sortDescending, int page, int pageSize)
        {
            return DataContext.CreateQuery<WoundReport>()
                .FilterBy(x => x.Patient.Id == patient.Id)
                .FilterBy(x => x.Deleted == null || x.Deleted == false)
                .SortBy(sortByExpression)
                .DescendingWhen(sortDescending)
                .PageSize(pageSize)
                .FetchPage(page);
        }


        public IEnumerable<WoundReport> FindForLineListing(Facility facility,
            int? wingId,
            int? floorId,
            bool includeResolved,
            bool resolvedOnly,
            string startDate,
            string endDate,
            int? currentStage,
            int? classification,
            int? type)
        {

            var query = DataContext.CreateQuery<WoundReport>()
               .FilterBy(x => x.Room.Wing.Floor.Facility == facility)
               .FilterBy(x => x.CurrentStage != null)
               .FilterBy(x => x.Deleted == null || x.Deleted == false);


            if (!includeResolved)
            {
                query = query.FilterBy(x => x.IsResolved == null || x.IsResolved == false);
            }

            if (resolvedOnly)
            {
                query = query.FilterBy(x => x.IsResolved == true);
            }

            if (startDate.IsNotNullOrEmpty())
            {
                DateTime pStartDate;

                if(DateTime.TryParse(startDate, out pStartDate))
                {
                    query = query.FilterBy(x => x.ResolvedOn == null || x.ResolvedOn >= pStartDate || x.FirstNotedOn >= pStartDate);
                }

            }


            if (endDate.IsNotNullOrEmpty())
            {
                DateTime pEndDate;

                if (DateTime.TryParse(endDate, out pEndDate))
                {
                    query = query.FilterBy(x => (x.ResolvedOn == null && x.FirstNotedOn <= pEndDate) || x.ResolvedOn <= pEndDate || x.FirstNotedOn <= pEndDate);
                }

            }

            if (classification.HasValue)
            {
                var enumClassification = (IQI.Intuition.Domain.Enumerations.WoundClassification)classification.Value;
                query = query.FilterBy(x => x.Classification == enumClassification);
            }

            if (currentStage.HasValue)
            {
                query = query.FilterBy(x => x.CurrentStage.Id == currentStage.Value);
            }

            if (type.HasValue)
            {
                query = query.FilterBy(x => x.WoundType.Id == type.Value);
            }



            return query.FetchAll()
                .OrderByDescending(x => x.FirstNotedOn);
        }

        public IPagedQueryResult<WoundReport> FindReport(Facility facility, 
            string patientName, 
            string roomAndWingName, 
            string firstNoted, 
            string dateResolved, 
            bool includeResolved, 
            string stage,
            string siteName,
            string typeName,
            Expression<Func<WoundReport, object>> sortByExpression, 
            bool sortDescending, int page, int pageSize)
        {
            var query = DataContext.CreateQuery<WoundReport>()
               .FilterBy(x => x.Room.Wing.Floor.Facility == facility
               && (includeResolved || x.IsResolved.Value != true))
               .FilterBy(x => x.Deleted == null || x.Deleted == false);


            if (stage.IsNotNullOrEmpty())
            {
                query = query.FilterBy(x => x.CurrentStage.Name.Contains(stage));
            }

            if (siteName.IsNotNullOrEmpty())
            {
                query = query.FilterBy(x => x.Site.Name.Contains(siteName));
            }

            if (typeName.IsNotNullOrEmpty())
            {
                query = query.FilterBy(x => x.WoundType.Name.Contains(typeName));
            }

            if (dateResolved.IsNotNullOrWhiteSpace())
            {
                DateTime date;

                if (DateTime.TryParse(dateResolved, out date))
                {
                    query = query.FilterBy(i => i.ResolvedOn.Value == date);
                }
            }

            if (roomAndWingName.IsNotNullOrWhiteSpace())
            {
                query = query.FilterBy(i => (i.Room.Name + " " + i.Room.Wing.Name).Contains(roomAndWingName));
            }


            if (firstNoted.IsNotNullOrWhiteSpace())
            {
                DateTime date;

                if (DateTime.TryParse(firstNoted, out date))
                {
                    query = query.FilterBy(i => i.FirstNotedOn.Value == date);
                }
            }

            var results = query.FetchAll();

            if (patientName.IsNotNullOrWhiteSpace())
            {
                results = results.Where(i => (i.Patient.GetFirstName() + " " + i.Patient.GetLastName()).ToLower().Contains(patientName.ToLower()));
            }

            if (sortDescending)
            {
                results = results.AsQueryable().OrderByDescending(sortByExpression);
            }
            else
            {
                results = results.AsQueryable().OrderBy(sortByExpression);
            }


            var pager = new RedArrow.Framework.Persistence.PagedQueryResult<WoundReport>();
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


        public IPagedQueryResult<WoundAssessment> FindAssessment(WoundReport report,
            Expression<Func<WoundAssessment, object>> sortByExpression,
            bool sortDescending, int page, int pageSize)
        {
            var query = DataContext.CreateQuery<WoundAssessment>()
                .FilterBy(x => x.Report.Id == report.Id);

            return query.SortBy(sortByExpression)
                .DescendingWhen(sortDescending)
                .PageSize(pageSize)
                .FetchPage(page);
        }

        public IEnumerable<WoundReport> FindActive(Facility facility)
        {
            var query = DataContext.CreateQuery<WoundReport>()
           .FilterBy(x => x.Room.Wing.Floor.Facility == facility && x.IsResolved.Value != true)
           .FilterBy(x => x.CurrentStage != null)
           .FilterBy(x => x.Deleted == null || x.Deleted == false);

            return query.FetchAll();
        }

        public IEnumerable<WoundStage> AllStages
        {
            get {

                return DataContext.CreateQuery<WoundStage>()
                    .FetchAll();
                    
            }
        }

        public IEnumerable<WoundSite> AllSites
        {
            get
            {

                return DataContext.CreateQuery<WoundSite>()
                    .FetchAll();

            }
        }

        public IEnumerable<WoundType> AllTypes
        {
            get
            {

                return DataContext.CreateQuery<WoundType>()
                    .FetchAll();

            }
        }

    }
}
