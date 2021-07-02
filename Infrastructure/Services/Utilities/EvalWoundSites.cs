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
using RedArrow.Framework.Extensions.Common;

namespace IQI.Intuition.Infrastructure.Services.Utilities
{
    public class EvalWoundSites: IConsoleService
    {
        private ILog _Log;
        private IStatelessDataContext _DataContext;

        public EvalWoundSites(
            IStatelessDataContext dataContext,
            ILog log
            )
        {
            _DataContext = dataContext;
            _Log = log;
        }

        public void Run(string[] args)
        {
            var data = _DataContext.CreateQuery<WoundReport>()
                .FetchAll();

            var sites = _DataContext.CreateQuery<WoundSite>()
                .FetchAll();

            foreach (var wound in data)
            {
                foreach (var site in sites)
                {
                    if (site.TopLeftX <= wound.LocationX && site.TopLeftY <= wound.LocationY && site.BottomRightX >= wound.LocationX && site.BottomRightY >= wound.LocationY)
                    {
                        wound.Site = site;
                        _DataContext.Update(wound);
                    }
                }
            }
        }

    }
}
