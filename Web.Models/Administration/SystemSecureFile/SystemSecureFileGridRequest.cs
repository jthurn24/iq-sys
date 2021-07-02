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
using SnyderIS.sCore.Web.Mvc.JQGridHelpers;
using System.Linq.Expressions;

namespace IQI.Intuition.Web.Models.Administration.SystemSecureFile
{
    [ModelBinder(typeof(AjaxRequestModelBinder<SystemSecureFileGridRequest>))]
    public class SystemSecureFileGridRequest : AjaxRequestModel<SystemSecureFileGridRequest>
    {
		public int? Id { get; set; }
        public string Description { get; set; }
        public string ExpiresOn { get; set; }
        public string CreatedOn { get; set; }
        public string FileExtension { get; set; }

        public Expression<Func<Domain.Models.SystemSecureFile, object>> SortBy
        {
            get
            {
                if (RequestedSortBy(model => model.ExpiresOn))
                {
                    return x => x.ExpiresOn;
                }

                if (RequestedSortBy(model => model.CreatedOn))
                {
                    return x => x.CreatedOn;
                }

                if (RequestedSortBy(model => model.Description))
                {
	                return x => x.Description;
                }

                if (RequestedSortBy(model => model.FileExtension))
                {
	                return x => x.FileExtension;
                }

                return x => x.Id;
            }
        }
    }
}

