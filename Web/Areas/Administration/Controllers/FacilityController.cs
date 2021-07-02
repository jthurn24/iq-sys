using System;
using System.Web.Mvc;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Mvc.ModelMapper;
using IQI.Intuition.Domain.Models;
using IQI.Intuition.Domain.Repositories;
using IQI.Intuition.Infrastructure.Services;
using IQI.Intuition.Web.Models.Administration.Facility;
using System.Collections;
using IQI.Intuition.Web.Attributes;
using System.Collections.Generic;
using RedArrow.Framework.Mvc.Security;
using IQI.Intuition.Web.Extensions;

namespace IQI.Intuition.Web.Areas.Administration.Controllers
{
    [AnonymousAccess, RequiresSystemUserAttribute(Domain.Enumerations.SystemUserRole.Admin)]
	public class FacilityController : Controller
	{

		public FacilityController(
			IActionContext actionContext, 
			IModelMapper modelMapper, 
			IFacilityRepository facilityRepository,
            IAccountRepository accountRepository,
            IAuthentication authentication,
            IWarningRepository warningRepository,
            IInfectionRepository infectionRepository,
            ISystemRepository systemRepository)
		{
			ActionContext = actionContext.ThrowIfNullArgument("actionContext");
			ModelMapper = modelMapper.ThrowIfNullArgument("modelMapper");
			FacilityRepository = facilityRepository.ThrowIfNullArgument("facilityRepository");
            AccountRepository = accountRepository.ThrowIfNullArgument("accountRepository");
            Authentication = authentication.ThrowIfNullArgument("authentication");
            WarningRepository = warningRepository.ThrowIfNullArgument("warningRepository");
            InfectionRepository = infectionRepository.ThrowIfNullArgument("infectionRepository");
            SystemRepository = systemRepository.ThrowIfNullArgument("systemRepository");
		}

		protected virtual IActionContext ActionContext { get; private set; }

		protected virtual IModelMapper ModelMapper { get; private set; }

		protected virtual IFacilityRepository FacilityRepository { get; private set; }

        protected virtual IAccountRepository AccountRepository { get; private set; }

        protected virtual IAuthentication Authentication { get; private set; }

        protected virtual IWarningRepository WarningRepository { get; private set; }

        protected virtual IInfectionRepository InfectionRepository { get; private set; }

        protected virtual ISystemRepository SystemRepository { get; private set; }

        [HttpGet]
        public ActionResult AccountFacilityList(int? accountId)
        {
            // We are setting up the grid for rendering so we pass in the data URL
            string dataUrl = Url.Action("AccountFacilityListData", new { accountId = accountId });
            var gridModel = new FacilityGrid(dataUrl);

            return PartialView(gridModel);
        }

		[HttpGet]
        public ActionResult AccountFacilityListData(int? accountId, FacilityGridRequest requestModel)
		{
			var facilityQuery = FacilityRepository.Find(
								accountId,
                                requestModel.Name,
                                requestModel.SubDomain,
                                requestModel.State,
								requestModel.SortBy,
				                requestModel.SortDescending,
				                requestModel.PageNumber,
				                requestModel.PageSize);

			// We are setting up the grid to provide data so we pass in the patient chart link formatter
			var gridModel = new FacilityGrid();
			ModelMapper.MapForReadOnly(facilityQuery, gridModel);

			return gridModel.GetJsonResult();
		}


		[HttpGet]
        public ActionResult Add(int? accountId)
		{
			var formModel = ModelMapper.MapForCreate<FacilityAddForm>();  
			return View(formModel);
		}

		[HttpPost, SupportsFormCancel]
		public ActionResult Add(int? accountId,FacilityAddForm formModel, bool formCancelled)
		{
            var account = AccountRepository.Get(accountId.Value);

			try
			{
				if (formCancelled != true)
				{

                    var facility = new Facility(account);
					ModelMapper.MapForCreate(formModel, facility);
                    facility.InfectionDefinition = InfectionRepository.AllInfectionDefinitions.Where(x => x.Id == 1).FirstOrDefault();
                    account.AddFacility(facility);

                    this.ControllerContext.SetUserMessage("Facility has been created");
				}

                return RedirectToAction("View","Account", new { id = account.Id });
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
			var facility = FacilityRepository.Get(id ?? 0);

			if (facility == null)
			{
				return RedirectToAction("View", new { id = id.Value });
			}

			var formModel = ModelMapper.MapForUpdate<FacilityEditForm>(facility);

			return View(formModel);
		}

		[HttpPost, SupportsFormCancel]
		public ActionResult Edit(FacilityEditForm formModel, bool formCancelled)
		{

			try
			{
				if (formCancelled != true)
				{
					var facility = FacilityRepository.Get(formModel.Id ?? 0);

					if (facility != null)
					{
						ModelMapper.MapForUpdate(formModel, facility);
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
			var facility = FacilityRepository.Get(id ?? 0);

			if (facility == null)
			{
                return RedirectToAction("View", "Account", new { id = facility.Account.Id });
			}

			var formModel = ModelMapper.MapForUpdate<FacilityViewForm>(facility);

			return View(formModel);
		}

		[HttpPost, SupportsFormCancel]
		public ActionResult View(int? id, bool formCancelled)
		{
            var facility = FacilityRepository.Get(id ?? 0);

			if (formCancelled)
			{
                return RedirectToAction("View", "Account", new { id = facility.Account.Id });
			}

			return RedirectToAction("Edit", new { id = id.Value });

		}

        public ActionResult Login(int? id)
        {

            var facility = FacilityRepository.Get(id ?? 0);
            var systemUser = facility.Account.Users.Where(x => x.SystemUser == true).FirstOrDefault();

            if (systemUser == null)
            {
                throw new Exception("Unable to locate system user");
            }

            /* redirect to correct url */
            var hostNameSegments = this.HttpContext.Request.Url.Host.Split('.');
            string subDomain = hostNameSegments[0];

            var newUrl = string.Concat(facility.SubDomain,".",this.HttpContext.Request.Url.Host.Split('.').Skip(1).ToDelimitedString('.'));

            this.Authentication.SignOutCurrentUser();
            this.Authentication.SignInUser(systemUser.Guid);

            var auditEntry = new Domain.Models.AuditEntry();
            auditEntry.DetailsText = "User logged in";
            auditEntry.PerformedBy = systemUser.Login;
            auditEntry.AuditType = Domain.Enumerations.AuditEntryType.SuccessfulLogin;
            auditEntry.Facility = facility;
            auditEntry.PerformedAt = DateTime.Now;
            auditEntry.Facility = facility;

            SystemRepository.Add(auditEntry);

            return Redirect(string.Concat(this.HttpContext.Request.Url.Scheme,"://",newUrl,":",this.HttpContext.Request.Url.Port,"/Home"));

        }


        public ActionResult ApplyDefaultWarningRules(int id)
        {
            var facility = FacilityRepository.Get(id);
            var rules = WarningRepository.GetForFacility(id);


            foreach(var defaultRule in WarningRepository.GetDefaults())
            {

                var ruleMatch = rules
                    .Where(x => x.Arguments == defaultRule.Arguments && x.TypeName == defaultRule.TypeName)
                    .FirstOrDefault();

                if (ruleMatch == null)
                {

                    var newRule = new WarningRule();
                    newRule.Facility = facility;
                    newRule.Description = defaultRule.Description;
                    newRule.Arguments = defaultRule.Arguments;
                    newRule.ItemTemplate = defaultRule.ItemTemplate;
                    newRule.Title = defaultRule.Title;
                    newRule.TypeName = defaultRule.TypeName;
                    WarningRepository.Add(newRule);
                }
            }

            ActionContext.SetUserMessage("Warning rules have been assigned");

            return RedirectToAction("View", new { id = id });

        }

  }

}

