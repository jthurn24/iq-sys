using System;
using System.Collections.Generic;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.ObjectModel.AuditTracking;
using RedArrow.Framework.Utilities;

namespace IQI.Intuition.Domain.Models
{
    public class Floor : AuditTrackingEntity<Floor>
    {

        public virtual Facility Facility { get; set; }

        public virtual Guid Guid { get; set; }

        public virtual string Name { get; set; }

        private IList<Wing> _Wings;
        public virtual IEnumerable<Wing> Wings
        {
            get
            {
                return _Wings;
            }
        }

        public virtual void AddWing(Wing wing)
        {
            if(_Wings == null)
            {
                _Wings = new List<Wing>();
            }

            if (wing.ThrowIfNullArgument("wing").Floor != this)
            {
                throw new ArgumentException(@"The wing being added does not belong to this floor");
            }

            if (_Wings.Any(existingWing => existingWing.Name == wing.Name))
            {
                throw new ArgumentException(@"A wing with the name '{0}' has already been added to this floor"
                    .FormatWith(wing.Name));
            }

            _Wings.Add(wing);
        }

        public virtual void RemoveWing(Wing wing)
        {
            _Wings.Remove(wing.ThrowIfNullArgument("wing"));
        }
    }
}
