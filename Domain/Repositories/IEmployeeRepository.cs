using System;
using System.Linq.Expressions;
using RedArrow.Framework.Persistence;
using IQI.Intuition.Domain.Models;
using System.Collections.Generic;

namespace IQI.Intuition.Domain.Repositories
{
    public interface IEmployeeRepository
    {
        void Add(Employee employee);
        Employee Get(int id);

        IEnumerable<Employee> Find(Facility facility);
    }
}
