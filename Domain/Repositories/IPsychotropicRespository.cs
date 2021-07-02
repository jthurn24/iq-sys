using System;
using System.Linq.Expressions;
using RedArrow.Framework.Persistence;
using IQI.Intuition.Domain.Models;
using System.Collections.Generic;

namespace IQI.Intuition.Domain.Repositories
{
    public interface IPsychotropicRespository
    {
        void Add(PsychotropicAdministration entity);
        void Add(PsychotropicDosageChange entity);
        void Add(PsychotropicAdministrationPRN entity);

        void Delete(PsychotropicDosageChange entity);
        void Delete(PsychotropicAdministrationPRN entity);

        PsychotropicAdministration GetAdministration(int id);
        PsychotropicDosageChange GetDosageChange(int id);
        PsychotropicAdministrationPRN GetPRN(int id);

        IPagedQueryResult<PsychotropicAdministration> FindAdministration(Facility facility,
            string drugName,
            string patientName, 
            string roomAndWingName,
            string drugTypeName,
            Expression<Func<PsychotropicAdministration, object>> sortByExpression,
            bool sortDescending, int page, int pageSize);

        IPagedQueryResult<PsychotropicAdministration> FindAdministration(Patient patient,
            Expression<Func<PsychotropicAdministration, object>> sortByExpression,
            bool sortDescending, int page, int pageSize);

        IPagedQueryResult<PsychotropicDosageChange> FindDosageChange(PsychotropicAdministration admin,
            Expression<Func<PsychotropicDosageChange, object>> sortByExpression,
            bool sortDescending, int page, int pageSize);


        IPagedQueryResult<PsychotropicAdministrationPRN> FindPRN(PsychotropicAdministration admin,
            Expression<Func<PsychotropicAdministrationPRN, object>> sortByExpression,
            bool sortDescending, int page, int pageSize);

        IEnumerable<PsychotropicAdministration> FindForLineListing(Facility facility,
            int? wingId,
            DateTime startDate,
            DateTime endDate,
            bool activeOnly);

        IEnumerable<PsychotropicDrugType> AllDrugTypes { get; }
        IEnumerable<PsychotropicFrequency> AllFrequencies { get; }
        IEnumerable<PsychotropicDosageForm> AllDosageForms { get; }
    }
}
