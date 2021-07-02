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
    public class PatientController : Controller
    {
        private IFacilityRepository _facilityRepository;
        private ISystemRepository _systemRepository;
        private IAccountRepository _accountRepository;
        private IPatientRepository _patientRepository;
        private IInfectionRepository _infectionRepository;


        public PatientController(IFacilityRepository facilityRepository,
            ISystemRepository systemRepository,
            IAccountRepository accountRepository,
            IPatientRepository patientRepository,
            IInfectionRepository infectionRepository)
        {
            _facilityRepository = facilityRepository;
            _systemRepository = systemRepository;
            _accountRepository = accountRepository;
            _patientRepository = patientRepository;
            _infectionRepository = infectionRepository;
        }

        public ActionResult Active(string token, string facility)
        {
            var mobileToken = _systemRepository.GetMobileToken(token);

            if (mobileToken == null) return Json(new { Success = false }, JsonRequestBehavior.AllowGet);

            var user = _systemRepository.GetUserById(mobileToken.AccountUserId);

            var fc = user.Facilities.Where(x => x.Name == facility).FirstOrDefault();

            if (fc == null) return Json(new { Success = false }, JsonRequestBehavior.AllowGet);

            var patients = _patientRepository.Find(fc)
                .Where(x => x.CurrentStatus == Domain.Enumerations.PatientStatus.Admitted)
                .Select(x => new {
                    FullName = x.FullName,
                    ID = x.Guid,
                    InfectionCount = x.InfectionVerifications.Where(xx => xx.IsResolved != true).Count()
                    })
                .Distinct()
                .OrderBy(x => x.FullName);

            return Json(new { Success = true, Patients = patients }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult Details(string token, string id)
        {
            var mobileToken = _systemRepository.GetMobileToken(token);

            if (mobileToken == null) return Json(new { Success = false }, JsonRequestBehavior.AllowGet);

            var user = _systemRepository.GetUserById(mobileToken.AccountUserId);

            var patient = _patientRepository.Get(Guid.Parse(id));

            var infections = _infectionRepository.FindForPatient(Guid.Parse(id), null, null)
                .Where(x => x.IsResolved != true && x.Deleted != true);

            if (patient.Account.Id != user.Account.Id)
            {
                return Json(new { Success = false }, JsonRequestBehavior.AllowGet);
            }

            var response = new
            {
                Patient = new {
                     FirstName = patient.GetFirstName(),
                     LastName = patient.GetLastName(),
                     Birthdate = patient.BirthDate.Value.ToShortDateString(),
                     Flags = patient.PatientFlags.Select(xx => new { Name = xx.Name , Id = xx.Id }),
                     Warnings = patient.Warnings.Select(xx => new { Title = xx.Title, Id = xx.Id }),
                     Room = patient.Room.Name
                },
                OpenInfections = infections.Select(x => new
                {
                    Type = x.InfectionSite.Type.Name,
                    Site = x.InfectionSite.Name,
                    Classification = System.Enum.GetName(typeof(InfectionClassification), x.Classification),
                    FirstNotedOn = x.FirstNotedOn.Value.ToShortDateString(),
                    Guid = x.Guid,
                    Notes = x.InfectionNotes.Select(xx => xx.Note).ToList()
                })
            };

            return Json(new { Success = true, Details = response }, JsonRequestBehavior.AllowGet);

        }
    }
}