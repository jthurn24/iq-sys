using System;
using System.Linq;
using System.Collections.Generic;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Mvc.Extensions;
using RedArrow.Framework.Mvc.ModelMapper.Mapping;
using RedArrow.Framework.Mvc.ModelMapper;
using IQI.Intuition.Domain.Models;
using IQI.Intuition.Domain;
using IQI.Intuition.Domain.Repositories;
using IQI.Intuition.Infrastructure.Services;
using IQI.Intuition.Web.Models.Extensions;
using System.Web.Mvc;

namespace IQI.Intuition.Web.Models.Patient
{
    public class PatientFormMap : ModelMap<PatientForm, Domain.Models.Patient>
    {
        private IPatientRepository PatientRepository { get; set; }

        public PatientFormMap(IActionContext actionContext,
            IPatientRepository patientRepository)
        {

            actionContext.ThrowIfNullArgument("actionContext");
            PatientRepository = patientRepository;

            AutoFormatDisplayNames();

            ForProperty(model => model.Guid)
                .Read(domain => domain.Guid)
                .Exclude(On.Create)
                .HiddenInput();

            ForProperty(model => model.FirstName)
                .Read(x => x.GetFirstName())
                .Write((domain,value) => domain.SetFirstName(value))
                .Required()
                .Length(255);

            ForProperty(model => model.MiddleInitial)
                .Read(x => x.GetMiddleInitial())
                .Write((domain, value) => domain.SetMiddleInitial(value))
                .Length(1)
                .DisplayName("Middle");

            ForProperty(model => model.LastName)
                .Read(x => x.GetLastName())
                .Write((domain, value) => domain.SetLastName(value))
                .Required()
                .Length(255);

            ForProperty(model => model.MDName)
                .Bind(domain => domain.MDName)
                .DisplayName("Physician")
                .Length(1000);

            ForProperty(model => model.BirthDate)
                .Bind(domain => domain.BirthDate)
                    .OnRead(value => value == null ? string.Empty : value.Value.ToString("MM/dd/yyyy"))
                    .OnWrite( value => DateTime.Parse(value))
                .Required()
                .Verify(ValidateBirthDate)
                .DisplayName("Date of Birth");

            ForProperty(model => model.NewStatus)
                .Read(value => (PatientInfoStatus?)((int?)value.CurrentStatus))
                .EnumList()
                .DisplayName("Status");

            ForProperty(model => model.CurrentStatus)
                .Read(value => (PatientInfoStatus?)((int?)value.CurrentStatus));

            ForProperty(model => model.StatusChangedAt)
                .DisplayName("On");

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


            ForProperty(model => model.CurrentRoom)
                .Read(value => value.Room != null ? value.Room.Id : (int?)null);

            ForProperty(model => model.RoomChangeAt)
                .DisplayName("Moved On");

            ForProperty(model => model.IsUpdateMode)
                .Read(domain => true) // The "read" delegate is only executed in update mode, so it works for setting this value
                .HiddenInput(); // Must store the value in a hidden field in case validation fails and the form is rendered again

            ForProperty(model => model.ClientData)
                .Map(domain => domain) // The SelectOptions model does not map directly to a  property of the domain model
                .ReadOnly();

            ForProperty(model => model.SelectedPatientFlags)
                .Read(domain => domain.PatientFlags.Select(item => item.Id))
                .Write((domain, value) => domain
                    .AssignFlags(ConvertFlagTypeSelections(value)))
                .Default(Enumerable.Empty<int>())
                .HorizontalCheckBoxList(GenerateIncidentInjuries());

        }

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

        private bool ValidateBirthDate(string value)
        {
            DateTime t;
            var r = DateTime.TryParse(value, out t);

            if (!r)
            {
                return false;
            }

            if (t > DateTime.Today)
            {
                return false;
            }

            if (t < new DateTime(1850, 1, 1))
            {
                return false;
            }

            return true;
        }

        private PatientFlagType[] ConvertFlagTypeSelections(IEnumerable<int> selections)
        {
            return PatientRepository
                .AllPatientFlags
                .Where(item => selections
                    .EmptyIfNull()
                    .Contains(item.Id))
                .ToArray();
        }

        private IEnumerable<SelectListItem> GenerateIncidentInjuries()
        {
            return PatientRepository.AllPatientFlags
                .ToSelectListItems(
                    item => item.Name,
                    item => item.Id);
        }
    }
}
