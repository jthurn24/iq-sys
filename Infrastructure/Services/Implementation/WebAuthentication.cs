using System;
using System.Web;
using System.Web.Security;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.ObjectModel.AuditTracking;
using System.Linq;

namespace IQI.Intuition.Infrastructure.Services.Implementation
{
    public class WebAuthentication : IAuthentication
    {
        public WebAuthentication(
            HttpContextBase httpContext)
        {
            this.HttpContext = httpContext.ThrowIfNullArgument("httpContext");
        }

        protected virtual HttpContextBase HttpContext { get; private set; }

        public virtual bool CurrentUserIsAuthenticated
        {
            get
            {
                try
                {
                    if (this.HttpContext == null || this.HttpContext.Request == null)
                    {
                        return false;
                    }

                    return this.HttpContext.Request.IsAuthenticated;
                }
                catch (HttpException)
                {
                    // Ignore exception caused by attempting to access the HttpRequest in Application_Start
                }

                return false;
            }
        }

        public virtual Guid? CurrentUserGuid
        {
            get
            {
                if (!CurrentUserIsAuthenticated)
                {
                    // Current Web request has no user specified
                    return null;
                }

                return new Guid(this.HttpContext.User.Identity.Name);
            }
        }

        // No easy way to test this method since it uses a static reference
        public virtual void SignInUser(Guid userGuid)
        {
            FormsAuthentication.SetAuthCookie(userGuid.ToString(), false);
            var cookie = FormsAuthentication.GetAuthCookie(userGuid.ToString(), false);
            HttpContext.Request.Cookies.Add(cookie);
        }

        // No easy way to test this method since it uses a static reference
        public virtual void SignOutCurrentUser()
        {
            FormsAuthentication.SignOut();
        }

        string IUserNameProvider.CurrentUserName
        {
            get
            {
                return CurrentUserGuid.ToStringSafely("Unknown User");
            }
        }

        public Guid? CurrentSystemUserGuid
        {
            get {

                if (this.HttpContext.Request.Cookies.AllKeys.Contains("IQI.Administration.User.Guid") == false)
                {
                    return null;
                }

                return new Guid(this.HttpContext.Request.Cookies["IQI.Administration.User.Guid"].Value);
            }
        }

        public void SignInSystemUser(Guid userGuid)
        {
            this.HttpContext.Response.Cookies.Add(new HttpCookie("IQI.Administration.User.Guid", userGuid.ToString()) { Domain = FormsAuthentication.CookieDomain });
        }

        public void SignOutCurrentSystemUser()
        {
            this.HttpContext.Response.Cookies.Add(new HttpCookie("IQI.Administration.User.Guid", null) { Expires = DateTime.Today.AddDays(-10), Domain = FormsAuthentication.CookieDomain });
        }

    }
}