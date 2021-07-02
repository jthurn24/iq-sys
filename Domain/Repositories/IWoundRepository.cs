using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using RedArrow.Framework.Persistence;
using IQI.Intuition.Domain.Models;
using IQI.Intuition.Domain;


namespace IQI.Intuition.Domain.Repositories
{
    public interface IWoundRepository
    {
        void Add(WoundReport report);
        void Add(WoundAssessment assessment);

        WoundReport GetReport(int id);
        WoundReport GetReport(Guid guid);
        WoundAssessment GetAssessment(int id);

        void Delete(WoundAssessment assessment);

        IPagedQueryResult<WoundReport> Find(Patient patient,
            Expression<Func<WoundReport, object>> sortByExpression,
            bool sortDescending, int page, int pageSize);

        IPagedQueryResult<WoundReport> FindReport(Facility facility, 
            string patientName, 
            string roomAndWingName,
            string firstNoted, 
            string dateResolved, 
            bool includeResolved,
            string stage,
            string siteName,
            string typeName,
            Expression<Func<WoundReport, object>> sortByExpression,
            bool sortDescending, int page, int pageSize);


        IEnumerable<WoundReport> FindForLineListing(Facility facility,
            int? wingId,
            int? floorId,
            bool includeResolved,
            bool resolvedOnly,
            string startDate,
            string endDate,
            int? currentStage,
            int? classification,
            int? type);

        IEnumerable<WoundReport> FindReportsForFacility(Facility facility, IEnumerable<Guid> guids); 

        IEnumerable<WoundReport> FindActive(Facility facility);

        IPagedQueryResult<WoundAssessment> FindAssessment(WoundReport report,
            Expression<Func<WoundAssessment, object>> sortByExpression,
            bool sortDescending, int page, int pageSize);

        IEnumerable<WoundStage> AllStages { get; }
        IEnumerable<WoundSite> AllSites { get; }
        IEnumerable<WoundType> AllTypes { get; }

    }
}
