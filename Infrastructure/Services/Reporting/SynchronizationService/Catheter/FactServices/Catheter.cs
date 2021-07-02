using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQI.Intuition.Domain.Repositories;
using IQI.Intuition.Domain.Models;
using Dimensions = IQI.Intuition.Reporting.Models.Dimensions;
using Cubes = IQI.Intuition.Reporting.Models.Cubes;
using Facts = IQI.Intuition.Reporting.Models.Facts;
using IQI.Intuition.Reporting.Repositories;
using RedArrow.Framework.Persistence;
using RedArrow.Framework.Logging;
using SnyderIS.sCore.Persistence;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.GridFS;
using MongoDB.Driver.Linq;
using IQI.Intuition.Reporting.Containers;

namespace IQI.Intuition.Infrastructure.Services.Reporting.SynchronizationService.Catheter.FactServices
{
    public class Catheter : AbstractFactService
    {

        public Catheter(
            IDimensionBuilderRepository db,
            IDimensionRepository d,
            ICubeBuilderRepository cb,
            IStatelessDataContext dc,
            IFactBuilderRepository fb,
            ILog log,
            IDocumentStore ds) : base(db,d,cb,dc,fb,log, ds)
        {

        }

        public void Run(
            IList<Domain.Models.CatheterEntry> dCatheters,
            Domain.Models.Facility dFacility,
            DataDimensions dimensions)
        {
            foreach (var dCatheter in dCatheters)
            {

                _Log.Info(string.Format("Syncing catheter: {0}  facility: {1}", dCatheter.Guid, dFacility.Name));


                var dAccount = _DataContext.Fetch<Domain.Models.Account>(dFacility.Account.Id);

                var facility = _DimensionBuilderRepository.GetOrCreateFacility(dFacility.Guid);
                var account = _DimensionBuilderRepository.GetOrCreateAccount(dAccount.Guid, dAccount.Name);

                var record = _FactBuilderRespository.GetOrCreateCatheter(dCatheter.Guid);

                TrackDimensionChanges(dimensions, record);

                if (dCatheter.Type.HasValue)
                {
                    var enumVal = (Domain.Enumerations.CatheterType)dCatheter.Type.Value;
                    var cType = _DimensionBuilderRepository.GetOrCreateCatheterType(System.Enum.GetName(typeof(Domain.Enumerations.CatheterType), enumVal));
                    record.CatheterType = cType;
                }           

                record.Account = account;
                record.Facility = facility;
                record.Deleted = dCatheter.Deleted;
                record.StartedDate = dCatheter.StartedOn;
                record.DiscontinuedDate = dCatheter.DiscontinuedOn;

                TrackDimensionChanges(dimensions, record);

                Save<Facts.Catheter>(record);

            }
        }

        private void TrackDimensionChanges(DataDimensions dimensions,
            Facts.Catheter catheter
            )
        {

            if (catheter.StartedDate.HasValue)
            {
                DateTime startDate = catheter.StartedDate.Value;
                DateTime endDate = DateTime.Today;

                if (catheter.CatheterType != null)
                {
                    dimensions.CatheterTypes.Add(catheter.CatheterType);
                }

                if (dimensions.EndDate.HasValue == false || dimensions.StartDate.Value > startDate)
                {
                    dimensions.StartDate = startDate;
                }

                if (dimensions.EndDate.HasValue == false || dimensions.EndDate.Value < endDate)
                {
                    dimensions.EndDate = endDate;
                }
            }   

        }
    }
}
