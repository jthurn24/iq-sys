﻿using System;
using System.Collections.Generic;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.ObjectModel.AuditTracking;
using RedArrow.Framework.Utilities;

namespace IQI.Intuition.Domain.Models
{
    public class VaccineAdministrativeSite : AuditTrackingEntity<VaccineAdministrativeSite>
    {
        protected VaccineAdministrativeSite() { }

        public virtual string Value { get; set; }
        public virtual string Description { get; set; }
    }
}
