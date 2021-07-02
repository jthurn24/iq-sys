using System;
using System.Collections.Generic;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Mvc.Extensions;
using RedArrow.Framework.Mvc.ModelMapper.Mapping;
using RedArrow.Framework.Mvc.ModelMapper;
using IQI.Intuition.Domain.Models;
using IQI.Intuition.Domain.Repositories;
using System.Linq;
using System.Web.Mvc;
using IQI.Intuition.Web.Models.Extensions;
using IQI.Intuition.Web.Models.PsychotropicDosageChange;
using IQI.Intuition.Domain.Services.Psychotropic;

namespace IQI.Intuition.Web.Models.PsychotropicAdministration
{
    public class PsychotropicAdministrationFormAddMap : ModelMap<PsychotropicAdministrationFormAdd, Domain.Models.PsychotropicAdministration>
    {
        protected IPsychotropicRespository PsychotropicRespository { get; set; }

        public PsychotropicAdministrationFormAddMap(
            IPsychotropicRespository psychotropicRespository)
        {

            PsychotropicRespository = psychotropicRespository;

            ForProperty(model => model.DrugType)
                .Bind(domain => domain.DrugType)
                    .OnRead(domain => domain.Id)
                    .OnWrite(SelectDrugType)
                .DisplayName("Drug Type")
                .DropDownList(GenerateDrugTypes)
                .Required();

            ForProperty(model => model.Name)
                .Bind(domain => domain.Name)
                .DisplayName("Medication Name")
                .Required();

            ForProperty(model => model.SideEffects)
                .Bind(domain => domain.SideEffects)
                .DisplayName("Side Effects")
                .MultilineText();

            ForProperty(model => model.StartDate)
                .Required()
                .DisplayName("Start Date")
                .Default(DateTime.Today)
                .Required();

            ForProperty(model => model.DosageForm)
                .Bind(domain => domain.DosageForm)
                    .OnRead(domain => domain.Id)
                    .OnWrite(SelectDosageForm)
                .DisplayName("Dosage Form")
                .DropDownList(GenerateDosageForms)
                .Required();

            ForProperty(model => model.Frequency)
                .DisplayName("Frequency")
                .DropDownList(GenerateFrequencies)
                .Required();

            ForProperty(model => model.TargetBehavior)
                .Bind(domain => domain.TargetBehavior)
                .DisplayName("Target Behavior")
                .Required();

            ForProperty(model => model.Segments)
                .Write(WriteSegments)
                .Verify(VerifySegments)
                .ErrorMessage("The dosages specified are not valid");

        }

        private PsychotropicDrugType SelectDrugType(int? id)
        {
            if (!id.HasValue)
            {
                return null;
            }

            return PsychotropicRespository.AllDrugTypes
                .Where(x => x.Id == id)
                .FirstOrDefault();
        }

        private IEnumerable<SelectListItem> GenerateDrugTypes()
        {
            return PsychotropicRespository.AllDrugTypes
                .ToSelectListItems(
                    item => item.Name,
                    item => item.Id)
                .Prepend(new SelectListItem[] { new SelectListItem() { Text = "Select...", Value = string.Empty } });
        }

        private PsychotropicDosageForm SelectDosageForm(int? id)
        {
            if (!id.HasValue)
            {
                return null;
            }

            return PsychotropicRespository.AllDosageForms
                .Where(x => x.Id == id)
                .FirstOrDefault();
        }

        private IEnumerable<SelectListItem> GenerateDosageForms()
        {
            return PsychotropicRespository.AllDosageForms
                .ToSelectListItems(
                    item => item.Name,
                    item => item.Id)
                .Prepend(new SelectListItem[] { new SelectListItem() { Text = "Select...", Value = string.Empty } });
        }

        private PsychotropicFrequency SelectFrequency(int? id)
        {
            if (!id.HasValue)
            {
                return null;
            }

            return PsychotropicRespository.AllFrequencies
                .Where(x => x.Id == id)
                .FirstOrDefault();
        }

        private IEnumerable<SelectListItem> GenerateFrequencies()
        {
            return PsychotropicRespository.AllFrequencies
                .ToSelectListItems(
                    item => item.Name,
                    item => item.Id)
                   .Prepend(new SelectListItem[] { new SelectListItem() { Text ="Select...", Value = string.Empty }});
        }

        private void WriteSegments(Domain.Models.PsychotropicAdministration admin, PsychotropicAdministrationFormAdd form, IEnumerable<DosageSegmentEntry> segments)
        {
            var change = new IQI.Intuition.Domain.Models.PsychotropicDosageChange();
            change.Administration = admin;
            change.Frequency = PsychotropicRespository.AllFrequencies.Where(x => x.Id == form.Frequency.Value).First();
            change.StartDate = form.StartDate;

            var s = segments.EmptyIfNull().Select(x => new DosageSegment()
            {
                Description = x.Description,
                ID = x.ID,
                Dosage = x.Dosage,
                Label = x.Label,
                DescriptionOptions = x.DescriptionOptions
            });

            change.DosageSegments = change.Frequency.GetFrequencyDefinition().WriteSegments(s);

            admin.DosageChanges = new List<Domain.Models.PsychotropicDosageChange>();
            admin.DosageChanges.Add(change);
        }

        private bool VerifySegments(IEnumerable<DosageSegmentEntry> segments)
        {
            foreach (var segment in segments.EmptyIfNull())
            {
                if (segment.Dosage.HasValue == false)
                {
                    return false;
                }

                if (segment.Description.IsNullOrEmpty())
                {
                    return false;
                }
            }

            return true;
        }
    }
}
