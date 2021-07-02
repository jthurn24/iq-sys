using System;
using System.Web.Mvc;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Mvc.ModelMapper;
using IQI.Intuition.Domain.Models;
using IQI.Intuition.Domain.Repositories;
using IQI.Intuition.Infrastructure.Services;
using IQI.Intuition.Web.Models.Administration.FacilityProduct;
using System.Collections;
using IQI.Intuition.Web.Attributes;
using System.Collections.Generic;
using RedArrow.Framework.Mvc.Security;
using IQI.Intuition.Web.Extensions;

namespace IQI.Intuition.Web.Areas.Administration.Controllers
{
    [AnonymousAccess, RequiresSystemUserAttribute(Domain.Enumerations.SystemUserRole.Admin)]
	public class FacilityProductController : Controller
	{

		public FacilityProductController(
			IActionContext actionContext, 
			IModelMapper modelMapper, 
			IFacilityRepository facilityRepository,
            ISystemRepository systemRepository)
		{
			ActionContext = actionContext.ThrowIfNullArgument("actionContext");
			ModelMapper = modelMapper.ThrowIfNullArgument("modelMapper");
            FacilityRepository = facilityRepository.ThrowIfNullArgument("facilityRepository");
            SystemRepository = systemRepository.ThrowIfNullArgument("systemRepository");
		}

		protected virtual IActionContext ActionContext { get; private set; }

		protected virtual IModelMapper ModelMapper { get; private set; }

		protected virtual IFacilityRepository FacilityRepository { get; private set; }

        protected virtual ISystemRepository SystemRepository { get; private set; }

		[HttpGet]
        public ActionResult ListForFacility(int? facilityId)
		{
            string dataUrl = Url.Action("ListDataForFacility", new { facilityId = facilityId});
			var gridModel = new FacilityProductGrid(dataUrl);
			return PartialView(gridModel);
		}

		[HttpGet]
		public ActionResult ListDataForFacility(FacilityProductGridRequest requestModel, int? facilityId)
		{
			var facilityproductQuery = FacilityRepository.FindProduct(
								facilityId,
                                requestModel.SystemProduct,
                                requestModel.Fee,
                                requestModel.FeeType,
                                requestModel.StartOn,
								requestModel.SortBy,
				                requestModel.SortDescending,
				                requestModel.PageNumber,
				                requestModel.PageSize);

			// We are setting up the grid to provide data so we pass in the patient chart link formatter
			var gridModel = new FacilityProductGrid();
			ModelMapper.MapForReadOnly(facilityproductQuery, gridModel);

			return gridModel.GetJsonResult();
		}


		[HttpGet]
		public ActionResult Add(int? facilityId)
		{
			var formModel = ModelMapper.MapForCreate<FacilityProductAddForm>();  
			return View(formModel);
		}

		[HttpPost, SupportsFormCancel]
		public ActionResult Add(FacilityProductAddForm formModel, 
            bool formCancelled,
            int? facilityId)
		{
            var facility = FacilityRepository.Get(facilityId.Value);

			try
			{
				if (formCancelled != true)
				{

					var facilityproduct = new FacilityProduct();
					ModelMapper.MapForCreate(formModel, facilityproduct);

                    facilityproduct.Facility = facility;
                    facility.FacilityProducts.Add(facilityproduct);

                    this.ControllerContext.SetUserMessage("Product has been added");
				}

                return RedirectToAction("View", "Facility", new { id = facility.Id });
			}
			catch (ModelMappingException ex)
			{
				ModelState.AddModelMappingErrors(ex);
			}
			return View(formModel);
		}

        public ActionResult Remove(int id)
        {
            var product = FacilityRepository.GetProduct(id);
            var facilityId = product.Facility.Id;

            product.Facility.FacilityProducts.Remove(product);
            product.Facility = null;

            this.ControllerContext.SetUserMessage("Product has been removed");

            return RedirectToAction("View", "Facility", new { id = facilityId });
        }

		[HttpGet]
		public ActionResult View(int? id)
		{
			var facilityproduct = FacilityRepository.GetProduct(id ?? 0);

			if (facilityproduct == null)
			{
				return RedirectToAction("Index");
			}

			var formModel = ModelMapper.MapForUpdate<FacilityProductViewForm>(facilityproduct);

			return View(formModel);
		}


        public ActionResult GetProductDetails(int id)
        {
            var product = SystemRepository.GetSystemProducts().Where(x => x.Id == id).FirstOrDefault();
            var data = new { Fee = product.DefaultFee, FeeType = (int)product.DefaultFeeType };

            return Json(data,JsonRequestBehavior.AllowGet);

        }
  }

}

