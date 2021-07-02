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
using IQI.Intuition.Infrastructure.Services;

namespace IQI.Intuition.Web.Models.Wound
{
    public class WoundFormMap : ModelMap<WoundForm, Domain.Models.WoundReport>
    {
        protected IWoundRepository WoundRespository { get; set; }

        public WoundFormMap(
           IActionContext actionContext,
            IWoundRepository woundRespository)
        {

            WoundRespository = woundRespository;

            ForProperty(model => model.WoundReportId)
                .Read(domain => domain.Id)
                .Exclude(On.Create)
                .HiddenInput();



            ForProperty(model => model.WoundType)
                .Bind(domain => domain.WoundType)
                    .OnRead(x => x.Id)
                    .OnWrite(x => SelectType(x))
                .Required()
                .DropDownList(GetTypes)
                .DisplayName("Type");

            ForProperty(model => model.Site)
                .Bind(domain => domain.Site)
                    .OnRead(x => x.Id)
                    .OnWrite(x => SelectSite(x))
                .DropDownList(GetSites)
                .Required().ErrorMessage("You must select a valid site");


            ForProperty(model => model.FirstNoted)
                .Bind(domain => domain.FirstNotedOn)
                .DisplayName("First Noted")
                .Required();

            ForProperty(model => model.IsResolved)
                .Bind(domain => domain.IsResolved)
                .DisplayName("Resolved");

            ForProperty(model => model.ResolvedOn)
                .Bind(domain => domain.ResolvedOn)
                .DisplayName("On");

            ForProperty(model => model.LocationX)
                .Bind(domain => domain.LocationX);

            ForProperty(model => model.AdditionalSiteDetails)
                .Bind(domain => domain.AdditionalSiteDetails)
                .DisplayName("Additional Site Details")
                .MultilineText();

            ForProperty(model => model.LocationY)
                .Bind(domain => domain.LocationY);

            ForProperty(model => model.IsUpdateMode)
                .Read(domain => true)
                .HiddenInput();

            ForProperty(model => model.Classification)
                .Bind(x => x.Classification)
                .EnumList();

        }


        private Room SelectFacilityRoom(Facility facility, int? roomId)
        {
            if (!roomId.HasValue)
            {
                return null;
            }

            return facility.Floors
                .SelectMany(floor => floor.Wings)
                .SelectMany(wing => wing.Rooms)
                .SingleOrDefault(room => room.Id == roomId);
        }

        public WoundStage SelectStage(int? id)
        {
            if (id == null)
            {
                return null;
            }

            return WoundRespository.AllStages.Where(x => x.Id == id.Value).FirstOrDefault();
        }

        public WoundSite SelectSite(int? id)
        {
            if (id == null)
            {
                return null;
            }

            return WoundRespository.AllSites.Where(x => x.Id == id.Value).FirstOrDefault();
        }

        public WoundType SelectType(int? id)
        {
            if (id == null)
            {
                return null;
            }

            return WoundRespository.AllTypes.Where(x => x.Id == id.Value).FirstOrDefault();
        }

        public IEnumerable<SelectListItem> GetStages()
        {
            return WoundRespository.AllStages
                .Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() })
                .Prepend(new SelectListItem() { Text = string.Empty, Value = string.Empty });
        }

        public IEnumerable<SelectListItem> GetSites()
        {
            return WoundRespository.AllSites
                .Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() })
                .Prepend(new SelectListItem() { Text = string.Empty, Value = string.Empty });
        }

        public IEnumerable<SelectListItem> GetTypes()
        {
            return WoundRespository.AllTypes
                .Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() })
                .Prepend(new SelectListItem() { Text = string.Empty, Value = string.Empty });
        }
    }
}
