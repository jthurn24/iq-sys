using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQI.Intuition.Reporting.Repositories;
using IQI.Intuition.Domain.Repositories;
using RedArrow.Framework.Persistence;
using RedArrow.Framework.Logging;
using IQI.Intuition.Domain.Models;
using SnyderIS.sCore.Persistence;
using RedArrow.Framework.Extensions.Common;

namespace IQI.Intuition.Infrastructure.Services.Utilities
{
    public class AuditEmails : IConsoleService
    {
        private ILog _Log;
        private IStatelessDataContext _DataContext;

        public AuditEmails(
            IStatelessDataContext dataContext,
            ILog log
            )
        {
            _DataContext = dataContext;
            _Log = log;
        }

        public void Run(string[] args)
        {
            var p = _DataContext.Fetch<Permission>((int)Domain.Enumerations.KnownPermision.ManageUsers);

            var users = _DataContext.CreateQuery<AccountUser>()
                .FilterBy(x => x.Permissions.Contains(p))
                 .FilterBy(x => x.SystemUser == false)
                 .FilterBy(x => x.IsActive == true)
                .FetchAll();

            foreach (var u in users)
            {
                var facilities = _DataContext.CreateQuery<Facility>()
                    .FilterBy(x => x.AccountUsers.Contains(u))
                    .FetchAll();

                foreach (var facility in facilities)
                {
                    ProcessAccount(u, facility);
                }

            }
        }

        private void ProcessAccount(AccountUser u, Facility f)
        {
            var failedLogins = _DataContext.CreateQuery<AuditEntry>()
                .FilterBy(x => x.Facility.Id == f.Id)
                .FilterBy(x => x.AuditType == Domain.Enumerations.AuditEntryType.FailedLogin)
                .FilterBy(x => x.PerformedAt > DateTime.Today.AddDays(-30))
                .FetchAll();

            var inactiveUser = _DataContext.CreateQuery<AccountUser>()
                .FilterBy(x => x.Facilities.Contains(f))
                .FilterBy(x => x.MostRecentSignInAt.HasValue && x.MostRecentSignInAt < DateTime.Today.AddDays(-90))
                .FilterBy(x => x.SystemUser == false)
                .FetchAll();

            
            if (failedLogins.Count() > 0 || inactiveUser.Count() > 0)
            {


                var messageBuilder = new System.Text.StringBuilder();

                messageBuilder.Append(u.FirstName);
                messageBuilder.Append(" ");
                messageBuilder.Append(u.LastName);
                messageBuilder.Append(Environment.NewLine);
                messageBuilder.Append(Environment.NewLine);
                messageBuilder.Append("You are receiving this monthly report because your account is configured to manage users within IQI Intuition.  In order to ensure patient data remains safe and secure, IQI will provide a list of inactive users and failed login attempts to you on a monthly basis.");
                messageBuilder.Append(Environment.NewLine);
                messageBuilder.Append(Environment.NewLine);
                messageBuilder.Append("We recommend that users who are no longer require access to the system be flagged as inactive. If you see an unusually high number of failed logins for a user, it is recommended that  you contact that person to determine if this is a legitimate failed attempt.");
                messageBuilder.Append(Environment.NewLine);
                messageBuilder.Append(Environment.NewLine);
                messageBuilder.Append("If at any time you have questions or concerns regarding the security of your system, please do not hesitate to contact us at support@iqisystems.com.");
                messageBuilder.Append(Environment.NewLine);
                messageBuilder.Append(Environment.NewLine);


                if (failedLogins.Count() > 0)
                {
                    messageBuilder.Append("Failed logins in the last 30 days: ");
                    messageBuilder.Append(Environment.NewLine);

                    foreach (var login in failedLogins.Select(x => x.PerformedBy).Distinct())
                    {
                        messageBuilder.Append(login);
                        messageBuilder.Append(" : ");
                        messageBuilder.Append(failedLogins.Count(x => x.PerformedBy == login));
                        messageBuilder.Append(" failed logins ");
                        messageBuilder.Append(Environment.NewLine);
                    }

                }


                if (inactiveUser.Count() > 0)
                {
                    messageBuilder.Append("Inactive users 90 days: ");

                    foreach (var login in inactiveUser)
                    {
                        messageBuilder.Append(login.Login);
                        messageBuilder.Append(" : Last Login - ");
                        messageBuilder.Append(login.MostRecentSignInAt.Value.ToString("MM/dd/yyyy"));
                        messageBuilder.Append(Environment.NewLine);
                    }
                }


                messageBuilder.Append(Environment.NewLine);
                messageBuilder.Append(Environment.NewLine);
                messageBuilder.Append("This email has been sent to all users with permission to manage user accounts: ");


                messageBuilder.Append(Environment.NewLine);
                messageBuilder.Append(Environment.NewLine);


                var email = new Domain.Models.SystemEmailNotification();
                email.Subject = string.Concat("IQI monthly login audit report for ",f.Name);
                email.SendTo  ="mark@iqisystems.com";
                email.MessageText = messageBuilder.ToString();
                
                _DataContext.Insert(email);
                
                

            }




        }
    }
}
