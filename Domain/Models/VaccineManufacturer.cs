using System;
using System.Collections.Generic;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.ObjectModel.AuditTracking;
using RedArrow.Framework.Utilities;

namespace IQI.Intuition.Domain.Models
{
    public class VaccineManufacturer : AuditTrackingEntity<VaccineManufacturer>
    {
        protected VaccineManufacturer() { }

        public virtual int ManufacturerId { get; set; }
        public virtual string MVXCode { get; set; }
        public virtual string ManufacturerName { get; set; }
        public virtual string Notes { get; set; }
        public virtual int? Status { get; set; } // Use VaccineStatus enum!
    }
}
