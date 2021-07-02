using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQI.Intuition.Domain.Models;

namespace IQI.Intuition.Infrastructure.Services.BusinessLogic.FacilityTimeLine.EventSource.Patient
{
    public class MovementEvent : BaseEvent
    {
        public enum PatientMovementType
        {
            Admission,
            RoomChange,
            Discharge,
            Expiration
        }

        public Domain.Models.Patient Patient { get; set; }
        public Domain.Models.Room CurrentRoom { get; set; }
        public PatientMovementType MovementType { get; set; }

        public override string GetDescription()
        {
            if (MovementType == PatientMovementType.Admission)
            {
                return string.Format("Admitted to room {0}", CurrentRoom.Name);
            }

            if (MovementType == PatientMovementType.Discharge)
            {
                return string.Format("Discharged from room {0}", CurrentRoom.Name);
            }

            if (MovementType == PatientMovementType.RoomChange)
            {
                return string.Format("Moved to room {0}", CurrentRoom.Name);
            }

            if (MovementType == PatientMovementType.Expiration)
            {
                return string.Format("Expired in room {0}", CurrentRoom.Name);
            }

            return string.Empty;
        }

        public override string GetShortDescription()
        {
            if (MovementType == PatientMovementType.Admission)
            {
                return "Admitted";
            }

            if (MovementType == PatientMovementType.Discharge)
            {
                return "Discharged";
            }

            if (MovementType == PatientMovementType.RoomChange)
            {
                return "Moved";
            }

            if (MovementType == PatientMovementType.Expiration)
            {
                return "Expired";
            }

            return string.Empty;
        }

        public override IList<string> GetTargetNames()
        {
            var targets = new List<string>();
            targets.Add(this.Patient.FullName);
            return targets;
        }

        public override IEnumerable<EventTag> GetEventTags()
        {
            var tags = new List<EventTag>();

            if (MovementType == PatientMovementType.Admission)
            {
                tags.Add(new EventTag()
                {
                    Css = "adt-admission",
                    GroupName = "ADT",
                    Name = "Admission"
                });

            }

            if (MovementType == PatientMovementType.Discharge)
            {
                tags.Add(new EventTag()
                {
                    Css = "adt-discharge",
                    GroupName = "ADT",
                    Name = "Discharge"
                });
            }

            if (MovementType == PatientMovementType.RoomChange)
            {
                tags.Add(new EventTag()
                {
                    Css = "adt-transfer",
                    GroupName = "ADT",
                    Name = "Transfer"
                });
            }

            if (MovementType == PatientMovementType.Expiration)
            {
                tags.Add(new EventTag()
                {
                    Css = "adt-expiration",
                    GroupName = "ADT",
                    Name = "Expired"
                });
            }

            return tags;
        }
    }
}
