using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQI.Intuition.Infrastructure.Services.BusinessLogic.FacilityTimeLine.EventSource;
using RedArrow.Framework.Persistence;
using RedArrow.Framework.ObjectModel;
using IQI.Intuition.Domain.Models;

namespace IQI.Intuition.Infrastructure.Services.BusinessLogic.FacilityTimeLine.EventSource.Wound
{
    public class WoundSource : BaseEventSource<WoundEvent, WoundState>
    {
        private IDataContext _DataContext;

        public WoundSource(IDataContext d)
        {
            _DataContext = d;
        }


        public override IEnumerable<WoundEvent> LoadEvents(SourceCriteria c)
        {
            var events = new List<WoundEvent>();

            var wQuery = _DataContext.CreateQuery<WoundReport>()
              .FilterBy(x => x.Deleted == null || x.Deleted == false); ;

            if (c.FacilityId.HasValue)
            {
                wQuery = wQuery.FilterBy(x => x.Room.Wing.Floor.Facility.Id == c.FacilityId.Value);
            }

            if (c.PatientId.HasValue)
            {
                wQuery = wQuery.FilterBy(x => x.Patient.Id == c.PatientId.Value);
            }

            foreach (var r in wQuery.FetchAll())
            {
                if (r.FirstNotedOn >= c.StartDate && r.FirstNotedOn <= c.EndDate)
                {
                    var assessments = r.Assessments
                        .OrderBy(x => x.AssessmentDate);

                    var firstAssesment = assessments.FirstOrDefault();
                    var lastAssesment = assessments.LastOrDefault();

                    events.Add(new WoundEvent()
                    {
                          EventType = WoundEvent.WoundEventType.New,
                          Patient = r.Patient,
                          WoundType = r.WoundType,
                          On = r.FirstNotedOn.Value,
                          WoundStage = firstAssesment != null ? firstAssesment.Stage : null
                    });

                    if (r.ResolvedOn.HasValue)
                    {
                        if (r.ResolvedOn.Value >= c.StartDate && r.ResolvedOn.Value <= c.EndDate)
                        {
                            events.Add(new WoundEvent()
                            {
                                EventType = WoundEvent.WoundEventType.Healed,
                                Patient = r.Patient,
                                WoundType = r.WoundType,
                                On = r.ResolvedOn.Value,
                                WoundStage = lastAssesment != null ? lastAssesment.Stage : null
                            });
                        }
                    }

                    Domain.Models.WoundStage currentStage = null;

                    foreach (var a in assessments)
                    {
                        if (currentStage == null)
                        {
                            currentStage = a.Stage;
                        }
                        else
                        {
                            if (currentStage.Name != a.Stage.Name)
                            {
                                currentStage = a.Stage;

                                events.Add(new WoundEvent()
                                {
                                    EventType = WoundEvent.WoundEventType.StageChange,
                                    Patient = r.Patient,
                                    WoundType = r.WoundType,
                                    On = a.AssessmentDate.Value,
                                    WoundStage = a.Stage
                                });

                            }
                        }
                    }

                }
            }

            return events.OrderByDescending(x => x.On);
        }

        public override IEnumerable<WoundState> LoadStates(SourceCriteria c)
        {
            var states = new List<WoundState>();

            /* In order to load state we have to analyze the entire timeline.. can't be limited to 
             * just events within the time frame specified in criteria */
            var events = LoadEvents(new SourceCriteria() { PatientId = c.PatientId, FacilityId = c.FacilityId });

            if (events.Count() > 0)
            {
                foreach (var patient in events.Select(x => x.Patient).Distinct())
                {
                    var patientStates = new List<WoundState>();

                    var patientEvents = events.Where(x => x.Patient.Id == patient.Id).OrderBy(x => x.On);

                    WoundState lastState = null;

                    foreach (var pe in patientEvents)
                    {
                        if (lastState != null)
                        {
                            lastState.EndDate = pe.On;
                        }

                        var newState = new WoundState();
                        newState.StartDate = pe.On;
                        newState.Patient = pe.Patient;
                        newState.WoundStage = pe.WoundStage;
                        newState.WoundType = pe.WoundType;
                        newState.Report = pe.Report;

                        patientStates.Add(newState);
                        lastState = newState;
                    }

                    /* Tag last state with todays date because it also represents the current state */
                    lastState.EndDate = DateTime.Today;

                    states.AddRange(patientStates);

                }
            }

            states = states.Where(x => x.StartDate <= c.EndDate && c.StartDate <= x.EndDate)
                .ToList();

            return states.OrderByDescending(x => x.EndDate);
        }
    }
}
