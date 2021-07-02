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
    public class SecureData2 : IConsoleService
    {
        private ILog _Log;
        private IStatelessDataContext _DataContext;

        public SecureData2(
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

            var patients = _DataContext.CreateQuery<Patient>()
                .FilterBy(x => x.SecureAlgorithm == Domain.Constants.SECURE_ALGORITHM_DES)
                .FetchAll();

            foreach (var p in patients)
            {
                var firstName = p.GetFirstName();
                var lastName = p.GetLastName();
                var middle = p.GetMiddleInitial();

                p.SecureAlgorithm = Domain.Constants.SECURE_ALGORITHM_AES;
                p.SetFirstName(firstName);
                p.SetLastName(lastName);
                p.SetMiddleInitial(middle);
                _DataContext.Update(p);
            }


            var employees = _DataContext.CreateQuery<EmployeeInfection>()
                .FilterBy(x => x.SecureAlgorithm == Domain.Constants.SECURE_ALGORITHM_DES)
                .FetchAll();

            foreach (var e in employees)
            {
                var firstName = e.GetFirstName();
                var lastName = e.GetLastName();

                e.SecureAlgorithm = Domain.Constants.SECURE_ALGORITHM_AES;
                e.SetFirstName(firstName);
                e.SetLastName(lastName);

                _DataContext.Update(e);
            }

        }

        #endregion
    }
}
