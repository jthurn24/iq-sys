using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQI.Intuition.Infrastructure.Services.BusinessLogic.FacilityTimeLine.EventSource;
using RedArrow.Framework.Persistence;
using RedArrow.Framework.ObjectModel;
using IQI.Intuition.Domain.Models;

namespace IQI.Intuition.Infrastructure.Services.BusinessLogic.FacilityTimeLine.EventSource.Infection
{
    public class InfectionSource : BaseEventSource<InfectionEvent, InfectionState>
    {
        private IDataContext _DataContext;

        public InfectionSource(IDataContext d)
        {
            _DataContext = d;
        }

        public override IEnumerable<InfectionEvent> LoadEvents(SourceCriteria c)
        {
            var events = new List<InfectionEvent>();

            var iQuery = _DataContext.CreateQuery<InfectionVerification>()
            .FilterBy(x => x.FirstNotedOn <= c.EndDate
                && (x.ResolvedOn == null ||c.StartDate <= x.ResolvedOn))
                .FilterBy(x => x.Deleted == null || x.Deleted == false);

            if (c.FacilityId.HasValue)
            {
                iQuery = iQuery.FilterBy(x => x.Room.Wing.Floor.Facility.Id == c.FacilityId.Value);
            }

            if (c.PatientId.HasValue)
            {
                iQuery = iQuery.FilterBy(x => x.Patient.Id == c.PatientId.Value);
            }

            foreach (var i in iQuery.FetchAll())
            {
                events.Add(new InfectionEvent()
                {
                     InfectionClassification = i.Classification,
                     InfectionSite = i.InfectionSite,
                     InfectionType = i.InfectionSite.Type,
                     On = i.FirstNotedOn.Value,
                     Patient = i.Patient,
                     EventType = InfectionEvent.InfectionEventType.New
                });

                if (i.ResolvedOn.HasValue)
                {
                    events.Add(new InfectionEvent()
                    {
                        InfectionClassification = i.Classification,
                        InfectionSite = i.InfectionSite,
                        InfectionType = i.InfectionSite.Type,
                        On = i.ResolvedOn.Value,
                        Patient = i.Patient,
                        EventType = InfectionEvent.InfectionEventType.Resolved
                    });
                }
            }

            return events.OrderByDescending(x => x.On);
        }


        public override IEnumerable<InfectionState> LoadStates(SourceCriteria c)
        {
            var states = new List<InfectionState>();

            states = states.Where(x => x.StartDate <= c.EndDate && c.StartDate <= x.EndDate)
                .ToList();

            return states.OrderByDescending(x => x.EndDate);
        }
    }
}
