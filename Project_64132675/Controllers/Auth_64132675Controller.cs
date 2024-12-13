using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Project_64132675.Services;
using Project_64132675.ViewModels;
using static Project_64132675.Services.AuthService_64132675;

namespace Project_64132675.Controllers
{
    public class Auth_64132675Controller : Controller
    {
        private readonly AuthService_64132675 _authService = new AuthService_64132675();

        // GET: Auth/Login
        public ActionResult Login()
        {
            return View();
        }

        // POST: Auth/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel_6132675 model)
        {
            if (!ModelState.IsValid)
            {
                return View(model); // Trả về view với thông báo lỗi nếu model không hợp lệ
            }

            try
            {
                // Gọi phương thức đăng nhập từ AuthService
                UserSession session = _authService.Login(model.Email, model.Password);

                // Lưu thông tin người dùng vào session
                Session["UserId"] = session.UserId;
                Session["Role"] = session.Role;
                Session["FullName"] = session.FullName;

                switch (session.Role)
                {
                    case "Quản trị viên":
                        return RedirectToAction("Index", "Admin_64132675");
                    case "Lễ tân":
                        return RedirectToAction("Index", "Receptionist_64132675");
                    case "Khách hàng":
                        return RedirectToAction("Index", "Customer_64132675");
                    default:
                        return RedirectToAction("Login", "Auth_64132675");
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                // Nếu thông tin đăng nhập sai, thêm lỗi vào ModelState để hiển thị thông báo cho người dùng
                ModelState.AddModelError(string.Empty, "Email hoặc mật khẩu không đúng.");
                return View(model); // Trả về view với lỗi đăng nhập
            }
        }

        // GET: Auth/Logout
        public ActionResult Logout()
        {
            Session.Clear(); // Xóa toàn bộ session
            return RedirectToAction("Login", "Auth_64132675");
        }
    }
}
