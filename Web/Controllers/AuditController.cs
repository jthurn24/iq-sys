using System;
using System.Web.Mvc;
using System.Linq;
using System.Collections.Generic;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Mvc.ModelMapper;
using IQI.Intuition.Infrastructure.Services;
using IQI.Intuition.Web.Models.Audit;
using RedArrow.Framework.Mvc.Extensions;
using RedArrow.Framework.Mvc.Security;
using IQI.Intuition.Web.Attributes;
using IQI.Intuition.Domain;
using IQI.Intuition.Domain.Repositories;
using RedArrow.Framework.Extensions.Formatting;

namespace IQI.Intuition.Web.Controllers
{
    [RequiresActiveAccount]
    public class AuditController : Controller
    {
        public AuditController(
            IActionContext actionContext,
            IModelMapper modelMapper,
            ISystemRepository systemRepository,
            IIncidentRepository incidentRepository,
            IInfectionRepository infectionRepository,
            IAuthentication authentication,
            ICatheterRepository catheterRepository,
            IWoundRepository woundRepository)
        {
            ActionContext = actionContext.ThrowIfNullArgument("actionContext");
            ModelMapper = modelMapper.ThrowIfNullArgument("modelMapper");
            SystemRepository = systemRepository.ThrowIfNullArgument("systemRepository");
            Authentication = authentication.ThrowIfNullArgument("authentication");
            IncidentRepository = incidentRepository.ThrowIfNullArgument("incidentRepository");
            InfectionRepository = infectionRepository.ThrowIfNullArgument("infectionRepository");
            CatheterRepository = catheterRepository.ThrowIfNullArgument("catheterRepository");
            WoundRepository = woundRepository.ThrowIfNullArgument("woundRepository");
        }

        protected virtual IActionContext ActionContext { get; private set; }

        protected virtual IModelMapper ModelMapper { get; private set; }

        protected virtual ISystemRepository SystemRepository { get; private set; }

        protected virtual IAuthentication Authentication { get; private set; }

        protected virtual IIncidentRepository IncidentRepository { get; private set; }

        protected virtual IInfectionRepository InfectionRepository { get; private set; }

        protected virtual ICatheterRepository CatheterRepository { get; private set; }

        protected virtual IWoundRepository WoundRepository { get; private set; }

        public ActionResult ViewAudit(AuditRequest request)
        {
            var data = SystemRepository.FindAuditEntry(
                request.PatientId,
                request.ComponentId,
                request.Types,
                ActionContext.CurrentFacility.Id,
                x => x.PerformedAt,
                true,
                request.Page,
                50);

            var results = new List<AuditEntry>();

            foreach (var item in data.PageValues)
            {

                var result = new AuditEntry();

                var user = ActionContext.CurrentFacility.AccountUsers.Where(x => x.Guid.ToString() == item.PerformedBy).FirstOrDefault();

                result.AuditType = System.Enum.GetName(typeof(Domain.Enumerations.AuditEntryType), item.AuditType).SplitPascalCase();
                result.IPAddress = item.IPAddress;
                result.PerformedAt = item.PerformedAt.ToString("MM/dd/yy HH:mm");
                result.PerformedBy = user != null ? user.Login : string.Empty;

                if (item.TargetPatient != null)
                {
                    var patient = ActionContext.CurrentFacility.FindPatient(item.TargetPatient.Value);
                    result.TargetPatient = patient != null ? patient.FullName : "Unknown";

                }
                else
                {
                    result.TargetPatient = "N/A";
                }


                if (item.DetailsMode == null)
                {
                    result.Details = item.DetailsText;
                }
                else if (item.DetailsMode == Domain.Models.AuditEntry.DETAILS_MODE_SERIALIZED_CHANGES)
                {
                    var changes = Infrastructure.Services.Protection.ChangeData.Load(item.DetailsText);

                    var detailsBuilder = new System.Text.StringBuilder();

                    detailsBuilder.Append(changes.Description);
                    detailsBuilder.Append("<hr>");

                    foreach (var c in changes.Fields)
                    {
                        detailsBuilder.Append("<div style='font-weight:bold'>");
                        detailsBuilder.Append(c.Name);
                        detailsBuilder.Append("</div>");

                        var values = c.Change.Split(',');

                        foreach (var v in values)
                        {
                            detailsBuilder.Append("<div style='font-style:italic'>");
                            detailsBuilder.Append(v);
                            detailsBuilder.Append("</div>");
                        }

                        detailsBuilder.Append("<hr>");

                    }

                    result.Details = detailsBuilder.ToString();

                }

                results.Add(result);

            }

            var model = new AuditResult();
            model.Results = results;
            model.Request = request;
            model.TotalPages = data.TotalPages;

            return PartialView(model);

        }

        public ActionResult QuickQAAuditSummary(int days)
        {
            var results = new List<AuditSummaryEntry>();
            var startDate = DateTime.Today.AddDays(0 - days);
            var auditEntries = SystemRepository.FindAuditEntry(ActionContext.CurrentFacility.Id, startDate);

            if (ActionContext.CurrentFacility.HasProduct(Domain.Enumerations.KnownProductType.InfectionTracking))
            {
                AppendAuditEntries(results, auditEntries.Where(x => x.AuditType == Domain.Enumerations.AuditEntryType.AddedInfection));
                AppendAuditEntries(results, auditEntries.Where(x => x.AuditType == Domain.Enumerations.AuditEntryType.ModifiedInfection));
                AppendAuditEntries(results, auditEntries.Where(x => x.AuditType == Domain.Enumerations.AuditEntryType.RemovedInfection));

                AppendAuditEntries(results, auditEntries.Where(x => x.AuditType == Domain.Enumerations.AuditEntryType.AddedCatheter));
                AppendAuditEntries(results, auditEntries.Where(x => x.AuditType == Domain.Enumerations.AuditEntryType.ModifiedCatheter));
                AppendAuditEntries(results, auditEntries.Where(x => x.AuditType == Domain.Enumerations.AuditEntryType.RemovedCatheter));
            }

            if (ActionContext.CurrentFacility.HasProduct(Domain.Enumerations.KnownProductType.IncidentTracking))
            {
                AppendAuditEntries(results, auditEntries.Where(x => x.AuditType == Domain.Enumerations.AuditEntryType.AddedIncident));
                AppendAuditEntries(results, auditEntries.Where(x => x.AuditType == Domain.Enumerations.AuditEntryType.ModifiedIncident));
                AppendAuditEntries(results, auditEntries.Where(x => x.AuditType == Domain.Enumerations.AuditEntryType.RemovedIncident));
            }


            if (ActionContext.CurrentFacility.HasProduct(Domain.Enumerations.KnownProductType.WoundTracking))
            {
                AppendAuditEntries(results, auditEntries.Where(x => x.AuditType == Domain.Enumerations.AuditEntryType.AddedWound));
                AppendAuditEntries(results, auditEntries.Where(x => x.AuditType == Domain.Enumerations.AuditEntryType.ModifiedWound));
                AppendAuditEntries(results, auditEntries.Where(x => x.AuditType == Domain.Enumerations.AuditEntryType.RemovedWound));
            }

            AppendAuditEntries(results, auditEntries.Where(x => x.AuditType == Domain.Enumerations.AuditEntryType.AddedPatient));
            AppendAuditEntries(results, auditEntries.Where(x => x.AuditType == Domain.Enumerations.AuditEntryType.ModifiedPatientDemographic));
            AppendAuditEntries(results, auditEntries.Where(x => x.AuditType == Domain.Enumerations.AuditEntryType.RemovedPatient));

            results = results.OrderBy(x => x.Minutes).ToList();

            return PartialView(results);
        }

        private void AppendAuditEntries(IList<AuditSummaryEntry> results, IEnumerable<Domain.Models.AuditEntry> entries)
        {
            foreach (var entry in entries)
            {
                var result = new AuditSummaryEntry();
                result.SetData(entry, ActionContext.CurrentFacility
                    , IncidentRepository, 
                    InfectionRepository, 
                    CatheterRepository, 
                    WoundRepository);

                results.Add(result);
            }
        }

    }
}