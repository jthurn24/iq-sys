using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using SnyderIS.sCore.Web.Mvc.JQGridHelpers;

namespace IQI.Intuition.Web.Models.EmployeeInfection
{
    [ModelBinder(typeof(AjaxRequestModelBinder<EmployeeInfectionGridRequest>))]
    public class EmployeeInfectionGridRequest : AjaxRequestModel<EmployeeInfectionGridRequest>
    {
        public int Id { get; set; }
        public string EmployeeName { get; set; }
        public int? InfectionTypeName { get; set; }
        public int? Department { get; set; }
        public string NotifiedOn { get; set; }
        public string WellOn { get; set; }

        public Expression<Func<Domain.Models.EmployeeInfection, object>> SortBy 
        {
            get
            {
                if (RequestedSortBy(model => model.EmployeeName))
                {
                    return infection => infection.FullName;
                }

                if (RequestedSortBy(model => model.InfectionTypeName))
                {
                    return infection => infection.InfectionType.Name;
                }

                if (RequestedSortBy(model => model.Department))
                {
                    return infection => infection.Department;
                }

                if (RequestedSortBy(model => model.NotifiedOn))
                {
                    return infection => infection.NotifiedOn;
                }

                if (RequestedSortBy(model => model.WellOn))
                {
                    return infection => infection.WellOn;
                }

                return i => i.Id;
            }
        }
    }
}
