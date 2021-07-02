using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQI.Intuition.Domain;
using IQI.Intuition.Domain.Models;
using IQI.Intuition.Domain.Repositories;

namespace IQI.Intuition.Infrastructure.Services.BusinessLogic.PatientEvents
{
    public class RemoveRecord
    {
        public void Handle(Patient src, Patient dest)
        {
            if (dest != null)
            {
                if (dest.Room.Wing.Floor.Facility != src.Room.Wing.Floor.Facility ||
                    dest.Id == src.Id)
                {
                    throw new Exception("Invalid destination patient");
                }

                foreach (var infection in src.InfectionVerifications)
                {
                    infection.Patient = dest;
                }

                foreach (var incident in src.IncidentReports)
                {
                    incident.Patient = dest;
                }

                foreach (var catheter in src.Catheters)
                {
                    catheter.Patient = dest;
                }

                foreach (var wound in src.Wounds)
                {
                    wound.Patient = dest;
                }

                foreach (var psych in src.PsychotropicAdministrations)
                {
                    psych.Patient = dest;
                }
            }
            else
            {
                foreach (var infection in src.InfectionVerifications)
                {
                    infection.Deleted = true;
                }

                foreach (var incident in src.IncidentReports)
                {
                    incident.Deleted = true;
                }

                foreach (var catheter in src.Catheters)
                {
                    catheter.Deleted = true;
                }

                foreach (var wound in src.Wounds)
                {
                    wound.Deleted = true;
                }

                foreach (var psych in src.PsychotropicAdministrations)
                {
                    psych.Deleted = true;
                }
            }
        }
    }
}
