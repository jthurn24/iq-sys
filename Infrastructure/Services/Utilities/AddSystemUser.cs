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

namespace IQI.Intuition.Infrastructure.Services.Utilities
{
    public class AddSystemUser
    {
        private ILog _Log;
        private IStatelessDataContext _DataContext;

        public AddSystemUser(
            IStatelessDataContext dataContext,
            ILog log
            )
        {
            _DataContext = dataContext;
            _Log = log;
        }

        public void Run(string[] args)
        {
            var username = args[1];
            var password = args[2];

            var u = new SystemUser();
            u.Login = username;
            u.ChangePassword(password);
            u.Guid = Guid.NewGuid();
            u.IsActive = true;
            _DataContext.Insert(u);
        }
    }
}
