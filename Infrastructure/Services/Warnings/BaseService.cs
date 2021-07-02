using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQI.Intuition.Reporting.Repositories;
using IQI.Intuition.Domain.Repositories;
using IQI.Intuition.Domain.Models;
using RedArrow.Framework.Persistence;
using RedArrow.Framework.Logging;
using SnyderIS.sCore.Persistence;
using RedArrow.Framework.Extensions.Common;

namespace IQI.Intuition.Infrastructure.Services.Warnings
{
    public abstract class BaseService : IWarningService
    {
        public static IWarningService Activate(WarningRule rule)
        {
            var t = System.Type.GetType(rule.TypeName);
            var service = (IWarningService)System.Activator.CreateInstance(t);
            return service;
        }

        public abstract string DescribeRuleType(WarningRule rule, IDataContext dataContext);
        public abstract IDictionary<string, string> DescribeArguments(WarningRule rule, IDataContext dataContext);

        public abstract void Run(
            WarningRule rule,
            Facility facility,
            ILog log,
            IStatelessDataContext dataContext);

        protected bool WarningExists(
            IStatelessDataContext dataContext,
            WarningRule warningRule, 
            int? facilityID, 
            int? patientID, 
            DateTime tiggeredOn)
        {
            var q = dataContext.CreateQuery<Warning>().FilterBy(x => x.WarningRule.Id == warningRule.Id);

            if(facilityID.HasValue)
            {
                q = q.FilterBy(x => x.Facility.Id == facilityID);
            }

            if(patientID.HasValue)
            {
                q = q.FilterBy(x => x.Patient.Id == patientID);
            }

            q = q.FilterBy(x => x.TriggeredOn == tiggeredOn);

            var record = q.FetchAll().FirstOrDefault();

            if (record == null)
            {
                return false;
            }

            return true;
        }

        protected void AddWarning(
            IStatelessDataContext dataContext,
            WarningRule warningRule,
            int? facilityID, 
            int? patientID, 
            DateTime tiggeredOn, 
            Dictionary<string,string> descriptionTokens, 
            List<Dictionary<string,string>> itemTokens,
            ILog log)
        {
            var warning = new Warning();

            string title = warningRule.Title;
            string description = warningRule.Description;

            foreach (var key in warningRule.ParsedArguments.Keys)
            {
                title = title.Replace(string.Concat("{", key, "}"), warningRule.ParsedArguments[key]);
                description = description.Replace(string.Concat("{", key, "}"), warningRule.ParsedArguments[key]);
            }

            foreach (var key in descriptionTokens.Keys)
            {
                title = title.Replace(string.Concat("{", key, "}"), descriptionTokens[key]);
                description = description.Replace(string.Concat("{", key, "}"), descriptionTokens[key]);
            }

            if (warningRule.ItemTemplate.IsNullOrEmpty() || itemTokens == null)
            {
                description = description.Replace("{Items}", "AAAA");
            }
            else
            {
                var itemBuilder = new StringBuilder();

                foreach(var itemToken in itemTokens)
                {
                    string template = warningRule.ItemTemplate;

                    foreach (string key in itemToken.Keys)
                    {
                        template = template.Replace(string.Concat("{", key, "}"), itemToken[key]);
                    }

                    itemBuilder.Append(template);
                }

                description = description.Replace("{Items}", itemBuilder.ToString());
            }

            warning.DescriptionText = description;
            warning.Title = title;

            if (facilityID.HasValue)
            {
                warning.Facility = dataContext.Fetch<Facility>(facilityID.Value);
                warning.Account = warning.Facility.Account;
            }

            if (patientID.HasValue)
            {
                warning.Patient = dataContext.Fetch<Patient>(patientID.Value);
                warning.Account = warning.Patient.Account;
            }

            warning.TriggeredOn = tiggeredOn;
            warning.WarningRule = warningRule;
            warning.Title = title;
            warning.DescriptionText = description;

            dataContext.Insert(warning);

            log.Info("Alert generated: {0}", warningRule.TypeName);

            var notifications = dataContext.CreateQuery<WarningRuleNotification>()
                .FilterBy(x => x.WarningRule.Id == warningRule.Id)
                .FilterBy(x => x.AccountUser.IsActive == true)
                .FetchAll();

            foreach (var notification in notifications)
            {
                var user = dataContext.Fetch<AccountUser>(notification.AccountUser.Id);

                if (user.NotificationMethod == Domain.Enumerations.NotificationMethod.CellPhone ||
                    user.NotificationMethod == Domain.Enumerations.NotificationMethod.CellPhoneAndEmail)
                {
                    if (user.CellPhone.IsNotNullOrEmpty())
                    {
                        var smsNotification = new SystemSMSNotification();
                        smsNotification.AllowAfterHours = false;
                        smsNotification.SendTo = user.CellPhone;
                        smsNotification.Message = this.DescribeRuleType(warningRule, dataContext);
                        dataContext.Insert(smsNotification);
                    }
                }

                if (user.NotificationMethod == Domain.Enumerations.NotificationMethod.Email ||
                    user.NotificationMethod == Domain.Enumerations.NotificationMethod.CellPhoneAndEmail)
                {
                    if (user.EmailAddress.IsNotNullOrEmpty())
                    {
                        var emailNotification = new SystemEmailNotification();
                        emailNotification.SendTo = user.EmailAddress;
                        emailNotification.Subject = "System alert notification";
                        emailNotification.MessageText = this.DescribeRuleType(warningRule, dataContext);
                        dataContext.Insert(emailNotification);
                    }
                }

            }


        }

    }
}
