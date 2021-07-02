using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IQI.Intuition.Infrastructure.Services.BusinessLogic.FacilityTimeLine.EventSource
{
    public abstract class BaseEventSource<T, TT> : IEventSource
        where T : BaseEvent
        where TT : BaseState 
    {
        public abstract IEnumerable<T> LoadEvents(SourceCriteria c);
        public abstract IEnumerable<TT> LoadStates(SourceCriteria c);

        IEnumerable<BaseEvent> IEventSource.GetEvents(SourceCriteria c)
        {
            return LoadEvents(c);
        }

        IEnumerable<BaseState> IEventSource.GetStates(SourceCriteria c)
        {
            return LoadStates(c);
        }
    }
}
