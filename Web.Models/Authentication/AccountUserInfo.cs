using System;

namespace IQI.Intuition.Web.Models.Authentication
{
    public class AccountUserInfo
    {
        public Guid Guid { get; set; }

        public Guid AccountGuid { get; set; }

        public string Login { get; set; }

        public string Name { get; set; }

        public string EmailAddress { get; set; }

        public bool IsActive { get; set; }
    }
}