using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project_64132675.Services
{
    public class CustomAuthorizeAttribute_64132675 : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            // Lấy Role từ Session
            var userRole = httpContext.Session["Role"] as string;

            if (string.IsNullOrEmpty(userRole))
            {
                return false; // Người dùng chưa đăng nhập
            }

            // So sánh Role trong Session với danh sách Roles
            if (!string.IsNullOrEmpty(Roles))
            {
                // Tách danh sách roles từ chuỗi Roles
                var rolesArray = Roles.Split(',').Select(role => role.Trim()).ToArray();
                return rolesArray.Contains(userRole.Trim(), StringComparer.OrdinalIgnoreCase);
            }

            return false; // Không có role nào được định nghĩa
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {

            // Chuyển hướng đến trang đăng nhập nếu không có quyền
            filterContext.Result = new RedirectResult("~/Auth_64132675/Login");
        }
    }
}