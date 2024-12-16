using System;
using System.Linq;
using Project_64132675.Models;

namespace Project_64132675.Services
{
    public class AuthService_64132675
    {
        private readonly Model_64132675 _dbContext;

        public AuthService_64132675()
        {
            _dbContext = new Model_64132675();
        }

        public UserSession Login(string email, string password)
        {
            // Kiểm tra trong bảng CUSTOMER
            var customer = _dbContext.CUSTOMER.FirstOrDefault(c => c.EMAIL == email);
            if (customer != null)
            {
                string hashedPassword = customer.PASSWORD;
                if (BCrypt.Net.BCrypt.Verify(password, hashedPassword))
                {
                    return new UserSession
                    {
                        UserId = customer.CUSTOMER_ID,
                        Role = "Khách hàng",
                        FullName = $"{customer.LAST_NAME} {customer.FIRST_NAME}"
                    };
                }
            }

            // Kiểm tra trong bảng EMPLOYEE
            var employee = _dbContext.EMPLOYEE.FirstOrDefault(e => e.EMAIL == email);
            if (employee != null)
            {
                string hashedPassword = employee.PASSWORD;
                if (BCrypt.Net.BCrypt.Verify(password, hashedPassword))
                {
                    return new UserSession
                    {
                        UserId = employee.EMPLOYEE_ID,
                        Role = employee.ROLE_NAME,
                        FullName = $"{employee.LAST_NAME} {employee.FIRST_NAME}"
                    };
                }
            }

            throw new UnauthorizedAccessException("Email hoặc mật khẩu không đúng.");
        }

        public class UserSession
        {
            public long UserId { get; set; }
            public string Role { get; set; }
            public string FullName { get; set; }
        }

        // Kiểm tra email đã tồn tại hay chưa
        public bool IsEmailExists(string email)
        {
            // Kiểm tra trong cơ sở dữ liệu xem email có tồn tại không
            return _dbContext.CUSTOMER.Any(c => c.EMAIL == email);
        }

        // Đăng ký khách hàng mới
        public void RegisterCustomer(string firstName, string lastName, string gender, DateTime dateOfBirth, string email, string phoneNumber ,string password)
        {
            if (IsEmailExists(email))
            {
                throw new InvalidOperationException("Email này đã được đăng ký.");
            }

            // Mã hóa mật khẩu trước khi lưu vào cơ sở dữ liệu
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            var customer = new CUSTOMER
            {
                FIRST_NAME = firstName,
                LAST_NAME = lastName,
                GENDER = gender,
                DATE_OF_BIRTH = dateOfBirth,
                EMAIL = email,
                PHONE_NUMBER = phoneNumber,
                PASSWORD = hashedPassword
            };

            _dbContext.CUSTOMER.Add(customer);
            _dbContext.SaveChanges();
        }
    }
}
