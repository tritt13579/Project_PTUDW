using Project_64132675.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project_64132675.Areas.Employee_64132675.Controllers
{
    [CustomAuthorizeAttribute_64132675(Roles = "Quản trị viên, Lễ tân")]
    public class Employeebase_64132675Controller : Controller
    {
        protected string GetSessionUserId()
        {
            return Session["UserId"] as string;
        }

        protected string GetSessionFullName()
        {
            return Session["FullName"] as string;
        }

        // Kiểm tra Session trước khi thực hiện hành động
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Session["Role"] == null)
            {
                // Nếu không có session Role, chuyển hướng đến trang đăng nhập
                filterContext.Result = RedirectToAction("Login", "Account");
            }

            base.OnActionExecuting(filterContext);
        }
    }
}