using System;
using System.Linq.Expressions;
using RedArrow.Framework.Persistence;
using IQI.Intuition.Domain.Models;
using System.Collections.Generic;


namespace IQI.Intuition.Domain.Repositories
{
    public interface IPrecautionRepository
    {
        void Add(PatientPrecaution entry);
        PatientPrecaution Get(int id);
        PatientPrecaution Get(Guid id);


        IEnumerable<PrecautionType> GetTypes(int? productId);
        IEnumerable<PatientPrecaution> Search(int patientiD, int? productType);

        IPagedQueryResult<PatientPrecaution> Find(Patient patient, int? productId,
          Expression<Func<PatientPrecaution, object>> sortByExpression,
          bool sortDescending, int page, int pageSize);

        IEnumerable<PatientPrecaution> GetActiveForPatient(int patientId);
        IEnumerable<PatientPrecaution> GetForPatient(int patientId);
        IEnumerable<PatientPrecaution> GetForFacilityAndProduct(int facilityId, Enumerations.KnownProductType product);
    }
}
