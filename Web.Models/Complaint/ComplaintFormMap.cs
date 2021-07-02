using System;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Extensions.Formatting;
using RedArrow.Framework.Mvc.ModelMapper.Mapping;
using IQI.Intuition.Domain.Models;
using IQI.Intuition.Infrastructure.Services;
using System.Linq;
using IQI.Intuition.Web.Models.Extensions;
using System.Web.Mvc;
using System.Collections.Generic;
using IQI.Intuition.Domain.Repositories;
using System.Collections;

namespace IQI.Intuition.Web.Models.Complaint
{
    public class ComplaintFormMap : ModelMap<ComplaintForm, Domain.Models.Complaint>
    {
        private IActionContext ActionContext;
        private IComplaintRepository ComplaintRepository;

        public ComplaintFormMap(
            IActionContext actionContext,
            IComplaintRepository complaintRepository)
        {
            ActionContext = actionContext.ThrowIfNullArgument("actionContext");
            ComplaintRepository = complaintRepository.ThrowIfNullArgument("complaintRepository");


            ForProperty(x => x.Id)
                .Bind(x => x.Id);

            ForProperty(x => x.PatientId)
                .Read(x => x.Patient != null ? x.Patient.Id : (int?)null)
                .Write(SelectPatient);

            ForProperty(x => x.PatientName)
                .Read(x => x.Patient != null ? x.Patient.FullName : string.Empty);


            ForProperty(x => x.Patient2Id)
            .Read(x => x.Patient2 != null ? x.Patient2.Id : (int?)null)
            .Write(SelectPatient2);

            ForProperty(x => x.Patient2Name)
                .Read(x => x.Patient2 != null ? x.Patient2.FullName : string.Empty);

            ForProperty(x => x.EmployeeId)
                .Read(x => x.Employee != null ? x.Employee.Id : (int?)null)
                .Write(SelectEmployee);

            ForProperty(x => x.EmployeeName)
                .Read(x => x.Employee != null ? x.Employee.FullName : string.Empty);

            ForProperty(x => x.Employee2Id)
                .Read(x => x.Employee2 != null ? x.Employee2.Id : (int?)null)
                .Write(SelectEmployee2);

            ForProperty(x => x.Employee2Name)
                .Read(x => x.Employee2 != null ? x.Employee2.FullName : string.Empty);

            ForProperty(x => x.Wing)
                .Read(x => x.Wing != null ? x.Wing.Id : (int?)null)
                .Write(SelectWing)
                .DisplayName("Wing")
                .Required()
                .DropDownList(GetWings);

            ForProperty(x => x.ComplaintType)
                .Read(x => x.ComplaintType != null ? x.ComplaintType.Id : (int?)null)
                .Write(SelectComplaintType)
                .DisplayName("Complaint Type")
                .Required()
                .DropDownList(GetComplaintTypes);

            ForProperty(x => x.DateOccurred)
                .Bind(x => x.DateOccurred)
                .Required()
                .DisplayName("Date Occurred");

            ForProperty(x => x.DateReported)
                .Bind(x => x.DateReported)
                .Required()
                .DisplayName("Date Reported");

            ForProperty(x => x.ReportedBy)
                .Bind(x => x.ReportedBy)
                .DisplayName("Reported By");

            ForProperty(x => x.DescriptionText)
                .Bind(x => x.DescriptionText);

            ForProperty(x => x.ComplaintTypeDescriptions)
                .Default(GetComplaintTypeDescriptions);

            ForProperty(x => x.Cleared)
                .Bind(x => x.Cleared);

            ForProperty(x => x.Reported)
                .Bind(x => x.Reported);

        }

        private void SelectEmployee(Domain.Models.Complaint complaint, int? employeeId)
        {
            if (!employeeId.HasValue)
            {
                complaint.Employee = null;
                return;
            }

            complaint.Employee = ActionContext.CurrentFacility.Employees
                .Where(x => x.Id == employeeId.Value)
                .FirstOrDefault();
        }

        private void SelectEmployee2(Domain.Models.Complaint complaint, int? employee2Id)
        {
            if (!employee2Id.HasValue)
            {
                complaint.Employee2 = null;
                return;
            }

            complaint.Employee2 = ActionContext.CurrentFacility.Employees
                .Where(x => x.Id == employee2Id.Value)
                .FirstOrDefault();
        }

        private void SelectPatient(Domain.Models.Complaint complaint, int? patientId)
        {
            if (!patientId.HasValue)
            {
                complaint.Patient = null;
                return;
            }

            complaint.Patient = ActionContext.CurrentFacility.FindPatient(patientId.Value);
        }

        private void SelectPatient2(Domain.Models.Complaint complaint, int? patient2Id)
        {
            if (!patient2Id.HasValue)
            {
                complaint.Patient2 = null;
                return;
            }

            complaint.Patient2 = ActionContext.CurrentFacility.FindPatient(patient2Id.Value);
        }

        private void SelectWing(Domain.Models.Complaint complaint, int? wingId)
        {
            if (!wingId.HasValue)
            {
                complaint.Wing = null;
                return;
            }

            complaint.Wing =  ActionContext.CurrentFacility.Floors
                .SelectMany(x => x.Wings)
                .Where(x => x.Id == wingId)
                .FirstOrDefault();
        }


        private void SelectComplaintType(Domain.Models.Complaint complaint, int? typeId)
        {
            if (!typeId.HasValue)
            {
                complaint.ComplaintType = null;
                return;
            }

            complaint.ComplaintType = ComplaintRepository.AllTypes
                .Where(x => x.Id == typeId)
                .First();
        }

        private IEnumerable GetComplaintTypeDescriptions()
        {
            var descriptions = ComplaintRepository.AllTypes
                    .Select(x => new { Id = x.Id, Description = x.DescriptionText });

            return descriptions;
        }

        private IEnumerable<SelectListItem> GetComplaintTypes()
        {
            return ComplaintRepository.AllTypes
            .Select(xx => new SelectListItem() { Text = xx.Name, Value = xx.Id.ToString() })
            .Prepend(new SelectListItem() { Value = string.Empty, Text = "Select..."});
        }

        private IEnumerable<SelectListItem> GetWings()
        {
            var wings = ActionContext
                   .CurrentFacility
                   .Floors
                   .SelectMany(xx => xx.Wings)
                   .ToList();

            return wings
                .Select(xx => new SelectListItem() { Text = string.Concat(xx.Floor.Name, " - ", xx.Name), Value = xx.Id.ToString() })
                .Prepend(new SelectListItem() { Value = string.Empty, Text = "Select..." }); 
        }
    }
}
