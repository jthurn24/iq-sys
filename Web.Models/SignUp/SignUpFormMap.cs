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
    public class SignUpFormMap : ModelMap<SignUpForm, SignUpRequest>
    {
        public SignUpFormMap(
                        IModelMapper modelMapper)
        {
            ModelMapper = modelMapper.ThrowIfNullArgument("modelMapper");

            AutoFormatDisplayNames();


            ForProperty(model => model.EmailAddress)
                .Bind(x => x.EmailAddress)
                .DisplayName("Your email address")
                .Required();

            ForProperty(model => model.Name)
                .Bind(x => x.FirstName)
                .DisplayName("Your first and last name")
                .Required();

        }


        private IModelMapper ModelMapper { get; set; }


    }
}
