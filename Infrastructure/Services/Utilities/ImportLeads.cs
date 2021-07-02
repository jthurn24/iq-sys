using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQI.Intuition.Reporting.Repositories;
using IQI.Intuition.Domain.Repositories;
using RedArrow.Framework.Persistence;
using RedArrow.Framework.Logging;
using IQI.Intuition.Domain.Models;
using SnyderIS.sCore.Persistence;
using System.Text.RegularExpressions;

namespace IQI.Intuition.Infrastructure.Services.Utilities
{
    public class ImportLeads
    {
        private ILog _Log;
        private IStatelessDataContext _DataContext;

        public ImportLeads(
            IStatelessDataContext dataContext,
            ILog log
            )
        {
            _DataContext = dataContext;
            _Log = log;
        }

        public void Run(string[] args)
        {
             var data = System.IO.File.ReadAllLines("c:\\leads.csv");

             foreach (var line in data)
             {
                 var fields = line.Split(",".ToCharArray());
                 var firstName = fields[0];
                 var lastName = fields[1];
                 var title = fields[2];
                 var organization = fields[3];
                 var email = fields[4];
                 var phone = fields[5];
                 var city = fields[6];
                 var state = fields[7];


                 var lead = _DataContext.CreateQuery<SystemLead>()
                     .FilterBy(x => x.Name == organization).FetchAll().FirstOrDefault();

                 if (lead == null)
                 {
                     lead = new SystemLead();
                     lead.Name = organization;
                     lead.Source = "N/A";
                     lead.Status = Domain.Enumerations.SystemLeadStatus.Hot;
                     lead.State = state;
                     lead.City = city;
                     lead.CreatedOn = DateTime.Today;
                     lead.LastContactedOn = new DateTime(2012, 1, 1);
                     lead.Details = string.Empty;
                     _DataContext.Insert(lead);

                     System.Console.WriteLine(string.Concat("Added lead ",lead.Name));
                 }

                 var contact = new SystemContact();
                 contact.FirstName = firstName;
                 contact.LastName = lastName;
                 contact.Title = title;
                 contact.Email = email;
                 contact.Direct = phone;
                 contact.Notes = string.Empty;
                 contact.SystemLead = lead;

                 _DataContext.Insert(contact);

                 System.Console.WriteLine(string.Concat("Added contact ", contact.FirstName," ",contact.LastName));
             }
        }
    }
}
