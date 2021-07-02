using System;
using System.Web.Mvc;
using System.Linq;
using System.Collections.Generic;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Mvc.ModelMapper;
using IQI.Intuition.Infrastructure.Services;
using IQI.Intuition.Web.Models.CmsMatrix;
using RedArrow.Framework.Mvc.Extensions;
using RedArrow.Framework.Mvc.Security;
using IQI.Intuition.Web.Attributes;
using IQI.Intuition.Domain;
using IQI.Intuition.Domain.Repositories;

namespace IQI.Intuition.Web.Controllers
{
    [RequiresActiveAccount]
    public class CmsMatrixController : Controller
    {
        public CmsMatrixController(
            IActionContext actionContext,
            IModelMapper modelMapper,
            IPatientRepository patientRepository,
            ICmsMatrixRepository cmsMatrixRepository)
        {
            ActionContext = actionContext.ThrowIfNullArgument("actionContext");
            ModelMapper = modelMapper.ThrowIfNullArgument("modelMapper");
            PatientRepository = patientRepository.ThrowIfNullArgument("patientRepository");
            CmsMatrixRepository = cmsMatrixRepository.ThrowIfNullArgument("cmsMatrixRepository");
        }

        protected virtual IActionContext ActionContext { get; private set; }

        protected virtual IModelMapper ModelMapper { get; private set; }

        protected virtual IPatientRepository PatientRepository { get; private set; }

        protected virtual ICmsMatrixRepository CmsMatrixRepository { get; private set; }

        [HttpGet]
        public ActionResult Index(IQI.Intuition.Web.Models.CmsMatrix.Dashboard model)
        {
            var patients = PatientRepository.Find(ActionContext.CurrentFacility)
                .Where(x => x.CurrentStatus == Enumerations.PatientStatus.Admitted);

            if (model.SelectedMatrixType.HasValue == false)
            {
                model.SelectedMatrixType = CmsMatrixRepository.AllTypes.First().Id;
            }

            model.MatrixTypeOptions = CmsMatrixRepository.AllTypes.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() });

            model.SortByOptions = new List<SelectListItem>()
            {
                new SelectListItem() { Text = "By Room" , Value = "0" },
                new SelectListItem() { Text = "By Name", Value = "1" }
            };

            if (model != null && model.SelectedWing.HasValue)
            {
                patients = patients.Where(x => x.Room.Wing.Id == model.SelectedWing.Value);
            }



            patients = patients.OrderBy(x => x.FullName);

            if(model.SortBy.HasValue == false || model.SortBy == 0)
            {
                patients = patients.OrderBy(x => x.Room.Name);
            }
            else
            {

                patients = patients.OrderBy(x => x.FullName);
            }


            var entries = CmsMatrixRepository.FindActiveEntriesForFacility(ActionContext.CurrentFacility.Id);
            var categories = CmsMatrixRepository.AllCategories.Where(x => x.Editable == true && x.Active == true);

            entries = entries.Where(x => x.Category.MatrixType.Id == model.SelectedMatrixType);
            categories = categories.Where(x => x.MatrixType.Id == model.SelectedMatrixType);

            var wings = ActionContext.CurrentFacility.Floors.SelectMany(x => x.Wings).ToList();
            model.WingOptions = wings.Select(m => new SelectListItem() { Value = m.Id.ToString(), Text = string.Concat(m.Floor.Name, " - ", m.Name) }).Prepend(new SelectListItem() { Text = "Show All", Value = string.Empty });
            model.CurrentFacilityName = ActionContext.CurrentFacility.Name;

            var notes = CmsMatrixRepository.FindActiveNotesForFacility(ActionContext.CurrentFacility.Id);

            model.SetData(categories, patients, entries, notes);

            return View(model);
        }

        public ActionResult EntryData(int patientId, int categoryId)
        {
            var model = new Entry();

            model.CategoryId = categoryId;
            model.PatientId = patientId;

            var patient = ActionContext.CurrentFacility.FindPatient(patientId);
            var category = CmsMatrixRepository.AllCategories.Where(x => x.Id == categoryId).FirstOrDefault();
            model.PatientName = patient.FullName;
            model.CategoryName = category.Name;
            model.CategoryDescription = category.DescriptionText;
            model.Options = category.Options.Select(x => new Entry.Option() { Description = x.OptionDescription, OptionValue = x.OptionValue }).ToList();

            var entry = CmsMatrixRepository.FindActiveEntryForPatient(patient.Id, category.Id);

            if (entry != null)
            {
                foreach (var selection in entry.SelectedOptions.Split(','))
                {
                    model.Options.Where(x => x.OptionValue == selection)
                        .ForEach(x => x.Selected = true);
                }
            }


            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateEntryData(int patientId, int categoryId, string selectedValues)
        {
            var patient = ActionContext.CurrentFacility.FindPatient(patientId);
            var category = CmsMatrixRepository.AllCategories.Where(x => x.Id == categoryId).FirstOrDefault();
            var entry = CmsMatrixRepository.FindActiveEntryForPatient(patient.Id, category.Id);

            if (entry != null)
            {
                entry.IsCurrent = false;
            }

            var activeEntry = new Domain.Models.CmsMatrixEntry();
            activeEntry.IsCurrent = true;
            activeEntry.Category = category;
            activeEntry.Patient = patient;
            activeEntry.SelectedOptions = selectedValues;

            CmsMatrixRepository.Add(activeEntry);

            return Json(new { SelectedOptions  = selectedValues }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdatePatientNote(Note note)
        {
            var patient = ActionContext.CurrentFacility.FindPatient(note.PatientId);
            var activeNote = CmsMatrixRepository.FindActiveNoteForPatient(patient.Id);

            if (activeNote != null)
            {
                activeNote.IsCurrent = false;
            }
  
            var newNote = new Domain.Models.CmsNote();
            newNote.Patient = patient;
            newNote.NoteText = note.NoteText;
            newNote.IsCurrent = true;
            CmsMatrixRepository.Add(newNote);

            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPatientNote(int id)
        {
            var patient = ActionContext.CurrentFacility.FindPatient(id);
            var activeNote = CmsMatrixRepository.FindActiveNoteForPatient(patient.Id);

            return Json(new { Note = activeNote != null ? activeNote.NoteText : string.Empty }, JsonRequestBehavior.AllowGet);
        }
    }
}