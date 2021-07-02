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

namespace IQI.Intuition.Web.Areas.Administration.Controllers
{
    [AnonymousAccess]
    public class AuthenticationController : Controller
    {
        public AuthenticationController(
            IActionContext actionContext,
            IAuthentication authentication,
            ISystemRepository systemRepository,
            IModelMapper modelMapper)
        {
            ActionContext = actionContext.ThrowIfNullArgument("actionContext");
            Authentication = authentication.ThrowIfNullArgument("authentication");
            ModelMapper = modelMapper.ThrowIfNullArgument("modelMapper");
            SystemRepository = systemRepository.ThrowIfNullArgument("systemRepository");
        }

        protected virtual IActionContext ActionContext { get; private set; }

        protected virtual IAuthentication Authentication { get; private set; }

        protected virtual IModelMapper ModelMapper { get; private set; }

        protected virtual ISystemRepository SystemRepository { get; private set; }


        [HttpGet]
        public ActionResult SignIn()
        {
            return View(ModelMapper.MapForCreate<SignInForm>());
        }

        [HttpPost]
        public ActionResult SignIn(SignInForm model)
        {

            if (ModelMapper.Validate(model, ModelState))
            {
                var user = SystemRepository.GetSystemUserByCredentials(model.Login, model.Password);

                if (user == null)
                {
                    ModelState.AddModelError("", "Sorry, that User Name and Password combination is incorrect");
                }
                else if (user.IsActive == false)
                {
                    ModelState.AddModelError("", "Account has been disabled");
                }
                else if (user != null)
                {
                    Authentication.SignInSystemUser(user.Guid);

                    if (user.Role == Domain.Enumerations.SystemUserRole.Prospector)
                    {
                        return RedirectToAction("Index", "SystemLead", new { area = "Administration" });
                    }

                    return RedirectToAction("Index", "Account", new { area="Administration" });
                }

            }

            return View(model);
        }


        [HttpGet]
        public ActionResult SignOut()
        {
            Authentication.SignOutCurrentSystemUser();
            return RedirectToAction("SignIn");
        }
    }
}
