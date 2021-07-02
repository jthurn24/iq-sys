using System;
using System.Collections.Generic;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.ObjectModel.AuditTracking;
using RedArrow.Framework.Utilities;

namespace IQI.Intuition.Domain.Models
{
    public class Facility : AuditTrackingEntity<Facility>
    {
        protected Facility() { }

        public Facility(Account account)
        {
            this.Account = account.ThrowIfNullArgument("account");
            this.Guid = GuidHelper.NewGuid();

            // Initialize collections
            _Floors = new List<Floor>();
            _Warnings = new List<Warning>();
        }

        public virtual Guid Guid { get; protected set; }

        public virtual Account Account { get; protected set; }

        public virtual string Name { get; set; }

        public virtual string SubDomain { get; set; }

        public virtual string State { get; set; }

        public virtual Enumerations.FacilityType FacilityType { get; set; }

        public virtual int? MaxBeds { get; set; }

        public virtual DateTime? LastSynchronizedAt { get; set; }

        public virtual InfectionDefinition InfectionDefinition { get; set; }

        public virtual bool? IsBetaSite { get; set; }

        public virtual bool? HasSingleFloorMap { get; set; }

        public virtual long? FinancialId { get; set; }

        public virtual bool? InActive { get; set; }

        private IList<Floor> _Floors;
        public virtual IEnumerable<Floor> Floors
        {
            get
            {
                return _Floors;
            }
        }

        public virtual void AddFloor(Floor floor)
        {
            if (floor.ThrowIfNullArgument("floor").Facility != this)
            {
                throw new ArgumentException(@"The floor being added does not belong to this facility");
            }

            if (_Floors.Any(existingFloor => existingFloor.Name == floor.Name))
            {
                throw new ArgumentException(@"A floor with the name '{0}' has already been added to this facility"
                    .FormatWith(floor.Name));
            }

            _Floors.Add(floor);
        }

        public virtual void RemoveFloor(Floor floor)
        {
            _Floors.Remove(floor.ThrowIfNullArgument("floor"));
        }

        private IList<Warning> _Warnings;
        public virtual IEnumerable<Warning> Warnings
        {
            get
            {
                return _Warnings;
            }
        }

        private IList<WarningRule> _WarningRules;
        public virtual IEnumerable<WarningRule> WarningRules
        {
            get
            {
                return _WarningRules;
            }
        }

        public virtual IList<FacilityProduct> FacilityProducts { get; set; }

        public virtual IList<Employee> Employees { get; set; }

        public virtual IEnumerable<AccountUser> AccountUsers { get; set; }

        public virtual Patient FindPatient(Guid id)
        {
            return Floors
                .SelectMany(floor => floor.Wings)
                .SelectMany(wing => wing.Rooms)
                .SelectMany(room => room.Patients)
                .FirstOrDefault(patient => patient.Guid == id);
        }

        public virtual Patient FindPatient(int id)
        {
            return Floors
                .SelectMany(floor => floor.Wings)
                .SelectMany(wing => wing.Rooms)
                .SelectMany(room => room.Patients)
                .FirstOrDefault(patient => patient.Id == id);
        }

        public virtual bool HasProduct(Enumerations.KnownProductType product)
        {
            return FacilityProducts.Where(x => x.SystemProduct.Id == (int)product).Count() > 0;
        }

        public virtual bool FreeMode()
        {
            return FacilityProducts.Where(x => x.Fee > 0).Count() == 0;
        }
    }
}
