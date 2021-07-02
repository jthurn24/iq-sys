using System;
using System.Web.Mvc;
using System.Linq;
using System.Collections.Generic;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Mvc.ModelMapper;
using IQI.Intuition.Infrastructure.Services;
using IQI.Intuition.Infrastructure.Services.Development;
using IQI.Intuition.Web.Models.Navigation;
using IQI.Intuition.Web.Models.Authentication;
using IQI.Intuition.Web.Attributes;
using RedArrow.Framework.Mvc.Security;
using IQI.Intuition.Domain.Repositories;
using RedArrow.Framework.Logging;

namespace IQI.Intuition.Web.Controllers
{
    [RequiresActiveAccount]
    public class AuthenticationController : Controller
    {
        public AuthenticationController(
            IActionContext actionContext,
            IAuthentication authentication,
            ISystemRepository  systemRepository,
            IModelMapper modelMapper,
            ILog log)
        {
            ActionContext = actionContext.ThrowIfNullArgument("actionContext");
            Authentication = authentication.ThrowIfNullArgument("authentication");
            SystemRepository = systemRepository.ThrowIfNullArgument("systemRepository");
            ModelMapper = modelMapper.ThrowIfNullArgument("modelMapper");
            Log = log;
        }

        protected virtual IActionContext ActionContext { get; private set; }

        protected virtual IAuthentication Authentication { get; private set; }

        protected virtual ISystemRepository SystemRepository { get; private set; }

        protected virtual IModelMapper ModelMapper { get; private set; }

        protected virtual ILog Log { get; private set; }

        [HttpGet]
        [AnonymousAccess]
        public ActionResult SignIn()
        {
            return View(ModelMapper.MapForCreate<SignInForm>());
        }

        [HttpPost]
        [AnonymousAccess]
        public ActionResult SignIn(SignInForm model)
        {
            var auditEntry = new Domain.Models.AuditEntry();
            auditEntry.PerformedAt = DateTime.Now;
            auditEntry.Facility = ActionContext.CurrentFacility;


            if (ModelMapper.Validate(model, ModelState))
            {
                var user = SystemRepository.GetUserByCredentials(ActionContext.CurrentAccount, model.Login, model.Password);


                if (user == null)
                {
                    ModelState.AddModelError("", "Sorry, that User Name and Password combination is incorrect");
                    auditEntry.DetailsText = "User Name and Password combination is incorrect";
                    auditEntry.PerformedBy = model.Login;
                    auditEntry.AuditType = Domain.Enumerations.AuditEntryType.FailedLogin;
                    SystemRepository.Add(auditEntry);


                }
                else if (user.IsActive == false || user.SystemUser)
                {
                    ModelState.AddModelError("", "Account has been disabled");
                    auditEntry.DetailsText = "Account has been disabled.";
                    auditEntry.PerformedBy = model.Login;
                    auditEntry.AuditType = Domain.Enumerations.AuditEntryType.FailedLogin;
                    SystemRepository.Add(auditEntry);

                }
                else if (user.Facilities.Contains(ActionContext.CurrentFacility) == false && user.SystemUser == false)
                {
                    ModelState.AddModelError("", "Access is denied to this facility");
                    auditEntry.DetailsText = "Access is denied to this facility";
                    auditEntry.PerformedBy = model.Login; 
                    auditEntry.AuditType = Domain.Enumerations.AuditEntryType.FailedLogin;
                    SystemRepository.Add(auditEntry);

                }
                else if (user != null)
                {
                    Authentication.SignInUser(user.Guid);
                    user.RecordSignIn();

                    Log.Info("User Login");

                    auditEntry.DetailsText = "User logged in";
                    auditEntry.PerformedBy = model.Login;
                    auditEntry.AuditType = Domain.Enumerations.AuditEntryType.SuccessfulLogin;
                    SystemRepository.Add(auditEntry);

                    if (user.AgreementSignedOn.HasValue == false && user.SystemUser == false)
                    {
                        return RedirectToAction("UserAgreement");
                    }

                    if (model.ReturnUrl.IsNotNullOrWhiteSpace())
                    {
                        return Redirect(model.ReturnUrl);
                    }


                    return RedirectToAction("Index", "Home");
                }

            }

            return View(model);
        }

        [AnonymousAccess]
        public ActionResult SignOut()
        {
            Authentication.SignOutCurrentUser();

            return RedirectToAction("Index", "Home");
        }

        [AnonymousAccess]
        public ActionResult AccessDenied()
        {
            return View();
        }

        [AnonymousAccess]
        public ActionResult UserPanel()
        {
            var accountUser = SystemRepository.GetUserByGuid(
                ActionContext.CurrentAccount,
                Authentication.CurrentUserGuid);

            if (accountUser == null)
            {
                // Model is null if no user is authenticated
                return PartialView();
            }

            return PartialView(ModelMapper.MapForReadOnly<AccountUserInfo>(accountUser));
        }

        [HttpGet]
        public ActionResult UserAgreement()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UserAgreement(FormCollection collection)
        {
            ActionContext.CurrentUser.AgreementSignedOn = DateTime.Now;

            return RedirectToAction("Index", "Home");
        }
    }
}