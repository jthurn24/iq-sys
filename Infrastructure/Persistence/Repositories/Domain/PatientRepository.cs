using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Persistence;
using IQI.Intuition.Domain.Models;
using IQI.Intuition.Domain.Repositories;
using IQI.Intuition.Domain;
using SnyderIS.sCore.Encryption;

namespace IQI.Intuition.Infrastructure.Persistence.Repositories.Domain
{
    public class PatientRepository : AbstractRepository<IDataContext>, IPatientRepository
    {
        public PatientRepository(IDataContext dataContext)
            : base(dataContext) { }

        public void Add(Patient patient)
        {
            DataContext.TrackChanges(patient);
        }

        public Patient Get(int id)
        {
            return DataContext.Fetch<Patient>(id);
        }

        public Patient Get(Guid guid)
        {
            return DataContext.CreateQuery<Patient>()
                .FilterBy(patient => patient.Guid == guid)
                .FetchFirst();
        }

        public IPagedQueryResult<Patient> Find(Facility facility, string firstName, string lastName, string birthDate, string wingName, string roomName,
           Enumerations.PatientStatus? status, Expression<Func<Patient, object>> sortByExpression, bool sortDescending, int page, int pageSize)
        {
            var query = DataContext.CreateQuery<Patient>()
                .FilterBy(patient => patient.Room.Wing.Floor.Facility == facility)
                .FilterBy(patient => patient.Deleted == null || patient.Deleted == false);

            if (birthDate.IsNotNullOrWhiteSpace())
            {
                try
                {
                    var birthDateSegments = birthDate.Split('/', '-');

                    switch (birthDateSegments.Length)
                    {
                        case 1:
                            // It appears we were given a year only, try to find dates with the same year
                            int year = birthDate.ConvertToTypeOrDefault<int>();
                            query = query.FilterBy(patient => patient.BirthDate.Value.Year == year);

                            break;

                        case 2:
                            // It appears we were given a month/year or day/month combination, try to find dates with the same combination
                            int firstSegment = birthDateSegments[0].ConvertToTypeOrDefault<int>();
                            int secondSegment = birthDateSegments[1].ConvertToTypeOrDefault<int>();

                            if (secondSegment < 1000)
                            {
                                // Looks like day/month
                                query = query.FilterBy(patient => patient.BirthDate.Value.Day == secondSegment
                                    && patient.BirthDate.Value.Month == firstSegment);
                            }
                            else 
                            {
                                // Looks like month/year
                                query = query.FilterBy(patient => patient.BirthDate.Value.Year == secondSegment
                                    && patient.BirthDate.Value.Month == firstSegment);
                            }

                            break;

                        case 3:
                            // It appears we were given a full date, attempt to porse it and, if sucessful, try to find matching dates
                            DateTime date;

                            if (DateTime.TryParse(birthDate, out date))
                            {
                                query = query.FilterBy(patient => patient.BirthDate.Value == date);
                            }

                            break;
                    }
                }
                catch
                {
                    // Ignore any parsing errors
                }
            }

            if (wingName.IsNotNullOrWhiteSpace())
            {
                query = query.FilterBy(patient => patient.Room.Wing.Name.Contains(wingName));
            }

            if (roomName.IsNotNullOrWhiteSpace())
            {
                query = query.FilterBy(patient => patient.Room.Name == roomName);
            }

            if (status != null)
            {
                query = query.FilterBy(patient => patient.CurrentStatus == status);
            }


            var results = query.FetchAll();

            if (firstName.IsNotNullOrWhiteSpace())
            {
                results = results.Where(patient => patient.GetFirstName().ToLower().Contains(firstName.ToLower()));
            }

            if (lastName.IsNotNullOrWhiteSpace())
            {
                results = results.Where(patient => patient.GetLastName().ToLower().Contains(lastName.ToLower()));
            }

            if (sortDescending)
            {
                results = results.AsQueryable().OrderByDescending(sortByExpression);
            }
            else
            {
                results = results.AsQueryable().OrderBy(sortByExpression);
            }

            var pager = new RedArrow.Framework.Persistence.PagedQueryResult<Patient>();
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


        public IEnumerable<Patient> Find(Facility facility)
        {
            return DataContext.CreateQuery<Patient>()
                .FilterBy(patient => patient.Room.Wing.Floor.Facility == facility)
                .FilterBy(patient => patient.Deleted == null || patient.Deleted == false)
                .FetchAll();
        }

        public IEnumerable<Patient> FindByName(Facility facility, string name, int resultLimit)
        {
            name = name.Trim(' ', ',', '\t');
            int commaIndex = name.IndexOf(',');
            string firstNamePart = "";

            if (commaIndex > 0)
            {
                firstNamePart = name.Substring(commaIndex + 1).Trim();
                name = name.Substring(0, commaIndex).Trim();
            }

            var data = DataContext.CreateQuery<Patient>()
                .FilterBy(patient => patient.Room.Wing.Floor.Facility == facility)
                .FilterBy(patient => patient.Deleted == null || patient.Deleted == false)
                .FetchAll();

            return data.Where(patient => (patient.GetLastName().StartsWith(name,true,null) || patient.GetFirstName().StartsWith(name,true,null))
                 && (commaIndex < 0 || patient.GetFirstName().StartsWith(firstNamePart,true,null))
                 ).Take(resultLimit);
        }


        public PatientStatusChange GetStatusChange(int id)
        {
            return DataContext.Fetch<PatientStatusChange>(id);
        }

        public IEnumerable<PatientFlagType> AllPatientFlags
        {
            get
            {
                return DataContext.CreateQuery<PatientFlagType>()
                    .SortBy(x => x.SortOrder)
                    .FetchAll();
            }
        }
    }
}
