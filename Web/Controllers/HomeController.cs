using System;
using System.Web.Mvc;
using System.Linq;
using System.Collections.Generic;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Mvc.ModelMapper;
using IQI.Intuition.Infrastructure.Services;
using IQI.Intuition.Web.Models.Home;
using RedArrow.Framework.Mvc.Extensions;
using RedArrow.Framework.Mvc.Security;
using IQI.Intuition.Web.Attributes;
using IQI.Intuition.Domain;
using IQI.Intuition.Domain.Repositories;
using RedArrow.Framework.Extensions.Formatting;
using RedArrow.Framework.Persistence;
using IQI.Intuition.Domain.Models;

namespace IQI.Intuition.Web.Controllers
{

    public class HomeController : Controller
    {
        public HomeController(
            IActionContext actionContext,
            IModelMapper modelMapper,
            ISystemRepository systemRepository,
            IAuthentication authentication,
            IWarningRepository warningRepository,
            IDataContext dataContext)
        {
            ActionContext = actionContext.ThrowIfNullArgument("actionContext");
            ModelMapper = modelMapper.ThrowIfNullArgument("modelMapper");
            SystemRepository = systemRepository.ThrowIfNullArgument("systemRepository");
            Authentication = authentication.ThrowIfNullArgument("authentication");
            WarningRepository = warningRepository.ThrowIfNullArgument("warningRepository");
            DataContext = dataContext;
        }

        protected virtual IActionContext ActionContext { get; private set; }

        protected virtual IModelMapper ModelMapper { get; private set; }

        protected virtual ISystemRepository SystemRepository { get; private set; }

        protected virtual IAuthentication Authentication { get; private set; }
        
        protected virtual IWarningRepository WarningRepository { get; set; }

        protected virtual IDataContext DataContext { get; set; }

        [HttpGet, RequiresActiveAccount]
        public ActionResult Index()
        {
            if (ActionContext.CurrentUser == null)
            {
                // This can happen if the user is logged into a different account (should handle this elsewhere, of course)
                return RedirectToAction("SignOut", "Authentication");
            }

            var model = ModelMapper.MapForReadOnly<Dashboard>(ActionContext);
            
            return View(model);

            //return RedirectToAction("Index", "QIDashboard");
        }

        //public ActionResult Test()
        //{
        //    DateTime startDate = DateTime.Today.AddMonths(-12);
        //    DateTime curentDate = startDate;

        //    while (curentDate <= DateTime.Today)
        //    {

        //        var incidentTime = new DateTime(curentDate.Year, curentDate.Month, curentDate.Day,
        //            (int)new Random().Next(24),
        //            (int)new Random().Next(60),
        //            0);

        //        var patient = DataContext.CreateQuery<Patient>()
        //        .FilterBy(x => x.CurrentStatus == Domain.Enumerations.PatientStatus.Admitted
        //        && x.Room.Wing.Floor.Facility.Id == ActionContext.CurrentFacility.Id)
        //        .FetchAll()
        //        .Shuffle()
        //        .Last();

        //        var report = new IncidentReport(patient);
        //        report.Room = patient.Room;

        //        var injury = DataContext.CreateQuery<IncidentInjury>().FetchAll().Shuffle().First();
        //        var type = DataContext.CreateQuery<IncidentType>().FetchAll().Where(x => x.Name.Contains("Fall")).Shuffle().First();

        //        report.AssignTypes(new IncidentType[] { type });
        //        report.AssignInjuries(new IncidentInjury[] { injury });

        //        report.IncidentLocation = DataContext.CreateQuery<IncidentLocation>().FetchAll().Shuffle().Last();
        //        report.InjuryLevel = (Enumerations.InjuryLevel)(new Random().Next(3) + 1);

        //        report.DiscoveredOn = incidentTime;
        //        report.OccurredOn = incidentTime;

        //        DataContext.TrackChanges(report);

        //        curentDate = curentDate.AddDays((int)new Random().Next(4) + 1);
        //    }

        //    return View();
        //}


        public ActionResult HideWarning(int id)
        {
            var warning = WarningRepository.Get(id);
            warning.AddHiddenBy(ActionContext.CurrentUser.Id);
            return RedirectToAction("Index");

        }

        [AnonymousAccess, RequiresActiveAccount]
        public ActionResult FacilityPanel()
        {
            if (ActionContext.CurrentFacility == null)
            {
                return null; // The panel is not shown if there is no active facility
            }

            var model = new FacilityInfo();
            model.AccountName = ActionContext.CurrentFacility.Account.Name;
            model.SubDomain = ActionContext.CurrentFacility.SubDomain;

            if (ControllerContext.HttpContext.Request.IsSecureConnection)
            {
                model.Protocol = "https";
            }
            else
            {
                model.Protocol = "http";
            }


            if (ActionContext.CurrentUser == null)
            {
                var options = new List<SelectListItem>();

                options.Add(
                    new SelectListItem()
                    {
                        Text = ActionContext.CurrentFacility.Name,
                        Value = ActionContext.CurrentFacility.SubDomain
                    });

                model.FacilityOptions = options;
            }
            else if (ActionContext.CurrentUser.SystemUser)
            {
                model.FacilityOptions =
                    ActionContext.CurrentAccount.Facilities.Where(x => x.InActive != true)
                    .ToSelectListItems(facility => facility.Name, facility => facility.SubDomain)
                    .ToArray();
            }
            else
            {
                model.FacilityOptions =
                    ActionContext.CurrentUser.Facilities.Where(x => x.InActive != true)
                    .ToSelectListItems(facility => facility.Name, facility => facility.SubDomain)
                    .ToArray();
            }



            return PartialView(model);
        }

        [HttpGet, RequiresActiveAccount]
        [AnonymousAccess]
        public ActionResult Error()
        {
            return View();
        }

        [AnonymousAccess, AllowCrossSiteJson]
        public ActionResult TPV(string page, string title, string url)
        {

            var userGuid = this.Authentication.CurrentUserGuid;


            if (userGuid.HasValue)
            {

                var user = SystemRepository.GetUserByGuid(userGuid);      

                ////var record = new Domain.Models.SystemPageView();
                ////record.Account = user.Account;
                ////record.AccountUser = user;
                ////record.Browser = string.Concat(Request.Browser.Browser, " ",Request.Browser.Version);
                ////record.ViewedOn = DateTime.Now;
                ////record.Title = title;
                ////record.Page = page;
                ////record.Url = url;
                ////record.IPAddress = Request.UserHostAddress;
                ////SystemRepository.Add(record);

                user.LastIpAddress = Request.UserHostAddress;


            }

            return new JsonResult() { Data = userGuid , JsonRequestBehavior= JsonRequestBehavior.AllowGet };

        }




    }
}