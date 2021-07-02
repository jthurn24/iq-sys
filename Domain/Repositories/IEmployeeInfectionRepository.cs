using System;
using System.Linq.Expressions;
using RedArrow.Framework.Persistence;
using IQI.Intuition.Domain.Models;
using System.Collections.Generic;

namespace IQI.Intuition.Domain.Repositories
{
    public interface IEmployeeInfectionRepository
    {
        void Add(EmployeeInfection infection);
        void Remove(EmployeeInfection infection);
       
        IEnumerable<EmployeeInfection> FindForLineListing(Facility facility, 
            DateTime? startDate, 
            DateTime? endDate, 
            int? type);
        
        EmployeeInfection Get(int id);


        IPagedQueryResult<EmployeeInfection> Find(Facility facility,
            string employeeName,
            int? infectionType,
            int? department,
            string firstNotedOn,
            string wellOn,
            Expression<Func<EmployeeInfection, object>> sortByExpression,
            bool sortDescending, int page, int pageSize);

        IEnumerable<EmployeeInfection> FindCreatedIn(int month, int year);
    }
}
