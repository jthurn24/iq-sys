using System;
using System.Linq;
using System.Web.Mvc;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Mvc.ModelMapper;
using IQI.Intuition.Domain.Models;
using IQI.Intuition.Domain.Repositories;
using IQI.Intuition.Infrastructure.Services;
using IQI.Intuition.Web.Models.Warning;
using IQI.Intuition.Web.Attributes;

namespace IQI.Intuition.Web.Controllers
{
    [RequiresActiveAccount]
    public class WarningController : Controller
    {
        public WarningController(
            IActionContext actionContext, 
            IModelMapper modelMapper, 
            IWarningRepository warningRepository)
        {
            ActionContext = actionContext.ThrowIfNullArgument("actionContext");
            ModelMapper = modelMapper.ThrowIfNullArgument("modelMapper");
            WarningRepository = warningRepository.ThrowIfNullArgument("warningRepository");
        }

        protected virtual IActionContext ActionContext { get; private set; }

        protected virtual IModelMapper ModelMapper { get; private set; }

        protected virtual IWarningRepository WarningRepository { get; private set; }

        [HttpGet]
        public ActionResult List()
        {
            string dataUrl = Url.Action("ListData");
            var gridModel = new WarningGrid(dataUrl);
            return View(gridModel);
        }

        [HttpGet]
        public ActionResult ListData(WarningGridRequest requestModel)
        {

            var warningQuery = WarningRepository.SearchFacility(
                ActionContext.CurrentFacility.Id,
                requestModel.TriggeredOn,
                requestModel.Title,
                requestModel.PatientName,
                requestModel.SortBy,
                requestModel.SortDescending,
                requestModel.PageNumber,
                requestModel.PageSize);

            // Here we are setting up the grid to provide data so we pass in the patient chart link formatter
            var gridModel = new WarningGrid();
            ModelMapper.MapForReadOnly(warningQuery, gridModel);

            var result = gridModel.GetJsonResult();

            return result;
        }

        public ActionResult View(int? id, string returnUrl)
        {
            var warning = WarningRepository.Get(id ?? 0);

            if (warning == null)
            {
                return RedirectToAction("List");
            }

            var formModel = ModelMapper.MapForReadOnly<WarningInfo>(warning);
 
            return View(formModel);
        }

        [HttpPost]
        public ActionResult View(int? id, string returnUrl, FormCollection data)
        {
            return Redirect(returnUrl);
        }
    }
}
