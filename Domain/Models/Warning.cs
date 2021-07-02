using System;
using System.Collections.Generic;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.ObjectModel.AuditTracking;
using RedArrow.Framework.Utilities;
using SnyderIS.sCore.Persistence.Protection;

namespace IQI.Intuition.Domain.Models
{
    public class Warning : AuditTrackingEntity<Warning>, IRestrictable<Account>
    {
         
        public virtual Patient Patient { get; set; }

        public virtual Facility Facility { get; set; }

        public virtual Account Account { get; set; }

        public virtual WarningRule WarningRule { get; set; }

        public virtual string Title { get; set; }

        public virtual DateTime TriggeredOn { get; set; }

        // Ending the propety name with "Text" is a convention that indicates an unlimited sized string value
        public virtual string DescriptionText { get; set; }

        public virtual string HiddenBy { get; set; }

        public virtual WarningTarget GetTarget()
        {
            if (this.Patient != null)
            {
                return WarningTarget.Patient;
            }

            if (this.Facility != null)
            {
                return WarningTarget.Facility;
            }

            return WarningTarget.Unknown;
        }

        public virtual bool IsHiddenBy(int accountUserId)
        {
            if (this.HiddenBy == null || this.HiddenBy == string.Empty)
            {
                return false;
            }

            var ids = this.HiddenBy.Split(',');

            if(ids.Contains(accountUserId.ToString()))
            {
                return true;
            }

            return false;

        }

        public virtual void AddHiddenBy(int accountUserId)
        {
            IList<string> list;

            if (this.HiddenBy != null && this.HiddenBy != string.Empty)
            {
                list = this.HiddenBy.Split(',').ToList();
            }
            else
            {
                list = new List<string>();
            }

            if (list.Contains(accountUserId.ToString()) == false)
            {
                list.Add(accountUserId.ToString());
            }

            this.HiddenBy = list.ToDelimitedString(',');
        }


        public virtual bool CanBeAccessedBy(Account source)
        {
            return source.Id == this.Account.Id;
        }


    }
}
