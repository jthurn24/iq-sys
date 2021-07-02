using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using IQI.Intuition.Domain.Models;

namespace IQI.Intuition.Infrastructure.Services.BusinessLogic.FacilityTimeLine.EventSource
{


    public interface IEventSource
    {
        IEnumerable<BaseEvent> GetEvents(SourceCriteria c);
        IEnumerable<BaseState> GetStates(SourceCriteria c);
    }
}
