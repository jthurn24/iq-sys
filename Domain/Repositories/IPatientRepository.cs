using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using RedArrow.Framework.Persistence;
using IQI.Intuition.Domain.Models;
using IQI.Intuition.Domain;

namespace IQI.Intuition.Domain.Repositories
{
    public interface IPatientRepository
    {
        void Add(Patient patient);

        Patient Get(int id);

        Patient Get(Guid guid);

        PatientStatusChange GetStatusChange(int id);

        IPagedQueryResult<Patient> Find(Facility facility, string firstName, string lastName, string birthDate, string wingName, string roomName,
            Enumerations.PatientStatus? status, Expression<Func<Patient, object>> sortByExpression, bool sortDescending, int page, int pageSize);

        IEnumerable<Patient> FindByName(Facility facility, string name, int resultLimit);

        IEnumerable<Patient> Find(Facility facility);

        IEnumerable<PatientFlagType> AllPatientFlags { get; }
    }
}
