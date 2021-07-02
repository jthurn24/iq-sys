using System;
using System.Linq.Expressions;
using RedArrow.Framework.Persistence;
using IQI.Intuition.Domain.Models;
using System.Collections.Generic;


namespace IQI.Intuition.Domain.Repositories
{
    public interface IIncidentRepository
    {
        void Add(IncidentReport incident);
        IncidentReport Get(int id);
        IncidentReport Get(Guid guid);

        void Delete(IncidentWitness value);

        IPagedQueryResult<IncidentReport> Find(Facility facility, string patientName, string roomAndWingName,
            string type, string discoveredOn, string occurredOn,string injury,
            Expression<Func<IncidentReport, object>> sortByExpression,
            bool sortDescending, int page, int pageSize);

        IPagedQueryResult<IncidentReport> Find(Patient patient,
            Expression<Func<IncidentReport, object>> sortByExpression,
            bool sortDescending, int page, int pageSize);

        IEnumerable<IncidentReport> FindForLineListing(Facility facility, 
            int? wingId, int? floorId, IList<string> groups, IList<int> injuries, DateTime? startDate, DateTime? endDate);

        IEnumerable<IncidentReport> FindForFacility(Facility facility, IEnumerable<Guid> guids); 

        IEnumerable<IncidentType> AllTypes { get; }
        IEnumerable<IncidentLocation> AllLocations { get; }
        IEnumerable<IncidentInjury> AllInjuries { get; }

        IEnumerable<IncidentReport> FindCreatedIn(int month, int year);
        
    }
}
