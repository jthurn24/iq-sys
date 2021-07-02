using System;
using System.Linq.Expressions;
using RedArrow.Framework.Persistence;
using IQI.Intuition.Domain.Models;
using System.Collections.Generic;

namespace IQI.Intuition.Domain.Repositories
{
    public interface IVaccineRepository
    {
        void Add(VaccineEntry vaccine);

        VaccineEntry Get(int id);

        void Delete(VaccineEntry value);

        IPagedQueryResult<VaccineEntry> Find(Facility facility, string patientName, string roomAndWingName,
            string vaccineType, DateTime? administeredOn, string refusalReason,
            Expression<Func<VaccineEntry, object>> sortByExpression,
            bool sortDescending, int page, int pageSize);

        IPagedQueryResult<VaccineEntry> Find(Patient patient,
            Expression<Func<VaccineEntry, object>> sortByExpression,
            bool sortDescending, int page, int pageSize);


        IEnumerable<VaccineEntry> FindForLineListing(Facility facility,
            int? wingId,
            int? vaccineType,
            int? refusalReason,
            DateTime? startDate,
            DateTime? endDate);

        IEnumerable<VaccineEntry> FindCreatedIn(int month, int year);

        IEnumerable<VaccineEntry> FindVaccinations(Facility facility, DateTime startDate, DateTime endDate, IQI.Intuition.Domain.Enumerations.VaccineStatus status);

        IEnumerable<VaccineTradeName> AllVaccineTradeNamesForType(int type);


        IEnumerable<VaccineAdministrativeSite> AllVaccineAdministrativeSites { get; }

        IEnumerable<VaccineManufacturer> AllVaccineManufacturers { get; }

        IEnumerable<VaccineRefusalReason> AllVaccineRefusalReasons { get; }

        IEnumerable<VaccineRoute> AllVaccineRoutes { get; }

        IEnumerable<VaccineTradeName> AllVaccineTradeNames { get; }

        IEnumerable<VaccineType> AllVaccineTypes { get; }

        IEnumerable<VaccineUnitOfMeasure> AllVaccineUnitOfMeasures { get; }
    }
}
