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

namespace IQI.Intuition.Web.Models.PsychotropicAdministration
{
    public class PsychotropicAdministrationFormEditMap : ModelMap<PsychotropicAdministrationFormEdit, Domain.Models.PsychotropicAdministration>
    {
        protected IPsychotropicRespository PsychotropicRespository { get; set; }

        public PsychotropicAdministrationFormEditMap(
            IPsychotropicRespository psychotropicRespository)
        {

            PsychotropicRespository = psychotropicRespository;

            ForProperty(model => model.DrugType)
                .Bind(domain => domain.DrugType)
                    .OnRead(domain => domain.Id)
                    .OnWrite(SelectDrugType)
                .DisplayName("Drug Type")
                .DropDownList(GenerateDrugTypes);

            ForProperty(model => model.Name)
                .Bind(domain => domain.Name)
                .DisplayName("Medication Name");

            ForProperty(model => model.SideEffects)
                .Bind(domain => domain.SideEffects)
                .DisplayName("Side Effects")
                .MultilineText();

            ForProperty(model => model.DosageForm)
                .Bind(domain => domain.DosageForm)
                    .OnRead(domain => domain.Id)
                    .OnWrite(SelectDosageForm)
                .DisplayName("Dosage Form")
                .DropDownList(GenerateDosageForms);

            ForProperty(model => model.TargetBehavior)
                .Bind(domain => domain.TargetBehavior)
                .DisplayName("Target Behavior")
                .Required();

            ForProperty(model => model.Id)
            .Bind(domain => domain.Id);
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
                    item => item.Id);
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
                    item => item.Id);
        }
    }
}
