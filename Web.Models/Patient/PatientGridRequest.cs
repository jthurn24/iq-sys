using System;
using System.Collections.Specialized;
using System.Linq.Expressions;
using System.Web.Mvc;
using RedArrow.Framework.Extensions.Common;
using SnyderIS.sCore.Web.Mvc.JQGridHelpers;

namespace IQI.Intuition.Web.Models.Patient
{
    [ModelBinder(typeof(AjaxRequestModelBinder<PatientGridRequest>))]
    public class PatientGridRequest : AjaxRequestModel<PatientGridRequest>
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string BirthDate { get; set; }

        public string RoomWingName { get; set; }

        public string RoomName { get; set; }

        public Domain.Enumerations.PatientStatus? Status { get; set; }

        public Expression<Func<Domain.Models.Patient, object>> SortBy 
        {
            get
            {
                if (RequestedSortBy(model => model.FirstName))
                {
                    return patient => patient.GetFirstName();
                }

                if (RequestedSortBy(model => model.LastName))
                {
                    return patient => patient.GetLastName();
                }

                if (RequestedSortBy(model => model.BirthDate))
                {
                    return patient => patient.BirthDate;
                }

                if (RequestedSortBy(model => model.RoomWingName))
                {
                    return patient => patient.Room.Wing.Name;
                }

                if (RequestedSortBy(model => model.RoomName))
                {
                    return patient => patient.Room.Name;
                }

                if (RequestedSortBy(model => model.Status))
                {
                    return patient => patient.CurrentStatus;
                }

                return patient => patient.GetLastName();
            }
        }
    }
}
