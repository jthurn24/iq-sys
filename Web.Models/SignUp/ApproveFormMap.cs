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

namespace IQI.Intuition.Web.Models.SignUp
{
    public class ApproveFormMap : ModelMap<ApproveForm, SignUpRequest>
    {
        public ApproveFormMap(
                        IModelMapper modelMapper)
        {
            ModelMapper = modelMapper.ThrowIfNullArgument("modelMapper");

            AutoFormatDisplayNames();


            ForProperty(model => model.FacilityName)
                .Bind(x => x.Name)
                .DisplayName("Name of your facility")
                .Required();

            ForProperty(model => model.State)
                .Bind(x => x.State)
                .DisplayName("State that your facility is located in")
                .Required();


            ForProperty(model => model.Password)
                .DisplayName("The password you wish to use")
                .Password()
                .Required();

            ForProperty(model => model.ConfirmPassword)
                .DisplayName("Confirm your password")
                .Password()
                .Required();


        }


        private IModelMapper ModelMapper { get; set; }


    }
}
