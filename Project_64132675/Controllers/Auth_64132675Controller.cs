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

        // GET: Auth_64132675/Login
        public ActionResult Login()
        {
            return View();
        }

        // POST: Auth_64132675/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel_64132675 model)
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
                        return RedirectToAction("Index", "Dashboard_64132675", new { area = "Employee_64132675" });
                    case "Lễ tân":
                        return RedirectToAction("Index", "Dashboard_64132675", new { area = "Employee_64132675" });
                    case "Khách hàng":
                        return RedirectToAction("Index", "Home_64132675", new { area = "Customer_64132675" });
                    default:
                        return RedirectToAction("Login", "Auth_64132675");

                }

            }
            catch (UnauthorizedAccessException ex)
            {
                // Nếu thông tin đăng nhập sai, thêm lỗi vào ModelState để hiển thị thông báo cho người dùng
                ModelState.AddModelError(string.Empty, "Email hoặc mật khẩu không đúng." + ex);
                return View(model); // Trả về view với lỗi đăng nhập
            }
        }

        // GET: Auth_64132675/Register
        public ActionResult Register()
        {
            return View();
        }

        // POST: Auth_64132675/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel_64132675 model)
        {
            if (!ModelState.IsValid)
            {
                return View(model); // Trả về view với thông báo lỗi nếu model không hợp lệ
            }
            try
            {
                // Gọi phương thức đăng ký từ AuthService
                _authService.RegisterCustomer(model.FirstName, model.LastName, model.Gender, model.DateOfBirth, model.Email, model.PhoneNumber, model.Password);

                // Đăng ký thành công, chuyển hướng đến trang đăng nhập
                return RedirectToAction("Login", "Auth_64132675");
            }
            catch (InvalidOperationException ex)
            {
                // Nếu email đã tồn tại, hiển thị thông báo lỗi
                ModelState.AddModelError("Email", ex.Message);
                return View(model); // Trả về view với thông báo lỗi
            }
            catch (Exception ex)
            {
                // Xử lý lỗi chung
                ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi trong quá trình đăng ký." + ex);
                return View(model); // Trả về view nếu có lỗi
            }
        }
        // GET: Auth_64132675/Logout
        public ActionResult Logout()
        {
            Session.Clear(); // Xóa toàn bộ session
            return RedirectToAction("Login", "Auth_64132675");
        }
    }
}
