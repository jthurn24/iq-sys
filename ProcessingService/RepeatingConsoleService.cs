using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using StructureMap;
using RedArrow.Framework.Persistence;
using RedArrow.Framework.Logging;
using IQI.Intuition.Infrastructure.Services;

namespace IQI.Intuition.ProcessingService
{

    public class RepeatingConsoleService<T> : ServiceBase where T : IConsoleService
    {
        protected StructureMap.IContainer _Container;
        private bool _RunControl = false;
        private bool _Running = false;
        private System.Threading.Thread _MainThread;
        private int _SleepRate;
        private ILog _Log;
        private string[] _Args;
        private string _Environment = System.Configuration.ConfigurationSettings.AppSettings["ServiceSuffix"];
        private DateTime _LastHeartBeat = DateTime.Now;

        public RepeatingConsoleService(
            string[] args,
            int sleepRate, 
            StructureMap.IContainer container,
            ILog log)
        {
            this.ServiceName = string.Concat("IQI - ", typeof(T).Name, " ", _Environment);
            _Container = container;
            _SleepRate = sleepRate;
            _Log = log;
            _Args = args;
        }

        public RepeatingConsoleService(
        StructureMap.IContainer container,
        ILog log)
        {
            string[] args = {};

            this.ServiceName = string.Concat("IQI - ", typeof(T).Name, " ", _Environment);
            _Container = container;
            _SleepRate = 1000;
            _Log = log;
            _Args = args;

            
        }


        protected override void OnStart(string[] args)
        {
            _Log.Info("Starting Service: {0}", this.ServiceName);
            _RunControl = true;

            if (_MainThread == null)
            {
                var threadStart = new System.Threading.ThreadStart(Run);
                _MainThread = new System.Threading.Thread(threadStart);
                _MainThread.Start();
            }
        }

        protected void Run()
        {
            this._Running = true;

            _Log.Info(" Service: {0} is now running", this.ServiceName);

            while (this._RunControl)
            {
                if (DateTime.Now.Subtract(_LastHeartBeat).TotalMinutes > 3)
                {
                    _Log.Info(string.Concat(this.ServiceName, " HeartBeat"));
                    _LastHeartBeat = DateTime.Now;
                }

                try
                {

                    var service = _Container.GetInstance<T>();
                    service.Run(_Args);

                    System.Threading.Thread.Sleep(_SleepRate);
                }
                catch (Exception ex)
                {
                    _Log.Error(ex);
                }
            }

            this._Running = false;
        }

        protected override void OnStop()
        {
            _Log.Info("Stopping Service: {0}", this.ServiceName);
            this._RunControl = false;

            while (this._Running)
            {
                System.Threading.Thread.Sleep(300);
            }

            _MainThread = null;

            _Log.Info("Service Shutdown: {0}", this.ServiceName);
        }

    }
}
