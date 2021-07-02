using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQI.Intuition.Infrastructure.Services.BusinessLogic.FacilityTimeLine.EventSource;
using RedArrow.Framework.Persistence;
using RedArrow.Framework.ObjectModel;
using IQI.Intuition.Domain.Models;


namespace IQI.Intuition.Infrastructure.Services.BusinessLogic.FacilityTimeLine
{
    public class FacilityTimeLineService
    {
        private IList<IEventSource> _Sources;

        public FacilityTimeLineService(IDataContext d)
        {
            _Sources = new List<IEventSource>();
            _Sources.Add(new EventSource.Patient.MovementSource(d));
            _Sources.Add(new EventSource.Infection.InfectionSource(d));
            _Sources.Add(new EventSource.Incident.IncidentSource(d));
            _Sources.Add(new EventSource.Wound.WoundSource(d));
            _Sources.Add(new EventSource.Precaution.PrecautionSource(d));
        }

        public IEnumerable<BaseEvent> GetEvents(SourceCriteria c)
        {
            var events = new List<BaseEvent>();

            foreach (var s in _Sources)
            {
                events.AddRange(s.GetEvents(c));
            }

            events = events.OrderByDescending(x => x.On).ToList();

            return events;
        }

        public IEnumerable<BaseState> GetState(SourceCriteria c)
        {
            var states = new List<BaseState>();

            foreach (var s in _Sources)
            {
                states.AddRange(s.GetStates(c));
            }

            states = states.OrderByDescending(x => x.StartDate).ToList();

            return states;
        }

    }
}
