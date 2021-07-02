using System;
using System.Collections.Generic;
using System.Linq;
using IQI.Intuition.Domain.Utilities;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.ObjectModel;
using RedArrow.Framework.Utilities;
using SnyderIS.sCore.Encryption;


namespace IQI.Intuition.Domain.Models
{
    public class SystemUser : Entity<SystemUser>
    {
        public SystemUser() { }

        private static readonly int PasswordSaltSeedLength = 8;

        public virtual Guid Guid { get; set; }

        public virtual string Login { get;  set; }

        public virtual string FirstName { get; set; }

        public virtual string LastName { get; set; }

        public virtual string EmailAddress { get; set; }

        public virtual string PasswordHash { get; protected set; }

        public virtual string PasswordSalt { get; protected set; }

        public virtual bool IsActive { get; set; }

        public virtual Domain.Enumerations.SystemUserRole Role { get; set; }

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
