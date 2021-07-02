using System;
using System.Web.Mvc;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Mvc.ModelMapper;
using IQI.Intuition.Domain.Models;
using IQI.Intuition.Domain.Repositories;
using IQI.Intuition.Infrastructure.Services;
using IQI.Intuition.Web.Models.Complaint;
using System.Collections;
using IQI.Intuition.Web.Attributes;
using System.Collections.Generic;
using RedArrow.Framework.Utilities;

namespace IQI.Intuition.Web.Controllers
{
    [RequiresActiveAccount]
    public class ComplaintController : Controller
    {
        public ComplaintController(
            IActionContext actionContext, 
            IModelMapper modelMapper,
            IComplaintRepository complaintRepository,
            IEmployeeInfectionRepository employeeInfectionRepository)
        {
            ActionContext = actionContext.ThrowIfNullArgument("actionContext");
            ModelMapper = modelMapper.ThrowIfNullArgument("modelMapper");
            ComplaintRepository = complaintRepository.ThrowIfNullArgument("complaintRepository");
            EmployeeInfectionRepository = employeeInfectionRepository.ThrowIfNullArgument("employeeInfectionRepository");
        }

        protected virtual IActionContext ActionContext { get; private set; }

        protected virtual IModelMapper ModelMapper { get; private set; }

        protected virtual IComplaintRepository ComplaintRepository { get; private set; }

        protected virtual IEmployeeInfectionRepository EmployeeInfectionRepository { get; private set; }


        [HttpGet]
        public ActionResult List()
        {
            string dataUrl = Url.Action("ListData");
            var gridModel = new ComplaintGrid(dataUrl);
            return View(gridModel);
        }

        [HttpGet]
        public ActionResult ListData(ComplaintGridRequest requestModel)
        {
            var query = ComplaintRepository.Find(
                ActionContext.CurrentFacility,
                requestModel.EmployeeName,
                requestModel.PatientName,
                requestModel.DateOccurred,
                requestModel.DateReported,
                requestModel.Wing,
                requestModel.ComplaintTypeName,
                requestModel.SortBy,
                requestModel.SortDescending,
                requestModel.PageNumber,
                requestModel.PageSize);

            // We are setting up the grid to provide data so we pass in the patient chart link formatter
            var gridModel = new ComplaintGrid();
            ModelMapper.MapForReadOnly(query, gridModel);
            return gridModel.GetJsonResult();
        }


        [HttpGet]
        public ActionResult Add()
        {
            var formModel = ModelMapper.MapForCreate<ComplaintForm>();
            ModelMapper.AssignDefaults(formModel);
            return View(formModel);
        }

        [HttpPost, SupportsFormCancel]
        public ActionResult Add(ComplaintForm formModel, bool formCancelled)
        {
            try
            {
                if (formCancelled != true)
                {

                    var complaint = new Complaint();
                    complaint.Facility = ActionContext.CurrentFacility;
                    ModelMapper.MapForCreate(formModel, complaint);
                    complaint.Guid = GuidHelper.NewGuid();
                    ComplaintRepository.Add(complaint);
                }

                return RedirectToAction("List");
            }
            catch (ModelMappingException ex)
            {
                ModelState.AddModelMappingErrors(ex);
            }

            ModelMapper.AssignDefaults(formModel);
            return View(formModel);
        }


        [HttpGet, SupportsTokenAuthentication]
        public ActionResult Edit(int? id, string returnUrl)
        {
            var entity = ComplaintRepository.Get(id ?? 0);

            if (entity == null)
            {
                return RedirectToAction("List");
            }

            var formModel = ModelMapper.MapForUpdate<ComplaintForm>(entity);
            ModelMapper.AssignDefaults(formModel);
            return View(formModel);
        }

        [HttpPost, SupportsFormCancel]
        public ActionResult Edit(ComplaintForm formModel, bool formCancelled)
        {

            try
            {
                if (formCancelled != true)
                {
                    var entity = ComplaintRepository.Get(formModel.Id ?? 0);

                    if (entity != null)
                    {
                        ModelMapper.MapForUpdate(formModel, entity);
                    }
                }

                return RedirectToAction("List");
            }
            catch (ModelMappingException ex)
            {
                ModelState.AddModelMappingErrors(ex);
            }

            ModelMapper.AssignDefaults(formModel);
            return View(formModel);
        }


        public ActionResult Remove(int id)
        {
            var entity = ComplaintRepository.Get(id);
            entity.Deleted = true;

            return RedirectToAction("List");

        }

    }
}
