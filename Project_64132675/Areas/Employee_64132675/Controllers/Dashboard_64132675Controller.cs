using Project_64132675.Areas.Employee_64132675.ViewModels;
using Project_64132675.Models;
using Project_64132675.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Project_64132675.Areas.Employee_64132675.Controllers
{
    public class Dashboard_64132675Controller : Employeebase_64132675Controller
    {
        private readonly Model_64132675 db = new Model_64132675();

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetKPIData()
        {
            var today = DateTime.Now.Date;
            var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            // Tổng doanh thu tháng hiện tại
            var totalRevenue = db.BOOKING
                .Where(b => DbFunctions.TruncateTime(b.BOOKING_DATE) >= firstDayOfMonth &&
                           DbFunctions.TruncateTime(b.BOOKING_DATE) <= lastDayOfMonth &&
                           b.PAYMENTSTATUS.PAYMENT_STATUS_NAME == "Đã Thanh Toán")
                .Sum(b => (decimal?)b.BOOKING_AMOUNT) ?? 0;

            // Số phòng đã đặt - sử dụng ToList() để thực hiện query trước khi đếm
            var currentBookings = db.BOOKING
                .Where(b => DbFunctions.TruncateTime(b.CHECKIN_DATE) <= today &&
                           DbFunctions.TruncateTime(b.CHECKOUT_DATE) >= today)
                .ToList();
            var bookedRooms = currentBookings.SelectMany(b => b.ROOM).Count();

            // Tổng số phòng
            var totalRooms = db.ROOM.Count();

            // Dịch vụ sử dụng nhiều nhất
            var bookingsThisMonth = db.BOOKING
                .Where(b => DbFunctions.TruncateTime(b.BOOKING_DATE) >= firstDayOfMonth)
                .Include(b => b.SERVICE)
                .ToList();

            var topService = bookingsThisMonth
                .SelectMany(b => b.SERVICE)
                .GroupBy(s => s.SERVICE_NAME)
                .Select(g => new { Name = g.Key, Count = g.Count() })
                .OrderByDescending(g => g.Count)
                .FirstOrDefault();

            // Số khách hiện tại
            var currentGuests = currentBookings.Sum(b => b.NUM_ADULT + b.NUM_CHILDREN);

            return Json(new
            {
                TotalRevenue = totalRevenue,
                Occupancy = new { Booked = bookedRooms, Total = totalRooms },
                TopService = topService,
                CurrentGuests = currentGuests
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetRevenueByRoomClass(DateTime? startDate, DateTime? endDate)
        {
            if (!startDate.HasValue) startDate = DateTime.Now.AddMonths(-1);
            if (!endDate.HasValue) endDate = DateTime.Now;

            var bookings = db.BOOKING
                .Where(b => DbFunctions.TruncateTime(b.BOOKING_DATE) >= startDate &&
                           DbFunctions.TruncateTime(b.BOOKING_DATE) <= endDate &&
                           b.PAYMENTSTATUS.PAYMENT_STATUS_NAME == "Đã Thanh Toán")
                .Include(b => b.ROOM.Select(r => r.ROOMCLASS))
                .ToList();

            var revenueByClass = bookings
                .SelectMany(b => b.ROOM)
                .GroupBy(r => r.ROOMCLASS.CLASS_NAME)
                .Select(g => new
                {
                    RoomClass = g.Key,
                    Revenue = g.Sum(r => r.ROOMCLASS.BASE_PRICE)
                })
                .OrderByDescending(x => x.Revenue)
                .ToList();

            return Json(new
            {
                labels = revenueByClass.Select(x => x.RoomClass),
                data = revenueByClass.Select(x => x.Revenue)
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetBookingSources()
        {
            var sources = db.BOOKING
                .GroupBy(b => b.BOOKING_SOURCE)
                .Select(g => new
                {
                    Source = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count)
                .ToList();

            return Json(new
            {
                labels = sources.Select(x => x.Source),
                data = sources.Select(x => x.Count)
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetOccupancyTrend(DateTime? startDate, DateTime? endDate)
        {
            try
            {
                if (!startDate.HasValue) startDate = DateTime.Now.AddMonths(-1);
                if (!endDate.HasValue) endDate = DateTime.Now;

                var totalRooms = db.ROOM.Count();

                var bookings = db.BOOKING
                    .Where(b => DbFunctions.TruncateTime(b.CHECKIN_DATE) >= startDate &&
                               DbFunctions.TruncateTime(b.CHECKOUT_DATE) <= endDate)
                    .Include(b => b.ROOM)
                    .ToList();

                var occupancyByDate = bookings
                    .GroupBy(b => b.CHECKIN_DATE.Date)
                    .Select(g => new
                    {
                        Date = g.Key,
                        OccupancyRate = (double)g.SelectMany(b => b.ROOM).Count() / totalRooms * 100
                    })
                    .OrderBy(x => x.Date)
                    .ToList();

                return Json(new
                {
                    labels = occupancyByDate.Select(x => x.Date.ToString("dd/MM")),
                    data = occupancyByDate.Select(x => x.OccupancyRate)
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return Json(new { error = "Error fetching occupancy data" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetServiceRevenue()
        {
            var bookings = db.BOOKING
                .Include(b => b.SERVICE)
                .ToList();

            var serviceRevenue = bookings
                .SelectMany(b => b.SERVICE)
                .GroupBy(s => s.SERVICE_NAME)
                .Select(g => new
                {
                    ServiceName = g.Key,
                    Revenue = g.Sum(s => s.PRICE)
                })
                .OrderByDescending(x => x.Revenue)
                .ToList();

            return Json(new
            {
                labels = serviceRevenue.Select(x => x.ServiceName),
                data = serviceRevenue.Select(x => x.Revenue)
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetRecentBookings()
        {
            try
            {
                var recentBookings = db.BOOKING
                    .Include(b => b.CUSTOMER)
                    .Include(b => b.ROOM.Select(r => r.ROOMCLASS))
                    .OrderByDescending(b => b.BOOKING_DATE)
                    .Take(10)
                    .ToList()
                    .Select(b => new
                    {
                        CustomerName = b.CUSTOMER.LAST_NAME + " " + b.CUSTOMER.FIRST_NAME,
                        RoomTypes = string.Join(", ", b.ROOM.Select(r => r.ROOMCLASS.CLASS_NAME).Distinct()),
                        CheckIn = b.CHECKIN_DATE,
                        CheckOut = b.CHECKOUT_DATE,
                        Amount = b.BOOKING_AMOUNT
                    })
                    .ToList();

                return Json(recentBookings, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return Json(new { error = "Error fetching bookings data" }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}