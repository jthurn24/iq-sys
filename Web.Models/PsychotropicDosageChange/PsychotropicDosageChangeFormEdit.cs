﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace IQI.Intuition.Web.Models.PsychotropicDosageChange
{
    public class PsychotropicDosageChangeFormEdit
    {
        public string Id { get; set; }
        public int? Frequency { get; set; }
        public DateTime? StartDate { get; set; }

        public IEnumerable<DosageSegmentEntry> Segments { get; set; }

        public DosageForm GetSegmentForm()
        {
            return new DosageForm()
            {
                Entries = this.Segments,
                Prefix = "Segments"
            };
        }
    }
}