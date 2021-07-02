using System;
using System.Collections.Generic;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Persistence;
using IQI.Intuition.Domain.Models;
using IQI.Intuition.Domain.Repositories;
using System.Linq.Expressions;


namespace IQI.Intuition.Infrastructure.Persistence.Repositories.Domain
{
    public class ComplaintRepository : AbstractRepository<IDataContext>, IComplaintRepository
    {
        public ComplaintRepository(IDataContext dataContext)
            : base(dataContext) { }

        public void Add(Complaint complaint)
        {
            DataContext.TrackChanges(complaint);
        }

        public Complaint Get(int id)
        {
            return DataContext.Fetch<Complaint>(id);
        }

        public Complaint Get(Guid guid)
        {
            return DataContext.CreateQuery<Complaint>()
                .FilterBy(c => c.Guid == guid)
                .FetchFirst();
        }

        public IEnumerable<Complaint> FindForFacility(Facility facility, IEnumerable<Guid> guids)
        {
            var q = DataContext.CreateQuery<Complaint>()
            .FilterBy(x => x.Deleted == null || x.Deleted != true)
            .FilterBy(x => x.Facility.Id == facility.Id)
            .FilterBy(x => guids.ToList().Contains(x.Guid));

            return q.FetchAll();
        }

        public IPagedQueryResult<Complaint> Find(Facility facility, 
            string employeeName, 
            string patientName, 
            string occurredDate, 
            string reportedDate, 
            string wingName,
            string complaintType,
            Expression<Func<Complaint, object>> sortByExpression, 
            bool sortDescending, 
            int page, 
            int pageSize)
        {

            var q = DataContext.CreateQuery<Complaint>()
                .FilterBy(x => x.Deleted == null || x.Deleted != true)
                .FilterBy(x => x.Facility.Id == facility.Id);

            if (wingName.IsNotNullOrEmpty())
            {
                q = q.FilterBy(x => x.Wing.Name.Contains(wingName));
            }

            if (occurredDate.IsNotNullOrEmpty())
            {
                DateTime od;

                if (DateTime.TryParse(occurredDate, out od))
                {
                    q = q.FilterBy(x => x.DateOccurred == od);
                }

            }

            if (reportedDate.IsNotNullOrEmpty())
            {
                DateTime rd;

                if (DateTime.TryParse(reportedDate, out rd))
                {
                    q = q.FilterBy(x => x.DateReported == rd);
                }

            }

            if (complaintType.IsNotNullOrEmpty())
            {
                q = q.FilterBy(x => x.ComplaintType.Name.Contains(complaintType));
            }

            var results = q.FetchAll();

            if (patientName.IsNotNullOrEmpty())
            {
                results = results
                    .Where(x => (x.Patient != null && x.Patient.FullName.Contains(patientName)) || (x.Patient2 != null && x.Patient2.FullName.Contains(patientName)));
            }


            if (employeeName.IsNotNullOrEmpty())
            {
                results = results.Where(x => (x.Employee != null && x.Employee.FullName.Contains(employeeName)) || (x.Employee2 != null && x.Employee2.FullName.Contains(employeeName)));
            }

            var pager = new RedArrow.Framework.Persistence.PagedQueryResult<Complaint>();
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


        public IEnumerable<Complaint> FindForLineListing(Facility facility,
            int? wingId,
            DateTime? startDate,
            DateTime? endDate,
            int? type,
            int? employee)
        {
            var q = DataContext.CreateQuery<Complaint>()
               .FilterBy(x => x.Deleted == null || x.Deleted != true)
               .FilterBy(x => x.Facility.Id == facility.Id);

            if (wingId.HasValue)
            {
                q = q.FilterBy(x => x.Wing.Id == wingId.Value);
            }

            if (type.HasValue)
            {
                q = q.FilterBy(x => x.ComplaintType.Id == type.Value);
            }

            if (employee.HasValue)
            {
                q = q.FilterBy(x => x.Employee.Id == employee.Value);
            }

            if (startDate.HasValue)
            {
                q = q.FilterBy(x => x.DateOccurred >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                q = q.FilterBy(x => x.DateOccurred <= endDate.Value);
            }


            return q.FetchAll()
                .OrderByDescending(x => x.DateOccurred);
        }

        public IEnumerable<ComplaintType> AllTypes
        {
            get { 

                return DataContext.CreateQuery<ComplaintType>()
                    .FetchAll()
                    .OrderBy(x => x.SortOrder); 
            }
        }

    }
}
