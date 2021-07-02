using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQI.Intuition.Reporting.Repositories;
using IQI.Intuition.Domain.Repositories;
using RedArrow.Framework.Persistence;
using RedArrow.Framework.Logging;
using SnyderIS.sCore.Persistence;


namespace IQI.Intuition.Infrastructure.Services.Utilities
{
    public class Cleanup
    {
        private ILog _Log;
        private IStatelessDataContext _DataContext;
        private IUserRepository _UserRepository;

        public Cleanup(
            IStatelessDataContext dataContext,
            ILog log,
            IUserRepository userRepository
            )
        {
            _DataContext = dataContext;
            _Log = log;
            _UserRepository = userRepository;
        }


        public void Run(string[] args)
        {
            CleanupExports();
        }

        public void CleanupExports()
        {
            var exports = _UserRepository.GetExportCreatedBefore(DateTime.Today.AddDays(-1));

            foreach(var e in exports)
            {
                System.Console.WriteLine("Purging export {0}",e.Id);
                _UserRepository.Delete(e);
            }

        }

    }
}
