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
using IQI.Intuition.Web.Models.Precaution;
using RedArrow.Framework.Utilities;
using RedArrow.Framework.ObjectModel.AuditTracking;
using RedArrow.Framework.Extensions.Common;

namespace IQI.Intuition.Web.Controllers
{
    [RequiresActiveAccount]
    public class PrecautionController : Controller
    {

        public PrecautionController(
            IActionContext actionContext,
            IModelMapper modelMapper,
            IPrecautionRepository precautionRepository,
            IPatientRepository patientRepository,
            AuditTrackingWorker auditWorker)
        {
            ActionContext = actionContext.ThrowIfNullArgument("actionContext");
            ModelMapper = modelMapper.ThrowIfNullArgument("modelMapper");
            PrecautionRepository = precautionRepository.ThrowIfNullArgument("precautionRepository");
            PatientRepository = patientRepository.ThrowIfNullArgument("patientRepository");
            AuditWorker = auditWorker;
        }

        protected virtual IActionContext ActionContext { get; private set; }
        protected virtual IModelMapper ModelMapper { get; private set; }
        protected virtual IPrecautionRepository PrecautionRepository { get; private set; }
        protected virtual IPatientRepository PatientRepository { get; private set; }
        protected virtual AuditTrackingWorker AuditWorker { get; private set; }


        public ActionResult PatientList(Guid? id, int? productId, DateTime? startDate, DateTime? endDate)
        {
            // We are setting up the grid for rendering so we pass in the data URL
            string dataUrl = Url.Action("PatientListData", new { PatientGuid = id, ProductId = productId });
            var gridModel = new PatientPrecautionGrid(dataUrl);

            return PartialView(gridModel);
        }

        [HttpGet]
        public ActionResult PatientListData(PatientPrecautionGridRequest requestModel)
        {
            var patient = ActionContext.CurrentFacility.FindPatient(requestModel.PatientGuid ?? Guid.Empty);

            if (patient == null)
            {
                return null;
            }

            var query = PrecautionRepository.Find(
                patient,
                requestModel.ProductId,
                requestModel.SortBy,
                requestModel.SortDescending,
                requestModel.PageNumber,
                requestModel.PageSize);

            // We are setting up the grid to provide data so we pass in the patient chart link formatter
            var gridModel = new PatientPrecautionGrid();
            ModelMapper.MapForReadOnly(query, gridModel);

            return gridModel.GetJsonResult();
        }



        public ActionResult Delete(Guid id)
        {

            var entity = PrecautionRepository.Get(id);
            entity.Deleted = true;
            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Form(int patientId, int? productId)
        {
            var products = ActionContext.CurrentFacility.FacilityProducts.Select(x => x.SystemProduct);

            if (productId.HasValue) products = products.Where(x => x.Id == productId);

            var types = PrecautionRepository.GetTypes(null)
                .Where(x => products.Select(xx => xx.Id).Contains(x.SystemProduct.Id))
                .Select(x => new PatientPrecautionOption()
                {
                     PrecautionId = x.Id,
                     PrecautionName = x.Name,
                     ProductId = x.SystemProduct.Id,
                     ProductName = x.SystemProduct.Name
                }).ToList();


            products = products.Where(x => types.Select(xx => xx.ProductId).Contains(x.Id));

            return PartialView(new PatientPrecautionFormData() {
                Products = products.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }),
                Types = types,
                PatientId = patientId });             
        }


        public ActionResult Get(int id)
        {
            var entity = PrecautionRepository.Get(id);
            var model = new PatientPrecautionForm()
            {
                 AdditionalDescription = entity.AdditionalDescription,
                 EndDate = ConvertDate(entity.EndDate),
                 StartDate = ConvertDate(entity.StartDate),
                Ended = entity.EndDate.HasValue,
                 Guid =entity.Guid,
                 PatientId = entity.Patient.Id,
                 PrecautionTypeId = entity.PrecautionType.Id,
                 ProductId = entity.PrecautionType.SystemProduct.Id
            };
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Update(PatientPrecautionForm form)
        {

            if(form.StartDate == string.Empty)
            {
                return Json(new { Success = false, Message = "Start Date must be provided" });
            }

            if (String.IsNullOrEmpty(form.EndDate) && ConvertDate(form.StartDate) > ConvertDate(form.EndDate))
            {
                return Json(new { Success = false, Message = "End Date must occur after the start date" });
            }

            if(form.PrecautionTypeId.HasValue == false)
            {
                return Json(new { Success = false, Message = "You must select a precaution type" });
            }

            PatientPrecaution entity;

            if(form.Guid.HasValue)
            {
                entity = PrecautionRepository.Get(form.Guid.Value);
            }
            else
            {
                entity = new PatientPrecaution();
                entity.Guid = GuidHelper.NewGuid();
                entity.Patient = ActionContext.CurrentFacility.FindPatient(form.PatientId);
                entity.PrecautionType = PrecautionRepository.GetTypes(null).Where(x => x.Id == form.PrecautionTypeId).First();
                PrecautionRepository.Add(entity);
            }

            entity.AdditionalDescription = form.AdditionalDescription;
            entity.EndDate = ConvertDate(form.EndDate);
            entity.StartDate =  ConvertDate(form.StartDate);


            return Json(new { Success = true });
        }

        private string ConvertDate(DateTime? src)
        {
            if (!src.HasValue) return string.Empty;

            return src.Value.ToString("yyyy-MM-dd");
        }

        private DateTime? ConvertDate(string src)
        {
            if (String.IsNullOrEmpty(src)) return null;

            return DateTime.Parse(src);
        }
 
    }
}