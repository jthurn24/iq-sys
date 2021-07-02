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

namespace IQI.Intuition.Web.Models.Administration.SystemTicket
{
    public class SystemTicketAddFormMap : ModelMap<SystemTicketAddForm, Domain.Models.SystemTicket>
    {
        private ISystemTicketRepository SystemTicketRepository;
        private ISystemRepository SystemRepository;

        public SystemTicketAddFormMap(
            ISystemTicketRepository systemTicketRepository,
            ISystemRepository systemRepository)
        {
            SystemTicketRepository = systemTicketRepository;
            SystemRepository = systemRepository;


            ForProperty(model => model.SystemTicketType)
                .Bind(domain => domain.SystemTicketType)
                    .OnRead( x=> x.Id)
                    .OnWrite(GetSystemTicketType)
	            .DisplayName("Ticket Type")
                .DropDownList(GenerateTicketTypes);

            ForProperty(model => model.SystemTicketStatus)
                .Bind(domain => domain.Status)
	            .DisplayName("Ticket Status")
                .EnumList();

            ForProperty(model => model.Details)
                .Bind(domain => domain.Details)
	            .DisplayName("Details");

            ForProperty(model => model.Priority)
                .Bind(domain => domain.Priority)
	            .DisplayName("Priority")
                .DropDownList(GeneratePriorityLevels);

            ForProperty(model => model.Release)
                .Bind(domain => domain.Release)
                .DisplayName("Release");

            ForProperty(model => model.SystemUser)
                .Bind(domain => domain.SystemUser)
                    .OnRead( x=> x.Id)
                    .OnWrite(GetSystemUser)
	            .DisplayName("Assigned To")
                .DropDownList(GetSystemUsers);


        }

        private IEnumerable<SelectListItem> GenerateTicketTypes()
        {
            return SystemTicketRepository.AllTicketTypes
                .ToSelectListItems(x => x.Name, x => x.Id);
        }

        private IEnumerable<SelectListItem> GeneratePriorityLevels()
        {
            return new List<SelectListItem>() { 
                new SelectListItem() { Text = "1", Value = "1" },
                new SelectListItem() { Text = "2", Value = "2" },
                new SelectListItem() { Text = "3", Value = "3" },
                new SelectListItem() { Text = "4", Value = "4" },
                new SelectListItem() { Text = "5", Value = "5" },
                new SelectListItem() { Text = "6", Value = "6" },
                new SelectListItem() { Text = "7", Value = "7" },
                new SelectListItem() { Text = "8", Value = "8" },
                new SelectListItem() { Text = "9", Value = "9" },
                new SelectListItem() { Text = "10", Value = "10" }
            };
        }

        public SystemTicketType GetSystemTicketType(int? id)
        {
            if (id.HasValue == false)
            {
                return null;
            }

            return SystemTicketRepository.AllTicketTypes.Where(x => x.Id == id.Value).FirstOrDefault();
        }

        private IEnumerable<SelectListItem> GetSystemUsers()
        {
            return SystemRepository
                .GetSystemUsers()
                .ToSelectListItems(xx => xx.Login, xx => xx.Id)
                .Prepend(new SelectListItem() { Text = string.Empty, Value = string.Empty });
        }

        private SystemUser GetSystemUser(int? id)
        {
            if (id.HasValue == false)
            {
                return null;
            }

            return SystemRepository.GetSystemUsers().Where(x => x.Id == id.Value).FirstOrDefault();
        }

    }
}


