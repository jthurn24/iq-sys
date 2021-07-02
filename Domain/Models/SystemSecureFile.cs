using System;
using System.Collections.Generic;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.ObjectModel;
using RedArrow.Framework.Utilities;
using SnyderIS.sCore.Encryption;

namespace IQI.Intuition.Domain.Models
{
    public class SystemSecureFile : Entity<SystemSecureFile>
    {
        public virtual string Description { get; set; }
        public virtual byte[] FileData { get; set; }
        public virtual DateTime? ExpiresOn { get; set; }
        public virtual DateTime? CreatedOn { get; set; }
        public virtual Account Account { get; set; }
        public virtual SystemLead Lead { get; set; }
        public virtual SystemTicket Ticket { get; set; }
        public virtual string FileExtension { get; set; }

        public virtual void SetFile(byte[] data)
        {
            var eHelper = new DESHelper(Constants.SYSTEM_KEY_DES);
            this.FileData = eHelper.Encrypt(data);
        }

        public virtual byte[] GetFile()
        {
            var eHelper = new DESHelper(Constants.SYSTEM_KEY_DES);
            return eHelper.Decrypt(this.FileData);
        }

    }
}
