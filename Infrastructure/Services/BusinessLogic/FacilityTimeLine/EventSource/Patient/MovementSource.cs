using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQI.Intuition.Infrastructure.Services.BusinessLogic.FacilityTimeLine.EventSource;
using RedArrow.Framework.Persistence;
using RedArrow.Framework.ObjectModel;
using IQI.Intuition.Domain.Models;

namespace IQI.Intuition.Infrastructure.Services.BusinessLogic.FacilityTimeLine.EventSource.Patient
{
    public class MovementSource : BaseEventSource<MovementEvent, MovementState>
    {
        private IDataContext _DataContext;

        public MovementSource(IDataContext d)
        {
            _DataContext = d;
        }
        
        public override IEnumerable<MovementEvent> LoadEvents(SourceCriteria c)
        {
            var events = new List<MovementEvent>();

            var rcQuery = _DataContext.CreateQuery<PatientRoomChange>()
                .FilterBy(x => x.RoomChangedAt >= c.StartDate 
                    && x.RoomChangedAt <= c.EndDate);

            var scQuery = _DataContext.CreateQuery<PatientStatusChange>()
                .FilterBy(x => x.StatusChangedAt >= c.StartDate && x.StatusChangedAt <= c.EndDate);

            if (c.FacilityId.HasValue)
            {
                rcQuery = rcQuery.FilterBy(x => x.Room.Wing.Floor.Facility.Id == c.FacilityId.Value);
                scQuery = scQuery.FilterBy(x => x.Patient.Room.Wing.Floor.Facility.Id == c.FacilityId.Value);
            }

            if (c.PatientId.HasValue)
            {
                rcQuery = rcQuery.FilterBy(x => x.Patient.Id == c.PatientId.Value);
                scQuery = scQuery.FilterBy(x => x.Patient.Id == c.PatientId.Value);
            }

            var roomChanges = rcQuery.FetchAll();
            var statusChanges = scQuery.FetchAll();

            foreach (var rc in roomChanges)
            {
                events.Add(new MovementEvent() { 
                    On = rc.RoomChangedAt,
                    Patient = rc.Patient,
                    CurrentRoom = rc.Room,
                    MovementType =  MovementEvent.PatientMovementType.RoomChange
                });
            }

            foreach (var sc in statusChanges)
            {

                var patient = _DataContext.Fetch<Domain.Models.Patient>(sc.Patient.Id);

                var lastRoomChange = roomChanges
                    .Where(x => x.Patient.Id == sc.Patient.Id
                    && x.RoomChangedAt.Date <= sc.StatusChangedAt.Date)
                    .OrderByDescending(x => x.RoomChangedAt)
                    .FirstOrDefault();

                Room lastRoom;

                if (lastRoomChange == null)
                {
                    lastRoom = patient.Room;
                }
                else
                {
                    lastRoom = lastRoomChange.Room;
                }

                if (sc.Status == Domain.Enumerations.PatientStatus.Admitted)
                {
                    events.Add(new MovementEvent()
                    {
                         CurrentRoom = lastRoom,
                         Patient = patient,
                         On = sc.StatusChangedAt,
                         MovementType = MovementEvent.PatientMovementType.Admission
                    });
                }
                else if (sc.Status == Domain.Enumerations.PatientStatus.Discharged)
                {
                    events.Add(new MovementEvent()
                    {
                        CurrentRoom = lastRoom,
                        Patient = patient,
                        On = sc.StatusChangedAt,
                        MovementType = MovementEvent.PatientMovementType.Discharge
                    });
                }
                else if (sc.Status == Domain.Enumerations.PatientStatus.Expired)
                {
                    events.Add(new MovementEvent()
                    {
                        CurrentRoom = lastRoom,
                        Patient = patient,
                        On = sc.StatusChangedAt,
                        MovementType = MovementEvent.PatientMovementType.Expiration
                    });
                }
            }

            return events.OrderByDescending(x => x.On);
        }

        


        public override IEnumerable<MovementState> LoadStates(SourceCriteria c)
        {
            var states = new List<MovementState>();

            /* In order to load state we have to analyze the entire timeline.. can't be limited to 
             * just events within the time frame specified in criteria */
            var events = LoadEvents(new SourceCriteria() 
            { PatientId = c.PatientId, FacilityId = c.FacilityId });

            if (events.Count() > 0)
            {
                foreach (var patient in events.Select(x => x.Patient).Distinct())
                {
                    var patientStates = new List<MovementState>();

                    var patientEvents = events.Where(x => x.Patient.Id == patient.Id).OrderBy(x => x.On);

                    MovementState lastState = null;

                    foreach (var pe in patientEvents)
                    {
                        if (lastState != null)
                        {
                            lastState.EndDate = pe.On;
                        }

                        var newState = new MovementState();
                        newState.StartDate = pe.On;
                        newState.Room = pe.CurrentRoom;
                        newState.Patient = pe.Patient;

                        if (pe.MovementType == MovementEvent.PatientMovementType.Admission)
                        {
                            newState.Status = Domain.Enumerations.PatientStatus.Admitted;
                        }
                        else if (pe.MovementType == MovementEvent.PatientMovementType.Discharge)
                        {
                            newState.Status = Domain.Enumerations.PatientStatus.Discharged;
                        }
                        else if (pe.MovementType == MovementEvent.PatientMovementType.Expiration)
                        {
                            newState.Status = Domain.Enumerations.PatientStatus.Expired;
                        }
                        else if (pe.MovementType == MovementEvent.PatientMovementType.RoomChange)
                        {
                            newState.Status = Domain.Enumerations.PatientStatus.Admitted;
                        }

                        patientStates.Add(newState);
                        lastState = newState;
                    }

                    /* Tag last state with todays date because it also represents the current state */
                    lastState.EndDate = DateTime.Today;

                    states.AddRange(patientStates);
                    
                }
            }

            states = states.Where(x => x.StartDate <= c.EndDate && c.StartDate <= x.EndDate)
                .ToList();

            return states.OrderByDescending(x => x.EndDate);
        }
    }
}
