using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Mvc.Extensions;
using RedArrow.Framework.Mvc.ModelMapper.Mapping;
using RedArrow.Framework.Mvc.ModelMapper;
using IQI.Intuition.Domain.Models;
using IQI.Intuition.Domain.Repositories;
using IQI.Intuition.Infrastructure.Services;
using IQI.Intuition.Web.Models.Extensions;

namespace IQI.Intuition.Web.Models.Administration.Facility
{
    public class FacilityAddFormMap : ModelMap<FacilityAddForm, Domain.Models.Facility>
    {

        private IFacilityRepository _FacilityRepository { get; set; }

        public FacilityAddFormMap(
            IFacilityRepository facilityRepository)
        {

            _FacilityRepository = facilityRepository;

            ForProperty(model => model.Name)
                .Bind(domain => domain.Name)
	            .DisplayName("Name")
                .Required();

            ForProperty(model => model.SubDomain)
                .Bind(domain => domain.SubDomain)
	            .DisplayName("Sub Domain")
                .Verify(ValidateDomainAvailability).ErrorMessage("Sub domain is already in use")
                .Verify(ValidateDomainValidity).ErrorMessage("Sub domain must no contain spaces or special characters")
                .Required();

            ForProperty(model => model.State)
                .Bind(domain => domain.State)
	            .DisplayName("State")
                .Required();

            ForProperty(model => model.FacilityType)
                .Bind(domain => domain.FacilityType)
                .EnumList()
                .DisplayName("Type");

            ForProperty(model => model.MaxBeds)
                .Bind(domain => domain.MaxBeds)
                .DisplayName("Capacity (Beds)");

        }

        private bool ValidateDomainAvailability(FacilityAddForm form, string value)
        {
            if (this._FacilityRepository.Get(form.SubDomain) != null)
            {
                return false;
            }

            return true;
        }

        private bool ValidateDomainValidity(FacilityAddForm form, string value)
        {
            if (form.SubDomain.IsNullOrEmpty() || form.SubDomain.ContainsAny(new char[] { ' ', '@', '#','!','$','^','&','*','(',')' }.ToList()))
            {
                return false;
            }

            return true;
        }

    }
}


