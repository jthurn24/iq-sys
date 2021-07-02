using System;
using System.Web.Mvc;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Mvc.ModelMapper;
using IQI.Intuition.Domain.Models;
using IQI.Intuition.Domain.Repositories;
using IQI.Intuition.Infrastructure.Services;
using IQI.Intuition.Web.Models.Administration.SystemSecureFile;
using System.Collections;
using IQI.Intuition.Web.Attributes;
using System.Collections.Generic;
using System.IO;
using RedArrow.Framework.Mvc.Security;
using IQI.Intuition.Web.Extensions;

namespace IQI.Intuition.Web.Areas.Administration.Controllers
{
    [AnonymousAccess, RequiresSystemUserAttribute]
	public class SystemSecureFileController : Controller
	{

		public SystemSecureFileController(
			IActionContext actionContext, 
			IModelMapper modelMapper, 
			ISystemSecureFileRepository systemsecurefileRepository,
            IAccountRepository accountRepository,
            ISystemLeadRepository systemLeadRepository,
            ISystemTicketRepository systemTicketRepository
            )
		{
			ActionContext = actionContext.ThrowIfNullArgument("actionContext");
			ModelMapper = modelMapper.ThrowIfNullArgument("modelMapper");
			SystemSecureFileRepository = systemsecurefileRepository.ThrowIfNullArgument("systemsecurefileRepository");
            AccountRepository = accountRepository.ThrowIfNullArgument("accountRepository");
            SystemLeadRepository = systemLeadRepository.ThrowIfNullArgument("systemLeadRepository");
            SystemTicketRepository = systemTicketRepository.ThrowIfNullArgument("systemTicketRepository");
		}

		protected virtual IActionContext ActionContext { get; private set; }

		protected virtual IModelMapper ModelMapper { get; private set; }

		protected virtual ISystemSecureFileRepository SystemSecureFileRepository { get; private set; }

        protected virtual IAccountRepository AccountRepository { get; private set; }

        protected virtual ISystemLeadRepository SystemLeadRepository { get; private set; }

        protected virtual ISystemTicketRepository SystemTicketRepository { get; private set; }


        [HttpGet]
        public ActionResult Index()
        {
            string dataUrl = Url.Action("ListData");
            var gridModel = new SystemSecureFileGrid(dataUrl);
            return View(gridModel);
        }

		[HttpGet]
        public ActionResult ListFor(int? accountId,
            int? leadId,
            int? ticketId)
		{
            string dataUrl = Url.Action("ListData", new { accountId = accountId, leadId = leadId, ticketId = ticketId });
            var gridModel = new SystemSecureFileGrid(dataUrl);
			return PartialView(gridModel);
		}

		[HttpGet]
		public ActionResult ListData(int? accountId,
            int? leadId,
            int? ticketId,
            SystemSecureFileGridRequest requestModel)
		{
			var systemsecurefileQuery = SystemSecureFileRepository.Find(
                                accountId,
                                leadId,
                                ticketId,
                                null,
                                requestModel.Description,
								requestModel.SortBy,
				requestModel.SortDescending,
				requestModel.PageNumber,
				requestModel.PageSize);

			// We are setting up the grid to provide data so we pass in the patient chart link formatter
            var gridModel = new SystemSecureFileGrid(x => Url.Action("Download", new { id = x.Id }));
			ModelMapper.MapForReadOnly(systemsecurefileQuery, gridModel);

			return gridModel.GetJsonResult();
		}


		[HttpGet]
        public ActionResult Add(int? accountId,
            int? leadId,
            int? ticketId)
		{
            return View(new SystemSecureFileAddForm() {  ExpireDays = 180 } );
		}

		[HttpPost, SupportsFormCancel]
		public ActionResult Add(SystemSecureFileAddForm formModel, 
            bool formCancelled,
            int? accountId,
            int? leadId,
            int? ticketId)
		{
			try
			{
				if (formCancelled != true)
				{
                    if (formModel.File == null || formModel.File.ContentLength < 1)
                    {
                        throw new ModelMappingException(new List<ValidationError>() { new ValidationError(string.Empty,"Unable to upload file") });
                    }

                    if (formModel.Description.IsNullOrEmpty())
                    {
                        throw new ModelMappingException(new List<ValidationError>() { new ValidationError(string.Empty, "A description must be specified") });
                    }

					var systemsecurefile = new SystemSecureFile();

                    systemsecurefile.ExpiresOn = DateTime.Today.AddDays(formModel.ExpireDays);

                    systemsecurefile.FileExtension = System.IO.Path.GetExtension(formModel.File.FileName);

                    systemsecurefile.SetFile(this.ControllerContext.GetFileData(formModel.File));
                    systemsecurefile.Description = formModel.Description;

                    SystemSecureFileRepository.Add(systemsecurefile);

                    this.ControllerContext.SetUserMessage("File has been uploaded");

                    if (accountId.HasValue)
                    {
                        systemsecurefile.Account = AccountRepository.Get(accountId.Value);
                        return RedirectToAction("View", "Account", new { area = "Administration", id = accountId.Value });
                    }

                    if (leadId.HasValue)
                    {
                        systemsecurefile.Lead = SystemLeadRepository.Get(leadId.Value);
                        return RedirectToAction("View", "SystemLead", new { area = "Administration", id = leadId.Value });
                    }

                    if (ticketId.HasValue)
                    {
                        systemsecurefile.Ticket = SystemTicketRepository.Get(ticketId.Value);
                        return RedirectToAction("View", "SystemTicket", new { area = "Administration", id = ticketId.Value });
                    }

					
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
        public ActionResult Download(int id)
        {
            var entity = SystemSecureFileRepository.Get(id);
            var data = new MemoryStream(entity.GetFile());

            var result = new System.Web.Mvc.FileStreamResult(data, "application/octet-stream");
            result.FileDownloadName = string.Concat(id, ".", entity.FileExtension);
            return result;
        }

  }

}

