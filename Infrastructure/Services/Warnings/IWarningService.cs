using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQI.Intuition.Reporting.Repositories;
using IQI.Intuition.Domain.Repositories;
using IQI.Intuition.Domain.Models;
using RedArrow.Framework.Persistence;
using RedArrow.Framework.Logging;
using SnyderIS.sCore.Persistence;
using RedArrow.Framework.Extensions.Common;

namespace IQI.Intuition.Infrastructure.Services.Warnings
{
    public interface IWarningService
    {
        void Run(WarningRule rule, Facility facility, ILog log, IStatelessDataContext dataContext);
        string DescribeRuleType(WarningRule rule, IDataContext dataContext);
        IDictionary<string,string> DescribeArguments(WarningRule rule, IDataContext dataContext);
    }
}
