using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SnyderIS.sCore.Exi.Interfaces.DataSource;
using SnyderIS.sCore.Exi.Implementation.DataSource;
using IQI.Intuition.Domain.Repositories;
using StructureMap;
using StructureMap.Query;
using IQI.Intuition.Reporting.Repositories;
using IQI.Intuition.Infrastructure.Services;
using IQI.Intuition.Exi.Models.QIDashboard.Alerts;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Extensions.Formatting;
using IQI.Intuition.Web.Models.Audit;

namespace IQI.Intuition.Exi.DataSources.QIDashboard.Audits
{
    public class DataSource : IDataSource<AuditSummaryEntry>
    {

        private IActionContext _ActionContext;
        private IWarningRepository _WarningRepository;
        private ISystemRepository _SystemRepository;
        private IIncidentRepository _IncidentRepository;
        private IInfectionRepository _InfectionRepository;
        private IWoundRepository _WoundRepository;
        private ICatheterRepository _CatheterRepository;

        public DataSource(IActionContext actionContext,
         IWarningRepository warningRepository,
         ISystemRepository systemRepository,
         IIncidentRepository incidentRepository,
         IInfectionRepository infectionRepository,
         IWoundRepository woundRepository,
         ICatheterRepository catheterRepository)
        {
            _ActionContext = actionContext;
            _WarningRepository = warningRepository;
            _SystemRepository = systemRepository;
            _IncidentRepository = incidentRepository;
            _InfectionRepository = infectionRepository;
            _WoundRepository = woundRepository;
            _CatheterRepository = catheterRepository;
        }

        public IDataSourceResult<Web.Models.Audit.AuditSummaryEntry> GetResult(IDictionary<string, string> criteria)
        {
            var results = new List<AuditSummaryEntry>();
            var startDate = DateTime.Today.AddDays(0 - 3);
            var auditEntries = _SystemRepository.FindAuditEntry(_ActionContext.CurrentFacility.Id, startDate);

            if (_ActionContext.CurrentFacility.HasProduct(Domain.Enumerations.KnownProductType.InfectionTracking))
            {
                AppendAuditEntries(results, auditEntries.Where(x => x.AuditType == Domain.Enumerations.AuditEntryType.AddedInfection));
                AppendAuditEntries(results, auditEntries.Where(x => x.AuditType == Domain.Enumerations.AuditEntryType.ModifiedInfection));
                AppendAuditEntries(results, auditEntries.Where(x => x.AuditType == Domain.Enumerations.AuditEntryType.RemovedInfection));

                AppendAuditEntries(results, auditEntries.Where(x => x.AuditType == Domain.Enumerations.AuditEntryType.AddedCatheter));
                AppendAuditEntries(results, auditEntries.Where(x => x.AuditType == Domain.Enumerations.AuditEntryType.ModifiedCatheter));
                AppendAuditEntries(results, auditEntries.Where(x => x.AuditType == Domain.Enumerations.AuditEntryType.RemovedCatheter));
            }

            if (_ActionContext.CurrentFacility.HasProduct(Domain.Enumerations.KnownProductType.IncidentTracking))
            {
                AppendAuditEntries(results, auditEntries.Where(x => x.AuditType == Domain.Enumerations.AuditEntryType.AddedIncident));
                AppendAuditEntries(results, auditEntries.Where(x => x.AuditType == Domain.Enumerations.AuditEntryType.ModifiedIncident));
                AppendAuditEntries(results, auditEntries.Where(x => x.AuditType == Domain.Enumerations.AuditEntryType.RemovedIncident));
            }


            if (_ActionContext.CurrentFacility.HasProduct(Domain.Enumerations.KnownProductType.WoundTracking))
            {
                AppendAuditEntries(results, auditEntries.Where(x => x.AuditType == Domain.Enumerations.AuditEntryType.AddedWound));
                AppendAuditEntries(results, auditEntries.Where(x => x.AuditType == Domain.Enumerations.AuditEntryType.ModifiedWound));
                AppendAuditEntries(results, auditEntries.Where(x => x.AuditType == Domain.Enumerations.AuditEntryType.RemovedWound));
            }

            AppendAuditEntries(results, auditEntries.Where(x => x.AuditType == Domain.Enumerations.AuditEntryType.AddedPatient));
            AppendAuditEntries(results, auditEntries.Where(x => x.AuditType == Domain.Enumerations.AuditEntryType.ModifiedPatientDemographic));
            AppendAuditEntries(results, auditEntries.Where(x => x.AuditType == Domain.Enumerations.AuditEntryType.RemovedPatient));

            results = results.OrderBy(x => x.Minutes).ToList();

            var r = new DataSourceResult<AuditSummaryEntry>();
            r.Metrics = results;
            return r;
        }


        private void AppendAuditEntries(IList<AuditSummaryEntry> results, IEnumerable<Domain.Models.AuditEntry> entries)
        {
            foreach (var entry in entries)
            {
                var result = new AuditSummaryEntry();
                result.SetData(entry, _ActionContext.CurrentFacility
                    , _IncidentRepository,
                    _InfectionRepository,
                    _CatheterRepository,
                    _WoundRepository);

                results.Add(result);
            }
        }

        public string DefaultTitle(IDictionary<string, string> criteria)
        {
            return string.Concat("Recent Changes - Last 3 days");
        }

        IDataSourceResult IDataSource.GetResult(IDictionary<string, string> criteria)
        {
            return GetResult(criteria);
        }
    }
}
