using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Persistence;
using IQI.Intuition.Domain.Models;
using IQI.Intuition.Domain.Repositories;
using IQI.Intuition.Domain;

namespace IQI.Intuition.Infrastructure.Persistence.Repositories.Domain
{
    public class PsychotropicRespository : AbstractRepository<IDataContext>, IPsychotropicRespository
    {
        public PsychotropicRespository(IDataContext dataContext)
            : base(dataContext) { }

        public void Add(PsychotropicAdministration entity)
        {
            DataContext.TrackChanges(entity);
        }

        public void Add(PsychotropicDosageChange entity)
        {
            DataContext.TrackChanges(entity);
        }

        public void Add(PsychotropicAdministrationPRN entity)
        {
            DataContext.TrackChanges(entity);
        }

        public PsychotropicAdministration GetAdministration(int id)
        {
            return DataContext.Fetch<PsychotropicAdministration>(id);
        }

        public PsychotropicDosageChange GetDosageChange(int id)
        {
            return DataContext.Fetch<PsychotropicDosageChange>(id);
        }

        public PsychotropicAdministrationPRN GetPRN(int id)
        {
            return DataContext.Fetch<PsychotropicAdministrationPRN>(id);
        }

        public void Delete(PsychotropicDosageChange entity)
        {
            DataContext.Delete(entity);
        }

        public void Delete(PsychotropicAdministrationPRN entity)
        {
            DataContext.Delete(entity);
        }

        public IPagedQueryResult<PsychotropicAdministration> FindAdministration(Facility facility,
            string drugName,
            string patientName,
            string roomAndWingName,
            string drugTypeName,
            Expression<Func<PsychotropicAdministration, object>> sortByExpression,
            bool sortDescending, int page, int pageSize)
        {
            var q = DataContext.CreateQuery<PsychotropicAdministration>()
                .FilterBy(x => x.Patient.Room.Wing.Floor.Facility.Id == facility.Id);

            if (drugName.IsNotNullOrEmpty())
            {
                q = q.FilterBy(x => x.Name.Contains(drugName));
            }


            if (roomAndWingName.IsNotNullOrWhiteSpace())
            {
                q = q.FilterBy(i => (i.Patient.Room.Name + " " + i.Patient.Room.Wing.Name).Contains(roomAndWingName));
            }

            if (drugTypeName.IsNotNullOrEmpty())
            {
                q = q.FilterBy(i => i.DrugType.Name.Contains(drugTypeName));
            }


            var results = q.FetchAll();

            if (patientName.IsNotNullOrWhiteSpace())
            {
                results = results.Where(i => (i.Patient.GetFirstName() + " " + i.Patient.GetLastName()).ToLower().Contains(patientName.ToLower()));
            }

    
            if (sortDescending)
            {
                results = results.AsQueryable().OrderByDescending(sortByExpression);
            }
            else
            {
                results = results.AsQueryable().OrderBy(sortByExpression);
            }

            var pager = new RedArrow.Framework.Persistence.PagedQueryResult<PsychotropicAdministration>();
            pager.PageSize = pageSize;
            pager.PageNumber = page;
            pager.TotalResults = results.Count();

            if (results.Count() > 1)
            {
                pager.TotalPages = (int)Math.Ceiling((double)pager.TotalResults / pager.PageSize);
            }
            else
            {
                pager.TotalPages = 0;
            }

            pager.PageValues = results.Skip((pager.PageNumber - 1) * pager.PageSize).Take(pager.PageSize);

            return pager;
        }

        public IPagedQueryResult<PsychotropicAdministration> FindAdministration(Patient patient, 
            Expression<Func<PsychotropicAdministration, object>> sortByExpression, 
            bool sortDescending, int page, int pageSize)
        {
            return DataContext.CreateQuery<PsychotropicAdministration>()
                .FilterBy(x => x.Patient.Id == patient.Id)
                .FilterBy(x => x.Deleted == null || x.Deleted == false)
                .SortBy(sortByExpression)
                .DescendingWhen(sortDescending)
                .PageSize(pageSize)
                .FetchPage(page);
        }

        public IPagedQueryResult<PsychotropicAdministrationPRN> FindPRN(PsychotropicAdministration admin,
            Expression<Func<PsychotropicAdministrationPRN, object>> sortByExpression,
            bool sortDescending, int page, int pageSize)
        {
            return DataContext.CreateQuery<PsychotropicAdministrationPRN>()
                .FilterBy(x => x.Administration.Id == admin.Id)
                .SortBy(sortByExpression)
                .DescendingWhen(sortDescending)
                .PageSize(pageSize)
                .FetchPage(page);
        }


        public IEnumerable<PsychotropicAdministration> FindForLineListing(Facility facility,
        int? wingId,
        DateTime startDate,
        DateTime endDate,
        bool activeOnly)
        {

            var q = DataContext.CreateQuery<PsychotropicAdministration>()
                .FilterBy(x => x.Deleted == null || x.Deleted == false)
                .FilterBy(x => x.Patient.Room.Wing.Floor.Facility.Id == facility.Id);

            if (wingId.HasValue)
            {
                q = q.FilterBy(x => x.Patient.Room.Wing.Id == wingId.Value);
            }

            if (activeOnly)
            {
                q = q.FilterBy(x => x.Active == true);
            }

            var data = q.FetchAll();

            data = data.Where(x => x.GetStartDate() <= endDate);
            data = data.Where(x => x.GetEndDate() == null || x.GetEndDate() >= startDate);

            return data;

        }


        public IPagedQueryResult<PsychotropicDosageChange> FindDosageChange(PsychotropicAdministration admin,
            Expression<Func<PsychotropicDosageChange, object>> sortByExpression,
            bool sortDescending, int page, int pageSize)
        {
            return DataContext.CreateQuery<PsychotropicDosageChange>()
                .FilterBy(x => x.Administration.Id == admin.Id)
                .SortBy(sortByExpression)
                .DescendingWhen(sortDescending)
                .PageSize(pageSize)
                .FetchPage(page);
        }

        public IEnumerable<PsychotropicDrugType> AllDrugTypes
        {
            get {

                return DataContext.CreateQuery<PsychotropicDrugType>().FetchAll();
            }
        }

        public IEnumerable<PsychotropicFrequency> AllFrequencies
        {
            get {

                return DataContext.CreateQuery<PsychotropicFrequency>().FetchAll();

            }
        }

        public IEnumerable<PsychotropicDosageForm> AllDosageForms
        {
            get
            {

                return DataContext.CreateQuery<PsychotropicDosageForm>().FetchAll();

            }
        }
    }
}
