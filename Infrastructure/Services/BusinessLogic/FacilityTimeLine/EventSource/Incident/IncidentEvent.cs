using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQI.Intuition.Domain.Models;
using RedArrow.Framework.Extensions.Formatting;
using RedArrow.Framework.Extensions.Common;

namespace IQI.Intuition.Infrastructure.Services.BusinessLogic.FacilityTimeLine.EventSource.Incident
{
    public class IncidentEvent : BaseEvent
    {

        public Domain.Models.Patient Patient { get; set; }
        public IList<Domain.Models.IncidentType> IncidentTypes { get; set;}
        public IList<Domain.Models.IncidentInjury> IncidentInjuries { get; set; }


        public override string GetDescription()
        {

            var descBuilder = new StringBuilder();


            descBuilder.Append("Incident discovered: ");

            foreach (var t in IncidentTypes)
            {
                descBuilder.Append(t.Name);
                descBuilder.Append(" ");
            }

            descBuilder.Append("Associated injuries: ");

            foreach (var i in IncidentInjuries)
            {
                descBuilder.Append(i.Name);
                descBuilder.Append(" ");
            }

            return descBuilder.ToString();
        }

        public override string GetShortDescription()
        {
            return string.Format("Incident: {0}", this.IncidentTypes.Select(x => x.Name).ToDelimitedString(','));
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

            foreach (var i in this.IncidentInjuries)
            {
                tags.Add(new EventTag()
                {
                    Css = string.Concat("event-incident-", ScrubForCss(i.Name)),
                    GroupName = "Incident Injuries",
                    Name = i.Name
                });
            }

            foreach (var i in this.IncidentTypes)
            {
                tags.Add(new EventTag()
                {
                    Css = string.Concat("event-incident-", ScrubForCss(i.Name)),
                    GroupName = "Incident Types",
                    Name = i.Name
                });
            }


            return tags;
        }
    }
}
