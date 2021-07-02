using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQI.Intuition.Infrastructure.Services.BusinessLogic.FacilityTimeLine.EventSource;
using RedArrow.Framework.Persistence;
using RedArrow.Framework.ObjectModel;
using IQI.Intuition.Domain.Models;

namespace IQI.Intuition.Infrastructure.Services.BusinessLogic.FacilityTimeLine.EventSource.Incident
{
    public class IncidentSource : BaseEventSource<IncidentEvent, IncidentState>
    {
        private IDataContext _DataContext;

        public IncidentSource(IDataContext d)
        {
            _DataContext = d;
        }

        public override IEnumerable<IncidentEvent> LoadEvents(SourceCriteria c)
        {
            var events = new List<IncidentEvent>();

            var iQuery = _DataContext.CreateQuery<IncidentReport>()
            .FilterBy(x => x.DiscoveredOn >= c.StartDate && x.DiscoveredOn <= c.EndDate)
            .FilterBy(x => x.Deleted == null || x.Deleted == false); ;

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
                events.Add(new IncidentEvent()
                {
                     IncidentTypes = i.IncidentTypes,
                     IncidentInjuries = i.IncidentInjuries,
                     On = i.DiscoveredOn.Value,
                     Patient = i.Patient
                });

            }

            return events.OrderByDescending(x => x.On);
        }




        public override IEnumerable<IncidentState> LoadStates(SourceCriteria c)
        {
            var states = new List<IncidentState>();

            states = states.Where(x => x.StartDate <= c.EndDate && c.StartDate <= x.EndDate)
                .ToList();

            return states.OrderByDescending(x => x.EndDate);
        }
    }
}
