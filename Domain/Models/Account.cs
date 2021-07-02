using System;
using System.Collections.Generic;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.ObjectModel.AuditTracking;
using RedArrow.Framework.Utilities;

namespace IQI.Intuition.Domain.Models
{
    public class Account : AuditTrackingEntity<Account>
    {
        protected Account() { }

        public Account(string name)
        {
            Name = name;
            Guid = GuidHelper.NewGuid();

            // Initialize collections
            _Users = new List<AccountUser>();
            _Facilities = new List<Facility>();
        }

        public virtual Guid Guid { get; protected set; }

        public virtual string Name { get; protected set; }

        public virtual bool? InActive { get; set; }

        public virtual long? FinancialId { get; set; }

        public virtual bool? FacilityInvoicing { get; set; }

        private IList<AccountUser> _Users;
        public virtual IEnumerable<AccountUser> Users
        {
            get
            {
                return _Users;
            }
        }

        public virtual void AddUser(AccountUser user)
        {
            if (user.ThrowIfNullArgument("user").Account != this)
            {
                throw new ArgumentException(@"The user being added does not belong to this account");
            }

            if (_Users.Any(existingUser => existingUser.Login == user.Login))
            {
                throw new ArgumentException(@"A user with the login '{0}' has already been added to this account"
                    .FormatWith(user.Login));
            }

            _Users.Add(user);
        }

        public virtual void RemoveUser(AccountUser user)
        {
            _Users.Remove(user.ThrowIfNullArgument("user"));
        }

        private IList<Facility> _Facilities;
        public virtual IEnumerable<Facility> Facilities
        {
            get
            {
                return _Facilities;
            }
        }

        public virtual void AddFacility(Facility facility)
        {
            if (facility.ThrowIfNullArgument("facility").Account != this)
            {
                throw new ArgumentException(@"The facility being added does not belong to this account");
            }

            if (_Facilities.Any(existingFacility => existingFacility.Name == facility.Name))
            {
                throw new ArgumentException(@"A facility with the name '{0}' has already been added to this account"
                    .FormatWith(facility.Name));
            }

            _Facilities.Add(facility);
        }

        public virtual void RemoveFacility(Facility facility)
        {
            _Facilities.Remove(facility.ThrowIfNullArgument("facility"));
        }

    }
}