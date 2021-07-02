using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQI.Intuition.Infrastructure.Services.BusinessLogic.FacilityTimeLine.EventSource;
using RedArrow.Framework.Extensions.Common;

namespace IQI.Intuition.Web.Models.Timeline
{
    public class FacilityTimelineView
    {
        public IList<Entry> Entries { get; set; }
        public IList<TagGroup> TagGroups { get; set; }
        public bool PatientMode { get; set; }
        public Guid PatientGuid { get; set; }
        public string Title { get; set; }

        public void LoadData(IEnumerable<BaseEvent> events)
        {
            Entries = new List<Entry>();
            TagGroups = new List<TagGroup>();

            foreach (var e in events)
            {

                Entries.Add(new Entry()
                {
                    Description = e.GetDescription(),
                    On = e.On,
                    Title = !PatientMode ? e.GetTargetNames().ToDelimitedString(',') : e.GetShortDescription(),
                    CSSClass = e.GetEventTags().Select(x => x.Css).ToDelimitedString(' ')
                });

                foreach (var t in e.GetEventTags())
                {
                    var g = TagGroups.Where(x => x.Name == t.GroupName).FirstOrDefault();

                    if (g == null)
                    {
                        g = new TagGroup();
                        g.Options = new List<TagOption>();
                        g.Name = t.GroupName;
                        TagGroups.Add(g);
                    }

                    var o = g.Options.Where(x => x.Name == t.Name).FirstOrDefault();

                    if (o == null)
                    {
                       o = new TagOption() {
                             Name = t.Name,
                             Css = t.Css
                        };

                       g.Options.Add(o);
                    }

                    o.Count++;
                }
            }
        }

        public class Entry
        {
            public string Description { get; set; }
            public DateTime On { get; set; }
            public string Title { get; set; }
            public string CSSClass { get; set;}
        }

        public class TagGroup
        {
            public string Name { get; set;}
            public IList<TagOption> Options { get; set; }
        }

        public class TagOption
        {
            public string Name { get; set;}
            public string Css { get; set;}
            public int Count { get; set; }
        }
    }
}
