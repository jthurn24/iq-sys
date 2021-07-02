using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using SnyderIS.sCore.Web.Mvc.JQGridHelpers;

namespace IQI.Intuition.Web.Models.Incident
{
    [ModelBinder(typeof(AjaxRequestModelBinder<IncidentGridRequest>))]
    public class IncidentGridRequest : AjaxRequestModel<IncidentGridRequest>
    {

        // This is only used by the patient Incident grid, it was added here so that  we do not have to implement
        //  two, nearly identical request models (and inheritence is problematic due to the generic model binder)
        public Guid? PatientGuid { get; set; }

        public string PatientFullName { get; set; }

        public string RoomAndWingName { get; set; }

        public string IncidentTypes { get; set; }

        public string InjuryLevel { get; set; }

        public string DiscoveredOn { get; set; }

        public string OccurredOn { get; set; }


        public Expression<Func<Domain.Models.IncidentReport, object>> SortBy 
        {
            get
            {
                if (RequestedSortBy(model => model.PatientFullName))
                {
                    return infection => infection.Patient.GetLastName() + "-" + infection.Patient.GetFirstName();
                }

                if (RequestedSortBy(model => model.RoomAndWingName))
                {
                    return infection => infection.Patient.Room.Wing.Name + "-" + infection.Patient.Room.Name;
                }

                if (RequestedSortBy(model => model.InjuryLevel))
                {
                    return i => i.InjuryLevel;
                }


                if (RequestedSortBy(model => model.DiscoveredOn))
                {
                    return i => i.DiscoveredOn;
                }

                if (RequestedSortBy(model => model.OccurredOn))
                {
                    return i => i.OccurredOn;
                }

                return i => i.Id;
            }
        }
    }
}
