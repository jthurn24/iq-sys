using System;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Extensions.Formatting;
using RedArrow.Framework.Mvc.ModelMapper.Mapping;
using IQI.Intuition.Domain.Models;

namespace IQI.Intuition.Web.Models.PsychotropicAdministration
{
    public class PsychotropicAdministrationInfoMap : ReadOnlyModelMap<PsychotropicAdministrationInfo, Domain.Models.PsychotropicAdministration>
    {
        public PsychotropicAdministrationInfoMap()
        {
            ForProperty(model => model.PatientGuid)
                .Read(domain => domain.Patient.Guid.ToString());

            ForProperty(model => model.DrugTypeName)
                .Read(domain => domain.DrugType.Name);

            ForProperty(model => model.Id)
                .Read(domain => domain.Id.ToString());

            ForProperty(model => model.Name)
                .Read(domain => domain.Name);

            ForProperty(model => model.CurrentDosage)
                .Read(domain => CurrentDosage(domain));

            ForProperty(model => model.PreviousDosage)
                .Read(domain => PreviousDosage(domain));

            ForProperty(model => model.StartDate)
                .Read(domain => domain.GetStartDate().FormatAsShortDate());

            ForProperty(model => model.SideEffects)
                .Read(domain => domain.SideEffects);

            ForProperty(model => model.EndDate)
                .Read(domain => domain.GetEndDate().FormatAsShortDate());

            ForProperty(model => model.Active)
                .Read(domain => GetActiveStatus(domain));

            ForProperty(model => model.TargetBehavior)
                .Read(domain => domain.TargetBehavior);

            ForProperty(model => model.PatientFullName)
                .Read(domain => domain.Patient.FullName);

            ForProperty(model => model.PatientRoomWingName)
                .Read(x => String.Concat(x.Patient.Room.Name, " ", x.Patient.Room.Wing.Name));
        }

        private string CurrentDosage(Domain.Models.PsychotropicAdministration domain)
        {
            if (domain.DosageChanges == null || domain.DosageChanges.Count() < 1)
            {
                return "N/A";
            }

            var dosage = domain.DosageChanges.OrderByDescending(x => x.StartDate).First();
            return string.Concat(dosage.GetTotalDosage(), " ", domain.DosageForm.Name, " (",dosage.Frequency.Name,")");
        }

        private string PreviousDosage(Domain.Models.PsychotropicAdministration domain)
        {
            if (domain.DosageChanges == null || domain.DosageChanges.Count() < 1)
            {
                return "N/A";
            }

            var dosage = domain.DosageChanges.OrderByDescending(x => x.StartDate).Skip(1).FirstOrDefault();

            if (dosage == null)
            {
                return "N/A";
            }

            return string.Concat(dosage.GetTotalDosage(), " ", domain.DosageForm.Name, " (", dosage.Frequency.Name, ")");
        }


        private string GetActiveStatus(Domain.Models.PsychotropicAdministration domain)
        {
            if (domain.Active.HasValue == false || domain.Active == true)
            {
                return "Yes";
            }

            return string.Concat("Ended: ", domain.GetEndDate().FormatAsShortDate());
        }

    }
}
