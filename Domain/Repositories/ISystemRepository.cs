using System;
using System.Linq.Expressions;
using RedArrow.Framework.Persistence;
using IQI.Intuition.Domain.Models;
using System.Collections.Generic;

namespace IQI.Intuition.Domain.Repositories
{
    public interface ISystemRepository
    {
        IEnumerable<Permission> GetAllPermissions();
        void Add(AuditEntry src);


        SystemUser GetSystemUserByCredentials(string login, string password);
        SystemUser GetSystemUserByGuid(Guid? guid);
        IEnumerable<SystemUser> GetSystemUsers();
        IEnumerable<SystemProduct> GetSystemProducts();

        void Add(SystemEmailNotification notification);
        void Add(SystemSMSNotification notification);
        void Add(SignUpRequest requests);

        AccountUser GetUserByGuid(Account account, Guid? guid);
        AccountUser GetUserByGuid(Guid? guid);
        AccountUser GetUserByCredentials(Account account, string login);
        AccountUser GetUserByCredentials(Account account, string login, string password);
        AccountUser GetUserById(int id);
        SignUpRequest GetSignUpRequestByCode(string code);

        IPagedQueryResult<AccountUser> FindUser(Account account,
            string login,
            string firstName,
            string lastName,
            string emailAddress,
            bool includeSystemUsers,
            Expression<Func<AccountUser, object>> sortByExpression,
            bool sortDescending, int page, int pageSize);


        IPagedQueryResult<AuditEntry> FindAuditEntry(
            Guid? patientId,
            Guid? componentId,
            IEnumerable<Domain.Enumerations.AuditEntryType> types,
            int? facilityID,
            Expression<Func<AuditEntry, object>> sortByExpression,
            bool sortDescending, int page, int pageSize);

        IEnumerable<AuditEntry> FindAuditEntry(int facilityId, DateTime after);

        MobileToken GetMobileToken(string token);
        void Add(MobileToken token);

        string GenerateFacilitySubDomain(string name);
        
    }
}
