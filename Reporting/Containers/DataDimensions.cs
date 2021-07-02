using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dimensions = IQI.Intuition.Reporting.Models.Dimensions;
using Cubes = IQI.Intuition.Reporting.Models.Cubes;
using Facts = IQI.Intuition.Reporting.Models.Facts;

namespace IQI.Intuition.Reporting.Containers
{
    public class DataDimensions
    {
        public Dimensions.Facility Facility { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public IList<Dimensions.InfectionType> InfectionTypes { get; protected set; }
        public IList<Dimensions.InfectionSite> InfectionSites { get; protected set; }
        public IList<Dimensions.InfectionClassification> InfectionClassifications { get; protected set; }
        public IList<Dimensions.IncidentLocation> IncidentLocations { get; protected set; }
        public IList<Dimensions.IncidentType> IncidentTypes { get; protected set; }
        public IList<Dimensions.IncidentTypeGroup> IncidentTypeGroups { get; protected set; }
        public IList<Dimensions.IncidentInjury> IncidentInjuries { get; protected set; }
        public IList<Dimensions.IncidentInjuryLevel> IncidentInjuryLevels { get; protected set; }
        public IList<Dimensions.Floor> Floors { get; protected set; }
        public IList<Dimensions.Wing> Wings { get; protected set; }
        public IList<Dimensions.PsychotropicDrugType> PsychotropicDrugTypes { get; protected set; }
        public IList<Dimensions.WoundClassification> WoundClassifications { get; protected set; }
        public IList<Dimensions.WoundStage> WoundStages { get; protected set; }
        public IList<Dimensions.WoundSite> WoundSites { get; protected set; }
        public IList<Dimensions.ComplaintType> ComplaintTypes { get; protected set; }
        public IList<Dimensions.WoundType> WoundTypes { get; protected set; }
        public IList<Dimensions.CatheterType> CatheterTypes { get; protected set; }

        public DataDimensions()
        {
            this.InfectionTypes = new List<Dimensions.InfectionType>();
            this.InfectionSites = new List<Dimensions.InfectionSite>();
            this.InfectionClassifications = new List<Dimensions.InfectionClassification>();
            this.IncidentLocations = new List<Dimensions.IncidentLocation>();
            this.IncidentTypes = new List<Dimensions.IncidentType>();
            this.IncidentTypeGroups = new List<Dimensions.IncidentTypeGroup>();
            this.IncidentInjuries = new List<Dimensions.IncidentInjury>();
            this.IncidentInjuryLevels = new List<Dimensions.IncidentInjuryLevel>();
            this.Floors = new List<Dimensions.Floor>();
            this.Wings = new List<Dimensions.Wing>();
            this.PsychotropicDrugTypes = new List<Dimensions.PsychotropicDrugType>();
            this.WoundClassifications = new List<Dimensions.WoundClassification>();
            this.WoundStages = new List<Dimensions.WoundStage>();
            this.WoundSites = new List<Dimensions.WoundSite>();
            this.ComplaintTypes = new List<Dimensions.ComplaintType>();
            this.WoundTypes = new List<Dimensions.WoundType>();
            this.CatheterTypes = new List<Dimensions.CatheterType>();
        }

    }
}
