using System;
using System.Collections.Generic;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Persistence;
using IQI.Intuition.Domain.Models;
using IQI.Intuition.Domain;
using IQI.Intuition.Domain.Repositories;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace IQI.Intuition.Infrastructure.Persistence.Repositories.Domain
{
    public class SystemRepository  : AbstractRepository<IDataContext>, ISystemRepository
    {

        public SystemRepository(IDataContext dataContext)
            : base(dataContext) { }

        public IEnumerable<Permission> GetAllPermissions()
        {
            return DataContext.CreateQuery<Permission>().FetchAll();
        }

        public void Add(AuditEntry src)
        {
            DataContext.TrackChanges(src);
        }

        public void Add(SystemEmailNotification src)
        {
            DataContext.TrackChanges(src);
        }

        public void Add(SystemSMSNotification src)
        {
            DataContext.TrackChanges(src);
        }

        public void Add(SignUpRequest src)
        {
            DataContext.TrackChanges(src);
        }


        public AccountUser GetUserByCredentials(Account account, string login, string password)
        {
            if (login.IsNullOrWhiteSpace()
                || password.IsNullOrWhiteSpace())
            {
                return null;
            }

            var accountUser = account.ThrowIfNullArgument("account")
                .Users.SingleOrDefault(user =>
                    user.Login.CaseInsensitiveEquals(login));

            if (accountUser == null
                || !accountUser.IsActive
                || !accountUser.CheckPassword(password))
            {
                return null;
            }

            return accountUser;
        }

        public AccountUser GetUserByCredentials(Account account, string login)
        {
            if (login.IsNullOrWhiteSpace())
            {
                return null;
            }

            var accountUser = account.ThrowIfNullArgument("account")
                .Users.SingleOrDefault(user =>
                    user.Login.CaseInsensitiveEquals(login));

            if (accountUser == null
                || !accountUser.IsActive)
            {
                return null;
            }

            return accountUser;
        }


        public AccountUser GetUserByGuid(Guid? guid)
        {
            return DataContext.CreateQuery<AccountUser>().FilterBy(x => x.Guid == guid).FetchAll().FirstOrDefault();
        }

        public AccountUser GetUserByGuid(Account account, Guid? guid)
        {
            if (account != null && guid.HasValue)
            {
                return account.Users.SingleOrDefault(user =>
                    user.Guid == guid.Value);
            }

            return null;
        }

        public AccountUser GetUserById(int id)
        {
            return DataContext.CreateQuery<AccountUser>().FilterBy(x => x.Id == id).FetchAll().FirstOrDefault();
        }

        public IPagedQueryResult<AccountUser> FindUser(Account account,
            string login,
            string firstName,
            string lastName,
            string emailAddress,
            bool includeSystemUsers,
            System.Linq.Expressions.Expression<Func<AccountUser, object>> sortByExpression, bool sortDescending, int page, int pageSize)
                {
                    var q = DataContext.CreateQuery<AccountUser>()
                        .FilterBy(x => x.Account.Id == account.Id);

                    if (login.IsNotNullOrEmpty())
                    {
                        q = q.FilterBy(x => x.Login.Contains(login));
                    }

                    if (firstName.IsNotNullOrEmpty())
                    {
                        q = q.FilterBy(x => x.FirstName.Contains(login));
                    }

                    if (lastName.IsNotNullOrEmpty())
                    {
                        q = q.FilterBy(x => x.LastName.Contains(login));
                    }

                    if (emailAddress.IsNotNullOrEmpty())
                    {
                        q = q.FilterBy(x => x.EmailAddress.Contains(login));
                    }

                    if (!includeSystemUsers)
                    {
                        q = q.FilterBy(x => x.SystemUser == false);
                    }

                    return q.SortBy(sortByExpression)
                        .DescendingWhen(sortDescending)
                        .PageSize(pageSize)
                        .FetchPage(page);
                }

 
        public SystemUser GetSystemUserByCredentials(string login, string password)
        {
            var result = DataContext.CreateQuery<SystemUser>()
                .FilterBy(x => x.Login == login)
                .FetchAll().FirstOrDefault();

            if (result != null && result.CheckPassword(password))
            {
                return result;
            }

            return null;
        }

        public SystemUser GetSystemUserByGuid(Guid? guid)
        {
            return DataContext.CreateQuery<SystemUser>()
                .FilterBy(x => x.Guid == guid)
                .FetchAll().FirstOrDefault();
        }


        public IEnumerable<SystemUser> GetSystemUsers()
        {
            return DataContext.CreateQuery<SystemUser>()
                .FilterBy(x => x.IsActive)
                .FetchAll();
        }

        public IEnumerable<SystemProduct> GetSystemProducts()
        {
            return DataContext.CreateQuery<SystemProduct>()
                .FetchAll();
        }


        public IEnumerable<AuditEntry> FindAuditEntry(int facilityId, DateTime after)
        {
            return DataContext.CreateQuery<AuditEntry>()
                .FilterBy(x => x.PerformedAt > after)
                .FilterBy(x => x.Facility.Id == facilityId)
                .FetchAll();
        }

        public SignUpRequest GetSignUpRequestByCode(string code)
        {
            return DataContext.CreateQuery<SignUpRequest>()
                .FilterBy(x => x.Code == code)
                .FetchAll()
                .FirstOrDefault();
        }



        public IPagedQueryResult<AuditEntry> FindAuditEntry(
            Guid? patientId,
            Guid? componentId,
            IEnumerable<Enumerations.AuditEntryType> types,
            int? facilityID,
            Expression<Func<AuditEntry, object>> sortByExpression,
            bool sortDescending, int page, int pageSize)
        {

            var q = DataContext.CreateQuery<AuditEntry>()
                        .FilterBy(x => x.Facility.Id == facilityID);

            if (patientId != null)
            {
                q = q.FilterBy(x => x.TargetPatient == patientId);
            }


            if (componentId != null)
            {
                q = q.FilterBy(x => x.TargetComponent == componentId);
            }

            if (types != null && types.Count() > 0)
            {
                q = q.FilterBy(x => types.Contains(x.AuditType));
            }

            return q.SortBy(sortByExpression)
                .DescendingWhen(sortDescending)
                .PageSize(pageSize)
                .FetchPage(page);
        }

        public MobileToken GetMobileToken(string token)
        {
            return DataContext.CreateQuery<MobileToken>()
                .FilterBy(x => x.Token == token)
                .FetchAll()
                .FirstOrDefault();
        }

        public void Add(MobileToken token)
        {
            DataContext.TrackChanges(token);
        }

        public string GenerateFacilitySubDomain(string name)
        {
            return GenerateFacilitySubDomain(name, 0);
        }

        private string GenerateFacilitySubDomain(string name, int inc)
        {
            var cleanName = Regex.Replace(name, "[^a-zA-Z]", "").ToLower();
            if (inc > 0) cleanName = string.Concat(cleanName, inc);

            var match = DataContext.CreateQuery<Facility>()
                .FilterBy(x => x.SubDomain == cleanName)
                .FetchAll()
                .FirstOrDefault();

            var match2 = DataContext.CreateQuery<SignUpRequest>()
                .FilterBy(x => x.SubDomain == cleanName)
                .FetchAll()
                .FirstOrDefault();

            if (match == null && match2 == null)
            {
                return cleanName;
            }

            inc = inc + 1;
            return GenerateFacilitySubDomain(name, inc);

        }
    }
}
