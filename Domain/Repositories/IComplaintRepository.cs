using System;
using System.Linq.Expressions;
using RedArrow.Framework.Persistence;
using IQI.Intuition.Domain.Models;
using System.Collections.Generic;

namespace IQI.Intuition.Domain.Repositories
{
    public interface IComplaintRepository
    {
        void Add(Complaint complaint);

        Complaint Get(int id);

        Complaint Get(Guid guid);

        IPagedQueryResult<Complaint> Find(
            Facility facility,
            string employeeName,
            string patientName,
            string occurredDate,
            string reportedDate,
            string wingName,
            string complaintType,
            Expression<Func<Complaint, object>> sortByExpression,
            bool sortDescending, int page, int pageSize);


        IEnumerable<Complaint> FindForFacility(Facility facility, IEnumerable<Guid> guids); 

        IEnumerable<Complaint> FindForLineListing(Facility facility, 
            int? wingId, 
            DateTime? startDate, 
            DateTime? endDate, 
            int? type,
            int? employee);

        IEnumerable<ComplaintType> AllTypes { get; }

    }
}
