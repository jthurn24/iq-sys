using System;
using RedArrow.Framework.ObjectModel.AuditTracking;

namespace IQI.Intuition.Infrastructure.Services
{
    public interface IAuthentication : IUserNameProvider
    {
        bool CurrentUserIsAuthenticated { get; }

        Guid? CurrentUserGuid { get; }

        Guid? CurrentSystemUserGuid { get; }

        void SignInUser(Guid userGuid);

        void SignOutCurrentUser();

        void SignInSystemUser(Guid userGuid);

        void SignOutCurrentSystemUser();

    }
}