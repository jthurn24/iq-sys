using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RedArrow.Framework.ObjectModel.AuditTracking;

namespace IQI.Intuition.ProcessingService
{
    public class UserNameProvider : IUserNameProvider
    {
        public string CurrentUserName
        {
            get
            {
                return "System Service";
            }
        }
    }
}
