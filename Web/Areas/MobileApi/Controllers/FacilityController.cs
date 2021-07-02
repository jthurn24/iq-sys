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
    public class FacilityController : Controller
    {
        private IFacilityRepository _facilityRepository;
        private ISystemRepository _systemRepository;
        private IAccountRepository _accountRepository;
        

        public FacilityController(IFacilityRepository facilityRepository,
            ISystemRepository systemRepository,
            IAccountRepository accountRepository)
        {
            _facilityRepository = facilityRepository;
            _systemRepository = systemRepository;
            _accountRepository = accountRepository;
        }

        public ActionResult All(string token)
        {
            var mobileToken = _systemRepository.GetMobileToken(token);

            if (mobileToken == null) return Json(new { Success = false }, JsonRequestBehavior.AllowGet);

            var user = _systemRepository.GetUserById(mobileToken.AccountUserId);

            return Json(new { Success = true, Facilities = user.Facilities.Select(x => x.Name) }, JsonRequestBehavior.AllowGet);
        }
    }
}