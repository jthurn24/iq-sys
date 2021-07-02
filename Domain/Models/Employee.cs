using System;
using System.Collections.Generic;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.ObjectModel.AuditTracking;
using RedArrow.Framework.Utilities;


namespace IQI.Intuition.Domain.Models
{
    public class Employee : AuditTrackingEntity<Employee>
    {
        public virtual string FirstName { get; set; }
        public virtual string MiddleName { get; set; }
        public virtual string LastName { get; set; }
        public virtual Facility Facility { get; set; }
        public virtual bool? IsActive { get; set; }


        public virtual string FullName
        {
            get
            {

                return "{0} , {2} {1}"
                    .FormatWith(LastName,
                    MiddleName.WhenNotNullOrWhiteSpace(value => " {0}.".FormatWith(value)),
                    FirstName).TrimEnd();
            }
        }
    }
}
