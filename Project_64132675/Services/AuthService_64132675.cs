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
                        FullName = $"{employee.FIRST_NAME} {employee.LAST_NAME}"
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
    }
}
