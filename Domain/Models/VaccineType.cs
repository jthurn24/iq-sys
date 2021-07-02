using System;
using System.Collections.Generic;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.ObjectModel.AuditTracking;
using RedArrow.Framework.Utilities;

namespace IQI.Intuition.Domain.Models
{
    public class VaccineType : AuditTrackingEntity<VaccineType>
    {
        protected VaccineType() { }

        public virtual int CVXCode { get; set; }
        public virtual string CVXShortDescription { get; set; }
        public virtual string FullVaccineName { get; set; }
        public virtual int? InternalID { get; set; }
        public virtual bool? NonVaccine { get; set; }
    }
}
