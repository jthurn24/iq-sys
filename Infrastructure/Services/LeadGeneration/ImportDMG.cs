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
using System.Data.OleDb;

namespace IQI.Intuition.Infrastructure.Services.LeadGeneration
{
    public class ImportDMG : IConsoleService
    {

        private ILog _Log;
        private IStatelessDataContext _DataContext;

        public ImportDMG(
            IStatelessDataContext dataContext,
            ILog log
            )
        {
            _DataContext = dataContext;
            _Log = log;
        }

        public void Run(string[] args)
        {
            OleDbConnection conn = null;
            OleDbDataReader reader = null;

            conn = new OleDbConnection(
            "Provider=Microsoft.ACE.OLEDB.12.0;" +
            "Data Source=c:\\DataMedicareGov.mdb");
            conn.Open();

            OleDbCommand cmd =
                new OleDbCommand("Select provnum,PROVNAME,CITY,STATE,BEDCERT,OWNERSHIP,INHOSP FROM ProviderInfo", conn);
            reader = cmd.ExecuteReader();

            while(reader.Read())
            {

                string provNum = reader[0].ToString();


                if (_DataContext.CreateQuery<SystemLead>()
                    .FilterBy(x => x.ProviderNumber == provNum)
                    .Exists() == false)
                {

                    System.Console.WriteLine("Importing prov # {0}", provNum);

                    var lead = new SystemLead();
                    lead.Name = reader[1].ToString();
                    lead.City = reader[2].ToString();
                    lead.State = reader[3].ToString();
                    lead.Beds = Convert.ToInt32(reader[4]);
                    lead.TypeOfOwnership = reader[5].ToString();
                    lead.Status = Domain.Enumerations.SystemLeadStatus.Call;
                    lead.MailingStatus = Domain.Enumerations.SystemLeadMailingStatus.NotAvailable;
                    lead.Details = string.Empty;

                    if (reader[6] == "YES")
                    {
                        lead.FacilityType = Domain.Enumerations.FacilityType.MultiComplex;
                    }
                    else
                    {
                        lead.FacilityType = Domain.Enumerations.FacilityType.SkilledNursing;
                    }

                    lead.ProviderNumber = reader[0].ToString();

                    lead.Source = "DMG";

                    _DataContext.Insert(lead);
                }

            }

            reader.Dispose();
            conn.Close();

        }
    }
}
