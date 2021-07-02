using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using SnyderIS.sCore.Web.Mvc.JQGridHelpers;

namespace IQI.Intuition.Web.Models.Catheter
{
    [ModelBinder(typeof(AjaxRequestModelBinder<CatheterGridRequest>))]
    public class CatheterGridRequest : AjaxRequestModel<CatheterGridRequest>
    {

        // This is only used by the patient Catheter grid, it was added here so that  we do not have to implement
        //  two, nearly identical request models (and inheritence is problematic due to the generic model binder)
        public Guid? PatientGuid { get; set; }

        public string PatientFullName { get; set; }

        public string RoomAndWingName { get; set; }

        public string Diagnosis { get; set; }

        public string StartedOn { get; set; }

        public string DiscontinuedOn { get; set; }


        public Expression<Func<Domain.Models.CatheterEntry, object>> SortBy 
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

                if (RequestedSortBy(model => model.Diagnosis))
                {
                    return i => i.Diagnosis;
                }


                if (RequestedSortBy(model => model.StartedOn))
                {
                    return i => i.StartedOn;
                }

                if (RequestedSortBy(model => model.DiscontinuedOn))
                {
                    return i => i.DiscontinuedOn;
                }

                return i => i.Id;
            }
        }
    }
}
