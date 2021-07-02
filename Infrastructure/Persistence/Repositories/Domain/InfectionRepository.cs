using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Persistence;
using IQI.Intuition.Domain.Models;
using IQI.Intuition.Domain.Repositories;

namespace IQI.Intuition.Infrastructure.Persistence.Repositories.Domain
{
    public class InfectionRepository : AbstractRepository<IDataContext>, IInfectionRepository
    {
        public InfectionRepository(IDataContext dataContext)
            : base(dataContext) { }

        public void Add(InfectionVerification infection)
        {
            DataContext.TrackChanges(infection);
        }

        public InfectionVerification Get(int id)
        {
            return DataContext.Fetch<InfectionVerification>(id);
        }

        public InfectionVerification Get(Guid guid)
        {
            return DataContext.CreateQuery<InfectionVerification>().FilterBy(x => x.Guid == guid).FetchAll().FirstOrDefault();
        }

        public void Delete(InfectionTreatment value)
        {
            DataContext.Delete(value);
        }

        public void Delete(InfectionLabResult value)
        {
            if (value.ResultPathogens != null)
            {
                foreach (var i in value.ResultPathogens)
                {
                    DataContext.Delete(i);
                }

            }

            DataContext.Delete(value);
        }

        public void Delete(InfectionLabResultPathogen value)
        {
            DataContext.Delete(value);
        }

        public IPagedQueryResult<InfectionVerification> Find(Facility facility, string patientName, string roomAndWingName,
            string type, string firstNoted, string reasonForEntry, string labOrXrayDate, string labFindings,
            string treatement,  string dateResolved, bool includeResolved, 
            Expression<Func<InfectionVerification, object>> sortByExpression,
            bool sortDescending, int page, int pageSize)
        {
            var query = DataContext.CreateQuery<InfectionVerification>()
               .FilterBy(infection => infection.Patient.Room.Wing.Floor.Facility == facility
               && (includeResolved || infection.IsResolved.Value != true))
               .FilterBy(infection => infection.Deleted == null || infection.Deleted == false);



            if (roomAndWingName.IsNotNullOrWhiteSpace())
            {
                query = query.FilterBy(infection => (infection.Room.Name + " " + infection.Room.Wing.Name).Contains(roomAndWingName));
            }

            if (type.IsNotNullOrWhiteSpace())
            {
                query = query.FilterBy(infection => infection.InfectionSite.Type.Name.Contains(type));
            }

            if (firstNoted.IsNotNullOrWhiteSpace())
            {
                DateTime date;

                if (DateTime.TryParse(firstNoted, out date))
                {
                    query = query.FilterBy(infection => infection.FirstNotedOn.Value == date);
                }
            }

            if (labOrXrayDate.IsNotNullOrWhiteSpace())
            {
                DateTime date;

                if (DateTime.TryParse(labOrXrayDate, out date))
                {
                    query = query.FilterBy(infection =>  infection.ChestXrayCompletedOn.Value == date);
                }
            }

            if (labFindings.IsNotNullOrWhiteSpace())
            {
                query = query.FilterBy(infection => infection.LabResultsText.Contains(labFindings));
            }

            if (treatement.IsNotNullOrWhiteSpace())
            {
                query = query.FilterBy(infection => infection.TreatementText.Contains(treatement));
            }


            if (dateResolved.IsNotNullOrWhiteSpace())
            {
                DateTime date;

                if (DateTime.TryParse(dateResolved, out date))
                {
                    query = query.FilterBy(infection => infection.ResolvedOn.Value == date);
                }
            }

            var results = query.FetchAll();

            if (patientName.IsNotNullOrWhiteSpace())
            {
                results = results.Where(infection => (infection.Patient.GetFirstName() + " " + infection.Patient.GetLastName()).ToLower().Contains(patientName.ToLower()));
            }

            if (sortDescending)
            {
                results = results.AsQueryable().OrderByDescending(sortByExpression);
            }
            else
            {
                results = results.AsQueryable().OrderBy(sortByExpression);
            }


            var pager = new RedArrow.Framework.Persistence.PagedQueryResult<InfectionVerification>();
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

        public IPagedQueryResult<InfectionVerification> Find(Patient patient,
            Expression<Func<InfectionVerification, object>> sortByExpression,
            bool sortDescending, int page, int pageSize)
        {
            return DataContext.CreateQuery<InfectionVerification>()
                .FilterBy(infection => infection.Patient == patient)
                .FilterBy(infection => infection.Deleted == null || infection.Deleted == false)
                .SortBy(sortByExpression)
                .DescendingWhen(sortDescending)
                .PageSize(pageSize)
                .FetchPage(page);
        }

        public IEnumerable<InfectionVerification> FindActiveForWing(int id)
        {
            return DataContext.CreateQuery<InfectionVerification>()
                            .FilterBy(infection => infection.Room.Wing.Id == id && infection.ResolvedOn.HasValue == false)
                            .FilterBy(infection => infection.Deleted == null || infection.Deleted == false)
                            .SortBy(x => x.CreatedAt)
                            .Descending()
                            .FetchAll();
        }

        public IEnumerable<InfectionVerification> FindForRoom(int id)
        {
            return DataContext.CreateQuery<InfectionVerification>()
                            .FilterBy(infection => infection.Room.Id == id)
                            .FilterBy(infection => infection.Deleted == null || infection.Deleted == false)
                            .SortBy(x => x.CreatedAt)
                            .Descending()
                            .FetchAll();
        }

        public IEnumerable<InfectionVerification> FindForLineListing(Facility facility, 
            int? wingId,
            int? floorId, 
            bool includeResolved, 
            DateTime? startDate, 
            DateTime? endDate, 
            int? type,
           bool pendingLabsOnly,
            IList<int> pathogens,
            IList<string> antibiotics,
            IList<int> labTests)
        {
            var q = DataContext.CreateQuery<InfectionVerification>()
                .FilterBy(x => x.Room.Wing.Floor.Facility.Id == facility.Id)
                .FilterBy(infection => infection.Deleted == null || infection.Deleted == false);

            if (wingId.HasValue)
            {
                q = q.FilterBy(x => x.Room.Wing.Id == wingId.Value);
            }

            if (floorId.HasValue)
            {
                q = q.FilterBy(x => x.Room.Wing.Floor.Id == floorId.Value);
            }

            if (!includeResolved)
            {
                q = q.FilterBy(x => x.ResolvedOn == null);
            }

            if (startDate.HasValue)
            {
                q = q.FilterBy(x => x.ResolvedOn == null || x.ResolvedOn >= startDate || x.FirstNotedOn >= startDate );
            }

            if (endDate.HasValue)
            {
                q = q.FilterBy(x => (x.ResolvedOn == null && x.FirstNotedOn <= endDate.Value) || x.ResolvedOn <= endDate.Value || x.FirstNotedOn <= endDate.Value);
            }

            if (type.HasValue)
            {
                q = q.FilterBy(x => x.InfectionSite.Type.Id == type.Value);
            }

            if (pendingLabsOnly)
            {
                q = q.FilterBy(x => x.LabsPending == true);
            }

            var results = q.FetchAll();

            if (pathogens != null && pathogens.Count() > 0)
            {
                results = results.Where(x => x.LabResults.SelectMany(xx => xx.ResultPathogens).ToList().Select(xx => xx.Pathogen.Id).ContainsAny(pathogens));
            }

            if (antibiotics != null && antibiotics.Count() > 0)
            {
                results = results.Where(x => x.Treatments.Select(xx => xx.TreatmentName).ContainsAny(antibiotics));
            }

            if (labTests != null && labTests.Count() > 0)
            {
                results = results.Where(x => 
                    x.LabResults.Where(xx => xx.LabResult.Positive)
                    .Select(xx => xx.LabTestType.Id).ContainsAny(labTests));
            }

            return results.OrderByDescending(x => x.FirstNotedOn);
        }

        public IEnumerable<InfectionVerification> FindForPatient(Guid guid, int? typeId, int? siteId)
        {
            var q = DataContext.CreateQuery<InfectionVerification>()
               .FilterBy(x => x.Patient.Guid == guid);

            if (typeId.HasValue)
            {
                q = q.FilterBy(x => x.InfectionSite.Type.Id == typeId.Value);
            }


            if (siteId.HasValue)
            {
                q = q.FilterBy(x => x.InfectionSite.Id == siteId.Value);
            }

            return q.FetchAll();
        }

        public IEnumerable<InfectionTreatment> FindTreatments(Facility facility,DateTime startDate, DateTime endDate, int? infectionType)
        {
            var q = DataContext.CreateQuery<InfectionTreatment>()
                .FilterBy(x => x.InfectionVerification.Deleted == null || x.InfectionVerification.Deleted == false)
                .FilterBy(x => x.AdministeredOn >= startDate && x.AdministeredOn <= endDate)
                .FilterBy(x => x.TreatmentType.IsAntibiotic == true)
                .FilterBy(x => x.InfectionVerification.Room.Wing.Floor.Facility.Id == facility.Id);

            if (infectionType.HasValue)
            {
                q = q.FilterBy(x => x.InfectionVerification.InfectionSite.Type.Id == infectionType.Value);
            }

            return q.FetchAll();
        }

        public IEnumerable<InfectionVerification> FindActiveFacility(Facility facility)
        {
            return DataContext.CreateQuery<InfectionVerification>()
                .FilterBy(infection => infection.Room.Wing.Floor.Facility.Id == facility.Id && infection.ResolvedOn.HasValue == false)
                .FilterBy(infection => infection.Deleted == null || infection.Deleted == false)
                .SortBy(x => x. CreatedAt)
                .Descending()
                .FetchAll();
        }

        public IEnumerable<InfectionVerification> FindForFacility(Facility facility, IEnumerable<Guid> guids)
        {
            return DataContext.CreateQuery<InfectionVerification>()
                .FilterBy(infection => infection.Room.Wing.Floor.Facility.Id == facility.Id )
                .FilterBy(infection => infection.Deleted == null || infection.Deleted == false)
                .FilterBy(infection => guids.ToList().Contains(infection.Guid))
                .SortBy(x => x.CreatedAt)
                .Descending()
                .FetchAll();
        }

        public IEnumerable<InfectionVerification> FindCreatedIn(int month, int year)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            return DataContext.CreateQuery<InfectionVerification>()
                .FilterBy(x => x.CreatedAt >= startDate && x.CreatedAt <= endDate)
                .FetchAll();
        }

        public IEnumerable<InfectionType> AllInfectionTypes
        {
            get
            {
                return DataContext.CreateQuery<InfectionType>()
                    .SortBy(type => type.SortOrder)
                    .FetchAll();
            }
        }

        public IEnumerable<InfectionSite> AllInfectionSites
        {
            get
            {
                return DataContext.CreateQuery<InfectionSite>()
                    .SortBy(site => site.Name)
                    .FetchAll();
            }
        }

        public IEnumerable<InfectionCriteria> AllInfectionCriteria
        {
            get
            {
                return DataContext.CreateQuery<InfectionCriteria>()
                    .FetchAll();
            }
        }

        public IEnumerable<InfectionRiskFactor> AllRiskFactors
        {
            get 
            {
                return DataContext.CreateQuery<InfectionRiskFactor>()
                    .SortBy(riskFactor => riskFactor.Name)
                    .FetchAll(); 
            }
        }


        public IEnumerable<InfectionSymptom> AllSymptoms
        {
            get
            {
                return DataContext.CreateQuery<InfectionSymptom>()
                    .SortBy(x => x.Name)
                    .FetchAll();
            }
        }

        public IEnumerable<InfectionDefinition> AllInfectionDefinitions
        {
            get
            {
                return DataContext.CreateQuery<InfectionDefinition>()
                    .SortBy(x => x.Name)
                    .FetchAll();
            }
        }

    }
}
