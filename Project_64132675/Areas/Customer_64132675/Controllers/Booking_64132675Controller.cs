﻿using Project_64132675.Areas.Customer_64132675.Data;
using Project_64132675.Areas.Customer_64132675.ViewModels;
using Project_64132675.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project_64132675.Areas.Customer_64132675.Controllers
{
    public class Booking_64132675Controller : Customerbase_64132675Controller
    {
        private readonly Model_64132675 db = new Model_64132675();

        // GET: Customer_64132675/Booking_64132675
        public ActionResult Booking()
        {
            ViewBag.RoomClass = db.ROOMCLASS.ToList();
            return View();
        }

        [ChildActionOnly]
        public PartialViewResult BookingPartial(DateTime? checkin = null, DateTime? checkout = null, long? roomClassId = null)
        {
            // Mặc định là từ hôm nay đến ngày mai
            DateTime checkInDate = checkin ?? DateTime.Now.Date;
            DateTime checkOutDate = checkout ?? DateTime.Now.Date.AddDays(1);

            // Lấy danh sách phòng đã được đặt trong khoảng thời gian này
            var bookedRoomIds = db.BOOKING
                .Where(b => !((b.CHECKOUT_DATE <= checkInDate) || (b.CHECKIN_DATE >= checkOutDate)))
                .SelectMany(b => b.ROOM)
                .Select(r => r.ROOM_ID)
                .ToList();

            // Lấy tất cả phòng trống và thông tin liên quan
            var query = db.ROOM
                .Where(r => !bookedRoomIds.Contains(r.ROOM_ID))
                .Where(r => r.ROOMSTATUS.ROOM_STATUS_NAME == "Trống");

            if (roomClassId.HasValue)
            {
                query = query.Where(r => r.ROOM_CLASS_ID == roomClassId.Value);
            }

            var availableRoomsData = query
                .Select(r => new
                {
                    r.ROOM_CLASS_ID,
                    r.ROOM_NUMBER,
                    r.BED_TYPE,
                    r.NUM_BEDS,
                    r.ROOMCLASS.CLASS_NAME,
                    BasePrice = r.ROOMCLASS.BASE_PRICE * r.NUM_BEDS,
                    Features = r.ROOMCLASS.FEATURE.Select(f => f.FEATURE_NAME).ToList()
                })
                .ToList();

            // Nhóm và định dạng kết quả
            var availableRooms = availableRoomsData
                .GroupBy(r => new { r.ROOM_CLASS_ID, r.BED_TYPE, r.NUM_BEDS })
                .Select(g => new AvailableRoomViewModel_64132675
                {
                    RoomClassId = g.Key.ROOM_CLASS_ID,
                    BedType = g.Key.BED_TYPE,
                    NumBeds = g.Key.NUM_BEDS,
                    ClassName = g.First().CLASS_NAME,
                    BasePrice = g.First().BasePrice,
                    AvailableCount = g.Count(),
                    RoomNumbers = string.Join(", ", g.Select(r => r.ROOM_NUMBER).OrderBy(n => n)),
                    Features = g.First().Features
                })
                .OrderBy(r => r.RoomClassId)
                .ThenBy(r => r.BedType)
                .ThenBy(r => r.NumBeds)
                .ToList();

            ViewBag.CheckInDate = checkInDate;
            ViewBag.CheckOutDate = checkOutDate;
            return PartialView("BookingPartial", availableRooms);
        }

        [HttpPost]
        public PartialViewResult UpdateAvailableRooms(DateTime checkin, DateTime checkout, long? roomClassId)
        {
            return BookingPartial(checkin, checkout, roomClassId);
        }

        [ChildActionOnly]
        public PartialViewResult ServicesPartial()
        {
            var services = db.SERVICE.ToList();
            return PartialView(services);
        }

        [ChildActionOnly]
        public PartialViewResult BookingSummaryPartial()
        {
            var model = new BookingSummaryViewModel_64132675
            {
                // Đặt giá trị mặc định
                CheckIn = DateTime.Now.Date,
                CheckOut = DateTime.Now.Date.AddDays(1),
                Adults = 2,
                Children = 0,
                // Thêm các giá trị mặc định khác nếu cần
            };

            return PartialView(model);
        }

        [HttpPost]
        public ActionResult UpdateBookingSummary(DateTime checkin, DateTime checkout, int adults, int children, List<RoomSelection> selectedRooms = null,
        List<long> selectedServices = null)
        {
            var model = new BookingSummaryViewModel_64132675
            {
                CheckIn = checkin,
                CheckOut = checkout,
                Adults = adults,
                Children = children,
                SelectedRooms = selectedRooms ?? new List<RoomSelection>(),
                SelectedServices = selectedServices ?? new List<long>()
            };
            CalculateTotalAmount(model);
            // Return the partial view with updated data
            return PartialView("BookingSummaryPartial", model);
        }

        private void CalculateTotalAmount(BookingSummaryViewModel_64132675 model)
        {
            // Implement logic to calculate total amount based on rooms and services
            decimal roomTotal = 0;
            decimal serviceTotal = 0;

            foreach (var room in model.SelectedRooms)
            {
                roomTotal += room.Price * room.Quantity * model.Nights;
            }
            model.TotalRoomAmount = roomTotal;
            if (model.SelectedServices.Any())
            {
                var servicesPrices = db.SERVICE
                    .Where(s => model.SelectedServices.Contains(s.SERVICE_ID))
                    .Select(s => s.PRICE)
                    .ToList();

                serviceTotal += servicesPrices.Sum();
            }
            model.TotalServiceAmount = serviceTotal;

            model.TotalAmount = roomTotal + serviceTotal;

            model.SelectedServiceDetails = db.SERVICE
            .Where(s => model.SelectedServices.Contains((int)s.SERVICE_ID))
            .Select(s => new ServiceDetail
            {
                ServiceId = s.SERVICE_ID,
                ServiceName = s.SERVICE_NAME,
                Price = s.PRICE
            })
            .ToList();
        }
    }
}