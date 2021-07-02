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
using System.Text.RegularExpressions;

namespace IQI.Intuition.Infrastructure.Services.Utilities
{
    public class FacilityManagement : IConsoleService
    {
        private ILog _Log;
        private IStatelessDataContext _DataContext;
        private IUserRepository _UserRepository;

        public FacilityManagement(
            IStatelessDataContext dataContext,
            ILog log,
            IUserRepository userRepository
            )
        {
            _DataContext = dataContext;
            _Log = log;
            _UserRepository = userRepository;
        }

        public void Run(string[] args)
        {
            bool running = true;

            Console.WriteLine("********* Interactive facility management *********");

            while(running)
            {
                string command = Console.ReadLine();

                try
                {
                    if (command == "exit")
                    {
                        running = false;
                        _DataContext.Close();
                    }
                    else
                    {
                        RegexOptions options = RegexOptions.None;
                        Regex regex = new Regex(@"[ ]{2,}", options);
                        string tempo = regex.Replace(command, @" ");
                        string[] subArgs = tempo.Split(" ".ToCharArray());

                        string typeName = string.Concat("IQI.Intuition.Infrastructure.Services.Utilities.FacilityManagementCommands.", subArgs[0]);
                        Type type = Type.GetType(typeName, true, true);

                        var instance = (FacilityManagementCommands.ICommand)Activator.CreateInstance(type);
                        instance.Run(subArgs,_DataContext, _UserRepository);

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.Write(ex.StackTrace);
                    Console.Write(Environment.NewLine);
                }
                   
            }

        }


    }
}
