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

namespace IQI.Intuition.Web.Models.Administration.FacilityProduct
{
    public class FacilityProductAddFormMap : ModelMap<FacilityProductAddForm, Domain.Models.FacilityProduct>
    {
        protected ISystemRepository SystemRepository;

        public FacilityProductAddFormMap(ISystemRepository systemRepository)
        {
            SystemRepository = systemRepository;

            ForProperty(model => model.Fee)
                .Bind(domain => domain.Fee)
	            .DisplayName("Fee")
                .Required();
            
            ForProperty(model => model.FeeType)
                .Bind(domain => domain.FeeType)
	            .DisplayName("Fee Type")
                .EnumList()
                .Required();

            ForProperty(model => model.SystemProductId)
                .Bind(domain => domain.SystemProduct)
                    .OnRead(x => x.Id)
                    .OnWrite(GetSystemProduct)
	            .DisplayName("Product")
                .DropDownList(GetSystemProducts)
                .Required();

            ForProperty(model => model.StartOn)
                .Bind(domain => domain.StartOn)
                .DisplayName("Start On")
                .Required()
                .Default(DateTime.Today);
            
            ForProperty(model => model.Note)
                .Bind(domain => domain.Note)
	            .DisplayName("Note")
                .MultilineText();

        }

        private SystemProduct GetSystemProduct(int? id)
        {
            if(id.HasValue == false)
            {
                return null;
            }

            return SystemRepository.GetSystemProducts().Where(x => x.Id == id).FirstOrDefault();
        }

        private IEnumerable<SelectListItem> GetSystemProducts()
        {
            return SystemRepository.GetSystemProducts()
                .ToSelectListItems(x => x.Name, x => x.Id.ToString());
        }

    }
}


