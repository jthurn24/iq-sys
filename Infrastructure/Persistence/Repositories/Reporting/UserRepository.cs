using System;
using System.Collections.Generic;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Persistence;
using IQI.Intuition.Reporting.Models.Dimensions;
using IQI.Intuition.Reporting.Models.Facts;
using IQI.Intuition.Reporting.Repositories;
using RedArrow.Framework.Utilities;
using SnyderIS.sCore.Persistence;
using IQI.Intuition.Reporting.Models.User;
using IQI.Intuition.Infrastructure.Persistence.Reporting;

namespace IQI.Intuition.Infrastructure.Persistence.Repositories.Reporting
{
    public class UserRepository : ReportingRepository, IUserRepository
    {

        public UserRepository(IDocumentStore store)
            : base(store)
        { }

        public QIDashboard GetOrCreateDashboardData(Guid id)
        {
            var result = GetQueryable<QIDashboard>()
                           .Where(src => src.Id == id)
                           .FirstOrDefault();

            if (result == null)
            {
                result = new QIDashboard();
                result.Id = id;
                Save<QIDashboard>(result);
            }

            return result;
        }

        public QICast GetQICastData(Guid id)
        {
            var result = GetQueryable<QICast>()
               .Where(src => src.Id == id)
               .FirstOrDefault();

            return result;
        }


        public QINote GetOrCreateNote(Guid id, Guid facilityId)
        {
            var result = GetQueryable<QINote>()
                           .Where(src => src.Id == id && src.FacilityId == facilityId)
                           .FirstOrDefault();

            if (result == null)
            {
                result = new QINote();
                result.Id = id;
                result.FacilityId = facilityId;
                Save<QINote>(result);
            }

            return result;
        }

        public IEnumerable<QICast> GetQICasts(Guid id)
        {
            return GetQueryable<QICast>()
               .Where(src => src.FacilityId == id);
        }
        

        public void Update(QIDashboard data)
        {
            Save<QIDashboard>(data);
        }

        public void Update(QICast data)
        {
            Save<QICast>(data);
        }

        public void Update(QINote data)
        {
            Save<QINote>(data);
        }

        public void Add(QICastCommand data)
        {
            Save<QICastCommand>(data);
        }

        public void Delete(QICastCommand data)
        {
            ((MongoDocumentStore)this._Store).Delete(data, data.Id);
        }

        public void Delete(ExportRequest data)
        {
            ((MongoDocumentStore)this._Store).Delete(data, data.Id);
        }


        public IEnumerable<QICastCommand> GetCommands(Guid qiCastId)
        {
            return GetQueryable<QICastCommand>()
               .Where(src => src.QiCastId == qiCastId);
        }


        public IEnumerable<ExportRequest> GetPendingExport()
        {
            return GetQueryable<ExportRequest>()
               .Where(src => src.Status == ExportRequest.ExportRequestStatus.Pending);
        }

        public IEnumerable<ExportRequest> GetExportCreatedBefore(DateTime date)
        {
            return GetQueryable<ExportRequest>()
                .Where(src => src.CreatedOn < date);
        }


        public void Add(ExportRequest data)
        {
            Save<ExportRequest>(data);
        }

        public void Update(ExportRequest data)
        {
            Save<ExportRequest>(data);
        }

        public ExportRequest GetExportRequest(Guid id)
        {
            return GetQueryable<ExportRequest>()
               .Where(src => src.Id == id)
               .FirstOrDefault();
        }

        public void Add(CubeSyncJob data)
        {
            Save<CubeSyncJob>(data);
        }

        public void Update(CubeSyncJob data)
        {
            Save<CubeSyncJob>(data);
        }

        public void Delete(CubeSyncJob data)
        {
            ((MongoDocumentStore)this._Store).Delete(data, data.Id);
        }

        public CubeSyncJob GetNextCubeSyncJob()
        {
            return GetQueryable<CubeSyncJob>()
                .OrderBy(x => x.Priority)
                .ThenBy(x => x.CreatedOn)
               .FirstOrDefault();
        }

        public int GetCubeSyncJobCount()
        {
            return GetQueryable<CubeSyncJob>().Count();
        }
    }
}
