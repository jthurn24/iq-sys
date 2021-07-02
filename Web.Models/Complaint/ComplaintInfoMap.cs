using System;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Extensions.Formatting;
using RedArrow.Framework.Mvc.ModelMapper.Mapping;
using IQI.Intuition.Domain.Models;

namespace IQI.Intuition.Web.Models.Complaint
{
    public class ComplaintInfoMap : ReadOnlyModelMap<ComplaintInfo, Domain.Models.Complaint>
    {
        public ComplaintInfoMap()
        {

            ForProperty(model => model.Id)
                .Read(domain => domain.Id.ToString());

            ForProperty(model => model.EmployeeName)
                .Read(GetEmployees);

            ForProperty(model => model.PatientName)
                .Read(GetPatients);

            ForProperty(model => model.DateOccurred)
                .Read(domain => domain.DateOccurred.FormatAsShortDate());

            ForProperty(model => model.DateReported)
                .Read(domain => domain.DateReported.FormatAsShortDate());

            ForProperty(model => model.Wing)
                .Read(domain => domain.Wing != null ? domain.Wing.Name  : string.Empty);

            ForProperty(model => model.ReportedBy)
                .Read(domain => domain.ReportedBy);

            ForProperty(model => model.ComplaintTypeName)
                .Read(domain => domain.ComplaintType.Name);
            
        }


        private string GetEmployees(Domain.Models.Complaint c)
        {
            var nameBuilder = new System.Text.StringBuilder();

            if (c.Employee != null)
            {
                nameBuilder.Append(c.Employee.FullName);
            }

            if (c.Employee2 != null)
            {
                if (nameBuilder.ToString() != string.Empty)
                {
                    nameBuilder.Append(" ");
                }

                nameBuilder.Append(c.Employee2.FullName);
            }

            return nameBuilder.ToString();
        }

        private string GetPatients(Domain.Models.Complaint c)
        {
            var nameBuilder = new System.Text.StringBuilder();

            if (c.Patient != null)
            {
                nameBuilder.Append(c.Patient.FullName);
            }

            if (c.Patient2 != null)
            {
                if (nameBuilder.ToString() != string.Empty)
                {
                    nameBuilder.Append(" ");
                }

                nameBuilder.Append(c.Patient2.FullName);
            }

            return nameBuilder.ToString();
        }
    }
}
