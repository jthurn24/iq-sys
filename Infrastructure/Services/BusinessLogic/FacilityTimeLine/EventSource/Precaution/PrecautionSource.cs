using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQI.Intuition.Infrastructure.Services.BusinessLogic.FacilityTimeLine.EventSource;
using RedArrow.Framework.Persistence;
using RedArrow.Framework.ObjectModel;
using IQI.Intuition.Domain.Models;

namespace IQI.Intuition.Infrastructure.Services.BusinessLogic.FacilityTimeLine.EventSource.Precaution
{
    public class PrecautionSource : BaseEventSource<PrecautionEvent, PrecautionState>
    {
        private IDataContext _DataContext;

        public PrecautionSource(IDataContext d)
        {
            _DataContext = d;
        }

        public override IEnumerable<PrecautionEvent> LoadEvents(SourceCriteria c)
        {
            var events = new List<PrecautionEvent>();

            var iQuery = _DataContext.CreateQuery<PatientPrecaution>()
            .FilterBy(x => x.StartDate <= c.EndDate
                && (x.EndDate == null ||c.StartDate <= x.EndDate))
                .FilterBy(x => x.Deleted == null || x.Deleted == false);

            if (c.FacilityId.HasValue)
            {
                iQuery = iQuery.FilterBy(x => x.Patient.Room.Wing.Floor.Facility.Id == c.FacilityId.Value);
            }

            if (c.PatientId.HasValue)
            {
                iQuery = iQuery.FilterBy(x => x.Patient.Id == c.PatientId.Value);
            }

            foreach (var i in iQuery.FetchAll())
            {
                events.Add(new PrecautionEvent()
                {
                     On = i.StartDate.Value,
                     Patient = i.Patient,
                     EventType = PrecautionEvent.PrecautionEventType.New,
                     PrecautionType = i.PrecautionType,
                     AdditionalDetails = i.AdditionalDescription
                });

                if (i.EndDate.HasValue)
                {
                    events.Add(new PrecautionEvent()
                    {
                        On = i.StartDate.Value,
                        Patient = i.Patient,
                        EventType = PrecautionEvent.PrecautionEventType.Ended,
                        PrecautionType = i.PrecautionType,
                        AdditionalDetails = i.AdditionalDescription
                    });
                }
            }

            return events.OrderByDescending(x => x.On);
        }


        public override IEnumerable<PrecautionState> LoadStates(SourceCriteria c)
        {
            var states = new List<PrecautionState>();

            states = states.Where(x => x.StartDate <= c.EndDate && c.StartDate <= x.EndDate)
                .ToList();

            return states.OrderByDescending(x => x.EndDate);
        }
    }
}
