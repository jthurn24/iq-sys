using System;
using System.Web.Mvc;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Mvc.ModelMapper;
using IQI.Intuition.Domain.Models;
using IQI.Intuition.Domain.Repositories;
using IQI.Intuition.Infrastructure.Services;
using IQI.Intuition.Web.Models.Configuration.AccountUser;


namespace IQI.Intuition.Web.Areas.Configuration.Controllers
{
    public class AccountUserController : Controller
    {
        public AccountUserController(
            IActionContext actionContext,
            IModelMapper modelMapper,
            ISystemRepository systemRepository,
            IAccountRepository accountRepository)
        {
            ActionContext = actionContext.ThrowIfNullArgument("actionContext");
            ModelMapper = modelMapper.ThrowIfNullArgument("modelMapper");
            AccountRepository = accountRepository.ThrowIfNullArgument("AccountRepository");
            SystemRepository = systemRepository.ThrowIfNullArgument("systemRepository");
        }

        protected virtual IActionContext ActionContext { get; private set; }

        protected virtual IModelMapper ModelMapper { get; private set; }

        protected virtual IAccountRepository AccountRepository { get; private set; }

        protected virtual ISystemRepository SystemRepository { get; private set; }

        [HttpGet]
        public ActionResult Index()
        {

            string dataUrl = Url.Action("ListData");
            var gridModel = new AccountUserGrid(dataUrl);
            return View(gridModel);
        }

        [HttpGet]
        public ActionResult ListData(AccountUserGridRequest requestModel)
        {
            var accountUserQuery = SystemRepository.FindUser(
                ActionContext.CurrentAccount,
                requestModel.Login,
                requestModel.FirstName,
                requestModel.LastName,
                requestModel.EmailAddress,
                false,
                requestModel.SortBy,
                requestModel.SortDescending,
                requestModel.PageNumber,
                requestModel.PageSize);

            // We are setting up the grid to provide data so we pass in the patient chart link formatter
            var gridModel = new AccountUserGrid();
            ModelMapper.MapForReadOnly(accountUserQuery, gridModel);

            return gridModel.GetJsonResult();
        }

        [HttpGet]
        public ActionResult Add()
        {
            var formModel = ModelMapper.MapForCreate<AccountUserForm>();
            formModel.IsActive = true;
            return View("Edit", formModel);
        }

        [HttpPost]
        public ActionResult Add(AccountUserForm formModel, bool? cancel)
        {

            try
            {
                if (cancel.IsNotTrue())
                {
                    var entity = new AccountUser(ActionContext.CurrentAccount);
                    ModelMapper.MapForCreate(formModel, entity);

                    ActionContext.CurrentAccount.AddUser(entity);
                    
                }

                return RedirectToAction("Index");
            }
            catch (ModelMappingException ex)
            {
                ModelState.AddModelMappingErrors(ex);
            }


            return View("Edit", formModel);
        }


        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id.HasValue)
            {
                var entity = ActionContext.CurrentAccount.Users.Where(x => x.Id == id).FirstOrDefault();

                if (entity != null)
                {
                    var formModel = ModelMapper.MapForUpdate<AccountUserForm>(entity);

                    return View("Edit", formModel);
                }
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Edit(AccountUserForm formModel, bool? cancel)
        {

            try
            {
                if (cancel.IsNotTrue())
                {
                    var entity = ActionContext.CurrentAccount.Users.Where(x => x.Id == formModel.Id.Value).FirstOrDefault();

                    if (entity == null)
                    {
                        return RedirectToAction("Index");
                    }

                    ModelMapper.MapForUpdate(formModel, entity);
                }

                return RedirectToAction("Index");
            }
            catch (ModelMappingException ex)
            {
                ModelState.AddModelMappingErrors(ex);
            }


            return View("Edit", formModel);
        }


    }
}
