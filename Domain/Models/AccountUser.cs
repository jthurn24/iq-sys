using System;
using System.Collections.Generic;
using System.Linq;
using IQI.Intuition.Domain.Utilities;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.ObjectModel.AuditTracking;
using RedArrow.Framework.Utilities;
using SnyderIS.sCore.Encryption;

namespace IQI.Intuition.Domain.Models
{
    public class AccountUser : AuditTrackingEntity<AccountUser>
    {
        protected AccountUser() { }

        public AccountUser(Account account)
        {
            this.Account = account.ThrowIfNullArgument("account");
            _Facilities = new List<Facility>();
            _Permissions = new List<Permission>();
            this.Guid = GuidHelper.NewGuid();
        }

        private static readonly int PasswordSaltSeedLength = 8;

        public virtual Guid Guid { get; protected set; }

        public virtual Account Account { get; protected set; }

        public virtual string Login { get;  set; }

        public virtual string FirstName { get; set; }

        public virtual string LastName { get; set; }

        public virtual string EmailAddress { get; set; }

        public virtual string PasswordHash { get;  set; }

        public virtual string PasswordSalt { get;  set; }

        public virtual bool IsActive { get; set; }

        public virtual DateTime? AgreementSignedOn { get; set; }

        public virtual bool ResetPassword { get; set; }

        public virtual bool SystemUser { get; set; }

        public virtual string CellPhone { get; set; }

        public virtual string LastIpAddress { get; set; }

        private IList<Facility> _Facilities;
        public virtual IEnumerable<Facility> Facilities
        {
            get
            {
                return _Facilities;
            }
        }

        private IList<Permission> _Permissions;
        public virtual IEnumerable<Permission> Permissions
        {
            get
            {
                return _Permissions;
            }
        }

        public virtual void AssignFacilities(params Facility[] values)
        {
            UpdateList(_Facilities, values);
        }

        public virtual void AssignPermissions(params Permission[] values)
        {
            UpdateList(_Permissions, values);
        }



        public virtual IEnumerable<WarningRuleNotification> Notifications { get; set; }
        
        public virtual DateTime? MostRecentSignInAt { get; set; }

        public virtual DateTime? PreviousSignInAt { get; set; }

        public virtual Enumerations.NotificationMethod NotificationMethod { get; set; }

        public virtual void ChangePassword(string password)
        {
            password.ThrowIfNullOrWhitespaceArgument("password");
            var helper = GetCryptographyHelper();
            this.PasswordSalt = helper.GenerateSaltString(PasswordSaltSeedLength);
            this.PasswordHash = helper.GenerateHashString(password, this.PasswordSalt);
        }

        public virtual bool CheckPassword(string password)
        {
            password.ThrowIfNullOrWhitespaceArgument("password");
            var passwordHelper = GetCryptographyHelper();
            string passwordHash = passwordHelper.GenerateHashString(password, this.PasswordSalt);

            return this.PasswordHash == passwordHash;
        }

        public virtual void RecordSignIn()
        {
            this.PreviousSignInAt = this.MostRecentSignInAt ?? DateTime.Now;
            this.MostRecentSignInAt = DateTime.Now;
        }

        public virtual bool HasPermission(Enumerations.KnownPermision permission)
        {
            if (this.SystemUser)
            {
                return true;
            }

            if (this.Permissions.Where(x => x.Id == (int)permission).Count() > 0)
            {
                return true;
            }

            return false;
        }

        protected virtual DESHelper GetCryptographyHelper()
        {
            return new DESHelper(Constants.SYSTEM_KEY_DES);
        }

        private void UpdateList<T>(IList<T> list, T[] values)
        {
            values.ThrowIfNullArgument("values");
            list.Remove(item => !values.Contains(item));
            list.AddRange(values.Except(list));
        }
    }
}