using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedArrow.Framework.ObjectModel;
using SnyderIS.sCore.Encryption;

namespace IQI.Intuition.Domain.Models
{
    public class SignUpRequest : Entity<SignUpRequest>
    {
        public virtual string Name { get; set; }
        public virtual string State { get; set; }
        public virtual int MaxBeds { get; set; }
        public virtual string SubDomain { get; set; }
        public virtual string Login { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string EmailAddress { get; set; }
        public virtual string PasswordHash { get; set; }
        public virtual string PasswordSalt { get; set; }
        public virtual string IpAddress { get; set; }
        public virtual string Code { get; set; }
        public virtual bool Approved { get; set; }

        private static readonly int PasswordSaltSeedLength = 8;

        public virtual void SetPassword(string password)
        {
            var helper = GetCryptographyHelper();
            this.PasswordSalt = helper.GenerateSaltString(PasswordSaltSeedLength);
            this.PasswordHash = helper.GenerateHashString(password, this.PasswordSalt);
        }

        protected virtual DESHelper GetCryptographyHelper()
        {
            return new DESHelper(Constants.SYSTEM_KEY_DES);
        }
    }
}
