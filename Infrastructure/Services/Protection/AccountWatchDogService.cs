using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SnyderIS.sCore.Persistence.Protection;
using System.Web.Mvc;

namespace IQI.Intuition.Infrastructure.Services.Protection 
{
    public class AccountWatchDogService : BaseListener<Domain.Models.Account>
    {

        private const string RESTRICTION_KEY = "Restrict_Entities_For_Account";

        public static void EnableProtection()
        {
            System.Web.HttpContext.Current.Items[RESTRICTION_KEY] = "1";
        }

        public static void DisableProtection()
        {
            System.Web.HttpContext.Current.Items[RESTRICTION_KEY] = null;
        }

        protected override void OnEvaluate(IRestrictable<Domain.Models.Account> source)
        {
            if (source != null)
            {
                var key = System.Web.HttpContext.Current.Items[RESTRICTION_KEY];

                if (key != null && key.ToString() == "1")
                {
                    var actionContext = DependencyResolver.Current.GetService<IActionContext>();

                    if (source.CanBeAccessedBy(actionContext.CurrentAccount) == false)
                    {
                        throw new Exception("Protection mode prohibits access to this entity");
                    }
                }
            }
        }



    }
}
