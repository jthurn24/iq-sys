using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IQI.Intuition.Domain.Utilities;
using IQI.Intuition.Infrastructure.Services;
using IQI.Intuition.Domain.Repositories;
using RedArrow.Framework.Logging;

namespace IQI.Intuition.Web.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class SupportsTokenAuthenticationAttribute : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            base.HandleUnauthorizedRequest(filterContext);
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var logger = DependencyResolver.Current.GetService<ILog>();


            if (httpContext.Request[AuthenticationRequestToken.AUTHENTICATION_TOKEN_REQUEST_VAR] == null)
            {
                logger.Info("No token found");
                return base.AuthorizeCore(httpContext);
            }

            logger.Info(string.Concat("Token auth found: ", httpContext.Request[AuthenticationRequestToken.AUTHENTICATION_TOKEN_REQUEST_VAR]));

            AuthenticationRequestToken token;

            try
            {
                token = new AuthenticationRequestToken(httpContext.Request[AuthenticationRequestToken.AUTHENTICATION_TOKEN_REQUEST_VAR]);
            }
            catch (Exception ex)
            {
                logger.Error(ex);

                return base.AuthorizeCore(httpContext);
            }

            var actionContext = DependencyResolver.Current.GetService<IActionContext>();
            var authentication = DependencyResolver.Current.GetService<IAuthentication>();
            var systemRepository = DependencyResolver.Current.GetService<ISystemRepository>();

            logger.Info(string.Concat("Token user: ", token.Login));

            var user = systemRepository.GetUserByCredentials(actionContext.CurrentAccount, token.Login);


            if (user == null || user.PasswordHash != token.PasswordHash)
            {
                logger.Info(string.Concat("Invalid: ", token.Login, " ", token.PasswordHash));
                return base.AuthorizeCore(httpContext);
            }

            if (httpContext.Request.IsAuthenticated == false)
            {
                authentication.SignInUser(user.Guid);
                //httpContext.Response.Redirect(httpContext.Request.Url.AbsoluteUri);
            }

            return true;
        }
    }
}