using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQI.Intuition.Domain.Models;
using RedArrow.Framework.Extensions.Formatting;


namespace IQI.Intuition.Infrastructure.Services.BusinessLogic.FacilityTimeLine.EventSource.Wound
{
    public class WoundEvent : BaseEvent
    {

        public enum WoundEventType
        {
            New,
            Healed,
            StageChange
        }

        public Domain.Models.Patient Patient { get; set; }
        public Domain.Models.WoundStage WoundStage { get; set; }
        public Domain.Models.WoundType WoundType { get; set; }
        public WoundEventType EventType { get; set; }
        public Domain.Models.WoundReport Report { get; set; }

        public override string GetDescription()
        {
            if (EventType == WoundEventType.Healed)
            {
                if (WoundStage != null && WoundStage.RatingValue > 0)
                {
                    return string.Format("{0} wound healed -  {1}",
                        WoundType.Name,
                        WoundStage.Name);
                }

                return string.Format("{0} wound healed", WoundType.Name);
            }

            if (EventType == WoundEventType.New)
            {

                if (WoundStage != null && WoundStage.RatingValue > 0)
                {
                     return string.Format("New {0} wound identified -  {1}", 
                         WoundType.Name,
                         WoundStage.Name);
                 }

                 return string.Format("New {0} wound identified", WoundType.Name);
            }

            if (EventType == WoundEventType.StageChange)
            {
                if (WoundStage.RatingValue > 0)
                {
                    return string.Format("Change in {0} wound - {1}",
                        WoundType.Name,
                        WoundStage.Name);
                }
            }

            return string.Empty;
        }

        public override string GetShortDescription()
        {
            if (EventType == WoundEventType.Healed)
            {
                return "Wound healed";
            }

            if (EventType == WoundEventType.New)
            {
                return "New Wound";
            }

            if (EventType == WoundEventType.StageChange)
            {
                return "Change in Stage";
            }

            return string.Empty;
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


            if (EventType == WoundEventType.Healed)
            {
                tags.Add(new EventTag() { Css = "event-wound-healed", GroupName = "Wounds", Name = "Healed Wound" });
            }

            if (EventType == WoundEventType.New)
            {
                tags.Add(new EventTag() { Css = "event-wound-new", GroupName = "Wounds", Name = "New Wound" });
            }

            if (EventType == WoundEventType.StageChange)
            {
                tags.Add(new EventTag() { Css = "event-wound-stage", GroupName = "Wounds", Name = "Stage Change" });
            }


            return tags;
        }


    }
}
