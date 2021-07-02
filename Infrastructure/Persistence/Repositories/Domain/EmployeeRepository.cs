using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Persistence;
using IQI.Intuition.Domain.Models;
using IQI.Intuition.Domain.Repositories;
using IQI.Intuition.Domain;
using SnyderIS.sCore.Encryption;

namespace IQI.Intuition.Infrastructure.Persistence.Repositories.Domain
{
    public class EmployeeRepository : AbstractRepository<IDataContext>, IEmployeeRepository
    {
        public EmployeeRepository(IDataContext dataContext)
            : base(dataContext) { }

        public void Add(Employee employee)
        {
            DataContext.TrackChanges(employee);
        }

        public Employee Get(int id)
        {
            return DataContext.Fetch<Employee>(id);
        }

        public IEnumerable<Employee> Find(Facility facility)
        {
            return DataContext.CreateQuery<Employee>()
                .FilterBy(e => e.Facility == facility)
                .FetchAll();
        }

    }
}
