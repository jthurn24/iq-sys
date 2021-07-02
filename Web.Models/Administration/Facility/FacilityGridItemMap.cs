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

	public class FacilityGridItemMap : ReadOnlyModelMap<FacilityGridItem, Domain.Models.Facility>
	{
        private IPatientRepository PatientRepository { get; set; }

        public FacilityGridItemMap(IPatientRepository patientRepository)
		{
            PatientRepository = patientRepository;

			AutoConfigure();

			ForProperty(model => model.Id)
				.Read(domain => domain.Id);

            ForProperty(model => model.Guid)
	            .Read(domain => domain.Guid);

            ForProperty(model => model.Name)
	            .Read(domain => domain.Name);

            ForProperty(model => model.SubDomain)
	            .Read(domain => domain.SubDomain);

            ForProperty(model => model.State)
	            .Read(domain => domain.State);

            ForProperty(model => model.PatientCount)
                .Read(domain => PatientRepository
                    .Find(domain)
                    .Where(x => x.Deleted != true && x.CurrentStatus == Domain.Enumerations.PatientStatus.Admitted)
                    .Count())
                ;

		}
	}
}

