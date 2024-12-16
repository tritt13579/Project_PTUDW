using System.Web.Mvc;

namespace Project_64132675.Areas.Receptionist_64132675
{
    public class Receptionist_64132675AreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Receptionist_64132675";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Receptionist_64132675_default",
                "Receptionist_64132675/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}