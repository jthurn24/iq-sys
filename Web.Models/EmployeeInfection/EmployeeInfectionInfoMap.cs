using System;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Extensions.Formatting;
using RedArrow.Framework.Mvc.ModelMapper.Mapping;
using IQI.Intuition.Domain.Models;

namespace IQI.Intuition.Web.Models.EmployeeInfection
{
    public class EmployeeInfectionInfoMap : ReadOnlyModelMap<EmployeeInfectionInfo, Domain.Models.EmployeeInfection>
    {
        public EmployeeInfectionInfoMap()
        {
            AutoConfigure();

            ForProperty(x => x.NotifiedOn)
                .Read(x => x.NotifiedOn.FormatAsMinimalDate());

            ForProperty(x => x.WellOn)
                .Read(x => x.WellOn.FormatAsMinimalDate());

            ForProperty(x => x.EmployeeName)
                .Read(x => x.FullName);

            ForProperty(x => x.Department)
                .Read(x => System.Enum.GetName(typeof(Domain.Enumerations.EmployeeDepartment), x.Department).SplitPascalCase());
        }
    }
}
