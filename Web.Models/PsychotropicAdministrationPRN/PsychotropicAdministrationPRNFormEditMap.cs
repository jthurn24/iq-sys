using System;
using System.Collections.Generic;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Mvc.Extensions;
using RedArrow.Framework.Mvc.ModelMapper.Mapping;
using RedArrow.Framework.Mvc.ModelMapper;
using IQI.Intuition.Domain.Models;
using IQI.Intuition.Domain.Repositories;
using System.Linq;
using System.Web.Mvc;
using IQI.Intuition.Web.Models.Extensions;

namespace IQI.Intuition.Web.Models.PsychotropicAdministrationPRN
{
    public class PsychotropicAdministrationPRNFormEditMap : ModelMap<PsychotropicAdministrationPRNFormEdit, Domain.Models.PsychotropicAdministrationPRN>
    {
        protected IPsychotropicRespository PsychotropicRespository { get; set; }

        public PsychotropicAdministrationPRNFormEditMap(
            IPsychotropicRespository psychotropicRespository)
        {

            PsychotropicRespository = psychotropicRespository;


            ForProperty(model => model.GivenOn)
                .Bind(domain => domain.GivenOn)
                .Required()
                .DisplayName("Given On");

            ForProperty(model => model.Dosage)
                .Bind(domain => domain.Dosage)
                .Required();

            ForProperty(model => model.Id)
                .Bind(domain => domain.Id)
                .Exclude(On.Create);

        }


    }
}
