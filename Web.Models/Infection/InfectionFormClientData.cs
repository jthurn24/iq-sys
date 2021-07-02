using System;
using System.Collections;
using System.Collections.Generic;

namespace IQI.Intuition.Web.Models.Infection
{
    public class InfectionFormClientData
    {
        public IEnumerable Floors { get; set; }

        public IEnumerable Wings { get; set; }

        public IEnumerable Rooms { get; set; }

        public IEnumerable InfectionTypes { get; set; }

        public IEnumerable InfectionSites { get; set; }


        public IEnumerable LabTestTypes { get; set; }

        public IEnumerable SatisfiedRuleClassifications { get; set; }

        public IEnumerable UnSatisfiedRuleClassifications { get; set; }

    }
}
