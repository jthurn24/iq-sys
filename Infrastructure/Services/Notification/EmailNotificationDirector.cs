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
    public class EmailNotificationDirector : IConsoleService
    {
        private IUserRepository _UserRepository;
        private ILog _Log;
        private IStatelessDataContext _DataContext;


        public EmailNotificationDirector(
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

            foreach (var notification in _DataContext.CreateQuery<Domain.Models.SystemEmailNotification>().FetchAll())
            {

                Export(notification);
                _DataContext.Delete(notification);
            }

        }

        public void Export(Domain.Models.SystemEmailNotification notification)
        {

            SendGrid.SendGridMessage message; 


            if (notification.IsHtml.HasValue && notification.IsHtml.Value == true)
            {
                message = new SendGrid.SendGridMessage(
                new MailAddress("no-reply@iqisystems.com"),
                new MailAddress[] {new MailAddress(notification.SendTo)}, 
                notification.Subject,
                notification.MessageText, string.Empty);
            }
            else{
                message = new SendGrid.SendGridMessage(
                new MailAddress("no-reply@iqisystems.com"),
                new MailAddress[] { new MailAddress(notification.SendTo) },
                notification.Subject,
                string.Empty,notification.MessageText);
            }

            var credentials = new NetworkCredential("azure_2c3052349c44a4f8f3d33d491e08d381@azure.com", "HPaslG8Z4e8l86h");
            var transportWeb = new SendGrid.Web(credentials);
            transportWeb.DeliverAsync(message).Wait();
        }
    }
}
