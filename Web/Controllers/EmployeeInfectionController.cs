using System;
using System.Web.Mvc;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Mvc.ModelMapper;
using IQI.Intuition.Domain.Models;
using IQI.Intuition.Domain.Repositories;
using IQI.Intuition.Infrastructure.Services;
using IQI.Intuition.Web.Models.EmployeeInfection;
using System.Collections;
using IQI.Intuition.Web.Attributes;
using System.Collections.Generic;

namespace IQI.Intuition.Web.Controllers
{
    [RequiresActiveAccount]
    public class EmployeeInfectionController: Controller
    {
        public EmployeeInfectionController(
            IActionContext actionContext, 
            IModelMapper modelMapper, 
            IInfectionRepository infectionRepository,
            IEmployeeInfectionRepository employeeInfectionRepository)
        {
            ActionContext = actionContext.ThrowIfNullArgument("actionContext");
            ModelMapper = modelMapper.ThrowIfNullArgument("modelMapper");
            InfectionRepository = infectionRepository.ThrowIfNullArgument("infectionRepository");
            EmployeeInfectionRepository = employeeInfectionRepository.ThrowIfNullArgument("employeeInfectionRepository");
        }

        protected virtual IActionContext ActionContext { get; private set; }

        protected virtual IModelMapper ModelMapper { get; private set; }

        protected virtual IInfectionRepository InfectionRepository { get; private set; }

        protected virtual IEmployeeInfectionRepository EmployeeInfectionRepository { get; private set; }


        [HttpGet]
        public ActionResult List()
        {
            string dataUrl = Url.Action("ListData");
            var gridModel = new EmployeeInfectionGrid(dataUrl);
            return View(gridModel);
        }

        [HttpGet]
        public ActionResult ListData(EmployeeInfectionGridRequest requestModel)
        {
            var infectionQuery = EmployeeInfectionRepository.Find(
                ActionContext.CurrentFacility,
                requestModel.EmployeeName,
                requestModel.InfectionTypeName,
                requestModel.Department,
                requestModel.NotifiedOn,
                requestModel.WellOn,
                requestModel.SortBy,
                requestModel.SortDescending,
                requestModel.PageNumber,
                requestModel.PageSize);

            // We are setting up the grid to provide data so we pass in the patient chart link formatter
            var gridModel = new EmployeeInfectionGrid();
            ModelMapper.MapForReadOnly(infectionQuery, gridModel);

            return gridModel.GetJsonResult();
        }


        [HttpGet]
        public ActionResult Add()
        {
            var formModel = ModelMapper.MapForCreate<EmployeeInfectionForm>();  
            return View(formModel);
        }

        [HttpPost, SupportsFormCancel]
        public ActionResult Add(EmployeeInfectionForm formModel, bool formCancelled)
        {
            try
            {
                if (formCancelled != true)
                {

                    var infection = new EmployeeInfection(ActionContext.CurrentFacility);
                    ModelMapper.MapForCreate(formModel, infection);
                    EmployeeInfectionRepository.Add(infection);
                }

                return RedirectToAction("Index", "Home");
            }
            catch (ModelMappingException ex)
            {
                ModelState.AddModelMappingErrors(ex);
            }

            ModelMapper.AssignDefaults(formModel.ClientData);
            return View(formModel);
        }


        [HttpGet, SupportsTokenAuthentication]
        public ActionResult Edit(int? id, string returnUrl)
        {
            var infection = EmployeeInfectionRepository.Get(id ?? 0);

            if (infection == null)
            {
                return RedirectToAction("List");
            }

            var formModel = ModelMapper.MapForUpdate<EmployeeInfectionForm>(infection);

            return View(formModel);
        }

        [HttpPost, SupportsFormCancel]
        public ActionResult Edit(EmployeeInfectionForm formModel, bool formCancelled, string returnUrl)
        {

            try
            {
                if (formCancelled != true) 
                {
                    var infection = EmployeeInfectionRepository.Get(formModel.Id ?? 0);

                    if (infection != null)
                    {
                        ModelMapper.MapForUpdate(formModel, infection);
                    }
                }

                if (returnUrl.IsNotNullOrWhiteSpace())
                {
                    return Redirect(returnUrl);
                }


                return RedirectToAction("List");
            }
            catch (ModelMappingException ex)
            {
                ModelState.AddModelMappingErrors(ex);
            }

            ModelMapper.AssignDefaults(formModel.ClientData);
            return View(formModel);
        }


        public ActionResult Remove(int id)
        {
            var infection = EmployeeInfectionRepository.Get(id);
            infection.Deleted = true;

            return RedirectToAction("Index", "Home");

        }
    }
}