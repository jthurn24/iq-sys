using System;
using System.Linq;
using System.Text;
using IQI.Intuition.Domain.Models;
using RedArrow.Framework.Persistence;
using RedArrow.Framework.Persistence.NHibernate;
using RedArrow.Framework.Logging;
using SnyderIS.sCore.Persistence;
using System.Collections.Generic;
using RedArrow.Framework.Utilities;

namespace IQI.Intuition.Infrastructure.Services.Migration
{
    public class TrackRoomChanges
    {
        private IStatelessDataContext _DataContext;


        public TrackRoomChanges(IStatelessDataContext dataContext)
        {
            _DataContext = dataContext;
        }

        public void Run(string[] args)
        {
            var patients = _DataContext.CreateQuery<Patient>()
                .FetchAll();

            foreach (var p in patients)
            {
                processPatient(p);
            }
        }

        private void processPatient(Patient p)
        {
            var audits = _DataContext.CreateQuery<AuditEntry>()
                .FilterBy(x => x.TargetPatient == p.Guid)
                .FilterBy(x => x.AuditType == Domain.Enumerations.AuditEntryType.ModifiedPatientDemographic)
                .FetchAll();

            
            /* Attempt to identify original room */

            var infections = _DataContext.CreateQuery<InfectionVerification>()
                .FilterBy(x => x.Patient.Id == p.Id)
                .FetchAll();

            var incidents = _DataContext.CreateQuery<IncidentReport>()
                .FilterBy(x => x.Patient.Id == p.Id)
                .FetchAll();

            Room initialRoom = null;

            if (infections.Count() > 0)
            {
                initialRoom = infections.OrderBy(x => x.FirstNotedOn).First().Room;
            }

            if (initialRoom == null && incidents.Count() > 0)
            {
                initialRoom = incidents.OrderBy(x => x.DiscoveredOn).First().Room;
            }

            var statusChanges = _DataContext.CreateQuery<PatientStatusChange>()
                .FilterBy(x => x.Patient.Id == p.Id)
                .FetchAll();


            if (statusChanges.Count() < 1)
            {
                return;
            }

            var startDate = statusChanges.OrderBy(x => x.StatusChangedAt).First().StatusChangedAt;

            if (initialRoom != null)
            {
                addRoomChange(p, initialRoom, startDate);
            }


            var pRoom = _DataContext.Fetch<Room>(p.Room.Id);
            var pWing = _DataContext.Fetch<Wing>(pRoom.Wing.Id);
            var pFloor = _DataContext.Fetch<Floor>(pWing.Floor.Id);
            var pFacility = _DataContext.Fetch<Facility>(pFloor.Facility.Id);

            foreach (var a in audits)
            {
                var changes = Infrastructure.Services.Protection.ChangeData.Load(a.DetailsText);

                var roomData = changes.Fields.Where(x => x.Name == "Room").FirstOrDefault();

                if (roomData != null)
                {
                    string roomName = roomData.Change;

                    var room = _DataContext.CreateQuery<Room>()
                        .FilterBy(x => x.Wing.Floor.Facility.Id == pFacility.Id)
                        .FilterBy(x => x.Name == roomName)
                        .FetchAll()
                        .FirstOrDefault();

                    if (room != null)
                    {
                        addRoomChange(p, room, a.PerformedAt);
                    }
                }
            }
        }

        private void addRoomChange(Patient p, Room r, DateTime d)
        {
            var c = new PatientRoomChange();
            c.Patient = p;
            c.Room = r;
            c.RoomChangedAt = d;

            _DataContext.Insert(c);
        }
    }
}
