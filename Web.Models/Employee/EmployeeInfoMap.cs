using System;
using RedArrow.Framework.Mvc.ModelMapper.Mapping;

namespace IQI.Intuition.Web.Models.Employee
{
    public class EmployeeInfoMap : ReadOnlyModelMap<EmployeeInfo, Domain.Models.Employee>
    {
        public EmployeeInfoMap()
        {
            AutoConfigure();
        }
    }
}
