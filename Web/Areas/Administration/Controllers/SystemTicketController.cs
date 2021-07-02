using System;
using System.Web.Mvc;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Mvc.ModelMapper;
using IQI.Intuition.Domain.Models;
using IQI.Intuition.Domain.Repositories;
using IQI.Intuition.Infrastructure.Services;
using IQI.Intuition.Web.Models.Administration.SystemTicket;
using System.Collections;
using IQI.Intuition.Web.Attributes;
using System.Collections.Generic;
using RedArrow.Framework.Mvc.Security;
using IQI.Intuition.Web.Extensions;

namespace IQI.Intuition.Web.Areas.Administration.Controllers
{
    [AnonymousAccess, RequiresSystemUserAttribute(Domain.Enumerations.SystemUserRole.Admin)]
	public class SystemTicketController : Controller
	{

		public SystemTicketController(
			IActionContext actionContext, 
			IModelMapper modelMapper, 
			ISystemTicketRepository systemticketRepository,
            IAccountRepository accountRepository)
		{
			ActionContext = actionContext.ThrowIfNullArgument("actionContext");
			ModelMapper = modelMapper.ThrowIfNullArgument("modelMapper");
			SystemTicketRepository = systemticketRepository.ThrowIfNullArgument("systemticketRepository");
            AccountRepository = accountRepository.ThrowIfNullArgument("accountRepository");
		}

		protected virtual IActionContext ActionContext { get; private set; }

		protected virtual IModelMapper ModelMapper { get; private set; }

		protected virtual ISystemTicketRepository SystemTicketRepository { get; private set; }

        protected virtual IAccountRepository AccountRepository { get; private set;}

		[HttpGet]
        public ActionResult Index(SystemTicketList model, bool? includeClosed, int? upPriority, int? downPriority)
		{
            model.PageSize = 50;

            if (model.PageNumber.HasValue == false)
            {
                model.PageNumber = 1;
            }

            string systemUser = string.Empty;

            if (model.MyTickets == true)
            {
                systemUser = ActionContext.CurrentSystemUser.Login;
            }

            var systemticketQuery = SystemTicketRepository.Find(
                null,
                model.Account,
                model.AccountUser,
                model.SystemTicketType,
                model.Details,
                model.Priority,
                systemUser,
                model.Release,
                includeClosed == true ? Domain.Enumerations.SystemTicketSearchMode.All : Domain.Enumerations.SystemTicketSearchMode.OpenOnly,
                x => x.Priority,
                true,
                model.PageNumber.Value,
                model.PageSize.Value);

            model.PageNumber = systemticketQuery.PageNumber;
            model.PageSize = systemticketQuery.PageSize;
            model.TotalPages = systemticketQuery.TotalPages;
            model.TotalResults = systemticketQuery.TotalResults;
            model.PageValues = systemticketQuery.PageValues.Select(x => ModelMapper.MapForReadOnly<SystemTicketGridItem>(x)).ToList();
            model.SystemTicketTypeOptions = SystemTicketRepository
                .AllTicketTypes
                .Select(x => new SelectListItem() { Text = x.Name, Value = x.Name })
                .Prepend(new SelectListItem() { Value = string.Empty, Text = "All" });

            return View(model);
		}


        [HttpGet]
        public ActionResult ListForAccount(bool closed, int accountId)
        {
            string dataUrl = Url.Action("ListDataForAccount", new { closed = closed, accountId = accountId });
            var gridModel = new AccountSystemTicketGrid(dataUrl,closed.ToString());
            return PartialView(gridModel);
        }

        [HttpGet]
        public ActionResult ListDataForAccount(bool closed, int accountId, SystemTicketGridRequest requestModel)
        {

			var systemticketQuery = SystemTicketRepository.Find(
                accountId,
                requestModel.Account,
                requestModel.AccountUser,
                requestModel.SystemTicketType,
                requestModel.Details,
                requestModel.Priority,
                requestModel.SystemUser,
                requestModel.Release,
                closed ?  Domain.Enumerations.SystemTicketSearchMode.ClosedOnly : Domain.Enumerations.SystemTicketSearchMode.OpenOnly,
				requestModel.SortBy,
				requestModel.SortDescending,
				requestModel.PageNumber,
				requestModel.PageSize);

            // We are setting up the grid to provide data so we pass in the patient chart link formatter
            var gridModel = new AccountSystemTicketGrid(closed.ToString());
            ModelMapper.MapForReadOnly(systemticketQuery, gridModel);

            return gridModel.GetJsonResult();
        }

		[HttpGet]
		public ActionResult Add(int? accountId)
		{
			var formModel = ModelMapper.MapForCreate<SystemTicketAddForm>(); 
			return View(formModel);
		}

		[HttpPost, SupportsFormCancel]
        public ActionResult Add(SystemTicketAddForm formModel, bool formCancelled, int? accountId)
		{
			try
			{
				if (formCancelled != true)
				{

					var systemticket = new SystemTicket();
					ModelMapper.MapForCreate(formModel, systemticket);

                    systemticket.CreatedOn = DateTime.Now;

					SystemTicketRepository.Add(systemticket);

                    this.ControllerContext.SetUserMessage("Ticket has been created");


                    if (systemticket.SystemUser != null && systemticket.SystemUser.EmailAddress.IsNotNullOrEmpty())
                    {
                        ActionContext.SendEmailNotification(
                            systemticket.SystemUser.EmailAddress,
                            string.Format("Notification: New ticket from: {0}",ActionContext.CurrentSystemUser.Login)
                            ,systemticket.Details);
                    }

                    if (accountId.HasValue)
                    {
                        systemticket.Account = AccountRepository.Get(accountId.Value);
                        return RedirectToAction("View", "Account", new { area = "Administration", id = accountId.Value });
                    }
				}

				return RedirectToAction("Index");
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
			var systemticket = SystemTicketRepository.Get(id ?? 0);

			if (systemticket == null)
			{
				return RedirectToAction("View", new { id = id.Value });
			}

			var formModel = ModelMapper.MapForUpdate<SystemTicketEditForm>(systemticket);

			return View(formModel);
		}

		[HttpPost, SupportsFormCancel]
		public ActionResult Edit(SystemTicketEditForm formModel, bool formCancelled)
		{

			try
			{
				if (formCancelled != true)
				{
					var systemticket = SystemTicketRepository.Get(formModel.Id ?? 0);

					if (systemticket != null)
					{
                        var currentUser = systemticket.SystemUser;

						ModelMapper.MapForUpdate(formModel, systemticket);

                        if (currentUser == null && systemticket.SystemUser != null)
                        {
                            ActionContext.SendEmailNotification(
                                string.Format("Notification: Ticket assigned to: {0} by: {1}",systemticket.SystemUser.Login, ActionContext.CurrentSystemUser.Login)
                                , systemticket.Details);
                        }
                        else if (currentUser != systemticket.SystemUser && systemticket.SystemUser.EmailAddress.IsNotNullOrEmpty())
                        {
                            ActionContext.SendEmailNotification(
                                systemticket.SystemUser.EmailAddress,
                                string.Format("Notification: Ticket assigned to you from: {0}", ActionContext.CurrentSystemUser.Login)
                                , systemticket.Details);
                        }


                        if (systemticket.Status == Domain.Enumerations.SystemTicketStatus.Closed && systemticket.ClosedOn.HasValue == false)
                        {
                            systemticket.ClosedOn = DateTime.Now;
                        }

                        if (systemticket.Status != Domain.Enumerations.SystemTicketStatus.Closed)
                        {
                            systemticket.ClosedOn = null; 
                        }
					}
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
			var systemticket = SystemTicketRepository.Get(id ?? 0);

			if (systemticket == null)
			{
				return RedirectToAction("Index");
			}

			var formModel = ModelMapper.MapForUpdate<SystemTicketViewForm>(systemticket);

			return View(formModel);
		}

		[HttpPost, SupportsFormCancel]
		public ActionResult View(int? id, bool formCancelled)
		{

			if (formCancelled)
			{
				return RedirectToAction("Index");
			}

			return RedirectToAction("Edit", new { id = id.Value });

		}


        public ActionResult UpdatingRaiting(int id, int priority)
        {
            var systemticket = SystemTicketRepository.Get(id);
            systemticket.Priority = priority;

            return Json(new { success = "true" }, JsonRequestBehavior.AllowGet);
        }
  }

}

