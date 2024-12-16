using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project_64132675.Services
{
    public class CustomAuthorizeAttribute_64132675 : AuthorizeAttribute
    {
        public string Role { get; set; }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            // Lấy Role từ Session
            var userRole = httpContext.Session["Role"] as string;

            if (string.IsNullOrEmpty(userRole))
            {
                return false; // Người dùng chưa đăng nhập
            }

            // So sánh Role trong Session với Role được chỉ định
            return userRole == Role;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {

            // Chuyển hướng đến trang đăng nhập nếu không có quyền
            filterContext.Result = new RedirectResult("~/Auth_64132675/Login");
        }
    }
}