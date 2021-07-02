using System;
using System.Web.Mvc;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Mvc.ModelMapper;
using IQI.Intuition.Domain.Models;
using IQI.Intuition.Domain.Repositories;
using IQI.Intuition.Infrastructure.Services;
using IQI.Intuition.Web.Models.Administration.Account;
using System.Collections;
using IQI.Intuition.Web.Attributes;
using System.Collections.Generic;
using RedArrow.Framework.Mvc.Security;
using IQI.Intuition.Web.Extensions;

namespace IQI.Intuition.Web.Areas.Administration.Controllers
{
    [AnonymousAccess, RequiresSystemUserAttribute(Domain.Enumerations.SystemUserRole.Admin)]
	public class AccountController : Controller
	{

		public AccountController(
			IActionContext actionContext, 
			IModelMapper modelMapper, 
			IAccountRepository accountRepository)
		{
			ActionContext = actionContext.ThrowIfNullArgument("actionContext");
			ModelMapper = modelMapper.ThrowIfNullArgument("modelMapper");
			AccountRepository = accountRepository.ThrowIfNullArgument("accountRepository");
		}

		protected virtual IActionContext ActionContext { get; private set; }

		protected virtual IModelMapper ModelMapper { get; private set; }

		protected virtual IAccountRepository AccountRepository { get; private set; }


		[HttpGet]
		public ActionResult Index()
		{
			string dataUrl = Url.Action("ListData");
			var gridModel = new AccountGrid(dataUrl);
			return View(gridModel);
		}

		[HttpGet]
		public ActionResult ListData(AccountGridRequest requestModel)
		{
			var accountQuery = AccountRepository.Find(
								requestModel.Name,
								requestModel.SortBy,
				requestModel.SortDescending,
				requestModel.PageNumber,
				requestModel.PageSize);

			// We are setting up the grid to provide data so we pass in the patient chart link formatter
			var gridModel = new AccountGrid();
			ModelMapper.MapForReadOnly(accountQuery, gridModel);

			return gridModel.GetJsonResult();
		}


		[HttpGet]
		public ActionResult Add()
		{
			var formModel = ModelMapper.MapForCreate<AccountAddForm>();  
			return View(formModel);
		}

		[HttpPost, SupportsFormCancel]
		public ActionResult Add(AccountAddForm formModel, bool formCancelled)
		{
			try
			{
				if (formCancelled != true)
				{

					var account = new Account(formModel.Name);
					ModelMapper.MapForCreate(formModel, account);
					AccountRepository.Add(account);

                    var user = new AccountUser(account);
                    user.SystemUser = true;
                    user.IsActive = true;
                    user.Login = "system";
                    user.ChangePassword(string.Concat(Guid.NewGuid().ToString(),DateTime.Now.Ticks.ToString()));

                    account.AddUser(user);

                    this.ControllerContext.SetUserMessage("Account has been created");
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
			var account = AccountRepository.Get(id ?? 0);

			if (account == null)
			{
				return RedirectToAction("View", new { id = id.Value });
			}

			var formModel = ModelMapper.MapForUpdate<AccountEditForm>(account);

			return View(formModel);
		}

		[HttpPost, SupportsFormCancel]
		public ActionResult Edit(AccountEditForm formModel, bool formCancelled)
		{

			try
			{
				if (formCancelled != true)
				{
					var account = AccountRepository.Get(formModel.Id ?? 0);

					if (account != null)
					{
						ModelMapper.MapForUpdate(formModel, account);
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
			var account = AccountRepository.Get(id ?? 0);

			if (account == null)
			{
				return RedirectToAction("Index");
			}

			var formModel = ModelMapper.MapForUpdate<AccountViewForm>(account);

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

  }

}

