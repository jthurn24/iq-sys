using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQI.Intuition.Domain.Models;

namespace IQI.Intuition.Infrastructure.Services.BusinessLogic.FacilityTimeLine.EventSource
{
    public abstract class BaseEvent
    {
        public DateTime On { get; set; }
        public abstract string GetDescription();
        public abstract string GetShortDescription();
        public abstract IList<string> GetTargetNames();
        public abstract IEnumerable<EventTag> GetEventTags();

        public class EventTag
        {
            public string Name { get; set; }
            public string Css { get; set; }
            public string GroupName { get; set; }
        }

        protected string ScrubForCss(string val)
        {
            return val.Replace(" ","")
                .Replace("(","")
                .Replace(")","")
                .Replace(",","");
        }
    }
}
