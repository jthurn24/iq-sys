using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQI.Intuition.Domain.Models;
using RedArrow.Framework.Extensions.Formatting;

namespace IQI.Intuition.Infrastructure.Services.BusinessLogic.FacilityTimeLine.EventSource.Precaution
{
    public class PrecautionEvent : BaseEvent
    {
        public enum PrecautionEventType
        {
            New,
            Ended
        }

        public Domain.Models.Patient Patient { get; set; }
        public PrecautionEventType EventType { get; set; }
        public Domain.Models.PrecautionType PrecautionType { get; set; }
        public string AdditionalDetails { get; set; }

        public override string GetDescription()
        {
            if (EventType == PrecautionEventType.New)
            {
                return string.Format("Begin {0}", GetPrecautionDescription());
            }

            if (EventType == PrecautionEventType.Ended)
            {
                return string.Format("Ended {0}", GetPrecautionDescription());
            }


            return string.Empty;
        }

        public override string GetShortDescription()
        {
            if (EventType == PrecautionEventType.New)
            {
                return string.Format("New {0}", PrecautionType.Name);
            }

            if (EventType == PrecautionEventType.Ended)
            {
                return string.Format("Ended {0}", PrecautionType.Name);
            }

            return string.Empty;
        }


        private string GetPrecautionDescription()
        {
            return string.Format("{0} - {1}", 
                PrecautionType.Name, AdditionalDetails);

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

            if (EventType == PrecautionEventType.New)
            {
                tags.Add(new EventTag() { Css = "event-precaution-new", GroupName = "Preventions", Name = "New Preventions" }); 
            }

            if (EventType == PrecautionEventType.Ended)
            {
                tags.Add(new EventTag() { Css = "event-precaution-resolved", GroupName = "Preventions", Name = "Ended Preventions" }); 
            }

            tags.Add(new EventTag() {
                Css = string.Concat("event-precaution-", ScrubForCss(this.PrecautionType.Name)), 
                GroupName = "Prevention Types",
                Name = this.PrecautionType.Name
            });

            return tags;
        }
    }
}
