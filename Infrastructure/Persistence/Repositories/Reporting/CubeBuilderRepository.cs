using RedArrow.Framework.Extensions.Common;
using IQI.Intuition.Reporting.Models.Dimensions;
using IQI.Intuition.Reporting.Models.Cubes;
using IQI.Intuition.Reporting.Repositories;
using RedArrow.Framework.Utilities;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.GridFS;
using MongoDB.Driver.Linq;
using System.Linq;
using SnyderIS.sCore.Persistence;

namespace IQI.Intuition.Infrastructure.Persistence.Repositories.Reporting
{
    public class CubeBuilderRepository : ReportingRepository, ICubeBuilderRepository
    {
        public CubeBuilderRepository(IDocumentStore store)
            : base(store)
        { }

        public FacilityMonthCensus GetOrCreateCensus(Facility facility, Month month)
        {

            var result = GetQueryable<FacilityMonthCensus>()
               .Where(src => src.Facility.Id == facility.Id && src.Month.Id == month.Id)
               .FirstOrDefault();

            if (result == null)
            {
                result = new FacilityMonthCensus()
                {
                     Account = facility.Account,
                     Facility= facility,
                     Month = month
                };

                _Store.Save<FacilityMonthCensus>(result);
            }

            return result;
        }

    }
}
