using System;
using System.Linq.Expressions;
using RedArrow.Framework.Persistence;
using IQI.Intuition.Domain.Models;
using System.Collections.Generic;

namespace IQI.Intuition.Domain.Repositories
{
    public interface ICmsMatrixRepository
    {
        void Add(CmsMatrixEntry entry);
        void Add(CmsNote note);
        CmsMatrixEntry GetEntry(int id);

        IEnumerable<CmsMatrixCategory> AllCategories { get; }
        IEnumerable<CmsMatrixType> AllTypes { get; }

        IEnumerable<CmsMatrixEntry> FindActiveEntriesForPatient(int patientID);
        CmsMatrixEntry FindActiveEntryForPatient(int patientID, int categoryId);
        IEnumerable<CmsMatrixEntry> FindActiveEntriesForFacility(int facilityId);

        IEnumerable<CmsNote> FindActiveNotesForFacility(int facilityId);
        CmsNote FindActiveNoteForPatient(int patientId);

    }
}
