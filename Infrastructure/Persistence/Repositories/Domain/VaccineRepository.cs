using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Persistence;
using IQI.Intuition.Domain.Models;
using IQI.Intuition.Domain.Repositories;
using IQI.Intuition.Domain;

namespace IQI.Intuition.Infrastructure.Persistence.Repositories.Domain
{
    public class VaccineRepository : AbstractRepository<IDataContext>, IVaccineRepository
    {
        public VaccineRepository(IDataContext dataContext)
            : base(dataContext) { }

        public void Add(VaccineEntry vaccine)
        {
            DataContext.TrackChanges(vaccine);
        }

        public VaccineEntry Get(int id)
        {
            return DataContext.Fetch<VaccineEntry>(id);
        }

        public VaccineEntry Get(Guid guid)
        {
            return DataContext.CreateQuery<VaccineEntry>().FilterBy(x => x.Guid == guid).FetchAll().FirstOrDefault();
        }

        public void Delete(VaccineEntry value)
        {
            DataContext.Delete(value);
        }

        public IPagedQueryResult<VaccineEntry> Find(Facility facility, string patientName, string roomAndWingName, string vaccineType, DateTime? administeredOn, string refusalReason, Expression<Func<VaccineEntry, object>> sortByExpression, bool sortDescending, int page, int pageSize)
        {
            var query = DataContext.CreateQuery<VaccineEntry>()
               .FilterBy(i => i.Room.Wing.Floor.Facility == facility)
               .FilterBy(i => i.Deleted == null || i.Deleted == false);

            if (roomAndWingName.IsNotNullOrWhiteSpace())
            {
                query = query.FilterBy(i => (i.Room.Name + " " + i.Room.Wing.Name).Contains(roomAndWingName));
            }

            if (administeredOn != null)
            {
                query = query.FilterBy(vaccine => vaccine.AdministeredOn.Value == administeredOn);
            }

            var results = query.FetchAll();

            if (patientName.IsNotNullOrWhiteSpace())
            {
                results = results.Where(incident => (incident.Patient.GetFirstName() + " " + incident.Patient.GetLastName()).ToLower().Contains(patientName.ToLower()));
            }

            if (vaccineType.IsNotNullOrWhiteSpace())
            {
                query = query.FilterBy(vaccine => vaccineType.Contains((vaccine.VaccineType == null ? vaccine.VaccineType.FullVaccineName : string.Empty)));
            }

            if (sortDescending)
            {
                results = results.AsQueryable().OrderByDescending(sortByExpression);
            }
            else
            {
                results = results.AsQueryable().OrderBy(sortByExpression);
            }

            var pager = new RedArrow.Framework.Persistence.PagedQueryResult<VaccineEntry>();
            pager.PageSize = pageSize;
            pager.PageNumber = page;
            pager.TotalResults = results.Count();

            if (results.Count() > 1)
            {
                pager.TotalPages = (int)Math.Ceiling((double)pager.TotalResults / pager.PageSize);
            }
            else
            {
                pager.TotalPages = 0;
            }

            pager.PageValues = results.Skip((pager.PageNumber - 1) * pager.PageSize).Take(pager.PageSize);

            return pager;
        }

        public IPagedQueryResult<VaccineEntry> Find(Patient patient, Expression<Func<VaccineEntry, object>> sortByExpression, bool sortDescending, int page, int pageSize)
        {
            return DataContext.CreateQuery<VaccineEntry>()
            .FilterBy(i => i.Patient == patient)
            .FilterBy(i => i.Deleted == null || i.Deleted == false)
            .SortBy(sortByExpression)
            .DescendingWhen(sortDescending)
            .PageSize(pageSize)
            .FetchPage(page);
        }

        public IEnumerable<VaccineEntry> FindForLineListing(Facility facility, 
            int? wingId,
            int? vaccineType,
            int? refusalReason,
            DateTime? startDate, 
            DateTime? endDate)
        {
            var q = DataContext.CreateQuery<VaccineEntry>()
                .FilterBy(x => x.Room.Wing.Floor.Facility.Id == facility.Id)
                .FilterBy(vaccine => vaccine.Deleted == null || vaccine.Deleted == false);

            if (wingId.HasValue)
            {
                q = q.FilterBy(x => x.Room.Wing.Id == wingId.Value);
            }

            if (startDate.HasValue)
            {
                q = q.FilterBy(x => x.AdministeredOn >= startDate);
            }

            if (endDate.HasValue)
            {
                q = q.FilterBy(x => x.AdministeredOn <= endDate.Value);
            }

            if (vaccineType.HasValue)
            {
                q = q.FilterBy(x => x.VaccineType.Id == vaccineType.Value);
            }

            if (refusalReason.HasValue)
            {
                q = q.FilterBy(x => x.VaccineRefusalReason.Id == refusalReason.Value);
            }

            var results = q.FetchAll();
            return results.OrderByDescending(x => x.AdministeredOn);
        }

        public IEnumerable<VaccineEntry> FindCreatedIn(int month, int year)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            return DataContext.CreateQuery<VaccineEntry>()
                .FilterBy(x => x.CreatedAt >= startDate && x.CreatedAt <= endDate)
                .FetchAll();
        }

        public IEnumerable<VaccineEntry> FindVaccinations(Facility facility, DateTime startDate, DateTime endDate, Intuition.Domain.Enumerations.VaccineStatus status)
        {
            return null;
        }

        public IEnumerable<VaccineTradeName> AllVaccineTradeNamesForType(int type)
        {
            return DataContext.CreateQuery<VaccineTradeName>()
                .FetchAll().Where(x => x.VaccineType.Id == type);
        }

        public IEnumerable<VaccineAdministrativeSite> AllVaccineAdministrativeSites
        {
            get
            {
                return DataContext.CreateQuery<VaccineAdministrativeSite>()
                    .FetchAll();
            }
        }

        public IEnumerable<VaccineManufacturer> AllVaccineManufacturers
        {
            get
            {
                return DataContext.CreateQuery<VaccineManufacturer>()
                    .FetchAll();
            }
        }

        public IEnumerable<VaccineRefusalReason> AllVaccineRefusalReasons
        {
            get
            {
                return DataContext.CreateQuery<VaccineRefusalReason>()
                    .FilterBy(x => x.Display == true)
                    .FetchAll();
            }
        }

        public IEnumerable<VaccineRoute> AllVaccineRoutes
        {
            get
            {
                return DataContext.CreateQuery<VaccineRoute>()
                    .FetchAll();
            }
        }

        public IEnumerable<VaccineTradeName> AllVaccineTradeNames
        {
            get
            {
                return DataContext.CreateQuery<VaccineTradeName>()
                    .FetchAll();
            }
        }

        public IEnumerable<VaccineType> AllVaccineTypes
        {
            get
            {
                return DataContext.CreateQuery<VaccineType>()
                    .FetchAll();
            }
        }

        public IEnumerable<VaccineUnitOfMeasure> AllVaccineUnitOfMeasures
        {
            get
            {
                return DataContext.CreateQuery<VaccineUnitOfMeasure>()
                    .FetchAll();
            }
        }
    }
}
