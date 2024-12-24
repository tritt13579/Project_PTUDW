using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Web;

namespace Project_64132675.Services
{
    public class RoomStatusUpdater_64132675
    {
        private static Timer _timer;

        public static void Start()
        {
            // _timer sẽ bắt đầu chạy ngay lập tức và sau đó cứ mỗi ngày (24 giờ), nó sẽ thực hiện tác vụ
            _timer = new Timer(UpdateRoomStatus, null, TimeSpan.Zero, TimeSpan.FromDays(1));  // Cập nhật mỗi ngày
        }

        private static void UpdateRoomStatus(object state)
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["Model_64132675"].ConnectionString;
                string sqlQuery = "EXEC UpdateRoomStatus";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(sqlQuery, connection);
                    connection.Open();
                    command.ExecuteNonQuery();  // Thực thi câu lệnh SQL
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occurred: " + ex.Message);

            }
        }
    }
}