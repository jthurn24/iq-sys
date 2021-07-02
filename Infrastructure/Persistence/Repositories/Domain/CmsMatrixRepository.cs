using System;
using System.Collections.Generic;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Persistence;
using IQI.Intuition.Domain.Models;
using IQI.Intuition.Domain.Repositories;
using System.Linq.Expressions;

namespace IQI.Intuition.Infrastructure.Persistence.Repositories.Domain
{
    public class CmsMatrixRepository : AbstractRepository<IDataContext>, ICmsMatrixRepository
    {
        public CmsMatrixRepository(IDataContext dataContext)
            : base(dataContext) { }

        public void Add(CmsMatrixEntry entry)
        {
            DataContext.TrackChanges(entry);
        }

        public void Add(CmsNote note)
        {
            DataContext.TrackChanges(note);
        }

        public CmsMatrixEntry GetEntry(int id)
        {
            return DataContext.Fetch<CmsMatrixEntry>(id);
        }

        public IEnumerable<CmsMatrixCategory> AllCategories
        {
            get {

                return DataContext.CreateQuery<CmsMatrixCategory>()
                    .SortBy(x => x.ColumnNumber)
                    .FetchAll();
            
            }
        }

        public IEnumerable<CmsMatrixType> AllTypes 
        {
            get {
                return DataContext.CreateQuery<CmsMatrixType>()
                    .FetchAll();
            }
        }

        public IEnumerable<CmsMatrixEntry> FindActiveEntriesForPatient(int patientID)
        {
            return DataContext.CreateQuery<CmsMatrixEntry>()
                .FilterBy(x => x.Patient.Id == patientID)
                .FilterBy(x => x.IsCurrent == true)
                .FetchAll();
        }

        public IEnumerable<CmsMatrixEntry> FindActiveEntriesForFacility(int facilityID)
        {
            return DataContext.CreateQuery<CmsMatrixEntry>()
                .FilterBy(x => x.Patient.Room.Wing.Floor.Facility.Id == facilityID)
                .FilterBy(x => x.IsCurrent == true)
                .FilterBy(x => x.Patient.CurrentStatus == Intuition.Domain.Enumerations.PatientStatus.Admitted)
                .FilterBy(x => x.Patient.Deleted == null || x.Patient.Deleted == false)
                .FetchAll();
        }

        public CmsMatrixEntry FindActiveEntryForPatient(int patientID, int categoryId)
        {
            return DataContext.CreateQuery<CmsMatrixEntry>()
                .FilterBy(x => x.Patient.Id == patientID)
                .FilterBy(x => x.Category.Id == categoryId)
                .FilterBy(x => x.IsCurrent == true)
                .FetchAll()
                .FirstOrDefault();
        }


        public IEnumerable<CmsNote> FindActiveNotesForFacility(int facilityId)
        {
            return DataContext.CreateQuery<CmsNote>()
                .FilterBy(x => x.Patient.Room.Wing.Floor.Facility.Id == facilityId)
                .FilterBy(x => x.IsCurrent == true)
                .FilterBy(x => x.Patient.CurrentStatus == Intuition.Domain.Enumerations.PatientStatus.Admitted)
                .FilterBy(x => x.Patient.Deleted == null || x.Patient.Deleted == false)
                .FetchAll();
        }

        public CmsNote FindActiveNoteForPatient(int patientId)
        {
            return DataContext.CreateQuery<CmsNote>()
                .FilterBy(x => x.Patient.Id == patientId)
                .FilterBy(x => x.IsCurrent == true)
                .FetchAll()
                .FirstOrDefault();
        }

    }

}
