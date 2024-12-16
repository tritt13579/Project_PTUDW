using Project_64132675.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project_64132675.Areas.Receptionist_64132675.Controllers
{
    [CustomAuthorizeAttribute_64132675(Role = "Lễ tân")]
    public class Receptionistbase_64132675Controller : Controller
    {
        protected string GetSessionUserId()
        {
            return Session["UserId"] as string;
        }

        protected string GetSessionFullName()
        {
            return Session["FullName"] as string;
        }
    }
}