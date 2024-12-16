using System.Web.Mvc;

namespace Project_64132675.Areas.Customer_64132675
{
    public class Customer_64132675AreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Customer_64132675";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Customer_64132675_default",
                "Customer_64132675/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}