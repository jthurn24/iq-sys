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
    public class FacilityEditFormMap : ModelMap<FacilityEditForm, Domain.Models.Facility>
    {
        private IFacilityRepository _FacilityRepository { get; set; }

        public FacilityEditFormMap(
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

            ForProperty(model => model.AccountId)
                .Bind(domain => domain.Account.Id);

            ForProperty(model => model.FacilityType)
                .Bind(domain => domain.FacilityType)
                .EnumList()
                .DisplayName("Type");

            ForProperty(model => model.MaxBeds)
                .Bind(domain => domain.MaxBeds)
                .DisplayName("Capacity (Beds)");

        }

        private bool ValidateDomainAvailability(FacilityEditForm form, string value)
        {
            var exisiting = this._FacilityRepository.Get(form.SubDomain);

            if (exisiting != null && exisiting.Id != form.Id)
            {
                return false;
            }

            return true;
        }

        private bool ValidateDomainValidity(FacilityEditForm form, string value)
        {
            if (form.SubDomain.IsNullOrEmpty() || form.SubDomain.ContainsAny(new char[] { ' ', '@', '#', '!', '$', '^', '&', '*', '(', ')' }.ToList()))
            {
                return false;
            }

            return true;
        }

    }
}


