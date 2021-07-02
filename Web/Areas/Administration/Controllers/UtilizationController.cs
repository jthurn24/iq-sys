using System;
using System.Web.Mvc;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Mvc.ModelMapper;
using IQI.Intuition.Domain.Models;
using IQI.Intuition.Domain.Repositories;
using IQI.Intuition.Infrastructure.Services;
using IQI.Intuition.Web.Models.Administration.Utilization;
using System.Collections;
using IQI.Intuition.Web.Attributes;
using System.Collections.Generic;
using IQI.Intuition.Web.Extensions;
using IQI.Intuition.Reporting.Repositories;
using RedArrow.Framework.Mvc.Security;
using RedArrow.Framework.Persistence;

namespace IQI.Intuition.Web.Areas.Administration.Controllers
{
    [AnonymousAccess]
    public class UtilizationController : Controller
    {
        protected virtual IActionContext _ActionContext { get; private set; }
        protected virtual IDataContext _DataContext { get; private set; }

        public UtilizationController(
            IActionContext actionContext,
            IDataContext dataContext)
        {
            _ActionContext = actionContext.ThrowIfNullArgument("actionContext");
            _DataContext = dataContext;
        }

        public ActionResult Index(string key)
        {
            if(key != "jdskhdshkjdshjkdsiu2223hkhk1")
            {
                throw new Exception("Invalid key");
            }

            var model = new IndexView();
            model.Stats = new List<FacilityStats>();

            var facilities = _DataContext.CreateQuery<Facility>()
              .FilterBy(x => x.InActive == null || x.InActive == false)
              .FilterBy(x => x.Account.InActive == null || x.Account.InActive == false)
              .FetchAll();

            //foreach (var f in facilities.OrderBy(x => x.Account.Name))
            //{
            //    var item = new FacilityStats();
            //    item.AccountName = f.Account.Name;
            //    item.FacilityName = f.Name;

            //    var acc = _DataContext.Fetch<Account>(f.Account.Id);


            //    var infections = _DataContext.CreateQuery<InfectionVerification>()
            //        .FilterBy(x => x.Room.Wing.Floor.Facility.Id == f.Id)
            //        .FilterBy(x => x.CreatedAt.Value > DateTime.Today.AddMonths(-1))
            //        .Count();

            //    item.Infections = infections;

            //    var incidents = _DataContext.CreateQuery<IncidentReport>()
            //        .FilterBy(x => x.Room.Wing.Floor.Facility.Id == f.Id)
            //        .FilterBy(x => x.CreatedAt.Value > DateTime.Today.AddMonths(-1))
            //        .Count();

            //    item.Incidents = incidents;

            //    var wounds = _DataContext.CreateQuery<WoundReport>()
            //        .FilterBy(x => x.Room.Wing.Floor.Facility.Id == f.Id)
            //        .FilterBy(x => x.CreatedAt.Value > DateTime.Today.AddMonths(-1))
            //        .Count();

            //    item.Wounds = wounds;

            //    var psych = _DataContext.CreateQuery<PsychotropicAdministration>()
            //        .FilterBy(x => x.Patient.Room.Wing.Floor.Facility.Id == f.Id)
            //        .FilterBy(x => x.CreatedAt.Value > DateTime.Today.AddMonths(-1))
            //        .Count();

            //    item.Phsychotropics = psych;

            //    var cath = _DataContext.CreateQuery<CatheterEntry>()
            //        .FilterBy(x => x.Room.Wing.Floor.Facility.Id == f.Id)
            //        .FilterBy(x => x.CreatedAt.Value > DateTime.Today.AddMonths(-1))
            //        .Count();

            //    item.Catheters = cath;

            //    var eInfection = _DataContext.CreateQuery<EmployeeInfection>()
            //        .FilterBy(x => x.Facility.Id == f.Id)
            //        .FilterBy(x => x.CreatedAt.Value > DateTime.Today.AddMonths(-1))
            //        .Count();

            //    item.EmployeeInfections = eInfection;

            //    var complaints = _DataContext.CreateQuery<Complaint>()
            //        .FilterBy(x => x.Facility.Id == f.Id)
            //        .FilterBy(x => x.CreatedAt.Value > DateTime.Today.AddMonths(-1))
            //        .Count();

            //    item.Complaints = complaints;

            //    var activeLogins = _DataContext.CreateQuery<AuditEntry>()
            //        .FilterBy(x => x.Facility.Id == f.Id)
            //        .FilterBy(x => x.PerformedAt > DateTime.Today.AddMonths(-1))
            //        .FilterBy(x => x.AuditType == Domain.Enumerations.AuditEntryType.SuccessfulLogin)
            //        .FetchAll();

            //    item.TotalLogins = activeLogins.Count();

            //    var topUsers = activeLogins.GroupBy(x => x.PerformedBy).OrderByDescending(x => x.Count()).Take(5).Select(x => x.Key);

            //    var userAccounts = _DataContext.CreateQuery<AccountUser>()
            //        .FilterBy(x => x.Account.Id == f.Account.Id)
            //        .FetchAll();


            //    item.TopLogins = new List<String>();

            //    foreach(var login in topUsers)
            //    {
            //        var u = userAccounts.Where(xx => xx.Login == login).FirstOrDefault();

            //        if (u != null)
            //        {
            //            item.TopLogins.Add(string.Format("{0} {1}", u.FirstName, u.LastName));
            //        }
            //    }

            //    model.Stats.Add(item);
            //}

            model.Stats = model.Stats.OrderBy(x => x.AccountName).ToList();
            return View(model);
        }

        private List<string> BuildModuleList(DateTime startDate, DateTime endDate, int facilityId)
        {
            var model = new List<string>();

            var incidents = _DataContext.CreateQuery<IncidentReport>()
                   .FilterBy(x => x.Room.Wing.Floor.Facility.Id == facilityId)
                   .FilterBy(x => x.CreatedAt.Value >= startDate && x.CreatedAt <= endDate)
                   .Count();

            if (incidents > 0) model.Add("Incidents");

            var infections = _DataContext.CreateQuery<InfectionVerification>()
                   .FilterBy(x => x.Room.Wing.Floor.Facility.Id == facilityId)
                   .FilterBy(x => x.CreatedAt.Value >= startDate && x.CreatedAt <= endDate)
                   .Count();

            if (infections > 0) model.Add("Infections");

            var wounds = _DataContext.CreateQuery<WoundReport>()
                .FilterBy(x => x.Room.Wing.Floor.Facility.Id == facilityId)
                .FilterBy(x => x.CreatedAt.Value >= startDate && x.CreatedAt <= endDate)
                .Count();

            if (wounds > 0) model.Add("Wounds");

            var psych = _DataContext.CreateQuery<PsychotropicAdministration>()
                .FilterBy(x => x.Patient.Room.Wing.Floor.Facility.Id == facilityId)
                .FilterBy(x => x.CreatedAt.Value >= startDate && x.CreatedAt <= endDate)
                .Count();

            if (psych > 0) model.Add("Psych");

            var cath = _DataContext.CreateQuery<CatheterEntry>()
                .FilterBy(x => x.Room.Wing.Floor.Facility.Id == facilityId)
                .FilterBy(x => x.CreatedAt.Value >= startDate && x.CreatedAt <= endDate)
                .Count();

            if (cath > 0) model.Add("Catheter");

            var eInfection = _DataContext.CreateQuery<EmployeeInfection>()
                .FilterBy(x => x.Facility.Id == facilityId)
                .FilterBy(x => x.CreatedAt.Value >= startDate && x.CreatedAt <= endDate)
                .Count();

            if (eInfection > 0) model.Add("Employee Infection");

            var complaints = _DataContext.CreateQuery<Complaint>()
                .FilterBy(x => x.Facility.Id == facilityId)
                .FilterBy(x => x.CreatedAt.Value >= startDate && x.CreatedAt <= endDate)
                .Count();

            if (complaints > 0) model.Add("Complaints");


            return model;
        }
    }
}