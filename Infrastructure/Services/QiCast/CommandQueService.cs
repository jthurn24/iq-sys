using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQI.Intuition.Reporting.Models.User;
using IQI.Intuition.Reporting.Repositories;


namespace IQI.Intuition.Infrastructure.Services.QiCast
{
    public class CommandQueService
    {
        private IUserRepository UserRepository;

        public CommandQueService(IUserRepository userRepository)
        {
            UserRepository = userRepository;
        }

        public IEnumerable<QICastCommand> GetCommands(bool initialize, Guid qiCastId)
        {
            var commands = UserRepository.GetCommands(qiCastId).ToList();

            /* flush */
            foreach (var command in commands)
            {
                UserRepository.Delete(command);
            }

            if (initialize)
            {
                /* If this is the first call flush and initialize*/

                commands.Clear();

                var data = UserRepository.GetQICastData(qiCastId);

                if (data.Inactive != true)
                {
                    commands.Add(
                    CreateCommand(new Commands.MessageChange(data.Message), qiCastId)
                    );

                    commands.Add(
                    CreateCommand(new Commands.ShowActiveSection(), qiCastId)
                    );
                   

                }
                else
                {

                    commands.Add(
                    CreateCommand(new Commands.MessageChange(data.Message), qiCastId)
                    );

                    commands.Add(
                    CreateCommand(new Commands.HideActiveSection(), qiCastId)
                    );
                }

            }

            return commands;
        }

        public void SendCommand(object obj, Guid qiCastId)
        {
            UserRepository.Add(CreateCommand(obj, qiCastId));
        }

        private QICastCommand CreateCommand(object obj, Guid qiCastId)
        {
            return new QICastCommand()
            {
                Command = obj.ToString(),
                QiCastId = qiCastId,
                Id = RedArrow.Framework.Utilities.GuidHelper.NewGuid()
            };
        }
    }
}
