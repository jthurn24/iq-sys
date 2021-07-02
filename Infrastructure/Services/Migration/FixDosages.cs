using System.Linq;
using System.Text;
using IQI.Intuition.Domain.Models;
using RedArrow.Framework.Persistence;
using RedArrow.Framework.Persistence.NHibernate;
using RedArrow.Framework.Logging;
using SnyderIS.sCore.Persistence;
using System.Collections.Generic;
using RedArrow.Framework.Utilities;

namespace IQI.Intuition.Infrastructure.Services.Migration
{
    public class FixDosages
    {
        private IStatelessDataContext _DataContext;


        public FixDosages(IStatelessDataContext dataContext)
        {
            _DataContext = dataContext;
        }

        public void Run(string[] args)
        {
            var entries = _DataContext.CreateQuery<PsychotropicAdministration>()
                .FetchAll();

            foreach (var entry in entries)
            {
                entry.Guid = GuidHelper.NewGuid();
                _DataContext.Update(entry);

            }
        }
    }
}
