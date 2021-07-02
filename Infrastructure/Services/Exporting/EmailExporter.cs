using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQI.Intuition.Reporting.Models.User;
using System.Net;
using System.Net.Mail;
using RedArrow.Framework.Logging;
using SendGrid;

namespace IQI.Intuition.Infrastructure.Services.Exporting
{
    public class EmailExporter
    {
        private ExportRequest _Request;
        private ILog _Log;

        public EmailExporter(
            ExportRequest request,
            ILog log)
        {
            _Request = request;
            _Log = log;
        }



        public void Export()
        {


            var message = new SendGridMessage(
                new MailAddress("no-reply@iqisystems.com"),
                new MailAddress[]  { new MailAddress(_Request.EmailTo) }, 
                 "New Document",
                string.Empty,
                "See the attached document");

            var dataStream = new System.IO.MemoryStream(_Request.OutputFile);
            message.AddAttachment(dataStream,"Report.pdf");

            var credentials = new NetworkCredential("azure_2c3052349c44a4f8f3d33d491e08d381@azure.com", "HPaslG8Z4e8l86h");
            var transportWeb = new Web(credentials);
            transportWeb.DeliverAsync(message).Wait();

            _Log.Info("Export Email Complete");
        }

    }
}
