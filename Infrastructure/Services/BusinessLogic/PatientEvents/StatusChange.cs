using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQI.Intuition.Domain;
using IQI.Intuition.Domain.Models;
using IQI.Intuition.Domain.Repositories;

namespace IQI.Intuition.Infrastructure.Services.BusinessLogic.PatientEvents
{
    public class StatusChange
    {
        private IPsychotropicRespository _PsychotropicRespository;

        public StatusChange(IPsychotropicRespository psychotropicRespository)
        {
            _PsychotropicRespository = psychotropicRespository;
        }

        public void Handle(Patient src,
            Enumerations.PatientStatus newStatus,
            DateTime on)
        {
            /* If expiring or dischargin patient, close open infections */
            if ((Enumerations.PatientStatus)newStatus == Enumerations.PatientStatus.Discharged ||
                (Enumerations.PatientStatus)newStatus == Enumerations.PatientStatus.Expired)
            {
                foreach (var infection in src.InfectionVerifications.Where(x => x.IsResolved == false && x.Deleted != true))
                {
                    infection.IsResolved = true;
                    infection.ResolvedOn = on;
                }

                foreach (var catheter in src.Catheters.Where(x => x.DiscontinuedOn.HasValue == false && x.Deleted != true))
                {
                    catheter.DiscontinuedOn = on;
                }

                foreach (var wound in src.Wounds.Where(x => x.ResolvedOn.HasValue == false && x.Deleted != true))
                {
                    wound.ResolvedOn = on;
                    wound.IsResolved = true;
                }

                foreach (var psych in src.PsychotropicAdministrations.Where(x => x.Active == true && x.Deleted != true))
                {
                    var freq = new PsychotropicDosageChange();
                    freq.Frequency = _PsychotropicRespository.AllFrequencies.Where(x => x.Id == Constants.PSYCH_DISCONTINUED_FREQUENCY).First();
                    freq.Administration = psych;
                    freq.StartDate = on;
                    _PsychotropicRespository.Add(freq);

                    psych.Active = false;
                }
            }
        }
    }
}
