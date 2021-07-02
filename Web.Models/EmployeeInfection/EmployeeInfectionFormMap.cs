using System;
using System.Linq;
using System.Collections.Generic;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Mvc.Extensions;
using RedArrow.Framework.Mvc.ModelMapper.Mapping;
using IQI.Intuition.Domain.Models;
using IQI.Intuition.Domain;
using IQI.Intuition.Infrastructure.Services;
using IQI.Intuition.Web.Models.Extensions;
using System.Web.Mvc;
using IQI.Intuition.Domain.Repositories;
using RedArrow.Framework.Mvc.ModelMapper;

namespace IQI.Intuition.Web.Models.EmployeeInfection
{
    public class EmployeeInfectionFormMap : ModelMap<EmployeeInfectionForm, Domain.Models.EmployeeInfection>
    {
        public EmployeeInfectionFormMap(
            IActionContext actionContext,
            IInfectionRepository infectionRepository)
        {
            InfectionRepository = infectionRepository.ThrowIfNullArgument("infectionRepository");

            AutoFormatDisplayNames();

            ForProperty(model => model.Id)
                .Read(domain => domain.Id)
                .Exclude(On.Create);

            ForProperty(model => model.ClientData)
                .Map(domain => domain)
                .ReadOnly();

            ForProperty(model => model.InfectionSymptoms)
                .Read(domain => domain.InfectionSymptoms.Select(item => item.Id))
                .Write((domain, value) => domain
                    .AssignSymptoms(ConvertSymptomSelections(value)))
                .Default(Enumerable.Empty<int>())
                .HorizontalCheckBoxList(GenerateSymptomItems());

            ForProperty(model => model.WellOn)
                .Bind(x => x.WellOn);

            ForProperty(model => model.LastWorkedOn)
                .Bind(x => x.LastWorkedOn);

            ForProperty(model => model.ReturnToWorkOn)
                .Bind(x => x.ReturnToWorkOn);

            ForProperty(model => model.LabResults)
                .Bind(x => x.LabResults)
                .MultilineText();

            ForProperty(model => model.MDInstructions)
                .Bind(x => x.MDInstructions)
                .DisplayName("MD Instructions")
                .MultilineText();

            ForProperty(model => model.SeenByMD)
                .Bind(x => x.SeenByMD);


            ForProperty(model => model.InfectionType)
                .Bind(domain => domain.InfectionType)
                    .OnRead(domain => domain.Id)
                    .OnWrite(domain => InfectionRepository.AllInfectionTypes.Where(x => x.Id == domain).FirstOrDefault())
                .DropDownList(GenerateInfectionTypeItems);

            ForProperty(model => model.FirstSymptomOn)
                .Read(domain => domain.FirstSymptomOn)
                .Write((domain, value) => this.WriteDate(domain, x => domain.FirstSymptomOn, value))
                .DisplayName("First Symptom On");

            ForProperty(model => model.FirstSymptomHour)
                .Read(domain => domain.FirstSymptomOn.HasValue ? domain.FirstSymptomOn.Value.Hour : -1)
                .Write((domain, value) => this.WriteHour(domain, x => domain.FirstSymptomOn, value))
                .HourDropDown(true)
                .DisplayName("Time");

            ForProperty(model => model.FirstSymptomMinutes)
                .Read(domain => domain.FirstSymptomOn.HasValue ? domain.FirstSymptomOn.Value.Minute : -1)
                .Write((domain, value) => this.WriteMinute(domain, x => domain.FirstSymptomOn, value))
                .MinuteDropDown(true)
                .DisplayName(string.Empty);


            ForProperty(model => model.LastSymptomOn)
                .Read(domain => domain.LastSymptomOn)
                .Write((domain, value) => this.WriteDate(domain, x => domain.LastSymptomOn, value))
                .DisplayName("Last Symptom On");

            ForProperty(model => model.LastSymptomHour)
                .Read(domain => domain.LastSymptomOn.HasValue ? domain.LastSymptomOn.Value.Hour : -1)
                .Write((domain, value) => this.WriteHour(domain, x => domain.LastSymptomOn, value))
                .HourDropDown(true)
                .DisplayName("Time");

            ForProperty(model => model.LastSymptomMinutes)
                .Read(domain => domain.LastSymptomOn.HasValue ? domain.LastSymptomOn.Value.Minute : -1)
                .Write((domain, value) => this.WriteMinute(domain, x => domain.LastSymptomOn, value))
                .MinuteDropDown(true)
                .DisplayName(string.Empty);


            ForProperty(model => model.Notes)
                .Bind(x => x.Notes)
                .MultilineText();

            ForProperty(model => model.NotifiedOn)
                .Bind(x => x.NotifiedOn)
                .Required();

            ForProperty(model => model.FirstName)
                .Read(x => x.GetFirstName())
                .Write((domain, value) => domain.SetFirstName(value))
                .Required();

            ForProperty(model => model.LastName)
                .Read(x => x.GetLastName())
                .Write((domain, value) => domain.SetLastName(value))
                .Required();

            ForProperty(model => model.DateOfBirth)
                .Bind(x => x.DateOfBirth)
                .OnRead(ReadBirthDate)
                .OnWrite(ParseBirthDate);

            ForProperty(model => model.Gender)
                .Bind(x => x.Gender)
                .EnumList();

            ForProperty(model => model.LastShift)
                .Bind(x => x.LastShift)
                .EnumList();

            ForProperty(model => model.Department)
                .Bind(x => x.Department)
                .EnumList()
                .Required();

            ForProperty(model => model.Floor)
                .Read(domain => domain.Wing != null ? domain.Wing.Floor.Id : (int?)null)
                .DropDownList();

            ForProperty(model => model.Wing)
                .Bind(domain => domain.Wing)
                    .OnRead(wing => wing.Id)
                    .OnWrite(wingID => SelectFacilityWing(actionContext.CurrentFacility, wingID))
                .Required().When((x,v) => x.Floor.HasValue == true)
                .DropDownList();

        }

        private IInfectionRepository InfectionRepository { get; set; }

        private DateTime? ParseBirthDate(string value)
        {
            if (value.IsNullOrEmpty())
            {
                return null;
            }

            DateTime parsedVal;

            if(!DateTime.TryParse(value, out parsedVal))
            {
                throw new ModelMappingException(new List<ValidationError>()
                {
                    new ValidationError("DateOfBirth","Invalid DOB")
                });
            }

            return parsedVal;
        }

        private string ReadBirthDate(DateTime? value)
        {
            if (value.HasValue == false)
            {
                return string.Empty;
            }

            return value.Value.ToString("MM/dd/yyyy");
        }

        private Wing SelectFacilityWing(Facility facility, int? wingId)
        {
            if (!wingId.HasValue)
            {
                return null;
            }

            return facility.Floors
                .SelectMany(floor => floor.Wings)
                .SingleOrDefault(wing => wing.Id == wingId);
        }

        private IEnumerable<SelectListItem> GenerateSymptomItems()
        {
            return InfectionRepository.AllSymptoms
                .ToSelectListItems(
                    item => item.Name,
                    item => item.Id);
        }

        private InfectionSymptom[] ConvertSymptomSelections(IEnumerable<int> selections)
        {
            return InfectionRepository.AllSymptoms
                .Where(item => selections
                    .EmptyIfNull()
                    .Contains(item.Id))
                .ToArray();
        }


        private IEnumerable<SelectListItem> GenerateInfectionTypeItems()
        {
            return InfectionRepository.AllInfectionTypes.Where(x => x.IsHidden != true && x.UsedForEmployees == true)
                .ToSelectListItems(
                    item => item.Name,
                    item => item.Id)
                    .Prepend(
                    new SelectListItem() { Text = "Other (Non-Infection)", Value=string.Empty });
        }


    }
}
