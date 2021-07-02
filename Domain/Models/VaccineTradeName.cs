﻿using System;
using System.Collections.Generic;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.ObjectModel.AuditTracking;
using RedArrow.Framework.Utilities;

namespace IQI.Intuition.Domain.Models
{
    public class VaccineTradeName : AuditTrackingEntity<VaccineTradeName>
    {
        protected VaccineTradeName() { }

        public virtual VaccineType VaccineType { get; set; }
        public virtual string Name { get; set; }
    }
}