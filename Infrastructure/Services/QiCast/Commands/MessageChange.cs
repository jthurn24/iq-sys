using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IQI.Intuition.Infrastructure.Services.QiCast.Commands
{
    public class MessageChange
    {
        private string _Message;

        public MessageChange(string message)
        {
            _Message = message;
        }

        public override string ToString()
        {
            if (_Message == null || _Message == string.Empty)
            {
                return "sCoreExi.sendKioskCommand('MessageChange')";
            }

            return string.Format("sCoreExi.sendKioskCommand('MessageChange <span color=\"yellow\" bgcolor=\"black\">{0}</span>')", _Message.Replace("'", "\""));
        }
    }
}
