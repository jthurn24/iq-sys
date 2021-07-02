using System;
using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Extensions.Formatting;
using RedArrow.Framework.Mvc.ModelMapper;
using RedArrow.Framework.Mvc.Extensions;
using IQI.Intuition.Infrastructure.Services;
using IQI.Intuition.Web.Attributes;
using IQI.Intuition.Web.Models.Reporting;
using IQI.Intuition.Domain.Repositories;
using IQI.Intuition.Domain.Models;
using SnyderIS.sCore.Extensions;


namespace IQI.Intuition.Web.Areas.Reporting.Controllers
{
    [EnableAccountRestriction]
    public class AnnotationController : Controller
    {
        private Facility _Facility;

        public AnnotationController(
            IActionContext actionContext, 
            IModelMapper modelMapper,
            IInfectionRepository infectionRepository,
            IEmployeeInfectionRepository employeeInfectionRepository,
            IIncidentRepository incidentRepository,
            IPatientRepository patientRepository,
            IComplaintRepository complaintRepository,
            IEmployeeRepository employeeRepository,
            ILabResultRepository labResultRepository,
            ITreatmentRepository treatmentRepository,
            ICmsMatrixRepository cmsMatrixRepository,
            IPsychotropicRespository psychotropicRespository,
            IWoundRepository woundRepository,
            ISystemRepository systemRepository,
            ICatheterRepository catheterRepository,
            IVaccineRepository vaccineRepository)
        {
            ActionContext = actionContext.ThrowIfNullArgument("actionContext");
            ModelMapper = modelMapper.ThrowIfNullArgument("modelMapper");
            InfectionRepository = infectionRepository.ThrowIfNullArgument("infectionRepository"); 
            EmployeeInfectionRepository = employeeInfectionRepository.ThrowIfNullArgument("employeeInfectionRepository");
            IncidentRepository = incidentRepository.ThrowIfNullArgument("incidentRepository");
            PatientRepository = patientRepository.ThrowIfNullArgument("patientRepository");
            ComplaintRepository = complaintRepository.ThrowIfNullArgument("complaintRepository");
            EmployeeRepository = employeeRepository.ThrowIfNullArgument("employeeRepository");
            LabResultRepository = labResultRepository.ThrowIfNullArgument("labResultRepository");
            TreatmentRepository = treatmentRepository.ThrowIfNullArgument("treatmentRepository");
            CmsMatrixRepository = cmsMatrixRepository.ThrowIfNullArgument("cmsMatrixRepository");
            PsychotropicRespository = psychotropicRespository.ThrowIfNullArgument("psychotropicRespository");
            WoundRepository = woundRepository.ThrowIfNullArgument("WoundRepository");
            SystemRepository = systemRepository.ThrowIfNullArgument("systemRepository");
            CatheterRepository = catheterRepository.ThrowIfNullArgument("catheterRepository");
            VaccineRepository = vaccineRepository.ThrowIfNullArgument("vaccineRepository");

            _Facility = ActionContext.CurrentFacility;
        }

        protected virtual IActionContext ActionContext { get; private set; }
        protected virtual IModelMapper ModelMapper { get; private set; }
        protected virtual IInfectionRepository InfectionRepository { get; private set; }
        protected virtual IEmployeeInfectionRepository EmployeeInfectionRepository { get; private set; }
        protected virtual IIncidentRepository IncidentRepository { get; private set; }
        protected virtual IPatientRepository PatientRepository { get; private set; }
        protected virtual IComplaintRepository ComplaintRepository { get; private set; }
        protected virtual IEmployeeRepository EmployeeRepository { get; private set; }
        protected virtual ILabResultRepository LabResultRepository { get; private set; }
        protected virtual ITreatmentRepository TreatmentRepository { get; private set; }
        protected virtual ICmsMatrixRepository CmsMatrixRepository { get; private set; }
        protected virtual IPsychotropicRespository PsychotropicRespository { get; private set; }
        protected virtual IWoundRepository WoundRepository { get; private set; }
        protected virtual ISystemRepository SystemRepository { get; private set; }
        protected virtual ICatheterRepository CatheterRepository { get; private set; }
        protected virtual IVaccineRepository VaccineRepository { get; private set; }


        public ActionResult Infections(string guids)
        {
            var gList = guids.Split(',').Select(x => Guid.Parse(x));

            var lines = new List<String>();

            var infections = InfectionRepository.FindForFacility(_Facility, gList);

            foreach (var g in gList)
            {
                var i = infections.Where(x => x.Guid == g).FirstOrDefault();

                if (i != null)
                {
                    lines.Add(string.Format("{0} - {1}",
                        i.Patient.FullName,
                        i.FirstNotedOn.FormatAsShortDate()));
                }
                else
                {
                    lines.Add(g.ToString());
                }
            }

            return Json(lines, JsonRequestBehavior.AllowGet);
        }


        public ActionResult Incidents(string guids)
        {
            var gList = guids.Split(',').Select(x => Guid.Parse(x));

            var lines = new List<String>();

            var infections = IncidentRepository.FindForFacility(_Facility, gList);

            foreach (var g in gList)
            {
                var i = infections.Where(x => x.Guid == g).FirstOrDefault();

                if (i != null)
                {
                    lines.Add(string.Format("{0} - {1}",
                        i.Patient.FullName,
                        i.DiscoveredOn.FormatAsShortDate()));
                }
                else
                {
                    lines.Add(g.ToString());
                }
            }

            return Json(lines, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Complaints(string guids)
        {
            var gList = guids.Split(',').Select(x => Guid.Parse(x));

            var lines = new List<String>();

            var infections = ComplaintRepository.FindForFacility(_Facility, gList);

            foreach (var g in gList)
            {
                var i = infections.Where(x => x.Guid == g).FirstOrDefault();

                if (i != null)
                {
                    var descBuilder = new System.Text.StringBuilder();

                    if (i.Patient != null)
                    {
                        descBuilder.Append("P1 = ");
                        descBuilder.Append(i.Patient.FullName);
                        descBuilder.Append(" ");
                    }

                    if (i.Patient2 != null)
                    {
                        descBuilder.Append("P2 = ");
                        descBuilder.Append(i.Patient2.FullName);
                        descBuilder.Append(" ");
                    }

                    if (i.Employee != null)
                    {
                        descBuilder.Append("E1 = ");
                        descBuilder.Append(i.Employee.FullName);
                        descBuilder.Append(" ");
                    }

                    if (i.Employee2 != null)
                    {
                        descBuilder.Append("E2 = ");
                        descBuilder.Append(i.Employee2.FullName);
                        descBuilder.Append(" ");
                    }

                    descBuilder.Append(i.DateOccurred.FormatAsShortDate());

                    lines.Add(descBuilder.ToString());
                }
                else
                {
                    lines.Add(g.ToString());
                }
            }

            return Json(lines, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Wounds(string guids)
        {
            var gList = guids.Split(',').Select(x => Guid.Parse(x));

            var lines = new List<String>();

            var wounds = WoundRepository.FindReportsForFacility(_Facility, gList);

            foreach (var g in gList)
            {
                var i = wounds.Where(x => x.Guid == g).FirstOrDefault();

                if (i != null)
                {
                    lines.Add(i.Patient.FullName);
                }
                else
                {
                    lines.Add(g.ToString());
                }
            }

            return Json(lines, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Catheters(string guids)
        {
            var gList = guids.Split(',').Select(x => Guid.Parse(x));

            var lines = new List<String>();

            var catheters = CatheterRepository.FindForFacility(_Facility, gList);

            foreach (var g in gList)
            {
                var c = catheters.Where(x => x.Guid == g).FirstOrDefault();

                if (c != null)
                {
                    lines.Add(c.Patient.FullName);
                }
                else
                {
                    lines.Add(g.ToString());
                }
            }

            return Json(lines, JsonRequestBehavior.AllowGet);
        }
    }
}