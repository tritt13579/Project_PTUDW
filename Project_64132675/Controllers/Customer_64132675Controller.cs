using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project_64132675.Controllers
{
    public class Customer_64132675Controller : Controller
    {
        // GET: Customer_64132675
        public ActionResult Index()
        {
            ViewData["UserId"] = Session["UserId"];
            ViewData["Role"] = Session["Role"];
            ViewData["FullName"] = Session["FullName"];
            return View();
        }

    }
}