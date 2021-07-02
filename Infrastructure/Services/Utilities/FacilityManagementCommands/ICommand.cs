using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQI.Intuition.Reporting.Repositories;
using IQI.Intuition.Domain.Repositories;
using RedArrow.Framework.Persistence;
using RedArrow.Framework.Logging;
using IQI.Intuition.Domain.Models;
using SnyderIS.sCore.Persistence;
using System.Text.RegularExpressions;

namespace IQI.Intuition.Infrastructure.Services.Utilities.FacilityManagementCommands
{
    public interface ICommand
    {
        void Run(string[] args, 
            IStatelessDataContext dataContext,
            IUserRepository userRepository);
    }
}
