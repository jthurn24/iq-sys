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

namespace IQI.Intuition.Web.Models.Administration.SystemContact
{
    public class SystemContactEditFormMap : ModelMap<SystemContactEditForm, Domain.Models.SystemContact>
    {

        public SystemContactEditFormMap()
        {
            AutoFormatDisplayNames();

            ForProperty(model => model.FirstName)
                .Bind(domain => domain.FirstName)
	            .DisplayName("First Name");

            ForProperty(model => model.LastName)
                .Bind(domain => domain.LastName)
	            .DisplayName("Last Name");

            ForProperty(model => model.Title)
                .Bind(domain => domain.Title)
	            .DisplayName("Title");

            ForProperty(model => model.Cell)
                .Bind(domain => domain.Cell)
	            .DisplayName("Cell");

            ForProperty(model => model.Direct)
                .Bind(domain => domain.Direct)
	            .DisplayName("Direct");

            ForProperty(model => model.Email)
                .Bind(domain => domain.Email)
	            .DisplayName("Email");

            ForProperty(model => model.Notes)
                .Bind(domain => domain.Notes)
	            .DisplayName("Notes");

        }

    }
}


