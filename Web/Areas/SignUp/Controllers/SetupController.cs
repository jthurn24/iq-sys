using System;
using System.Web.Mvc;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Mvc.ModelMapper;
using IQI.Intuition.Domain.Models;
using IQI.Intuition.Domain.Repositories;
using IQI.Intuition.Infrastructure.Services;
using IQI.Intuition.Web.Models.Administration.Account;
using System.Collections;
using IQI.Intuition.Web.Attributes;
using System.Collections.Generic;
using RedArrow.Framework.Mvc.Security;
using IQI.Intuition.Web.Extensions;
using IQI.Intuition.Web.Models.SignUp;
using RedArrow.Framework.Utilities;

namespace IQI.Intuition.Web.Areas.SignUp.Controllers
{
    [AnonymousAccess]
	public class SetupController : Controller
	{

		public SetupController(
			IAccountRepository accountRepository,
            IFacilityRepository facilityRepository,
            ISystemRepository systemRepository,
            IModelMapper modelMapper,
            IInfectionRepository infectionRepository
            )
        {
			AccountRepository = accountRepository;
            SystemRepository = systemRepository;
            ModelMapper = modelMapper;
            InfectionRepository = infectionRepository;
            FacilityRepository = facilityRepository;
        }


		protected virtual IAccountRepository AccountRepository { get; private set; }
        protected virtual ISystemRepository SystemRepository { get; private set; }
        protected virtual IModelMapper ModelMapper { get; private set; }
        protected virtual IInfectionRepository InfectionRepository { get; private set; }
        protected virtual IFacilityRepository FacilityRepository { get; private set; }

        [HttpGet]
		public ActionResult Index()
		{
            var formModel = ModelMapper.MapForCreate<SignUpForm>();
            return View(formModel);
		}

        [HttpPost]
        public ActionResult Index(SignUpForm formModel)
        {
            try
            {
                var domain = new SignUpRequest();
                ModelMapper.MapForCreate(formModel, domain);

                domain.Code = Guid.NewGuid().ToString();
                domain.Login = domain.EmailAddress;
                domain.IpAddress = Request.UserHostAddress;
                domain.LastName = string.Empty;

                SystemRepository.Add(domain);

                var messageBuilder = new System.Text.StringBuilder();
                messageBuilder.Append("Thank you for registering your IQI account. In order to activate your account please click the link below: ");
                messageBuilder.Append("<br>");
                messageBuilder.Append("<a href=\"https://signup.iqisystems.com");
                messageBuilder.Append(Url.Action("Approve"));
                messageBuilder.Append("?code=");
                messageBuilder.Append(domain.Code);
                messageBuilder.Append("\">Click Here To Activate IQI Account</a>");

                var email = new SystemEmailNotification();
                email.SendTo = domain.EmailAddress;
                email.Subject = "Confirm IQI Account Registration";
                email.MessageText = "";
                email.IsHtml = true;
                email.MessageText = messageBuilder.ToString();
                SystemRepository.Add(email);
                

                return RedirectToAction("WaitingForConfirmation");
            }
            catch (ModelMappingException ex)
            {
                ModelState.AddModelMappingErrors(ex);
            }
            return View(formModel);
        }

        public ActionResult WaitingForConfirmation()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Approve(string code)
        {
            var formModel = ModelMapper.MapForCreate<ApproveForm>();
            formModel.Code = code;
            return View(formModel);
        }

        [HttpPost]
        public ActionResult Approve(ApproveForm formModel)
        {
            try
            {
                var request = SystemRepository.GetSignUpRequestByCode(formModel.Code);

                ModelMapper.MapForUpdate(formModel, request);

                if (formModel.Password != formModel.ConfirmPassword)
                {
                    throw new ModelMappingException(new List<ValidationError>()
                    {
                        new ValidationError("Password","Passwords must match")
                    });
                }

                request.SubDomain = SystemRepository.GenerateFacilitySubDomain(formModel.FacilityName);
                request.SetPassword(formModel.Password);

                CreateAccount(request);

                formModel.Link = string.Format("https://{0}.iqisystems.com/", request.SubDomain);
                formModel.Complete = true;
            }
            catch (ModelMappingException ex)
            {
                ModelState.AddModelMappingErrors(ex);
                return View(formModel);
            }

            return View("ApproveComplete",formModel);
        }

        private void CreateAccount(SignUpRequest request)
        {
            if (request != null && !request.Approved)
            {
                var account = new Account(request.Name);

                var adminUser = new AccountUser(account);
                adminUser.SystemUser = true;
                adminUser.IsActive = true;
                adminUser.Login = "system";
                adminUser.ChangePassword(string.Concat(Guid.NewGuid().ToString(), DateTime.Now.Ticks.ToString()));
                account.AddUser(adminUser);

                var user = new AccountUser(account);
                user.SystemUser = false;
                user.IsActive = true;
                user.Login = request.Login;
                user.EmailAddress = request.EmailAddress;
                user.FirstName = request.FirstName;
                user.LastName = request.LastName;
                user.PasswordHash = request.PasswordHash;
                user.PasswordSalt = request.PasswordSalt;
                account.AddUser(user);

                var permissions = SystemRepository.GetAllPermissions();

                user.AssignPermissions(permissions.ToArray());

                var facility = new Facility(account);
                facility.Name = request.Name;
                facility.SubDomain = request.SubDomain;
                facility.InActive = false;
                facility.InfectionDefinition = InfectionRepository.AllInfectionDefinitions.Where(x => x.Id == 1).FirstOrDefault();
                facility.IsBetaSite = false;
                facility.MaxBeds = request.MaxBeds;
                facility.FacilityType = Domain.Enumerations.FacilityType.SkilledNursing;
                facility.State = request.State;

                user.AssignFacilities(new Facility[] { facility });

                account.AddFacility(facility);

                facility.FacilityProducts = new List<FacilityProduct>();

                facility.FacilityProducts.Add(new FacilityProduct()
                {
                    Facility = facility,
                    Fee = 0,
                    FeeType = Domain.Enumerations.ProductFeeType.MonthlyPatientCensus,
                    StartOn = DateTime.Now,
                    SystemProduct = SystemRepository.GetSystemProducts().Where(x => x.Id == (int)Domain.Enumerations.KnownProductType.InfectionTracking).First()
                });

                var floor = new Floor() { Name = "First", Facility = facility, Guid = GuidHelper.NewGuid() };

                facility.AddFloor(floor);

                var wing = new Wing() { Name = "Main", Floor = floor, Guid = GuidHelper.NewGuid() };
                floor.AddWing(wing);

                var room = new Room() { Name = "1", Wing = wing, Guid = GuidHelper.NewGuid() };
                wing.AddRoom(room);

                AccountRepository.Add(account);

                var messageBuilder = new System.Text.StringBuilder();
                messageBuilder.AppendFormat("Your account has been activated. You can log in at any time using the link below. Your login is {0} ", request.EmailAddress);
                messageBuilder.Append("<br>");
                messageBuilder.AppendFormat("<a href=\"https://{0}.iqisystems.com/\">{0}.iqisystems.com</a>", request.SubDomain);

                var email = new SystemEmailNotification();
                email.SendTo = request.EmailAddress;
                email.Subject = "Welcome to IQI";
                email.IsHtml = true;
                email.MessageText = messageBuilder.ToString();
                SystemRepository.Add(email);

                request.Approved = true;
            }
        }
    }

}

