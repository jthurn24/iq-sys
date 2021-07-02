using System;
using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Mvc.ModelMapper;
using IQI.Intuition.Infrastructure.Services;
using IQI.Intuition.Web.Attributes;
using IQI.Intuition.Web.Models.Reporting;
using IQI.Intuition.Domain.Repositories;
using IQI.Intuition.Domain.Models;

namespace IQI.Intuition.Web.Areas.Reporting.Controllers
{
   [SupportsTokenAuthentication]
    public class StateWisconsinRealtimeController : Controller
    {
        private Facility _Facility;

        public StateWisconsinRealtimeController(
            IActionContext actionContext, 
            IModelMapper modelMapper,
            IInfectionRepository infectionRepository,
            IEmployeeInfectionRepository employeeInfectionRepository,
            IIncidentRepository incidentRepository)
        {
            ActionContext = actionContext.ThrowIfNullArgument("actionContext");
            ModelMapper = modelMapper.ThrowIfNullArgument("modelMapper");
            InfectionRepository = infectionRepository.ThrowIfNullArgument("infectionRepository"); 
            EmployeeInfectionRepository = employeeInfectionRepository.ThrowIfNullArgument("employeeInfectionRepository");
            IncidentRepository = incidentRepository.ThrowIfNullArgument("incidentRepository"); 

            _Facility = ActionContext.CurrentFacility;
        }

        protected virtual IActionContext ActionContext { get; private set; }
        protected virtual IModelMapper ModelMapper { get; private set; }
        protected virtual IInfectionRepository InfectionRepository { get; private set; }
        protected virtual IEmployeeInfectionRepository EmployeeInfectionRepository { get; private set; }
        protected virtual IIncidentRepository IncidentRepository { get; private set; }


        public ActionResult DHSStaffOutbreakCaseLog(Models.Reporting.StateOfWisconsin.DHSStaffOutbreakCaseLogView model)
        {
            if (model.StartDate.HasValue == false)
            {
                model.StartDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1, 0, 0, 0);
            }

            if (model.EndDate.HasValue == false)
            {
                model.EndDate = new DateTime(DateTime.Today.AddMonths(1).Year, DateTime.Today.AddMonths(1).Month, 1);
            }

            var infectionTypes = new List<SelectListItem>();
            infectionTypes.Add(new SelectListItem() { Text = "Show All", Value = string.Empty });

            foreach (var infectionType in InfectionRepository.AllInfectionTypes)
            {
                infectionTypes.Add(new SelectListItem() { Text = infectionType.Name, Value = infectionType.Id.ToString() });
            }
            model.InfectionTypeOptions = infectionTypes;


            var infections = this.EmployeeInfectionRepository.FindForLineListing(
                ActionContext.CurrentFacility,
                model.StartDate,
                model.EndDate,
                model.InfectionType);

            model.SetData(infections);

            int counter = 0;
            foreach (var item in model.Infections)
            {
                counter++;
                item.Number = counter.ToString();
            }

            return View(model);
        }

    }
}
