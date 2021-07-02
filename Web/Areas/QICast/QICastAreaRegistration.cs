using System.Web.Mvc;

namespace IQI.Intuition.Web.Areas.QICast
{
    public class QICastAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "QICast";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "QICast_default",
                "QICast/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
