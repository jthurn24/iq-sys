using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQI.Intuition.Domain.Models;
using RedArrow.Framework.Persistence;
using RedArrow.Framework.Logging;
using SnyderIS.sCore.Persistence;


namespace IQI.Intuition.Infrastructure.Services.Warnings
{
    public class CoreWarningDirector : IConsoleService
    {
        private ILog _Log;
        private IStatelessDataContext _DataContext;
        private static IList<WarningRule> _Rules = new List<WarningRule>();
        
        public CoreWarningDirector(
            ILog log,
            IStatelessDataContext dataContext
            )
        {
            _Log = log;
            _DataContext = dataContext;
        }

        public void Run(string[] args)
        {
            if(_Rules.Count < 1)
            {
                _Rules = _DataContext.CreateQuery<WarningRule>()
                    .FetchAll()
                    .Where(x => x.Facility != null)
                    .ToList();
            }

            var rule = _Rules.First();
            _Rules.Remove(rule);
            

            try
            {
                _Log.Error(string.Concat("Warning proc for ", rule.Id));
                var facility = _DataContext.Fetch<Facility>(rule.Facility.Id);
                var service = BaseService.Activate(rule);
                service.Run(rule, facility, _Log, _DataContext);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
            }
        }
    }
}
