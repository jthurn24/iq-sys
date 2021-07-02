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
using SnyderIS.sCore.Console;

namespace IQI.Intuition.Infrastructure.Services.LeadGeneration
{
    //public class SurveyDeficiencyMonitorService : IConsoleService
    //{
    //    private ILog _Log;
    //    private IStatelessDataContext _DataContext;

    //    public SurveyDeficiencyMonitorService(
    //        IStatelessDataContext dataContext,
    //        ILog log
    //        )
    //    {
    //        _DataContext = dataContext;
    //        _Log = log;
    //    }

    //    public void Run(string[] args)
    //    {
    //        var xml = new System.Xml.XmlDocument();
    //        xml.Load("http://data.medicare.gov/api/views/47ne-48x9/rows.xml");

    //        var newLeads = new List<Domain.Models.SystemLead>();

    //        var leadNodes = xml.SelectNodes("/response/row/row");

    //        foreach (System.Xml.XmlNode node in leadNodes)
    //        {
    //            var nameNode = node.SelectSingleNode("nursing_home_name");
    //            var streetNode = node.SelectSingleNode("street");
    //            var cityNode = node.SelectSingleNode("city");
    //            var stateNode = node.SelectSingleNode("state");
    //            var zipNode = node.SelectSingleNode("zip_code");
    //            var phoneNode = node.SelectSingleNode("phone");
    //            var surveyDateNode = node.SelectSingleNode("survey_date");
    //            var dateOfCorrectionNode = node.SelectSingleNode("date_of_correction");
    //            var defficiencyNode = node.SelectSingleNode("deficiency");

    //            string name = nameNode != null ? nameNode.InnerText : string.Empty;
    //            string street = streetNode != null ? streetNode.InnerText : string.Empty;
    //            string city = cityNode != null ? cityNode.InnerText : string.Empty;
    //            string state = stateNode != null ? stateNode.InnerText : string.Empty;
    //            string zip = zipNode != null ? zipNode.InnerText : string.Empty;
    //            string survey = surveyDateNode != null ? surveyDateNode.InnerText : string.Empty;
    //            string dateOfCorrection = dateOfCorrectionNode != null ? dateOfCorrectionNode.InnerText : string.Empty;
    //            string defficiency = defficiencyNode != null ? defficiencyNode.InnerText : string.Empty;

    //            var exisitingLead = _DataContext.CreateQuery<SystemLead>()
    //                .FilterBy(x => x.Name == name 
    //                && x.City == city
    //                && x.State == state)
    //                .FetchAll().FirstOrDefault();


    //            if (exisitingLead == null)
    //            {
    //                var lead = new Domain.Models.SystemLead();
    //                lead.Source = "Data.gov";
    //                lead.Status = Domain.Enumerations.SystemLeadStatus.WaitingForCitationMailing;
    //                lead.State = state;
    //                lead.Name = name;
    //                lead.City = city;
    //                lead.CreatedOn = DateTime.Today;
    //                lead.Details = string.Concat(
    //                    "Address: ",street,"  ",city," ",state," ",zip, 
    //                    Environment.NewLine,
    //                    Environment.NewLine,
    //                    "Date Of Correction: ",dateOfCorrection,
    //                    Environment.NewLine,
    //                    "Date Of Survey; ", survey,
    //                    Environment.NewLine,
    //                    Environment.NewLine,
    //                    defficiency);

    //                _DataContext.Insert(lead);
    //                newLeads.Add(lead);
    //            }
    //        }

    //        if (newLeads.Count() > 0)
    //        {
    //            var messageBuilder = new StringBuilder();

    //            foreach (var lead in newLeads)
    //            {
    //                messageBuilder.AppendLine("-------- New Citation Lead ---------");
    //                messageBuilder.AppendLine(lead.Name);
    //                messageBuilder.Append(lead.Details);
    //                messageBuilder.Append(Environment.NewLine);
    //                messageBuilder.Append(Environment.NewLine);
    //            }

    //            var email = new SystemEmailNotification();
    //            email.SendTo = "team@iqisystems.com";
    //            email.Subject = "New Citation Leads (from Data.gov)";
    //            email.MessageText = messageBuilder.ToString();
    //            _DataContext.Insert(email);
    //        }
    //    }

    //}
}
