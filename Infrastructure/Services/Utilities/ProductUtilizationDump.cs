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
    public class ProductUtilizationDump : IConsoleService
    {
        private ILog _Log;
        private IStatelessDataContext _DataContext;

        public ProductUtilizationDump(
            IStatelessDataContext dataContext,
            ILog log
            )
        {
            _DataContext = dataContext;
            _Log = log;
        }

        public void Run(string[] args)
        {
            var facilities = _DataContext.CreateQuery<Facility>()
                .FilterBy(x => x.InActive == null || x.InActive == false)
                .FilterBy(x => x.Account.InActive == null || x.Account.InActive == false)
                .FetchAll();

            var lbuilder = new StringBuilder();


            lbuilder.Append("Name");
            lbuilder.Append(",");
            lbuilder.Append("Infection");
            lbuilder.Append(",");
            lbuilder.Append("Incidents");
            lbuilder.Append(",");
            lbuilder.Append("Wounds");
            lbuilder.Append(",");
            lbuilder.Append("Psych");
            lbuilder.Append(",");
            lbuilder.Append("Catheters");
            lbuilder.Append(",");
            lbuilder.Append("Employee Infections");
            lbuilder.Append(",");
            lbuilder.Append("Complaints");
            lbuilder.Append(Environment.NewLine);

            foreach (var f in facilities)
            {

                var acc = _DataContext.Fetch<Account>(f.Account.Id);

                lbuilder.Append(string.Concat(acc.Name,"-",f.Name));
                lbuilder.Append(",");

                var infections = _DataContext.CreateQuery<InfectionVerification>()
                    .FilterBy(x => x.Room.Wing.Floor.Facility.Id == f.Id)
                    .FilterBy(x => x.CreatedAt.Value > new DateTime(2004, 1, 1))
                    .Count();

                lbuilder.Append(infections);
                lbuilder.Append(",");

                var incidents = _DataContext.CreateQuery<IncidentReport>()
                    .FilterBy(x => x.Room.Wing.Floor.Facility.Id == f.Id)
                    .FilterBy(x => x.CreatedAt.Value > new DateTime(2004, 1, 1))
                    .Count();

                lbuilder.Append(incidents);
                lbuilder.Append(",");

                var wounds = _DataContext.CreateQuery<WoundReport>()
                    .FilterBy(x => x.Room.Wing.Floor.Facility.Id == f.Id)
                    .FilterBy(x => x.CreatedAt.Value > new DateTime(2004, 1, 1))
                    .Count();

                lbuilder.Append(wounds);
                lbuilder.Append(",");

                var psych = _DataContext.CreateQuery<PsychotropicAdministration>()
                    .FilterBy(x => x.Patient.Room.Wing.Floor.Facility.Id == f.Id)
                    .FilterBy(x => x.CreatedAt.Value > new DateTime(2004, 1, 1))
                    .Count();

                lbuilder.Append(psych);
                lbuilder.Append(",");

                var cath = _DataContext.CreateQuery<CatheterEntry>()
                    .FilterBy(x => x.Room.Wing.Floor.Facility.Id == f.Id)
                    .FilterBy(x => x.CreatedAt.Value > new DateTime(2004, 1, 1))
                    .Count();

                lbuilder.Append(cath);
                lbuilder.Append(",");

                var eInfection = _DataContext.CreateQuery<EmployeeInfection>()
                    .FilterBy(x => x.Facility.Id == f.Id)
                    .FilterBy(x => x.CreatedAt.Value > new DateTime(2004, 1, 1))
                    .Count();

                lbuilder.Append(eInfection);
                lbuilder.Append(",");

                var complaints = _DataContext.CreateQuery<Complaint>()
                    .FilterBy(x => x.Facility.Id == f.Id)
                    .FilterBy(x => x.CreatedAt.Value > new DateTime(2004, 1, 1))
                    .Count();

                lbuilder.Append(complaints);
           

                lbuilder.Append(Environment.NewLine);
            }

            System.IO.File.WriteAllText("pud.txt", lbuilder.ToString());
        }
    }
}
