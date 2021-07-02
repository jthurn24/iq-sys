using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using SnyderIS.sCore.Web.Mvc.JQGridHelpers;

namespace IQI.Intuition.Web.Models.Vaccine
{
    [ModelBinder(typeof(AjaxRequestModelBinder<VaccineGridRequest>))]
    public class VaccineGridRequest : AjaxRequestModel<VaccineGridRequest>
    {
        public Guid? PatientGuid { get; set; }

        public string PatientFullName { get; set; }

        public string RoomAndWingName { get; set; }
        
        public string VaccineType { get; set; }

        public DateTime? AdministeredOn { get; set; }

        public string RefusalReason { get; set; }

        public Expression<Func<Domain.Models.VaccineEntry, object>> SortBy 
        {
            get
            {
                if (RequestedSortBy(model => model.PatientFullName))
                {
                    return vaccine => vaccine.Patient.GetLastName() + "-" + vaccine.Patient.GetFirstName();
                }

                if (RequestedSortBy(model => model.RoomAndWingName))
                {
                    return vaccine => vaccine.Patient.Room.Wing.Name + "-" + vaccine.Patient.Room.Name;
                }

                if (RequestedSortBy(model => model.VaccineType))
                {
                    return vaccine => vaccine.VaccineType == null ? string.Empty : vaccine.VaccineType.CVXCode + "-" + vaccine.VaccineType.FullVaccineName;
                }

                if (RequestedSortBy(model => model.AdministeredOn))
                {
                    return vaccine => vaccine.AdministeredOn;
                }

                if (RequestedSortBy(model => model.RefusalReason))
                {
                    return vaccine => vaccine.VaccineRefusalReason == null ? string.Empty : vaccine.VaccineRefusalReason.Description;
                }

                return vaccine => vaccine.Id;
            }
        }
    }
}
