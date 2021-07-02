using System;
using System.Collections.Generic;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.ObjectModel.AuditTracking;
using RedArrow.Framework.Utilities;

namespace IQI.Intuition.Domain.Models
{
    public class Wing : AuditTrackingEntity<Wing>
    {

        public virtual Guid Guid { get; set; }

        public virtual Floor Floor { get; set; }

        public virtual string Name { get; set; }

        private IList<Room> _Rooms;
        public virtual IEnumerable<Room> Rooms
        {
            get
            {
                return _Rooms;
            }
        }

        public virtual void AddRoom(Room room)
        {

            if(_Rooms == null)
            {
                _Rooms = new List<Room>();
            }

            if (room.ThrowIfNullArgument("room").Wing != this)
            {
                throw new ArgumentException(@"The room being added does not belong to this wing");
            }

            if (_Rooms.Any(existingRoom => existingRoom.Name == room.Name))
            {
                throw new ArgumentException(@"A room with the name '{0}' has already been added to this wing"
                    .FormatWith(room.Name));
            }
            
            _Rooms.Add(room);
        }

        public virtual void RemoveRoom(Room room)
        {
            _Rooms.Remove(room.ThrowIfNullArgument("room"));
        }
    }
}
