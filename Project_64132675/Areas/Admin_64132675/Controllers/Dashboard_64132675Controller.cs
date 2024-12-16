using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project_64132675.Areas.Admin_64132675.Controllers
{
    public class Dashboard_64132675Controller : Adminbase_64132675Controller
    {
        // GET: Admin_64132675/Dashboard_64132675
        public ActionResult Index()
        {
            //@ViewBag.FullName = GetSessionFullName();
            return View();
        }
    }
}