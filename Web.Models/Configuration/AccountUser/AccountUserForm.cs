using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IQI.Intuition.Web.Models.Configuration.AccountUser
{
    public class AccountUserForm
    {
        public int? Id { get; set; }
        public string Login { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; }
        public string CellPhone { get; set; }
        public IList<int> SelectedFacilities { get; set;}
        public IList<int> SelectedPermissions { get; set; }
        public IList<int> SelectedWarningRules { get; set; }
        public Domain.Enumerations.NotificationMethod NotificationMethod { get; set; }
    }
}
