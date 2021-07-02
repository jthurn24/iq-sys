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
using Freshbooks.Library;

namespace IQI.Intuition.Infrastructure.Services.Utilities
{
    public class SyncFreshBooks : IConsoleService
    {
        private ILog _Log;
        private IStatelessDataContext _DataContext;

        public SyncFreshBooks(
            IStatelessDataContext dataContext,
            ILog log
            )
        {
            _DataContext = dataContext;
            _Log = log;
        }

        public void Run(string[] args)
        {
            var api = new Freshbooks.Library.FreshbooksApi("iqisystemsllc", "developer-account");
            api.UseLegacyToken("0ad06d9f3d01b76da688186c6ea39844");

            var accountsToSync = _DataContext.CreateQuery<Account>()
                .FilterBy(x => x.FinancialId == null)
                .FilterBy(x => x.InActive == null || x.InActive == false)
                .FetchAll();


            foreach (var account in accountsToSync)
            {

                if (account.FacilityInvoicing != true && account.FinancialId.HasValue == false)
                {

                    var c = new Freshbooks.Library.Model.Client();
                    c.Organization = account.Name;
                    c.Email = "mark@iqisystems.com";

                    var id = api.Client.Create(new Freshbooks.Library.Model.ClientRequest() { Client = c });

                    account.FinancialId = (long)id.ClientId.Value;
                    _DataContext.Update(account);
                }
                else if (account.FacilityInvoicing == true)
                {
                    var facilities = _DataContext.CreateQuery<Facility>()
                        .FilterBy(x => x.Account.Id == account.Id)
                        .FilterBy(x => x.FinancialId.HasValue == false)
                        .FilterBy(x => x.InActive == null || x.InActive == false)
                        .FetchAll();


                    foreach (var facility in facilities)
                    {
                        var c = new Freshbooks.Library.Model.Client();
                        c.Organization = string.Concat(account.Name,"-",facility.Name);
                        c.Email = "mark@iqisystems.com";

                        var id = api.Client.Create(new Freshbooks.Library.Model.ClientRequest() { Client = c });

                        facility.FinancialId = (long)id.ClientId.Value;
                        _DataContext.Update(facility);
                    }
                }

            }
  
        }
    }
}
