using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQI.Intuition.Reporting.Repositories;
using IQI.Intuition.Domain.Repositories;
using RedArrow.Framework.Persistence;
using RedArrow.Framework.Logging;
using SnyderIS.sCore.Persistence;
using IQI.Intuition.Reporting.Models.User;

namespace IQI.Intuition.Infrastructure.Services.Exporting
{
    public class ExportThread
    {
        private IUserRepository _UserRepository;
        private ILog _Log;
        private ExportRequest _Request;

        public ExportThread(
            IUserRepository userRepository,
            ILog log,
            ExportRequest request
            )
        {
            _UserRepository = userRepository;
            _Log = log;
            _Request = request;
        }

        public void Run()
        {
            try
            {
                var exporter = new PdfExporter(_Request, _Log, _UserRepository);
                exporter.Export();

                if (_Request.EmailTo != null && _Request.EmailTo != string.Empty)
                {
                    var emailExporter = new EmailExporter(_Request, _Log);
                    emailExporter.Export();
                }
            }
            catch (Exception ex)
            {
                _Request.Status = ExportRequest.ExportRequestStatus.Error;
                _UserRepository.Update(_Request);
                _Log.Error(ex);
                return;
            }
        }
    }
}
