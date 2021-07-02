﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace IQI.Intuition.Web.Models.Precaution
{
    public class PatientPrecautionInfo
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public string PrecautionTypeName { get; set; }
        public int PrecautionTypeID { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; }
        public string AdditionalDescription { get; set; }
    }
}
