using System;
using System.Web.Mvc;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Mvc.ModelMapper;
using IQI.Intuition.Domain.Models;
using IQI.Intuition.Domain.Repositories;
using IQI.Intuition.Infrastructure.Services;
using IQI.Intuition.Web.Models.SupportCenter;
using System.Collections;
using IQI.Intuition.Web.Attributes;
using System.Collections.Generic;
using RedArrow.Framework.Mvc.Security;
using IQI.Intuition.Web.Extensions;

namespace IQI.Intuition.Web.Controllers
{
    [RequiresActiveAccount]
    public class SupportCenterController : Controller
    {
        public SupportCenterController(
			IActionContext actionContext, 
			IModelMapper modelMapper, 
			ISystemTicketRepository systemticketRepository,
            IAccountRepository accountRepository,
            ISystemSecureFileRepository systemsecurefileRepository)
		{
			ActionContext = actionContext.ThrowIfNullArgument("actionContext");
			ModelMapper = modelMapper.ThrowIfNullArgument("modelMapper");
			SystemTicketRepository = systemticketRepository.ThrowIfNullArgument("systemticketRepository");
            AccountRepository = accountRepository.ThrowIfNullArgument("accountRepository");
            SystemSecureFileRepository = systemsecurefileRepository.ThrowIfNullArgument("systemsecurefileRepository");
		}

        protected virtual IActionContext ActionContext { get; private set; }

        protected virtual IModelMapper ModelMapper { get; private set; }

        protected virtual ISystemTicketRepository SystemTicketRepository { get; private set; }

        protected virtual IAccountRepository AccountRepository { get; private set; }

        protected virtual ISystemSecureFileRepository SystemSecureFileRepository { get; private set; }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult References()
        {
            return View();
        }


        [HttpGet]
        public ActionResult ViewTickets(bool? includeClosed)
        {
            string dataUrl = Url.Action("ViewTicketsData");
            var gridModel = new SystemTicketGrid(dataUrl);
            return View(gridModel);
        }

        [HttpGet]
        public ActionResult ViewTicketsData(SystemTicketGridRequest requestModel)
        {
            var systemticketQuery = SystemTicketRepository.Find(
                ActionContext.CurrentAccount.Id,
                requestModel.Account,
                requestModel.AccountUser,
                requestModel.SystemTicketType,
                requestModel.Details,
                requestModel.Priority,
                requestModel.SystemUser,
                requestModel.Release,
                Domain.Enumerations.SystemTicketSearchMode.OpenOnly,
                requestModel.SortBy,
                requestModel.SortDescending,
                requestModel.PageNumber,
                requestModel.PageSize);

            // We are setting up the grid to provide data so we pass in the patient chart link formatter
            var gridModel = new SystemTicketGrid();
            ModelMapper.MapForReadOnly(systemticketQuery, gridModel);

            return gridModel.GetJsonResult();
        }


        [HttpGet]
        public ActionResult ViewTicket(int? id)
        {
            var systemticket = SystemTicketRepository.Get(id ?? 0);

            if (systemticket.Account.Id != ActionContext.CurrentAccount.Id)
            {
                throw new Exception("Unauthorized");
            }

            if (systemticket == null)
            {
                return RedirectToAction("Index");
            }

            var formModel = ModelMapper.MapForUpdate<SystemTicketViewForm>(systemticket);

            return View(formModel);
        }

        [HttpPost, SupportsFormCancel]
        public ActionResult ViewTicket(int? id, bool formCancelled)
        {

            if (formCancelled)
            {
                return RedirectToAction("Index");
            }

            return RedirectToAction("Edit", new { id = id.Value });

        }

        [HttpGet]
        public ActionResult RequestSupport()
        {
            var formModel = ModelMapper.MapForCreate<SystemTicketAddForm>();
            return View(formModel);
        }

        [HttpPost, SupportsFormCancel]
        public ActionResult RequestSupport(SystemTicketAddForm formModel, bool formCancelled)
        {
            try
            {
                if (formCancelled != true)
                {

                    var systemticket = new SystemTicket();
                    ModelMapper.MapForCreate(formModel, systemticket);

                    systemticket.Account = ActionContext.CurrentAccount;
                    systemticket.AccountUser = ActionContext.CurrentUser;
                    systemticket.SystemTicketType = SystemTicketRepository.AllTicketTypes.Where(x => x.Name == "Support Request").FirstOrDefault();
                    systemticket.Status = Domain.Enumerations.SystemTicketStatus.New;
                    systemticket.CreatedOn = DateTime.Now;

                    SystemTicketRepository.Add(systemticket);

                    ActionContext.SendEmailNotification(string.Format("Notification: New support request: {0} - {1}",
                        ActionContext.CurrentAccount.Name,
                        ActionContext.CurrentUser.Login)
                        , systemticket.Details);

                    this.ControllerContext.SetUserMessage("Support ticket has been created");
                }

                return RedirectToAction("ViewTickets");
            }
            catch (ModelMappingException ex)
            {
                ModelState.AddModelMappingErrors(ex);
            }
            return View(formModel);
        }


        [HttpGet]
        public ActionResult RequestFeature()
        {
            var formModel = ModelMapper.MapForCreate<SystemTicketAddForm>();
            return View(formModel);
        }

        [HttpPost, SupportsFormCancel]
        public ActionResult RequestFeature(SystemTicketAddForm formModel, bool formCancelled)
        {
            try
            {
                if (formCancelled != true)
                {

                    var systemticket = new SystemTicket();
                    ModelMapper.MapForCreate(formModel, systemticket);

                    systemticket.Account = ActionContext.CurrentAccount;
                    systemticket.AccountUser = ActionContext.CurrentUser;
                    systemticket.SystemTicketType = SystemTicketRepository.AllTicketTypes.Where(x => x.Name == "Feature Request").FirstOrDefault();
                    systemticket.Status = Domain.Enumerations.SystemTicketStatus.New;
                    systemticket.CreatedOn = DateTime.Now;

                    SystemTicketRepository.Add(systemticket);


                    ActionContext.SendEmailNotification(string.Format("Notification: New feature request: {0} - {1}",
                    ActionContext.CurrentAccount.Name,
                    ActionContext.CurrentUser.Login)
                    ,systemticket.Details);

                    this.ControllerContext.SetUserMessage("Feature request has been sent");
                }

                return RedirectToAction("Index");
            }
            catch (ModelMappingException ex)
            {
                ModelState.AddModelMappingErrors(ex);
            }
            return View(formModel);
        }

        [HttpGet]
        public ActionResult UploadFile()
        {
            return View();
        }

        [HttpPost, SupportsFormCancel]
        public ActionResult UploadFile(SystemSecureFileAddForm formModel, bool formCancelled)
        {
            try
            {
                if (formCancelled != true)
                {
                    if (formModel.File == null || formModel.File.ContentLength < 1)
                    {
                        throw new ModelMappingException(new List<ValidationError>() { new ValidationError(string.Empty, "Unable to upload file") });
                    }

                    if (formModel.Description.IsNullOrEmpty())
                    {
                        throw new ModelMappingException(new List<ValidationError>() { new ValidationError(string.Empty, "A description must be specified") });
                    }

                    var systemsecurefile = new SystemSecureFile();

                    systemsecurefile.ExpiresOn = DateTime.Today.AddDays(180);

                    systemsecurefile.FileExtension = System.IO.Path.GetExtension(formModel.File.FileName);

                    systemsecurefile.SetFile(this.ControllerContext.GetFileData(formModel.File));
                    systemsecurefile.Description = formModel.Description;
                    systemsecurefile.Account = ActionContext.CurrentAccount;
                    SystemSecureFileRepository.Add(systemsecurefile);


                    ActionContext.SendEmailNotification(string.Format("Notification: New secure file: {0} - {1}",
                    ActionContext.CurrentAccount.Name,
                    ActionContext.CurrentUser.Login)
                    ,string.Concat(formModel.File.FileName," - ", formModel.Description));

                    this.ControllerContext.SetUserMessage("File has been uploaded");

                }

                return RedirectToAction("Index");
            }
            catch (ModelMappingException ex)
            {
                ModelState.AddModelMappingErrors(ex);
            }
            return View(formModel);
        }


    }
}
