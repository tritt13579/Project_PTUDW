using System.Web.Mvc;

namespace Project_64132675.Areas.Employee_64132675
{
    public class Employee_64132675AreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Employee_64132675";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Employee_64132675_default",
                "Employee_64132675/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}