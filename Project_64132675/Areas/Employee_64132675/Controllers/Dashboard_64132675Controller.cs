using Project_64132675.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project_64132675.Areas.Employee_64132675.Controllers
{
    [CustomAuthorizeAttribute_64132675(Roles = "Quản trị viên, Lễ tân")]
    public class Dashboard_64132675Controller : Controller
    {
        // GET: Employee_64132675/Dashboard_64132675
        public ActionResult Index()
        {
            return View();
        }
    }
}