using System;
using System.Web.Mvc;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Mvc.ModelMapper;
using IQI.Intuition.Domain.Models;
using IQI.Intuition.Domain.Repositories;
using IQI.Intuition.Infrastructure.Services;
using IQI.Intuition.Web.Models.Administration.SystemContact;
using System.Collections;
using IQI.Intuition.Web.Attributes;
using System.Collections.Generic;
using RedArrow.Framework.Mvc.Security;
using IQI.Intuition.Web.Extensions;

namespace IQI.Intuition.Web.Areas.Administration.Controllers
{
    [AnonymousAccess, RequiresSystemUserAttribute]
	public class SystemContactController : Controller
	{

		public SystemContactController(
			IActionContext actionContext, 
			IModelMapper modelMapper, 
			ISystemContactRepository systemcontactRepository,
            IAccountRepository accountRepository,
            ISystemLeadRepository systemLeadRepository)
		{
			ActionContext = actionContext.ThrowIfNullArgument("actionContext");
			ModelMapper = modelMapper.ThrowIfNullArgument("modelMapper");
			SystemContactRepository = systemcontactRepository.ThrowIfNullArgument("systemcontactRepository");
            AccountRepository = accountRepository.ThrowIfNullArgument("accountRepository");
            SystemLeadRepository = systemLeadRepository.ThrowIfNullArgument("systemLeadRepository");
		}

		protected virtual IActionContext ActionContext { get; private set; }

		protected virtual IModelMapper ModelMapper { get; private set; }

		protected virtual ISystemContactRepository SystemContactRepository { get; private set; }

        protected virtual IAccountRepository AccountRepository { get; private set; }

        protected virtual ISystemLeadRepository SystemLeadRepository { get; private set; }

		[HttpGet]
        public ActionResult ListFor(int? accountId, int? leadId)
		{
            string dataUrl = Url.Action("ListDataFor", new { accountId = accountId, leadId = leadId });
			var gridModel = new SystemContactGrid(dataUrl);
			return PartialView(gridModel);
		}

		[HttpGet]
		public ActionResult ListDataFor(SystemContactGridRequest requestModel, int? accountId, int? leadId)
		{
			var systemcontactQuery = SystemContactRepository.Find(
                                accountId,
                                leadId,
								requestModel.FirstName,
								requestModel.LastName,
								requestModel.Title,
								requestModel.Cell,
								requestModel.Direct,
								requestModel.Email,
								requestModel.SortBy,
				requestModel.SortDescending,
				requestModel.PageNumber,
				requestModel.PageSize);

			// We are setting up the grid to provide data so we pass in the patient chart link formatter
			var gridModel = new SystemContactGrid();
			ModelMapper.MapForReadOnly(systemcontactQuery, gridModel);

			return gridModel.GetJsonResult();
		}


		[HttpGet]
        public ActionResult Add(int? accountId, int? leadId)
		{
			var formModel = ModelMapper.MapForCreate<SystemContactAddForm>();  
			return View(formModel);
		}

		[HttpPost, SupportsFormCancel]
        public ActionResult Add(SystemContactAddForm formModel, bool formCancelled, int? accountId, int? leadId)
		{
			try
			{
				if (formCancelled != true)
				{

					var systemcontact = new SystemContact();
					ModelMapper.MapForCreate(formModel, systemcontact);

                    if (accountId.HasValue)
                    {
                        systemcontact.Account = AccountRepository.Get(accountId.Value); 
                    }

                    if (leadId.HasValue)
                    {
                        systemcontact.SystemLead = SystemLeadRepository.Get(leadId.Value);
                    }

					SystemContactRepository.Add(systemcontact);

                    this.ControllerContext.SetUserMessage("Contact has been added");
				}

                if (leadId.HasValue)
                {
                    return RedirectToAction("View", "SystemLead", new { id = leadId.Value });
                }

                if (accountId.HasValue)
                {
                    return RedirectToAction("View", "Account", new { id = accountId.Value });
                }
			}
			catch (ModelMappingException ex)
			{
				ModelState.AddModelMappingErrors(ex);
			}
			return View(formModel);
		}


		[HttpGet]
		public ActionResult Edit(int? id)
		{
			var systemcontact = SystemContactRepository.Get(id ?? 0);

			if (systemcontact == null)
			{
				return RedirectToAction("View", new { id = id.Value });
			}

			var formModel = ModelMapper.MapForUpdate<SystemContactEditForm>(systemcontact);

			return View(formModel);
		}

		[HttpPost, SupportsFormCancel]
		public ActionResult Edit(SystemContactEditForm formModel, bool formCancelled)
		{
            var systemcontact = SystemContactRepository.Get(formModel.Id ?? 0);

			try
			{
				if (formCancelled != true)
				{


					if (systemcontact != null)
					{
						ModelMapper.MapForUpdate(formModel, systemcontact);
					}
				}

                if (systemcontact.SystemLead != null)
                {
                    return RedirectToAction("View", "SystemLead", new { id = systemcontact.SystemLead.Id });
                }

                if (systemcontact.Account != null)
                {
                    return RedirectToAction("View", "Account", new { id = systemcontact.Account.Id });
                }

				return RedirectToAction("View", new { id = formModel.Id });
			}
			catch (ModelMappingException ex)
			{
				ModelState.AddModelMappingErrors(ex);
			}

			return View(formModel);
		}


		[HttpGet]
		public ActionResult View(int? id)
		{
			var systemcontact = SystemContactRepository.Get(id ?? 0);

			if (systemcontact == null)
			{
				return RedirectToAction("Index");
			}

			var formModel = ModelMapper.MapForUpdate<SystemContactViewForm>(systemcontact);

			return View(formModel);
		}

		[HttpPost, SupportsFormCancel]
		public ActionResult View(int? id, bool formCancelled)
		{
            var systemcontact = SystemContactRepository.Get(id ?? 0);

			if (formCancelled)
			{
                if (systemcontact.SystemLead != null)
                {
                    return RedirectToAction("View", "SystemLead", new { id = systemcontact.SystemLead.Id });
                }

                if (systemcontact.Account != null)
                {
                    return RedirectToAction("View", "Account", new { id = systemcontact.Account.Id });
                }
			}

			return RedirectToAction("Edit", new { id = id.Value });

		}

  }

}

