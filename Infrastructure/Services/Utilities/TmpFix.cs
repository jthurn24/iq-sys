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
    public class TmpFix : IConsoleService
    {
        private ILog _Log;
        private IStatelessDataContext _DataContext;

        public TmpFix(
            IStatelessDataContext dataContext,
            ILog log
            )
        {
            _DataContext = dataContext;
            _Log = log;
        }

        public void Run(string[] args)
        {
            var psycc = _DataContext.CreateQuery<PsychotropicAdministration>()
                .FilterBy(x => x.Active == true)
                .FilterBy(x => x.Deleted == null || x.Deleted == false)
                .FetchAll();

            foreach(var p in psycc)
            {
                var patient = _DataContext.Fetch<Patient>(p.Patient.Id);

                if (patient.CurrentStatus == Domain.Enumerations.PatientStatus.Expired || patient.CurrentStatus == Domain.Enumerations.PatientStatus.Discharged)
                {
                    p.Active = false;
                    _DataContext.Update(p);

                    var lastStatus = _DataContext.CreateQuery<PatientStatusChange>()
                        .FilterBy(x => x.Patient.Id == patient.Id)
                        .SortBy(x => x.StatusChangedAt)
                        .Descending()
                        .FetchAll()
                        .First()
                        .StatusChangedAt;


                    var freq = new PsychotropicDosageChange();
                    freq.Frequency = _DataContext.Fetch<PsychotropicFrequency>(Domain.Constants.PSYCH_DISCONTINUED_FREQUENCY);
                    freq.Administration = p;
                    freq.StartDate = lastStatus;


                    _DataContext.Insert(freq);
                }
            }

        }
    }
}
