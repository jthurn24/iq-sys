using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace IQI.Intuition.Web.Models.Administration.SystemTicket
{
    public class SystemTicketList
    {
        public int? PageNumber { get; set;  }
        public int? PageSize { get; set;  }
        public IEnumerable<SystemTicketGridItem> PageValues { get; set; }
        public int TotalPages { get; set;  }
        public int TotalResults { get; set;  }
        public bool MyTickets { get; set; }
        public string Account { get; set; }
        public string AccountUser { get; set; }
        public string SystemTicketType { get; set; }
        public IEnumerable<SelectListItem> SystemTicketTypeOptions { get; set; }
        public string Details { get; set; }
        public int? Priority { get; set; }
        public string Release { get; set; }


        public string GetHeaderColor(SystemTicketGridItem item)
        {
            if (item.SystemTicketType == "Action Item")
            {
                return "#d5e0be";
            }
            else if(item.SystemTicketType == "Feature Request")
            {
                return "#bebfe0";
            }

            return "#e0c1be";
        }
    }
}
