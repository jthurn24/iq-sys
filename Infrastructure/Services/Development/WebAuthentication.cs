using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Web;
using RedArrow.Framework.Extensions.Common;

namespace IQI.Intuition.Infrastructure.Services.Development
{
    // This class should only used in the development environment
    public class WebAuthentication : Implementation.WebAuthentication
    {
        public WebAuthentication(HttpContextBase httpContext)
            : base(httpContext)
        {
            TrySignIn();
        }

        static WebAuthentication()
        {
            AutoSignInAsUser = ConfigurationManager.AppSettings["AutoSignInAsUser"];
        }

        private static readonly string AutoSignInAsUser;

        [Conditional("DEBUG")]
        private void TrySignIn()
        {
            try
            {
                if (AutoSignInAsUser.IsNullOrWhiteSpace()
                    || base.HttpContext.Request == null
                    || base.CurrentUserIsAuthenticated)
                {
                    // Do not auto sign in user if no default was specified, there
                    //   is no active Web request or user is already signed in
                    return;
                }

                base.SignInUser(new Guid(AutoSignInAsUser));
            }
            catch (HttpException)
            {
                // Ignore exception caused by attempting to access the HttpRequest in Application_Start
            }
        }
    }
}