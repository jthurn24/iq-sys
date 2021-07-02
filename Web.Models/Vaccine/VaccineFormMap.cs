using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Mvc.Extensions;
using RedArrow.Framework.Mvc.ModelMapper.Mapping;
using RedArrow.Framework.Mvc.ModelMapper;

using IQI.Intuition.Domain.Models;
using IQI.Intuition.Domain.Repositories;
using IQI.Intuition.Infrastructure.Services;
using IQI.Intuition.Web.Models.Extensions;
using IQI.Intuition.Infrastructure.Services.Implementation;

namespace IQI.Intuition.Web.Models.Vaccine
{
    public class VaccineFormMap : ModelMap<VaccineForm, VaccineEntry>
    {
        public VaccineFormMap(
                        IVaccineRepository vaccineRepository,
                        IActionContext actionContext,
                        IModelMapper modelMapper)
        {
            VaccineRepository = vaccineRepository.ThrowIfNullArgument("vaccineRepository");
            ModelMapper = modelMapper.ThrowIfNullArgument("modelMapper");
            ActionContext = actionContext.ThrowIfNullArgument("actionContext");

            AutoFormatDisplayNames();

            ForProperty(model => model.VaccineEntryId)
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

            //ForProperty(x => x.EmployeeId)
            //    .Read(x => x.AdministeringProvider != null ? x.AdministeringProvider.Id : (int?)null)
            //    .Write(SelectEmployee)
            //    .Required();

            //ForProperty(x => x.EmployeeName)
            //    .Read(x => x.AdministeringProvider != null ? x.AdministeringProvider.FullName : string.Empty);

            ForProperty(model => model.VaccineRefusalReasonId)
                .Bind(x => x.VaccineRefusalReason)
                    .OnRead(domain => domain.Id)
                    .OnWrite(SelectVaccineRefusalReason)
                .DropDownList(GenerateVaccineRefusalReasons)
                .DisplayName("Refusal Reason");

            //ForProperty(model => model.UnknownAdmisteredAmount)
            //    .Bind(x => x.UnknownAdministeredAmount)
            //    .DisplayName("Unknown Amount");

            //ForProperty(model => model.AdministeredAmount)
            //    .Bind(x => x.AdministeredAmount)
            //    .Required().When((model, value) =>
            //        model.UnknownAdmisteredAmount.IsFalse() && (!model.VaccineRefusalReasonId.HasValue || model.VaccineRefusalReasonId.HasValue && model.VaccineRefusalReasonId.Value <= 0))
            //    .DisplayName("Amount");
                
            //ForProperty(model => model.VaccineUnitOfMeasureId)
            //    .Bind(x => x.VaccineUnitOfMeasure)
            //        .OnRead(domain => domain.Id)
            //        .OnWrite(SelectVaccineUnitOfMeasure)
            //    .Required().When((model, value) =>
            //        model.UnknownAdmisteredAmount.IsFalse() && (!model.VaccineRefusalReasonId.HasValue || model.VaccineRefusalReasonId.HasValue && model.VaccineRefusalReasonId.Value <= 0))
            //    .DropDownList(GenerateVaccineUnitOfMeasures)
            //    .DisplayName("Units");


            //ForProperty(model => model.SubstanceLotNumber)
            //    .Bind(x => x.SubstanceLotNumber)
            //    .DisplayName("Substance Lot Number");

            //ForProperty(model => model.SubstanceExpirationDate)
            //    .Bind(x => x.SubstanceExpirationDate)
            //    .DisplayName("Substance Expiration Date")
            //    .AtLeast(new DateTime(2000, 1, 1, 0, 0, 0));


            ForProperty(model => model.VaccineTypeId)
                .Bind(x => x.VaccineType)
                    .OnRead(domain => domain.Id)
                    .OnWrite(SelectVaccineType)
                .DropDownList(GenerateVaccineTypes)
                .DisplayName("Vaccine")
                .Required();

            //ForProperty(model => model.VaccineTradeNameId)
            //    .Bind(x => x.VaccineTradeName)
            //        .OnRead(domain => domain.Id)
            //        .OnWrite(SelectVaccineTradeName)
            //    .DropDownList(GenerateVaccineTradeNames)
            //    .DisplayName("Trade Name")
            //    .Required();

            //ForProperty(model => model.VaccineRouteId)
            //    .Bind(x => x.VaccineRoute)
            //        .OnRead(domain => domain.Id)
            //        .OnWrite(SelectVaccineRoute)
            //    .DropDownList(GenerateVaccineRoutes)
            //    .DisplayName("Route")
            //    .Required().When((model, value) =>
            //        model.UnknownAdmisteredAmount.IsFalse() && (!model.VaccineRefusalReasonId.HasValue || model.VaccineRefusalReasonId.HasValue && model.VaccineRefusalReasonId.Value <= 0));

            //ForProperty(model => model.VaccineAdministrativeSiteId)
            //    .Bind(x => x.VaccineAdministrativeSite)
            //        .OnRead(domain => domain.Id)
            //        .OnWrite(SelectVaccineAdministrativeSite)
            //    .DropDownList(GenerateVaccineAdministrativeSites)
            //    .DisplayName("Site")
            //    .Required().When((model, value) =>
            //        model.UnknownAdmisteredAmount.IsFalse() && (!model.VaccineRefusalReasonId.HasValue || model.VaccineRefusalReasonId.HasValue && model.VaccineRefusalReasonId.Value <= 0));

            //ForProperty(model => model.VaccineManufacturerId)
            //    .Bind(x => x.VaccineManufacturer)
            //        .OnRead(domain => domain.Id)
            //        .OnWrite(SelectVaccineManufacturer)
            //    .DropDownList(GenerateVaccineManufacturers)
            //    .DisplayName("Manufacturer")
            //    .Required();

            ForProperty(model => model.AdministeredOn)
                .Bind(domain => domain.AdministeredOn)
                .AtMostToday("Administered On")
                .AtLeast(new DateTime(2000,1,1,0,0,0))
                .DisplayName("On")
                .Required();

                //.Bind(domain => domain.AdministeredOn)
                //.RequiredWhen(model => model.IsResolved, "Resolved On")
                //.AtMostToday("Resolved On")
                //.DisplayName("On");
            //    .SubText("(If diabetic)");

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

        private IVaccineRepository VaccineRepository { get; set; }

        private IModelMapper ModelMapper { get; set; }

        private IActionContext ActionContext;
        
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

        private void SelectEmployee(Domain.Models.VaccineEntry vaccine, int? employeeId)
        {
            if (!employeeId.HasValue)
            {
                vaccine.AdministeringProvider = null;
                return;
            }

            vaccine.AdministeringProvider = ActionContext.CurrentFacility.Employees
                .Where(x => x.Id == employeeId.Value)
                .FirstOrDefault();
        }

        private VaccineType SelectVaccineType(int? vaccineTypeId)
        {
            return VaccineRepository.AllVaccineTypes
                .SingleOrDefault(item => item.Id == vaccineTypeId);
        }

        private IEnumerable<SelectListItem> GenerateVaccineTypes()
        {
            return VaccineRepository.AllVaccineTypes
                .OrderBy(x => x.CVXShortDescription)
                .ToSelectListItems(
                    item => item.CVXShortDescription,
                    item => item.Id)
                .Prepend(new SelectListItem[] { new SelectListItem() { Text = "Select...", Value = string.Empty } });
        }

        private VaccineTradeName SelectVaccineTradeName(int? vaccineTradeNameId)
        {
            return VaccineRepository.AllVaccineTradeNames
                .SingleOrDefault(item => item.Id == vaccineTradeNameId);
        }

        private IEnumerable<SelectListItem> GenerateVaccineTradeNames()
        {
            var temp = new List<SelectListItem>();
            temp.Add(new SelectListItem() { Text = "Select...", Value = string.Empty });
            return (IEnumerable<SelectListItem>)temp;
        }

        private VaccineUnitOfMeasure SelectVaccineUnitOfMeasure(int? vaccineUOMId)
        {
            return VaccineRepository.AllVaccineUnitOfMeasures
                .SingleOrDefault(item => item.Id == vaccineUOMId);
        }

        private IEnumerable<SelectListItem> GenerateVaccineUnitOfMeasures()
        {
            return VaccineRepository.AllVaccineUnitOfMeasures
                .ToSelectListItems(
                    item => item.Value,
                    item => item.Id)
                .Prepend(new SelectListItem[] { new SelectListItem() { Text = "Select...", Value = string.Empty } });
        }

        private VaccineRefusalReason SelectVaccineRefusalReason(int? vaccineRefusalId)
        {
            return VaccineRepository.AllVaccineRefusalReasons
                .SingleOrDefault(item => item.Id == vaccineRefusalId);
        }

        private IEnumerable<SelectListItem> GenerateVaccineRefusalReasons()
        {
            return VaccineRepository.AllVaccineRefusalReasons
                .ToSelectListItems(
                    item => item.CodeValue,
                    item => item.Id)
                    .Prepend(new SelectListItem[] { new SelectListItem() { Text = "Select...", Value = string.Empty } });
        }

        private VaccineRoute SelectVaccineRoute(int? vaccineRouteId)
        {
            return VaccineRepository.AllVaccineRoutes
                .SingleOrDefault(item => item.Id == vaccineRouteId);
        }

        private IEnumerable<SelectListItem> GenerateVaccineRoutes()
        {
            return VaccineRepository.AllVaccineRoutes
                .ToSelectListItems(
                    item => item.Value,
                    item => item.Id)
                .Prepend(new SelectListItem[] { new SelectListItem() { Text = "Select...", Value = string.Empty } });
        }

        private VaccineAdministrativeSite SelectVaccineAdministrativeSite(int? vaccineSiteId)
        {
            return VaccineRepository.AllVaccineAdministrativeSites
                .SingleOrDefault(item => item.Id == vaccineSiteId);
        }

        private IEnumerable<SelectListItem> GenerateVaccineAdministrativeSites()
        {
            return VaccineRepository.AllVaccineAdministrativeSites
                .ToSelectListItems(
                    item => item.Value,
                    item => item.Id)
               .Prepend(new SelectListItem[] { new SelectListItem() { Text = "Select...", Value = string.Empty } });
        }

        private VaccineManufacturer SelectVaccineManufacturer(int? vaccineManufacturerId)
        {
            return VaccineRepository.AllVaccineManufacturers
                .SingleOrDefault(item => item.Id == vaccineManufacturerId);
        }

        private IEnumerable<SelectListItem> GenerateVaccineManufacturers()
        {
            return VaccineRepository.AllVaccineManufacturers
                .ToSelectListItems(
                    item => item.ManufacturerName,
                    item => item.Id)
                .Prepend(new SelectListItem[] { new SelectListItem() { Text = "Select...", Value = string.Empty } });
        }

    }
}
