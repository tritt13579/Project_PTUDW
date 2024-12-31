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
        public ActionResult UpdateBookingSummary(DateTime checkin, DateTime checkout, byte adults, byte children, List<RoomSelection> selectedRooms = null,
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmBooking(BookingSummaryViewModel_64132675 model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var customerId = GetSessionUserId();

                    if (model.CheckIn >= model.CheckOut)
                    {
                        ModelState.AddModelError("", "Ngày trả phòng phải sau ngày nhận phòng");
                        return View("Booking", model);
                    }

                    var booking = new BOOKING
                    {
                        CUSTOMER_ID = customerId,
                        PAYMENT_STATUS_ID = 1,
                        BOOKING_DATE = DateTime.Now,
                        CHECKIN_DATE = model.CheckIn,
                        CHECKOUT_DATE = model.CheckOut,
                        NUM_ADULT = model.Adults,
                        NUM_CHILDREN = model.Children,
                        SPECIAL_REQUESTS = model.SpecialRequests,
                        BOOKING_SOURCE = "Trực tuyến",
                        BOOKING_AMOUNT = model.TotalAmount
                    };

                    if (model.SelectedRooms == null || !model.SelectedRooms.Any())
                    {
                        ModelState.AddModelError("", "Vui lòng chọn ít nhất một phòng");
                        return View("Booking", model);
                    }

                    foreach (var roomSelection in model.SelectedRooms)
                    {
                        // Tách chuỗi số phòng thành mảng
                        var roomNumbers = roomSelection.RoomNumber.Split(',')
                                                     .Select(x => x.Trim())
                                                     .ToList();

                        // Kiểm tra số lượng phòng được chọn có khớp với số phòng
                        if (roomSelection.Quantity != roomNumbers.Count)
                        {
                            ModelState.AddModelError("", $"Số lượng phòng không khớp với danh sách phòng đã chọn");
                            return View("Booking", model);
                        }

                        foreach (var roomNumber in roomNumbers)
                        {
                            var room = db.ROOM.FirstOrDefault(r => r.ROOM_NUMBER.ToString() == roomNumber);

                            if (room != null)
                            {
                                if (model.CheckIn.Date == DateTime.Now.Date && room.ROOM_STATUS_ID == 1)
                                {
                                    room.ROOM_STATUS_ID = 2;
                                }
                                booking.ROOM.Add(room);
                            }
                        }
                    }

                    if (model.SelectedServices != null && model.SelectedServices.Any())
                    {
                        foreach (var serviceId in model.SelectedServices)
                        {
                            var service = db.SERVICE.Find(serviceId);
                            if (service != null)
                            {
                                booking.SERVICE.Add(service);
                            }
                        }
                    }

                    db.BOOKING.Add(booking);
                    db.SaveChanges();

                    return RedirectToAction("Index", "Home_64132675", new { area = "Customer_64132675" });
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra trong quá trình đặt phòng: " + ex.Message);
            }

            return View("Booking", model);
        }

        public ActionResult MyBookings()
        {
            try
            {
                // Lấy ID của khách hàng đang đăng nhập
                var customerId = GetSessionUserId();

                // Lấy danh sách booking của khách hàng bao gồm cả thông tin phòng và dịch vụ
                var bookings = db.BOOKING
                    .Include(b => b.CUSTOMER)
                    .Include(b => b.PAYMENTSTATUS)
                    // Include thông tin về phòng
                    .Include(b => b.ROOM)
                    // Include thông tin về dịch vụ
                    .Include(b => b.SERVICE)
                    .Where(b => b.CUSTOMER_ID == customerId)
                    .OrderByDescending(b => b.BOOKING_DATE)
                    .ToList();

                // Nếu không có booking nào
                if (!bookings.Any())
                {
                    ViewBag.Message = "Bạn chưa có đơn đặt phòng nào.";
                }

                return View(bookings);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra: " + ex.Message);
                return View(new List<BOOKING>());
            }
        }

        public ActionResult BookingDetail(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            var booking = db.BOOKING
                .Include(b => b.CUSTOMER)
                .Include(b => b.PAYMENTSTATUS)
                .Include(b => b.ROOM)
                .Include(b => b.SERVICE)
                .FirstOrDefault(b => b.BOOKING_ID == id);
            if (booking == null)
            {
                return HttpNotFound();
            }
            return View(booking);
        }
    }
}