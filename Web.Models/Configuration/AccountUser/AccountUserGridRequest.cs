using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using SnyderIS.sCore.Web.Mvc.JQGridHelpers;

namespace IQI.Intuition.Web.Models.Configuration.AccountUser
{
    [ModelBinder(typeof(AjaxRequestModelBinder<AccountUserGridRequest>))]
    public class AccountUserGridRequest : AjaxRequestModel<AccountUserGridRequest>
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public bool IsActive { get; set; }

        public Expression<Func<Domain.Models.AccountUser, object>> SortBy 
        {
            get
            {
                if (RequestedSortBy(model => model.Login))
                {
                    return a => a.Login;
                }

                if (RequestedSortBy(model => model.FirstName))
                {
                    return a => a.FirstName;
                }

                if (RequestedSortBy(model => model.LastName))
                {
                    return a => a.LastName;
                }

                if (RequestedSortBy(model => model.EmailAddress))
                {
                    return a => a.EmailAddress;
                }

                if (RequestedSortBy(model => model.IsActive))
                {
                    return a => a.IsActive;
                }

                return a => a.Login;

            }
        }
    }
}
