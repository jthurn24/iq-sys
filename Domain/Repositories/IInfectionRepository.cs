using System;
using System.Linq.Expressions;
using RedArrow.Framework.Persistence;
using IQI.Intuition.Domain.Models;
using System.Collections.Generic;

namespace IQI.Intuition.Domain.Repositories
{
    public interface IInfectionRepository
    {
        void Add(InfectionVerification infection);

        InfectionVerification Get(int id);
        InfectionVerification Get(Guid guid);

        void Delete(InfectionTreatment value);
        void Delete(InfectionLabResult value);
        void Delete(InfectionLabResultPathogen value);

        IPagedQueryResult<InfectionVerification> Find(Facility facility, string patientName, string roomAndWingName,
            string type, string firstNoted, string reasonForEntry, string labOrXrayDate, string labFindings,
            string therapy, string dateResolved, bool includeResolved, 
            Expression<Func<InfectionVerification, object>> sortByExpression,
            bool sortDescending, int page, int pageSize);

        IPagedQueryResult<InfectionVerification> Find(Patient patient,
            Expression<Func<InfectionVerification, object>> sortByExpression,
            bool sortDescending, int page, int pageSize);

        IEnumerable<InfectionVerification> FindForFacility(Facility facility, IEnumerable<Guid> guids); 

        IEnumerable<InfectionVerification> FindForPatient(Guid guid, int? typeId, int? siteId);

        IEnumerable<InfectionVerification> FindActiveForWing(int id);

        IEnumerable<InfectionVerification> FindForLineListing(Facility facility, 
            int? wingId,
            int? floorId, 
            bool includeResolved, 
            DateTime? startDate, 
            DateTime? endDate, 
            int? type,
            bool pendingLabsOnly,
            IList<int> pathogens,
            IList<string> anitbiotics,
            IList<int> labTests);

        IEnumerable<InfectionVerification> FindActiveFacility(Facility facility);

        IEnumerable<InfectionVerification> FindCreatedIn(int month, int year);

        IEnumerable<InfectionVerification> FindForRoom(int id);

        IEnumerable<InfectionTreatment> FindTreatments(Facility facility, DateTime startDate, DateTime endDate, int? infectionType);

        IEnumerable<InfectionType> AllInfectionTypes { get; }

        IEnumerable<InfectionSite> AllInfectionSites { get; }

        IEnumerable<InfectionCriteria> AllInfectionCriteria { get; }

        IEnumerable<InfectionRiskFactor> AllRiskFactors { get; }


        IEnumerable<InfectionSymptom> AllSymptoms { get; }

        IEnumerable<InfectionDefinition> AllInfectionDefinitions { get; }
    }
}
