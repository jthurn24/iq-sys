using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IQI.Intuition.Web.Models.Audit
{
    public class AuditSummaryEntry
    {
        public string PatientName { get; set; }
        public Guid? PatientGuid { get; set; }

        public string ChangeDescription { get; set; }

        public string PerformedBy { get; set; }
        public int Minutes { get; set; }
        public string TimespanDescription { get; set; }

        public int? InfectionId { get; set; }
        public int? IncidentId { get; set; }
        public int? CatheterId { get; set; }
        public int? WoundId { get; set; }

        public Guid? ComponentGuid { get; set; }

        public AuditRequest Request { get; set; }

        public void SetData(Domain.Models.AuditEntry domain,
            Domain.Models.Facility facility,
            Domain.Repositories.IIncidentRepository incidentRepository,
            Domain.Repositories.IInfectionRepository infectionRepository,
            Domain.Repositories.ICatheterRepository catheterRepository,
            Domain.Repositories.IWoundRepository woundRepository)
        {

            this.Request = new AuditRequest();


            /* Patient */
            if (domain.TargetPatient.HasValue)
            {
                this.PatientGuid = domain.TargetPatient;
                this.Request.PatientId = domain.TargetPatient;
                var patient = facility.FindPatient(domain.TargetPatient.Value);

                if (patient == null)
                {
                    this.PatientName = "N/A";
                }
                else
                {
                    this.PatientName = patient.FullName;
                }
            }

            /* Description */
            if (domain.DetailsMode == null)
            {
                this.ChangeDescription = domain.DetailsText;
            }
            else if (domain.DetailsMode == Domain.Models.AuditEntry.DETAILS_MODE_SERIALIZED_CHANGES)
            {
                var changes = Infrastructure.Services.Protection.ChangeData.Load(domain.DetailsText);
                this.ChangeDescription = changes.Description;
            }

            var user = facility.Account.Users.Where(x => x.Guid.ToString() == domain.PerformedBy).FirstOrDefault();

            if (user != null)
            {
                if (user.SystemUser == true)
                {
                    this.PerformedBy = "System";
                }
                else
                {
                    this.PerformedBy = user.Login;
                }
            }
            else
            {
                this.PerformedBy = domain.PerformedBy;
            }

            /* Minutes */

            var tspan = DateTime.Now.Subtract(domain.PerformedAt);
            var minutes = tspan.TotalMinutes;
            var hours = tspan.TotalHours;

            this.Minutes = (int)minutes;

            TimeZoneInfo cstZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
            DateTime cstTime = TimeZoneInfo.ConvertTimeFromUtc(domain.PerformedAt, cstZone);

            if (domain.PerformedAt.Date == DateTime.Today)
            {


                if (hours > (double)2)
                {
                    this.TimespanDescription = string.Concat("Over ",Math.Floor(hours), " hours ago ", cstTime.ToString("HH:mm"), " CST");
                }
                else if (hours > (double)1)
                {
                    this.TimespanDescription = string.Concat("Over 1 hour ago ", cstTime.ToString("HH:mm"), " CST");
                }
                else
                {
                    this.TimespanDescription = string.Concat((int)minutes, " minutes ago ");
                }
            }
            else if (domain.PerformedAt.Date == DateTime.Today.AddDays(-1))
            {
                this.TimespanDescription = string.Concat("Yesterday at ", cstTime.ToString("HH:mm"), " CST");
            }
            else
            {
                this.TimespanDescription = string.Concat(cstTime.ToString("MM/dd/yy HH:mm"), " CST");
            }
            


            /* Determine component type */

            if (domain.TargetComponent.HasValue)
            {
                this.ComponentGuid = domain.TargetComponent.Value;
                this.Request.ComponentId = domain.TargetComponent.Value;

                if (domain.AuditType == Domain.Enumerations.AuditEntryType.AddedIncident ||
                    domain.AuditType == Domain.Enumerations.AuditEntryType.ModifiedIncident ||
                    domain.AuditType == Domain.Enumerations.AuditEntryType.RemovedIncident)
                {
                    var incident = incidentRepository.Get(domain.TargetComponent.Value);

                    if (incident != null)
                    {
                        this.IncidentId = incident.Id;
                    }
                }

                if (domain.AuditType == Domain.Enumerations.AuditEntryType.AddedInfection||
                    domain.AuditType == Domain.Enumerations.AuditEntryType.ModifiedInfection ||
                    domain.AuditType == Domain.Enumerations.AuditEntryType.RemovedInfection)
                {
                    var infection = infectionRepository.Get(domain.TargetComponent.Value);

                    if (infection != null)
                    {
                        this.InfectionId = infection.Id;
                    }
                }


                if (domain.AuditType == Domain.Enumerations.AuditEntryType.AddedCatheter ||
                domain.AuditType == Domain.Enumerations.AuditEntryType.ModifiedCatheter ||
                domain.AuditType == Domain.Enumerations.AuditEntryType.RemovedCatheter)
                {
                    var catheter = catheterRepository.Get(domain.TargetComponent.Value);

                    if (catheter != null)
                    {
                        this.CatheterId = catheter.Id;
                    }
                }

                if (domain.AuditType == Domain.Enumerations.AuditEntryType.AddedWound ||
                domain.AuditType == Domain.Enumerations.AuditEntryType.ModifiedWound ||
                domain.AuditType == Domain.Enumerations.AuditEntryType.RemovedWound)
                {
                    var wound = woundRepository.GetReport(domain.TargetComponent.Value);

                    if (wound != null)
                    {
                        this.WoundId = wound.Id;
                    }
                }
            }

        }
    }
}
