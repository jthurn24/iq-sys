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

namespace IQI.Intuition.Infrastructure.Services.Utilities
{
    public class SecureData : IConsoleService
    {
        private ILog _Log;
        private IStatelessDataContext _DataContext;

        public SecureData(
            IStatelessDataContext dataContext,
            ILog log
            )
        {
            _DataContext = dataContext;
            _Log = log;
        }

        #region IConsoleService Members

        public void Run(string[] args)
        {

            var patients = _DataContext.CreateQuery<Patient>().FetchAll();

            foreach (var p in patients)
            {
                p.SetFirstName(p.GetFirstName());
                p.SetLastName(p.GetLastName());
                p.SetMiddleInitial(p.GetMiddleInitial());
                _DataContext.Update(p);
            }

        }

        #endregion
    }
}
