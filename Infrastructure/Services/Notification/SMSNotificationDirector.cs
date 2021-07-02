using System;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQI.Intuition.Reporting.Repositories;
using IQI.Intuition.Domain.Repositories;
using RedArrow.Framework.Persistence;
using RedArrow.Framework.Logging;
using SnyderIS.sCore.Persistence;
using System.Net;
using System.Net.Mail;

namespace IQI.Intuition.Infrastructure.Services.Notification
{
    public class SMSNotificationDirector : IConsoleService
    {
        private IUserRepository _UserRepository;
        private ILog _Log;
        private IStatelessDataContext _DataContext;


        public SMSNotificationDirector(
            IUserRepository userRepository,
            IStatelessDataContext dataContext,
            ILog log
            )
        {
            _UserRepository = userRepository;
            _Log = log;
            _DataContext = dataContext;
        }

        public void Run(string[] args)
        {
            var cstZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
            var on = TimeZoneInfo.ConvertTime(DateTimeOffset.Now, cstZone);


            foreach (var notification in _DataContext.CreateQuery<Domain.Models.SystemSMSNotification>().FetchAll())
            {

                if ((notification.AllowAfterHours == true) || (on.Hour > 8 && on.Hour < 18))
                {
                    Export(notification);
                    _DataContext.Delete(notification);
                }
            }

        }

        public void Export(Domain.Models.SystemSMSNotification notification)
        {
            _Log.Info("Sending SMS notification {0}", notification.Id);

            string AccountSid = "AC6e602e62f1e8a99cab45618f168bac10";
            string AuthToken = "bf7c0c356b5b09ab3e0c315b5cb51138"; 

            var twilio = new Twilio.TwilioRestClient(AccountSid, AuthToken);

            var message = twilio.SendMessage("+14144090716", notification.SendTo, notification.Message, "");

        }
    }
}
