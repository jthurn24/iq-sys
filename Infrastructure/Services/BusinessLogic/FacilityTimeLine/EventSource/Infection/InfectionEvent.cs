using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQI.Intuition.Domain.Models;
using RedArrow.Framework.Extensions.Formatting;

namespace IQI.Intuition.Infrastructure.Services.BusinessLogic.FacilityTimeLine.EventSource.Infection
{
    public class InfectionEvent : BaseEvent
    {
        public enum InfectionEventType
        {
            New,
            Resolved
        }

        public Domain.Models.Patient Patient { get; set; }
        public InfectionEventType EventType { get; set; }
        public Domain.Models.InfectionType InfectionType { get; set;}
        public Domain.Models.InfectionClassification InfectionClassification { get; set; }
        public Domain.Models.InfectionSite InfectionSite { get; set; }


        public override string GetDescription()
        {
            if (EventType == InfectionEventType.New)
            {
                return string.Format("New {0}", GetInfectionDescription());
            }

            if (EventType == InfectionEventType.Resolved)
            {
                return string.Format("Resolved {0}", GetInfectionDescription());
            }


            return string.Empty;
        }

        public override string GetShortDescription()
        {
            if (EventType == InfectionEventType.New)
            {
                return string.Format("New {0}", InfectionType.Name);
            }

            if (EventType == InfectionEventType.Resolved)
            {
                return string.Format("Resolved {0}", InfectionType.Name);
            }

            return string.Empty;
        }


        private string GetInfectionDescription()
        {
            return string.Format("{0} - {1} Classification: {2}", 
                InfectionType.Name, 
                InfectionSite.Name, 
                System.Enum.GetName(typeof(InfectionClassification), 
                this.InfectionClassification).SplitPascalCase());

        }

        public override IList<string> GetTargetNames()
        {
            var targets = new List<string>();
            targets.Add(this.Patient.FullName);
            return targets;
        }

        public override IEnumerable<EventTag> GetEventTags()
        {
            var tags = new List<EventTag>();

            if (EventType ==  InfectionEventType.New)
            {
                tags.Add(new EventTag() { Css = "event-infection-new", GroupName = "Infections", Name = "New Infections" }); 
            }

            if (EventType == InfectionEventType.Resolved)
            {
                tags.Add(new EventTag() { Css = "event-infection-resolved", GroupName = "Infections", Name = "Resolved Infections"  }); 
            }

            tags.Add(new EventTag() {
                Css = string.Concat("event-infection-", ScrubForCss(this.InfectionType.ShortName)), 
                GroupName = "Infection Types",
                Name = this.InfectionType.ShortName
            });

            return tags;
        }
    }
}
