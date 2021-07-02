using System;
using System.Linq.Expressions;
using RedArrow.Framework.Persistence;
using IQI.Intuition.Domain.Models;
using System.Collections.Generic;


namespace IQI.Intuition.Domain.Repositories
{
    public interface IFacilityRepository
    {
        Facility Get(int id);

        Facility Get(Guid guid);

        Facility Get(string subDomain);

        FacilityProduct GetProduct(int id);

        IEnumerable<Wing> SearchWings(int facilityId, int wingId);
        IEnumerable<Room> SearchRooms(int facilityId, int roomId);

        IPagedQueryResult<Facility> Find(
            int? accountId,
            string name,
            string subdomain,
            string state,
            Expression<Func<Facility, object>> sortByExpression,
            bool sortDescending, int page, int pageSize);

        IPagedQueryResult<FacilityProduct> FindProduct(
            int? facilityId,
            string name,
            string fee,
            string feeType,
            string startedOn,
            Expression<Func<FacilityProduct, object>> sortByExpression,
            bool sortDescending, int page, int pageSize);
    }
}
