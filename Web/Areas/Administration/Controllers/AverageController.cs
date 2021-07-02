using System;
using System.Web.Mvc;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Mvc.ModelMapper;
using IQI.Intuition.Domain.Models;
using IQI.Intuition.Domain.Repositories;
using IQI.Intuition.Infrastructure.Services;
using IQI.Intuition.Web.Models.Administration.Average;
using System.Collections;
using IQI.Intuition.Web.Attributes;
using System.Collections.Generic;
using IQI.Intuition.Web.Extensions;
using IQI.Intuition.Reporting.Repositories;
using RedArrow.Framework.Mvc.Security;

namespace IQI.Intuition.Web.Areas.Administration.Controllers
{
    [AnonymousAccess, RequiresSystemUserAttribute(Domain.Enumerations.SystemUserRole.Admin)]
    public class AverageController : Controller
    {
        protected virtual IActionContext ActionContext { get; private set; }
        protected virtual IModelMapper ModelMapper { get; private set; }
        protected virtual IDimensionRepository DimensionRepository { get; set; }
        protected virtual IDimensionBuilderRepository DimensionBuilderRepository { get; set; }

        public AverageController(
			IActionContext actionContext, 
			IModelMapper modelMapper,
            IDimensionRepository dimensionRepository,
            IDimensionBuilderRepository dimensionBuilderRepository)
		{
			ActionContext = actionContext.ThrowIfNullArgument("actionContext");
			ModelMapper = modelMapper.ThrowIfNullArgument("modelMapper");
            DimensionRepository = dimensionRepository.ThrowIfNullArgument("dimensionRepository");
            DimensionBuilderRepository = dimensionBuilderRepository.ThrowIfNullArgument("dimensionBuilderRepository");
		}

        public ActionResult Index(Guid? id)
        {

            var model = new EditForm();
            var averageTypes = DimensionRepository.GetAllAverageTypes();
            model.AverageOptions = averageTypes.
                Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() })
                .ToList();

            if (id.HasValue == false)
            {
                id = averageTypes.First().Id;
                return RedirectToAction("Index", new { id = id });
            }

            model.AverageType = id.Value;

            var facilities = DimensionRepository.GetAllFacilities();
            model.FacilityAverageOptions = facilities
                .Select(x =>
                    new SelectListItem()
                    {
                        Value = x.Id.ToString(),
                        Text = string.Concat(x.Account.Name, "-", x.Name)
                    })
                    .ToList();

            var facilityAverageTypes = DimensionRepository.GetFacilityAverageTypesForType(id.Value);

            foreach (var fa in facilityAverageTypes)
            {
                var selectItem = model.FacilityAverageOptions.Where(x => x.Value == fa.Facility.Id.ToString()).First();
                selectItem.Selected = true;
            }

            return View(model);
        }

        public ActionResult Toggle(Guid facilityId, Guid typeid)
        {
            var currentEntry = DimensionRepository.GetFacilityAverageTypesForFacility(facilityId)
                .Where(x => x.AverageType.Id == typeid)
                .FirstOrDefault();

            if (currentEntry != null)
            {
                DimensionBuilderRepository.DeleteFacilityAverageType(currentEntry);
                return RedirectToAction("Index", new { id = typeid });
            }

            currentEntry = new Intuition.Reporting.Models.Dimensions.FacilityAverageType();
            currentEntry.Facility = DimensionRepository.GetFacility(facilityId);
            currentEntry.AverageType = DimensionRepository.GetAverageType(typeid);

            DimensionBuilderRepository.AddFacilityAverageType(currentEntry);
            return RedirectToAction("Index", new { id = typeid });
        }
    }
}