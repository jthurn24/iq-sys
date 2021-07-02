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

namespace IQI.Intuition.Web.Models.Incident
{
    public class IncidentFormMap : ModelMap<IncidentForm, IncidentReport>
    {
        public IncidentFormMap(
                        IIncidentRepository incidentRepository,
                        IActionContext actionContext,
                        IModelMapper modelMapper)
        {
            IncidentRepository = incidentRepository.ThrowIfNullArgument("incidentRepository");
            ModelMapper = modelMapper.ThrowIfNullArgument("modelMapper");
            ActionContext = actionContext.ThrowIfNullArgument("actionContext");

            AutoFormatDisplayNames();

            ForProperty(model => model.IncidentReportId)
                .Read(domain => domain.Id)
                .Exclude(On.Create)
                .HiddenInput();


            ForProperty(model => model.Guid)
                .Read(domain => domain.Guid)
                .Exclude(On.Create)
                .HiddenInput();

            ForProperty(model => model.SelectedIncidentTypes)
                .Read(domain => domain.IncidentTypes.Select(item => item.Id))
                .Write((domain, value) => domain
                    .AssignTypes(ConvertIncidentTypeSelections(value)))
                .Default(Enumerable.Empty<int>())
                .HorizontalCheckBoxList(GenerateIncidentTypes());

            ForProperty(model => model.SelectedIncidentInjuries)
                .Read(domain => domain.IncidentInjuries.Select(item => item.Id))
                .Write((domain, value) => domain
                    .AssignInjuries(ConvertIncidentInjurySelections(value)))
                .Default(Enumerable.Empty<int>())
                .HorizontalCheckBoxList(GenerateIncidentInjuries());

            ForProperty(model => model.IncidentLocation)
                .Bind(x => x.IncidentLocation)
                    .OnRead(domain => domain.Id)
                    .OnWrite(SelectIncidentLocation)
                .DropDownList(GenerateIncidentLocations);

            ForProperty(model => model.IsUpdateMode)
                .Read(domain => true)
                .HiddenInput();

            ForProperty(model => model.LocationDetails)
                .Bind(x => x.LocationDetails)
                .DisplayName("Location Details")
                .SubText("Additional location information:")
                .MultilineText();

            ForProperty(model => model.InjuryLevel)
                .Bind(x => x.InjuryLevel)
                .DisplayName("Injury Severity")
                .EnumList();

            ForProperty(model => model.DiscoveredOn)
                .Read( domain => domain.DiscoveredOn)
                .Write(WriteDiscoveredOnDate)
                .DisplayName("Discovered On");

            ForProperty(model => model.DiscoveredHour)
                .Read(domain => domain.DiscoveredOn.HasValue ? domain.DiscoveredOn.Value.Hour : 0)
                .Write(WriteDiscoveredOnHour)
                .HourDropDown()
                .DisplayName("Time");

            ForProperty(model => model.DiscoveredMinutes)
                .Read(domain => domain.DiscoveredOn.HasValue ? domain.DiscoveredOn.Value.Minute : 0)
                .Write(WriteDiscoveredOnMinute)
                .MinuteDropDown()
                .DisplayName(string.Empty);

            ForProperty(model => model.OccurredOn)
                .Read(domain => domain.OccurredOn)
                .Write(WriteOccurredOnDate)
                .DisplayName("Occurred On");

            ForProperty(model => model.OccurredHour)
                .Read(domain => domain.OccurredOn.HasValue ? domain.OccurredOn.Value.Hour : 0)
                .Write(WriteOccurredOnHour)
                .HourDropDown()
                .DisplayName("Time");

            ForProperty(model => model.OccurredMinutes)
                .Read(domain => domain.OccurredOn.HasValue ? domain.OccurredOn.Value.Minute : 0)
                .Write(WriteOccurredOnMinute)
                .MinuteDropDown()
                .DisplayName(string.Empty);

            ForProperty(model => model.OcurredUnknown)
                .Read(domain => domain.OccurredOn.HasValue ? false : true)
                .Write(WriteOccurredOnUnknown)
                .DisplayName("Ocurred Date & Time Unknown");

            ForProperty(model => model.ResidentStatement)
                .Bind(domain => domain.ResidentStatement)
                .DisplayName("Statement")
                .SubText("Ask resident what they were trying to do (indicate if unable to provide a statement)")
                .MultilineText();


            ForProperty(model => model.InjuryAndTreatmentDescription)
                .Bind(domain => domain.InjuryAndTreatmentDescription)
                .DisplayName("Injuries")
                .SubText("Describe all injuries and site(s) with measurements and any treatment provided:")
                .MultilineText();

            ForProperty(model => model.Temperature)
                .Bind(domain => domain.Temperature);

            ForProperty(model => model.Pulse)
                .Bind(domain => domain.Pulse);

            ForProperty(model => model.Respiratory)
                .Bind(domain => domain.Respiratory);

            ForProperty(model => model.BloodPressureStanding)
                .Bind(domain => domain.BloodPressureStanding)
                .DisplayName("Blood Pressure")
                .SubText("Lying Standing");

            ForProperty(model => model.BloodPressureSitting)
                .Bind(domain => domain.BloodPressureSitting)
                .DisplayName("Blood Pressure")
                .SubText("Lying sitting");

            ForProperty(model => model.NeuroCheckCompleted)
                .Bind(domain => domain.NeuroCheckCompleted)
                .DisplayName("Neuro Check Completed?");

            ForProperty(model => model.AssessmentCompleted)
                .Bind(domain => domain.AssessmentCompleted)
                .DisplayName("Assessment Completed?");


            ForProperty(model => model.BloodGlucos)
                .Bind(domain => domain.BloodGlucos)
                .DisplayName("Blood Glucose")
                .SubText("(If diabetic)");


            ForProperty(x => x.FoundById)
                .Read(x => x.Employee != null ? x.Employee.Id : (int?)null)
                .Write(SelectFoundBy);

            ForProperty(x => x.FoundByName)
                .Read(x => x.Employee != null ? x.Employee.FullName : string.Empty);


            ForProperty(x => x.CNAId)
            .Read(x => x.Employee2 != null ? x.Employee2.Id : (int?)null)
            .Write(SelectCNA);

            ForProperty(x => x.CNAName)
                .Read(x => x.Employee2 != null ? x.Employee2.FullName : string.Empty);


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

            ForProperty(model => model.IncidentWitnesses)
                .Read(ReadWitnesses)
                .Write(WriteWitnesses)
                .Default(new List<IncidentWitness>());
         
        }


        private IActionContext ActionContext;

        private IIncidentRepository IncidentRepository { get; set; }

        private IModelMapper ModelMapper { get; set; }

        private IEnumerable<SelectListItem> GenerateIncidentTypes()
        {
            return IncidentRepository.AllTypes
                .ToSelectListItems(
                    item => item.Name,
                    item => item.Id);
        }

        private IncidentType[] ConvertIncidentTypeSelections(IEnumerable<int> selections)
        {
            return IncidentRepository.AllTypes
                .Where(item => selections
                    .EmptyIfNull()
                    .Contains(item.Id))
                .ToArray();
        }


        private IEnumerable<SelectListItem> GenerateIncidentInjuries()
        {
            return IncidentRepository.AllInjuries
                .ToSelectListItems(
                    item => item.Name,
                    item => item.Id);
        }

        private IncidentInjury[] ConvertIncidentInjurySelections(IEnumerable<int> selections)
        {
            return IncidentRepository.AllInjuries
                .Where(item => selections
                    .EmptyIfNull()
                    .Contains(item.Id))
                .ToArray();
        }

        private IncidentLocation SelectIncidentLocation(int? locationId)
        {
            return  IncidentRepository.AllLocations
                .Single(item => item.Id == locationId);
        }

        private IEnumerable<SelectListItem> GenerateIncidentLocations()
        {
            return IncidentRepository.AllLocations
                .ToSelectListItems(
                    item => item.Name,
                    item => item.Id);
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

        private IList<IncidentWitness> ReadWitnesses(IncidentReport domain)
        {
            var results = new List<IncidentWitness>();

            if (domain != null)
            {
                foreach (var i in domain.IncidentWitnesses)
                {
                    results.Add(this.ModelMapper.MapForReadOnly<IncidentWitness>(i));
                }
            }

            return results;
        }

        private void WriteWitnesses(IncidentReport domain, IEnumerable<IncidentWitness> results)
        {
            if (results == null)
            {
                return;
            }

            if (domain.IncidentWitnesses == null)
            {
                domain.IncidentWitnesses = new List<Domain.Models.IncidentWitness>();
            }

            foreach (var witness in results)
            {

                Domain.Models.IncidentWitness domainWitness = null;


                if (witness.IncidentWitnessId.HasValue)
                {
                    if (witness.Removed)
                    {

                        IncidentRepository.Delete(domain.IncidentWitnesses.Where(x => x.Id == witness.IncidentWitnessId.Value).FirstOrDefault());
                        domain.IncidentWitnesses.Remove(x => x.Id == witness.IncidentWitnessId.Value);
                    }
                    else
                    {
                        domainWitness = domain.IncidentWitnesses.Where(x => x.Id == witness.IncidentWitnessId.Value).FirstOrDefault();
                    }
                }
                else
                {
                    if (witness.Removed == false)
                    {
                        domainWitness = new Domain.Models.IncidentWitness();
                        domain.IncidentWitnesses.Add(domainWitness);
                        domainWitness.IncidentReport = domain;
                    }
                }

                if (domainWitness != null)
                {
                    domainWitness.Name = witness.Name.ToStringSafely();
                    domainWitness.Role = witness.Role.ToStringSafely();
                    domainWitness.Statement = witness.Statement.ToStringSafely();
                }
            }

        }

        private void WriteOccurredOnHour(IncidentReport domain, int hour)
        {
            if (domain.OccurredOn.HasValue == false)
            {
                domain.OccurredOn = DateTime.Today;
            }

            domain.OccurredOn = new DateTime(
                domain.OccurredOn.Value.Year,
                domain.OccurredOn.Value.Month,
                domain.OccurredOn.Value.Day,
                hour,
                domain.OccurredOn.Value.Minute,
                0);
        }

        private void WriteOccurredOnMinute(IncidentReport domain, int minutes)
        {
            if (domain.OccurredOn.HasValue == false)
            {
                domain.OccurredOn = DateTime.Today;
            }

            domain.OccurredOn = new DateTime(
                domain.OccurredOn.Value.Year,
                domain.OccurredOn.Value.Month,
                domain.OccurredOn.Value.Day,
                domain.OccurredOn.Value.Hour,
                minutes,
                0);
        }

        private void WriteOccurredOnDate(IncidentReport domain, DateTime? src)
        {
            if (src.HasValue == false)
            {
                return;
            }

            if (domain.OccurredOn.HasValue == false)
            {
                domain.OccurredOn = DateTime.Today;
            }

            domain.OccurredOn = new DateTime(
                src.Value.Year,
                src.Value.Month,
                src.Value.Day,
                domain.OccurredOn.Value.Hour,
                domain.OccurredOn.Value.Minute,
                0);
        }

        private void WriteOccurredOnUnknown(IncidentReport domain, bool src)
        {
            if (src == true)
            {
                domain.OccurredOn = null;
            }
        }

        private void WriteDiscoveredOnHour(IncidentReport domain, int hour)
        {
            if (domain.DiscoveredOn.HasValue == false)
            {
                domain.DiscoveredOn = DateTime.Today;
            }

            domain.DiscoveredOn = new DateTime(
                domain.DiscoveredOn.Value.Year,
                domain.DiscoveredOn.Value.Month,
                domain.DiscoveredOn.Value.Day,
                hour,
                domain.DiscoveredOn.Value.Minute,
                0);
        }

        private void WriteDiscoveredOnMinute(IncidentReport domain, int minutes)
        {
            if (domain.DiscoveredOn.HasValue == false)
            {
                domain.DiscoveredOn = DateTime.Today;
            }

            domain.DiscoveredOn = new DateTime(
                domain.DiscoveredOn.Value.Year,
                domain.DiscoveredOn.Value.Month,
                domain.DiscoveredOn.Value.Day,
                domain.DiscoveredOn.Value.Hour,
                minutes,
                0);
        }

        private void WriteDiscoveredOnDate(IncidentReport domain, DateTime? src)
        {
            if (src.HasValue == false)
            {
                return;
            }

            if (domain.DiscoveredOn.HasValue == false )
            {
                domain.DiscoveredOn = DateTime.Today;
            }

            domain.DiscoveredOn = new DateTime(
                src.Value.Year,
                src.Value.Month,
                src.Value.Day,
                domain.DiscoveredOn.Value.Hour,
                domain.DiscoveredOn.Value.Minute,
                0);
        }


        private void SelectFoundBy(Domain.Models.IncidentReport report, int? employeeId)
        {
            if (!employeeId.HasValue)
            {
                report.Employee = null;
                return;
            }

            report.Employee = ActionContext.CurrentFacility.Employees
                .Where(x => x.Id == employeeId.Value)
                .FirstOrDefault();
        }

        private void SelectCNA(Domain.Models.IncidentReport report, int? employeeId)
        {
            if (!employeeId.HasValue)
            {
                report.Employee2 = null;
                return;
            }

            report.Employee2 = ActionContext.CurrentFacility.Employees
                .Where(x => x.Id == employeeId.Value)
                .FirstOrDefault();
        }
    }
}
