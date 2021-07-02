using System;
using System.Collections.Generic;
using IQI.Intuition.Domain.Models;

namespace IQI.Intuition.Infrastructure.Services
{
    public interface IActionContext
    {
        Account CurrentAccount { get; }

        Facility CurrentFacility { get; }

        AccountUser CurrentUser { get; }

        SystemUser CurrentSystemUser { get; }

        string BuildVersion { get; }

        string ServerName { get; }

        string EnvironmentName { get; }

        IDictionary<String,Object> MappingArguments { get; }

        void SetUserMessage(string message);
        void SendEmailNotification(string to, string subject, string message);
        void SendEmailNotification(string subject, string message);

        string GetUserMessage();
        void ClearUserMessage();

    }
}
