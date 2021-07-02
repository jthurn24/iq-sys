using System;
using System.Web.Mvc;
using System.Collections.Generic;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Mvc.ModelMapper;
using RedArrow.Framework.Mvc.Security;
using IQI.Intuition.Infrastructure.Services;
using IQI.Intuition.Infrastructure.Services.Development;
using IQI.Intuition.Web.Models.Navigation;
using IQI.Intuition.Reporting.Repositories;
using IQI.Intuition.Reporting.Models.User;
using IQI.Intuition.Domain.Utilities;
using IQI.Intuition.Web.Models.Reporting.Composite;
using System.Linq;
using IQI.Intuition.Web.Attributes;
using RedArrow.Framework.Mvc.Security;
using RedArrow.Framework.Logging;

namespace IQI.Intuition.Web.Controllers
{
    [RequiresActiveAccount]
    public class ExportController : Controller
    {
        private IUserRepository _UserRepository;
        private ILog _Logger;

        public ExportController(
            IActionContext actionContext,
            IModelMapper modelMapper,
            IUserRepository userRepository,
            ILog log)
        {
            ActionContext = actionContext.ThrowIfNullArgument("actionContext");
            ModelMapper = modelMapper.ThrowIfNullArgument("modelMapper");
            _UserRepository = userRepository;
            _Logger = log;
        }

        protected virtual IActionContext ActionContext { get; private set; }
        protected virtual IModelMapper ModelMapper { get; private set; }

        public ActionResult RenderExportPanel(string path, string title, bool disabledEmailExport, bool disableComposite, bool landscape)
        {
            if (disableComposite)
            {
                ViewData["disabledCompositeExport"] = true;
            }

            if (disabledEmailExport)
            {
                ViewData["disabledEmailExport"] = true;
            }

            ViewData["path"] = path;
            ViewData["title"] = System.Web.HttpUtility.UrlEncode(title);
            ViewData["landscape"] = landscape;
            return PartialView();
        }

    
        [AnonymousAccess]
        public ActionResult RenderPrintHeader(string title, string locationDescription)
        {

            if (locationDescription.IsNullOrEmpty())
            {
                ViewData["locationDescription"] = ActionContext.CurrentFacility.Name;
            }
            else
            {
                ViewData["locationDescription"] = locationDescription;
            }

            ViewData["user"] = string.Concat(ActionContext.CurrentUser.FirstName, " ", ActionContext.CurrentUser.LastName);
            ViewData["title"] = title;
            ViewData["on"] = DateTime.Now.ToString("MM/dd/yy HH:mm");
            return PartialView();
        }

        public ActionResult CreatePrintRequest(string path, bool landscape)
        {
            var entry = new CompositeRequest() { Link = UrlEncodeHelper.UrlDecode(path), Landscape = landscape };
            var request = CreateRequest(new List<CompositeRequest>() { entry }, path);
            return RedirectToAction("MonitorPrintRequest",new { id = request.Id});
        }


        public ActionResult CreateCompositePrintRequest()
        {
            var entries = GetCompositeRequests();
            var request = CreateRequest(entries, Url.Action("Index", "Home"));
            return RedirectToAction("MonitorPrintRequest", new { id = request.Id });
        }

        public ActionResult CreateEmailRequest(string path, bool landscape)
        {
            return View();
        }

        public ActionResult CreateCompositeEmailRequest()
        {
            return View("CreateEmailRequest");
        }

        [HttpPost,ValidateInput(false)]
        public ActionResult CreateEmailrequest(string path, string emailAddress, bool landscape)
        {
            var entry = new CompositeRequest() { Link = UrlEncodeHelper.UrlDecode(path), Landscape = landscape };

            var request = CreateRequest(new List<CompositeRequest>() { entry }, path);
            request.EmailTo = emailAddress;
            _UserRepository.Update(request);
            return Redirect(UrlEncodeHelper.UrlDecode(path));
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CreateCompositeEmailRequest(string emailAddress)
        {
            var entries = GetCompositeRequests();
            var request = CreateRequest(entries, Url.Action("Index", "Home"));
            request.EmailTo = emailAddress;
            _UserRepository.Update(request);
            return Redirect(Url.Action("Index", "Home"));
        }

        public ActionResult MonitorPrintRequest(Guid id)
        {
            var request = _UserRepository.GetExportRequest(id);

            if (request.AccountId != ActionContext.CurrentAccount.Id)
            {
                throw new Exception("Invalid request");
            }

            if (request.Status == ExportRequest.ExportRequestStatus.Completed)
            {
                return RedirectToAction("ViewPrintRequest", new { id = request.Id });
            }

            Response.AddHeader("Refresh", "3");

            return View(request);
        }

        public ActionResult ViewPrintRequest(Guid id)
        {
            var request = _UserRepository.GetExportRequest(id);

            if (request.AccountId != ActionContext.CurrentAccount.Id)
            {
                throw new Exception("Invalid request");
            }

            return View(request);
        }

        public ActionResult StreamPrintRequest(Guid id)
        {
            var request = _UserRepository.GetExportRequest(id);

            if (request.AccountId != ActionContext.CurrentAccount.Id)
            {
                throw new Exception("Invalid request");
            }

            HttpContext.Response.AddHeader("content-disposition","attachment; filename=form.pdf");

            var data = new System.IO.MemoryStream(request.OutputFile);
            data.Seek(0, System.IO.SeekOrigin.Begin);

            return new FileStreamResult(data, "application/pdf");

        }

        public ActionResult ViewCompositeRequests()
        {
            return View(GetCompositeRequests());
        }

        public ActionResult RemoveCompositeRequest(string id)
        {
            var requests = GetCompositeRequests();
            requests.Remove(x => x.ID == id);
            return RedirectToAction("ViewCompositeRequests");
        }

        public ActionResult AddCompositeRequest(string path, string title, bool landscape)
        {
            var r = new CompositeRequest();
            r.Title = title;
            r.Link = path;
            r.ID = DateTime.Now.Ticks.ToString();
            r.Landscape = landscape;

            var requests = GetCompositeRequests();

            if (requests.Where(x => x.Title == title).Count() < 1)
            {
                requests.Add(r);
            }

            return Json(new { Result = true }, JsonRequestBehavior.AllowGet);
        }

        private List<CompositeRequest> GetCompositeRequests()
        {
            if (this.Session["IQI.CompositeRequests"] == null)
            {
                this.Session["IQI.CompositeRequests"] = new List<CompositeRequest>();
            }

            return (List<CompositeRequest>)this.Session["IQI.CompositeRequests"];

        }

        private ExportRequest CreateRequest(List<CompositeRequest> entries, string returnPath)
        {
            var request = new ExportRequest(ActionContext.CurrentAccount.Id, ExportRequest.ExportRequestFormat.Pdf);
            request.ReturnPath = returnPath;

            foreach (CompositeRequest e in entries)
            {
                var token = new AuthenticationRequestToken(ActionContext.CurrentUser);
                string argChar = "?";

                if (e.Link.Contains("?"))
                {
                    argChar = "&";
                }

                string finalPath = string.Concat(e.Link, argChar, AuthenticationRequestToken.AUTHENTICATION_TOKEN_REQUEST_VAR, "=", token.ToToken(), "&export=true");

                _Logger.Info(string.Concat("Export request created: ", finalPath));

                request.ExportPaths.Add(new ExportRequest.ExportRequestPath() { Path = finalPath, Landscape = e.Landscape, Id = Guid.NewGuid() });
            }

            request.CreatedOn = DateTime.Now;
            request.Status = ExportRequest.ExportRequestStatus.Pending;
            _UserRepository.Add(request);
            return request;
        }
       
    }
}