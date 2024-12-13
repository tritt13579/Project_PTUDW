using System.Web.Mvc;

namespace Project_64132675.Areas.Admin_64132675
{
    public class Admin_64132675AreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Admin_64132675";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Admin_64132675_default",
                "Admin_64132675/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}