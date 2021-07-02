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
    public class PrecautionRepository : AbstractRepository<IDataContext>, IPrecautionRepository
    {
        public PrecautionRepository(IDataContext dataContext)
            : base(dataContext) { }

        public void Add(PatientPrecaution entry)
        {
            DataContext.TrackChanges(entry);
        }

        public PatientPrecaution Get(int id)
        {
            return DataContext.Fetch<PatientPrecaution>(id);
        }

        public PatientPrecaution Get(Guid id)
        {
            return DataContext.CreateQuery<PatientPrecaution>()
                .FilterBy(x => x.Guid == id).FetchFirst();
        }

        public IEnumerable<PrecautionType> GetTypes(int? productId)
        {
            var query = DataContext.CreateQuery<PrecautionType>();

            if(productId.HasValue)
            {
                query = query.FilterBy(x => x.SystemProduct.Id == productId);
            }

            return query.FetchAll();
        }

        public IEnumerable<PatientPrecaution> Search(int patientiD, int? productType)
        {
            var query = DataContext.CreateQuery<PatientPrecaution>()
                .FilterBy(x => x.Patient.Id == patientiD);

            if(productType.HasValue)
            {
                query = query.FilterBy(x => x.PrecautionType.Id == productType.Value);
            }

            return query.FetchAll();
        }

        public IPagedQueryResult<PatientPrecaution> Find(Patient patient, int? productId,
          Expression<Func<PatientPrecaution, object>> sortByExpression,
          bool sortDescending, int page, int pageSize)
        {
            var query = DataContext.CreateQuery<PatientPrecaution>()
            .FilterBy(i => i.Patient == patient)
            .FilterBy(i => i.Deleted == null || i.Deleted == false);

            if(productId.HasValue)
            {
                query = query.FilterBy(x => x.PrecautionType.SystemProduct.Id == productId);
            }

            return query.SortBy(sortByExpression)
            .DescendingWhen(sortDescending)
            .PageSize(pageSize)
            .FetchPage(page);

        }

        public IEnumerable<PatientPrecaution> GetActiveForPatient(int patientId)
        {
            return DataContext.CreateQuery<PatientPrecaution>()
            .FilterBy(i => i.Patient.Id == patientId)
            .FilterBy(i => i.Deleted == null || i.Deleted == false)
            .FilterBy(i => i.EndDate == null || i.EndDate > DateTime.Today)
            .FetchAll();

        }

        public IEnumerable<PatientPrecaution> GetForPatient(int patientId)
        {
            return DataContext.CreateQuery<PatientPrecaution>()
            .FilterBy(i => i.Patient.Id == patientId)
            .FilterBy(i => i.Deleted == null || i.Deleted == false)
            .FetchAll();
        }

        public IEnumerable<PatientPrecaution> GetForFacilityAndProduct(int facilityId, Enumerations.KnownProductType product)
        {
            return DataContext.CreateQuery<PatientPrecaution>()
            .FilterBy(i => i.Patient.Room.Wing.Floor.Facility.Id == facilityId)
            .FilterBy(i => i.PrecautionType.SystemProduct.Id == (int)product)
            .FilterBy(i => i.Deleted == null || i.Deleted == false)
            .FetchAll();
        }

    }
}
