using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Mvc.ModelMapper;
using IQI.Intuition.Domain.Models;
using IQI.Intuition.Domain.Repositories;
using IQI.Intuition.Infrastructure.Services;
using IQI.Intuition.Web.Models.Employee;
using IQI.Intuition.Domain;
using IQI.Intuition.Web.Attributes;

namespace IQI.Intuition.Web.Controllers
{
    [RequiresActiveAccount]
    public class EmployeeController : Controller
    {
        public EmployeeController(
            IActionContext actionContext, 
            IModelMapper modelMapper, 
            IEmployeeRepository employeeRepository)
        {
            ActionContext = actionContext.ThrowIfNullArgument("actionContext");
            ModelMapper = modelMapper.ThrowIfNullArgument("modelMapper");
            EmployeeRepository = employeeRepository.ThrowIfNullArgument("employeeRepository");
        }

        protected virtual IActionContext ActionContext { get; private set; }
        protected virtual IModelMapper ModelMapper { get; private set; }
        protected virtual IEmployeeRepository EmployeeRepository { get; private set; }

        public ActionResult SelectEmployee()
        {
            return PartialView();
        }


        public ActionResult SelectEmployeeSearch(string firstName, string lastName)
        {
            var patients = EmployeeRepository.Find(ActionContext.CurrentFacility);

            if (firstName.IsNotNullOrEmpty())
            {
                patients = patients.Where(x => x.FirstName.ToLower().Contains(firstName.ToLower()));
            }

            if (lastName.IsNotNullOrEmpty())
            {
                patients = patients.Where(x => x.LastName.ToLower().Contains(firstName.ToLower()));
            }

            var model = patients.Select(
                x => ModelMapper.MapForReadOnly<EmployeeInfo>(x));

            return PartialView(model);

        }

        public ActionResult SelectEmployeeSearchAdd(string firstName, string lastName, string middleName)
        {
            var newEmployee = new Domain.Models.Employee();
            newEmployee.LastName = lastName;
            newEmployee.FirstName = firstName;
            newEmployee.MiddleName = middleName;
            newEmployee.Facility = ActionContext.CurrentFacility;
            EmployeeRepository.Add(newEmployee);

            return Json(new { id = newEmployee.Id, fullName = newEmployee.FullName }, JsonRequestBehavior.AllowGet);
        }
    }
}
