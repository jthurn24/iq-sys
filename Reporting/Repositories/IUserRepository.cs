using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQI.Intuition.Reporting.Models.User;

namespace IQI.Intuition.Reporting.Repositories
{
    public interface IUserRepository
    {
        QIDashboard GetOrCreateDashboardData(Guid id);
        QICast GetQICastData(Guid id);
        IEnumerable<QICast> GetQICasts(Guid id);

        QINote GetOrCreateNote(Guid id, Guid facilityId);

        void Update(QIDashboard data);
        void Update(QICast data);
        void Update(QINote data);


        

        void Add(QICastCommand data);
        void Delete(QICastCommand data);
        IEnumerable<QICastCommand> GetCommands(Guid qiCastId);

        IEnumerable<ExportRequest> GetPendingExport();
        IEnumerable<ExportRequest> GetExportCreatedBefore(DateTime date);
        void Delete(ExportRequest data);

        CubeSyncJob GetNextCubeSyncJob();
        void Add(CubeSyncJob data);
        void Update(CubeSyncJob data);
        void Delete(CubeSyncJob data);
        int GetCubeSyncJobCount();

        void Add(ExportRequest data);
        void Update(ExportRequest data);
        ExportRequest GetExportRequest(Guid id);
        
    }
}
