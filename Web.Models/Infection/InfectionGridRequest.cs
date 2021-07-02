using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using SnyderIS.sCore.Web.Mvc.JQGridHelpers;

namespace IQI.Intuition.Web.Models.Infection
{
    [ModelBinder(typeof(AjaxRequestModelBinder<InfectionGridRequest>))]
    public class InfectionGridRequest : AjaxRequestModel<InfectionGridRequest>
    {
        public bool ShowOpenOnly { get; set; }

        // This is only used by the patient infection grid, it was added here so that  we do not have to implement
        //  two, nearly identical request models (and inheritence is problematic due to the generic model binder)
        public Guid? PatientGuid { get; set; }

        public string PatientFullName { get; set; }

        public string RoomAndWingName { get; set; }

        public string InfectionSiteTypeName { get; set; }

        public string FirstNotedOn { get; set; }

        public string ReasonForEntry { get; set; }

        public string XrayDate { get; set; }

        public string LabResultsText { get; set; }

        public string TreatementText { get; set; }


        public string ResolvedOn { get; set; }

        public Expression<Func<Domain.Models.InfectionVerification, object>> SortBy 
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

                if (RequestedSortBy(model => model.InfectionSiteTypeName))
                {
                    return infection => infection.InfectionSite.Type.Name;
                }

                if (RequestedSortBy(model => model.FirstNotedOn))
                {
                    return infection => infection.FirstNotedOn;
                }

                if (RequestedSortBy(model => model.XrayDate))
                {
                    return infection => infection.ChestXrayCompletedOn;
                }

                if (RequestedSortBy(model => model.LabResultsText))
                {
                    return infection => infection.LabResultsText;
                }

                if (RequestedSortBy(model => model.TreatementText))
                {
                    return infection => infection.TreatementText;
                }

                if (RequestedSortBy(model => model.ResolvedOn))
                {
                    return infection => infection.ResolvedOn;
                }

                return infection => infection.Id;
            }
        }
    }
}
