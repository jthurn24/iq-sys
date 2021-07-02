using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using SnyderIS.sCore.Web.Mvc.JQGridHelpers;

namespace IQI.Intuition.Web.Models.PsychotropicAdministration
{
    [ModelBinder(typeof(AjaxRequestModelBinder<PsychotropicAdministrationGridRequest>))]
    public class PsychotropicAdministrationGridRequest : AjaxRequestModel<PsychotropicAdministrationGridRequest>
    {
        public Guid? PatientGuid { get; set; }
        public string DrugTypeName { get; set; }
        public string Name { get; set; }
        public string CurrentDosage { get; set; }
        public string PreviousDosage { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string PatientFullName { get; set; }
        public string PatientRoomWingName { get; set; }

        public Expression<Func<Domain.Models.PsychotropicAdministration, object>> SortBy
        {
            get
            {
                if (RequestedSortBy(model => model.DrugTypeName))
                {
                    return i => i.DrugType.Name;
                }

                if (RequestedSortBy(model => model.Name))
                {
                    return i => i.Name;
                }

                if (RequestedSortBy(model => model.PatientFullName))
                {
                    return i => i.Patient.GetLastName() + "-" + i.Patient.GetFirstName();
                }

                if (RequestedSortBy(model => model.PatientRoomWingName))
                {
                    return i => i.Patient.Room.Wing.Name + "-" + i.Patient.Room.Name;
                }

                return i => i.Id;
            }
        }
    }
}
