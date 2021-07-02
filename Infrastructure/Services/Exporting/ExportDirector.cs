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
using System.Threading;

namespace IQI.Intuition.Infrastructure.Services.Exporting
{
    public class ExportDirector : IConsoleService
    {
        private IUserRepository _UserRepository;
        private ILog _Log;
        private IList<Thread> _Threads;

        public ExportDirector(
            IUserRepository userRepository,
            ILog log
            )
        {
            _UserRepository = userRepository;
            _Log = log;
            _Threads = new List<Thread>();
        }

        public void Run(string[] args)
        {

            foreach (var request in _UserRepository.GetPendingExport().Take(5))
            {
                request.Status = ExportRequest.ExportRequestStatus.Running;
                _UserRepository.Update(request);

                var eThread = new ExportThread(_UserRepository, _Log, request);
                var ts = new ThreadStart(eThread.Run);
                var thread = new Thread(ts);
                thread.Start();
                _Threads.Add(thread);
                _Log.Info(string.Format("Export Thread started for {0}", request.Id));
            }

            bool running = true;

            while(running)
            {
                var runningThreads = _Threads.Where(x => x.ThreadState == ThreadState.Running);

                if (runningThreads.Count() < 1)
                {
                    running = false;
                }

                Thread.Sleep(100);
            }

        }
    }
}
