using System;
using System.Web.Mvc;
using System.Linq;
using System.Collections.Generic;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Mvc.ModelMapper;
using IQI.Intuition.Infrastructure.Services;
using IQI.Intuition.Infrastructure.Services.BusinessLogic.FacilityTimeLine;
using IQI.Intuition.Web.Models.Timeline;
using RedArrow.Framework.Mvc.Extensions;
using RedArrow.Framework.Mvc.Security;
using IQI.Intuition.Web.Attributes;
using IQI.Intuition.Domain;
using IQI.Intuition.Domain.Repositories;
using RedArrow.Framework.Extensions.Formatting;
using RedArrow.Framework.Persistence;

namespace IQI.Intuition.Web.Controllers
{
    public class TimelineController : Controller
    {
        public TimelineController(
            IActionContext actionContext,
            IModelMapper modelMapper,
            IDataContext dataContext)
        {
            ActionContext = actionContext.ThrowIfNullArgument("actionContext");
            ModelMapper = modelMapper.ThrowIfNullArgument("modelMapper");
            DataContext = dataContext;
        }

        protected virtual IActionContext ActionContext { get; private set; }

        protected virtual IModelMapper ModelMapper { get; private set; }

        protected virtual IDataContext DataContext { get; private set; }

        public ActionResult FacilityTimeline(int? patientId)
        {
            var model = new FacilityTimelineView();

            var service = new FacilityTimeLineService(DataContext);

            var c = new Infrastructure.Services.BusinessLogic.FacilityTimeLine.EventSource.SourceCriteria()
                {
                    EndDate = DateTime.Today
                };

            if (patientId.HasValue)
            {
                var p = ActionContext.CurrentFacility.FindPatient(patientId.Value);
                c.PatientId = patientId.Value;
                model.PatientMode = true;
                model.PatientGuid = p.Guid;
                model.Title = string.Format("{0} - Timeline", p.FullName);
                c.StartDate = DateTime.Today.AddYears(-15);
            }
            else
            {
                c.FacilityId = ActionContext.CurrentFacility.Id;
                model.Title = string.Format("{0} - Timeline", ActionContext.CurrentFacility.Name);
                c.StartDate = DateTime.Today.AddMonths(-12);

            }

            var events = service.GetEvents(c);

            model.LoadData(events);

            return View(model);
        }



    }
}