using System.Web.Mvc;

namespace IQI.Intuition.Web.Areas.SignUp
{
    public class SignUpAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "SignUp";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "SignUp_default",
                "SignUp/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                new[] { "IQI.Intuition.Web.Areas.SignUp.Controllers" }
            );
        }
    }
}