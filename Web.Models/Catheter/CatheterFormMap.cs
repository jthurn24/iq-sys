using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Extensions.Formatting;
using RedArrow.Framework.Mvc.Extensions;
using RedArrow.Framework.Mvc.ModelMapper.Mapping;
using RedArrow.Framework.Mvc.ModelMapper;
using IQI.Intuition.Domain;
using IQI.Intuition.Domain.Models;
using IQI.Intuition.Domain.Repositories;
using IQI.Intuition.Infrastructure.Services;
using IQI.Intuition.Web.Models.Extensions;


namespace IQI.Intuition.Web.Models.Catheter
{
    public class CatheterFormMap : ModelMap<CatheterForm, CatheterEntry>
    {
        public CatheterFormMap(
                        ICatheterRepository CatheterRepository,
                        IActionContext actionContext,
                        IModelMapper modelMapper)
        {
            CatheterRepository = CatheterRepository.ThrowIfNullArgument("CatheterRepository");
            ModelMapper = modelMapper.ThrowIfNullArgument("modelMapper");

            AutoFormatDisplayNames();

            ForProperty(model => model.CatheterEntryId)
                .Read(domain => domain.Id)
                .Exclude(On.Create)
                .HiddenInput();


            ForProperty(model => model.Guid)
                .Read(domain => domain.Guid)
                .Exclude(On.Create)
                .HiddenInput();

            ForProperty(model => model.IsUpdateMode)
                .Read(domain => true)
                .HiddenInput();

            ForProperty(model => model.Type)
                .Bind(x => x.Type)
                    .OnRead(domain => domain)
                    .OnWrite(SelectType)
                .DropDownList(GenerateType)
                .Required();

            ForProperty(model => model.Material)
                .Bind(x => x.Material)
                    .OnRead(domain => domain)
                    .OnWrite(SelectMaterial)
                .DropDownList(GenerateMaterial)
                .Required();

            ForProperty(model => model.Reason)
                .Bind(x => x.Reason)
                    .OnRead(domain => domain)
                    .OnWrite(SelectReason)
                .DropDownList(GenerateReason)
                .Required();

            ForProperty(model => model.PatientEducated)
                .Bind(domain => domain.PatientEducated);

            ForProperty(model => model.FamilyEducated)
                .Bind(domain => domain.FamilyEducated);

            ForProperty(model => model.Diagnosis)
            .Bind(domain => domain.Diagnosis)
            .DisplayName("Diagnosis")
            .MultilineText();

            ForProperty(model => model.StartDate)
            .Bind(domain => domain.StartedOn)
            .DisplayName("Started On")
            .Required();

            ForProperty(model => model.DiscontinuedDate)
            .Bind(domain => domain.DiscontinuedOn)
            .DisplayName("Discontinued On");

            ForProperty(model => model.ClientData)
                .Map(domain => domain)
                .ReadOnly();

            ForProperty(model => model.Floor)
            .Read(domain => domain.Room.Wing.Floor.Id)
            .DropDownList();

            ForProperty(model => model.Wing)
                .Read(domain => domain.Room.Wing.Id)
                .DropDownList();

            ForProperty(model => model.Room)
                .Bind(domain => domain.Room)
                    .OnRead(room => room.Id)
                    .OnWrite(roomId => SelectFacilityRoom(actionContext.CurrentFacility, roomId))
                .Required()
                .DropDownList();

         
        }

        private ICatheterRepository CatheterRepository { get; set; }

        private IModelMapper ModelMapper { get; set; }


        private Room SelectFacilityRoom(Facility facility, int? roomId)
        {
            if (!roomId.HasValue)
            {
                return null;
            }

            return facility.Floors
                .SelectMany(floor => floor.Wings)
                .SelectMany(wing => wing.Rooms)
                .SingleOrDefault(room => room.Id == roomId);
        }

        private int? SelectType(int? type)
        {
            return type;
        }

        private IEnumerable<SelectListItem> GenerateType()
        {
            var temp = new List<SelectListItem>();
            temp.Add(new SelectListItem() { Text = "Select...", Value = string.Empty });
            foreach (Enumerations.CatheterType type in (Enumerations.CatheterType[])Enum.GetValues(typeof(Enumerations.CatheterType)))
            {
                temp.Add(new SelectListItem() { Text = type.ToString().SplitPascalCase(), Value = ((int)type).ToString() });
            }
            return (IEnumerable<SelectListItem>)temp;
        }

        private int? SelectMaterial(int? material)
        {
            return material;
        }

        private IEnumerable<SelectListItem> GenerateMaterial()
        {
            var temp = new List<SelectListItem>();
            temp.Add(new SelectListItem() { Text = "Select...", Value = string.Empty });
            foreach (Enumerations.CatheterMaterial type in (Enumerations.CatheterMaterial[])Enum.GetValues(typeof(Enumerations.CatheterMaterial)))
            {
                temp.Add(new SelectListItem() { Text = type.ToString().SplitPascalCase(), Value = ((int)type).ToString() });
            }
            return (IEnumerable<SelectListItem>)temp;
        }

        private int? SelectReason(int? reason)
        {
            return reason;
        }

        private IEnumerable<SelectListItem> GenerateReason()
        {
            var temp = new List<SelectListItem>();
            temp.Add(new SelectListItem() { Text = "Select...", Value = string.Empty });
            foreach (Enumerations.CatheterReason type in (Enumerations.CatheterReason[])Enum.GetValues(typeof(Enumerations.CatheterReason)))
            {
                temp.Add(new SelectListItem() { Text = type.ToString().SplitPascalCase(), Value = ((int)type).ToString() });
            }
            return (IEnumerable<SelectListItem>)temp;
        }

    }
}
