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
    public class SyncFreshBooksInvoices2 : IConsoleService
    {
        private ILog _Log;
        private IStatelessDataContext _DataContext;

        public SyncFreshBooksInvoices2(
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

            var activeMonth = new DateTime(DateTime.Today.AddMonths(-2).Year,DateTime.Today.AddMonths(-2).Month,1);
            var daysInMonth = activeMonth.AddMonths(1).AddDays(-1).Day;

            BuildAccountInvoices(activeMonth, daysInMonth, api);
            BuildFacilityInvoices(activeMonth, daysInMonth, api);

        }

        private void BuildAccountInvoices(DateTime activeMonth,
            int daysInMonth,
            Freshbooks.Library.FreshbooksApi api)
        {
                        
            var accountsToSync = _DataContext.CreateQuery<Account>()
                .FilterBy(x => x.FinancialId != null)
                .FilterBy(x => x.InActive == null || x.InActive == false)
                .FilterBy(x => x.FacilityInvoicing == null || x.FacilityInvoicing == false)
                .FetchAll();


            foreach (var account in accountsToSync)
            {
                var invoice = new Freshbooks.Library.Model.Invoice();
                invoice.Lines = new Freshbooks.Library.Model.LineItems();

                var id = new Freshbooks.Library.Model.ClientId((ulong)account.FinancialId);
                invoice.ClientId = id;

                var facilities = _DataContext.CreateQuery<Facility>()
                    .FilterBy(x => x.Account.Id == account.Id)
                    .FetchAll();

                foreach (var facility in facilities.Where(x => x.InActive == null || x.InActive == false))
                {
                    BuildInvoiceLines(facility, invoice, activeMonth, daysInMonth);
                }

                if (invoice.Lines.LineList.Count() > 0)
                {
                    api.Invoice.Create(new Freshbooks.Library.Model.InvoiceRequest() { Invoice = invoice });
                }

                
            }
        }


        private void BuildFacilityInvoices(DateTime activeMonth,
            int daysInMonth,
            Freshbooks.Library.FreshbooksApi api)
        {

            var facilitiesToSync = _DataContext.CreateQuery<Facility>()
                .FilterBy(x => x.FinancialId != null)
                .FilterBy(x => x.InActive == null || x.InActive == false)
                .FilterBy(x => x.Account.FacilityInvoicing == true)
                .FetchAll();

            foreach (var facility in facilitiesToSync)
            {
                var invoice = new Freshbooks.Library.Model.Invoice();
                invoice.Lines = new Freshbooks.Library.Model.LineItems();

                var id = new Freshbooks.Library.Model.ClientId((ulong)facility.FinancialId);
                invoice.ClientId = id;

                BuildInvoiceLines(facility, invoice, activeMonth, daysInMonth);

                if (invoice.Lines.LineList.Count() > 0)
                {
                    api.Invoice.Create(new Freshbooks.Library.Model.InvoiceRequest() { Invoice = invoice });
                }


            }

                
        }

        private void BuildInvoiceLines(Facility facility, 
            Freshbooks.Library.Model.Invoice invoice,
            DateTime activeMonth,
            int daysInMonth)
        {
            var fps = _DataContext.CreateQuery<FacilityProduct>()
                        .FilterBy(x => x.Facility.Id == facility.Id)
                        .FetchAll();

            var census = _DataContext.CreateQuery<PatientCensus>()
                .FilterBy(x => x.Month == activeMonth.Month)
                .FilterBy(x => x.Year == activeMonth.Year)
                .FilterBy(x => x.Facility.Id == facility.Id)
                .FetchAll()
                .FirstOrDefault();

            int bedCount = facility.MaxBeds.HasValue ? facility.MaxBeds.Value : 0;

            if (census != null && census.PatientDays > 0)
            {
                bedCount = (int)census.PatientDays / daysInMonth;
            }

            foreach (var fp in fps)
            {
                var product = _DataContext.Fetch<SystemProduct>(fp.SystemProduct.Id);

                if (fp.FeeType == Domain.Enumerations.ProductFeeType.MonthlyPatientCensus)
                {
                    string description = string.Concat(facility.Name, " - ", product.Name, " Avg Census - ", bedCount);

                    decimal fee = fp.Fee * bedCount;

                    if (fee > 0)
                    {
                        var line = new Freshbooks.Library.Model.LineItem();
                        line.UnitCost = (double)fp.Fee;
                        line.Quantity = bedCount;
                        line.Description = description;
                        invoice.Lines.LineList.Add(line);

                    }
                }

            }
        }
    }
}
