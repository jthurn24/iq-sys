using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RedArrow.Framework.ObjectModel.AuditTracking;
using RedArrow.Framework.Extensions.Common;
using IQI.Intuition.Domain.Models;
using IQI.Intuition.Domain.Repositories;

namespace IQI.Intuition.Infrastructure.Services.Implementation
{
    public class ActionContext : IActionContext
    {
        private static string _BuildVersion = typeof(IQI.Intuition.Domain.Enumerations).Assembly.GetName().Version.ToString();
        private static string _EnvironmentName = System.Configuration.ConfigurationSettings.AppSettings["EnvironmentName"];

        public ActionContext(
            HttpContextBase httpContext,
            IAuthentication authentication,
            IFacilityRepository facilityRepository,
            ISystemRepository systemRepository)
        {
            this.HttpContext = httpContext.ThrowIfNullArgument("httpContext");
            this.Authentication = authentication.ThrowIfNullArgument("authentication");
            this.FacilityRepository = facilityRepository.ThrowIfNullArgument("facilityRepository");
            this.SystemRepository = systemRepository.ThrowIfNullArgument("systemRepository");
        }

        protected virtual HttpContextBase HttpContext { get; private set; }

        protected virtual IAuthentication Authentication { get; private set; }

        protected virtual IFacilityRepository FacilityRepository { get; private set; }

        protected virtual ISystemRepository SystemRepository { get; private set; }

        public Account CurrentAccount
        {
            get 
            {
                if (this.CurrentFacility == null)
                {
                    return null;
                }

                return this.CurrentFacility.Account;
            }
        }

        private bool AttemptedToGetCurrentFacility { get; set; }

        private Facility _CurrentFacility;
        public Facility CurrentFacility
        {
            get 
            {
                if (this.HttpContext.Request == null)
                {
                    return null;
                }

                if (!this.AttemptedToGetCurrentFacility)
                {
                    var hostNameSegments = this.HttpContext.Request.Url.Host.Split('.');

                    if (hostNameSegments.Length < 2)
                    {
                        // Throw exception if the URL does not have at least one subdomain
                        throw new InvalidOperationException();
                    }

                    string subDomain = hostNameSegments[0];
                    _CurrentFacility = this.FacilityRepository.Get(subDomain);
                    this.AttemptedToGetCurrentFacility = true;
                }

                return _CurrentFacility;
            }
        }

        private bool AttemptedToGetCurrentUser { get; set; }

        private IDictionary<String, Object> _MappingArguments;
        public IDictionary<String, Object> MappingArguments {
            get
            {
                if(_MappingArguments == null)
                {
                    _MappingArguments = new Dictionary<string, object>();
                }

                return _MappingArguments;
            }
        }

        private AccountUser _CurrentUser;
        public AccountUser CurrentUser
        {
            get 
            {
                if (this.CurrentAccount == null)
                {
                    return null;
                }

                if (!this.AttemptedToGetCurrentUser)
                {
                    _CurrentUser = this.CurrentAccount.Users
                        .SingleOrDefault(user =>
                            user.Guid == this.Authentication.CurrentUserGuid);
                    this.AttemptedToGetCurrentUser = true;
                }

                return _CurrentUser;
            }
        }

        public string BuildVersion
        {
            get
            {
                return _BuildVersion;
            }

        }

        public string ServerName
        {
            get
            {
                return this.HttpContext.Server.MachineName;
            }

        }

        public string EnvironmentName
        {
            get
            {
                return _EnvironmentName;
            }

        }



        public SystemUser CurrentSystemUser
        {
            get 
            {
                if (this.Authentication.CurrentSystemUserGuid.HasValue == false)
                {
                    return null;
                }

                return SystemRepository.GetSystemUserByGuid(this.Authentication.CurrentSystemUserGuid.Value);
            }
        }

        public void SetUserMessage(string message)
        {
            this.HttpContext.Session["ActionContext.UserMessage"] = message;
        }

        public void SendEmailNotification(string to, string subject, string message)
        {
            var notification = new SystemEmailNotification();
            notification.SendTo = to;
            notification.Subject = subject;
            notification.MessageText = message;
            SystemRepository.Add(notification);
        }

        public void SendEmailNotification(string subject, string message)
        {
            var notification = new SystemEmailNotification();
            notification.SendTo = Domain.Constants.SYSTEM_EMAIL;
            notification.Subject = subject;
            notification.MessageText = message;
            SystemRepository.Add(notification);
        }

        public string GetUserMessage()
        {
            if (this.HttpContext.Session["ActionContext.UserMessage"] == null)
            {
                return string.Empty;
            }
            return this.HttpContext.Session["ActionContext.UserMessage"].ToString();
        }

        public void ClearUserMessage()
        {
            this.HttpContext.Session["ActionContext.UserMessage"] = null;
        }


    }
}
