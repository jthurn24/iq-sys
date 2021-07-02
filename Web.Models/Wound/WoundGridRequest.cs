using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using SnyderIS.sCore.Web.Mvc.JQGridHelpers;

namespace IQI.Intuition.Web.Models.Wound
{
    [ModelBinder(typeof(AjaxRequestModelBinder<WoundGridRequest>))]
    public class WoundGridRequest : AjaxRequestModel<WoundGridRequest>
    {
        public string Id { get; set; }
        public Guid PatientGuid { get; set; }
        public string PatientFullName { get; set; }
        public string RoomAndWingName { get; set; }
        public string FirstNoted { get; set; }
        public string ResolvedOn { get; set; }
        public string StageName { get; set; }
        public string SiteName { get; set; }
        public string TypeName { get; set; }

        public Expression<Func<Domain.Models.WoundReport, object>> SortBy
        {
            get
            {

                if (RequestedSortBy(model => model.PatientFullName))
                {
                    return x => x.Patient.GetLastName() + "-" + x.Patient.GetFirstName();
                }

                if (RequestedSortBy(model => model.RoomAndWingName))
                {
                    return x => x.Patient.Room.Wing.Name + "-" + x.Patient.Room.Name;
                }

                if(RequestedSortBy(model => model.FirstNoted))
                {
                    return x => x.FirstNotedOn;
                }

                if (RequestedSortBy(model => model.ResolvedOn))
                {
                    return x => x.ResolvedOn;
                }

                if (RequestedSortBy(model => model.StageName))
                {
                    return x => x.CurrentStage.Name;
                }

                if (RequestedSortBy(model => model.SiteName))
                {
                    return x => x.Site.Name;
                }

                if (RequestedSortBy(model => model.TypeName))
                {
                    return x => x.WoundType.Name;
                }

                return i => i.Id;
            }
        }
    }
}
