using IQI.Intuition.Domain.Models;
using IQI.Intuition.Domain.Repositories;
using RedArrow.Framework.Mvc.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IQI.Intuition.Web.Areas.MobileApi.Controllers
{
    [AnonymousAccess]
    public class TokenController : Controller
    {
        private IAccountRepository _accountRepository;
        private ISystemRepository _systemRepository;


        public TokenController(IAccountRepository accountRepository,
            ISystemRepository systemRepository)
        {
            _accountRepository = accountRepository;
            _systemRepository = systemRepository;
        }

        // GET: MobileApi/Token
        public ActionResult Create(string login, string password, string account)
        {
            var acct = _accountRepository.GetAll().Where(x => x.Name.ToLower() == account.ToLower()).FirstOrDefault();

            if(acct == null)
            {
                return Json(new { Success = false }, JsonRequestBehavior.AllowGet);
            }

            var user = _systemRepository.GetUserByCredentials(acct, login, password);

            if (user == null)
            {
                return Json(new { Success = false }, JsonRequestBehavior.AllowGet);
            }

            var token = new MobileToken()
            {
                 AccountUserId = user.Id,
                 Token = Guid.NewGuid().ToString(),
                 CreatedOn = DateTime.Now
            };

            _systemRepository.Add(token);

            return Json(new { Success = true, Token = token.Token }, JsonRequestBehavior.AllowGet);
        }
    }
}