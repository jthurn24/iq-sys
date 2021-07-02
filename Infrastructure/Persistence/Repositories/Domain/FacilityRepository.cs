using System;
using System.Collections.Generic;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Persistence;
using IQI.Intuition.Domain.Models;
using IQI.Intuition.Domain.Repositories;
using IQI.Intuition.Domain;

namespace IQI.Intuition.Infrastructure.Persistence.Repositories.Domain
{
    public class FacilityRepository : AbstractRepository<IDataContext>, IFacilityRepository
    {
        public FacilityRepository(IDataContext dataContext)
            : base(dataContext) { }

        public Facility Get(int id)
        {
            return DataContext.Fetch<Facility>(id);
        }

        public Facility Get(Guid guid)
        {
            return DataContext.CreateQuery<Facility>()
               .FilterBy(facility => facility.Guid == guid)
               .FetchFirst();
        }

        public Facility Get(string subDomain)
        {
            return DataContext.CreateQuery<Facility>()
               .FilterBy(facility => facility.SubDomain == subDomain)
               .FetchFirst();
        }

        public IEnumerable<Wing> SearchWings(int facilityId, int wingId)
        {
            return DataContext.CreateQuery<Wing>()
                .FilterBy(x => x.Floor.Facility.Id == facilityId)
                .FilterBy(x => x.Id == wingId)
                .FetchAll();
        }

        public IEnumerable<Room> SearchRooms(int facilityId, int roomId)
        {
            return DataContext.CreateQuery<Room>()
                .FilterBy(x => x.Wing.Floor.Facility.Id == facilityId)
                .FilterBy(x => x.Id == roomId)
                .FetchAll();
        }

        public IPagedQueryResult<Facility> Find(
            int? accountId, 
            string name, 
            string subdomain, 
            string state, 
            System.Linq.Expressions.Expression<Func<Facility, object>> sortByExpression, 
            bool sortDescending, 
            int page, 
            int pageSize)
        {
            var q = DataContext.CreateQuery<Facility>();

            if (name.IsNotNullOrEmpty())
            {
                q = q.FilterBy(x => x.Name.Contains(name));
            }

            if (accountId.HasValue)
            {
                q = q.FilterBy(x => x.Account.Id == accountId.Value);
            }

            if (subdomain.IsNotNullOrEmpty())
            {
                q = q.FilterBy(x => x.SubDomain.Contains(subdomain));
            }

            if (state.IsNotNullOrEmpty())
            {
                q = q.FilterBy(x => x.State.Contains(state));
            }

            return q.SortBy(sortByExpression)
                .DescendingWhen(sortDescending)
                .PageSize(pageSize)
                .FetchPage(page);
        }


        public FacilityProduct GetProduct(int id)
        {
            return DataContext.Fetch<FacilityProduct>(id);
        }

        public IPagedQueryResult<FacilityProduct> FindProduct(int? facilityId, 
            string name, 
            string fee,
            string feeType,
            string startedOn,
            System.Linq.Expressions.Expression<Func<FacilityProduct, object>> sortByExpression, 
            bool sortDescending, int page, int pageSize)
        {
            var q = DataContext.CreateQuery<FacilityProduct>();

            if (facilityId.HasValue)
            {
                q = q.FilterBy(x => x.Facility.Id == facilityId);
            }

            if (name.IsNotNullOrEmpty())
            {
                q = q.FilterBy(x => x.SystemProduct.Name.Contains(name));
            }

            if (fee.IsNotNullOrEmpty())
            {
                decimal feeVal;

                if (Decimal.TryParse(fee, out feeVal))
                {
                    q = q.FilterBy(x => x.Fee == feeVal);
                }

            }

            if (feeType.IsNotNullOrEmpty())
            {
                q = q.FilterBy(x => x.FeeType == (Enumerations.ProductFeeType)System.Enum.Parse(typeof(Enumerations.ProductFeeType), feeType) );
            }

            if (startedOn.IsNotNullOrEmpty())
            {
                DateTime startedDate;

                if (DateTime.TryParse(startedOn, out startedDate))
                {
                    q = q.FilterBy(x => x.StartOn == startedDate);
                }
            }

            return q.SortBy(sortByExpression)
                .DescendingWhen(sortDescending)
                .PageSize(pageSize)
                .FetchPage(page);
        }
    }
}
