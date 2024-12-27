using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Project_64132675.Models;

namespace Project_64132675.Areas.Employee_64132675.Controllers
{
    public class BOOKINGs_64132675Controller : Employeebase_64132675Controller
    {
        private readonly Model_64132675 db = new Model_64132675();

        // GET: Employee_64132675/BOOKINGs_64132675
        public ActionResult Index()
        {
            var bOOKING = db.BOOKING.Include(b => b.CUSTOMER).Include(b => b.PAYMENTSTATUS);
            return View(bOOKING.ToList());
        }

        // GET: Employee_64132675/BOOKINGs_64132675/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Lấy thông tin Booking kèm danh sách Rooms và Services
            var booking = db.BOOKING
                .Include(b => b.ROOM)
                .Include(b => b.SERVICE)
                .SingleOrDefault(b => b.BOOKING_ID == id);

            if (booking == null)
            {
                return HttpNotFound();
            }

            // Truyền dữ liệu qua ViewBag
            ViewBag.Rooms = booking.ROOM.ToList();
            ViewBag.Services = booking.SERVICE.ToList();

            return View(booking);
        }


        //GET: Employee_64132675/BOOKINGs_64132675/Create
        public ActionResult Create(int? customerId)
        {
            var customers = db.CUSTOMER.Select(c => new
            {
                c.CUSTOMER_ID,
                FULL_NAME = c.LAST_NAME + " " + c.FIRST_NAME
            }).ToList();

            // Lấy thông tin phòng kèm theo giá
            var rooms = db.ROOM.Select(r => new
            {
                r.ROOM_ID,
                r.ROOM_NUMBER,
                Price = r.ROOMCLASS.BASE_PRICE
            }).ToList();

            ViewBag.CUSTOMER_ID = new SelectList(customers, "CUSTOMER_ID", "FULL_NAME");
            ViewBag.PAYMENT_STATUS_ID = new SelectList(db.PAYMENTSTATUS, "PAYMENT_STATUS_ID", "PAYMENT_STATUS_NAME", 1);
            ViewBag.ROOMS = new SelectList(rooms, "ROOM_ID", "ROOM_NUMBER");
            ViewBag.SERVICES = new SelectList(db.SERVICE, "SERVICE_ID", "SERVICE_NAME");

            // Chuyển ID thành string khi lưu vào dictionary
            ViewBag.RoomPrices = rooms.ToDictionary(r => r.ROOM_ID.ToString(), r => r.Price);
            ViewBag.ServicePrices = db.SERVICE.ToDictionary(s => s.SERVICE_ID.ToString(), s => s.PRICE);

            if (customerId.HasValue)
            {
                ViewBag.SelectedCustomerId = customerId.Value;
            }

            var newBooking = new BOOKING
            {
                BOOKING_DATE = DateTime.Now // Đặt giá trị mặc định
            };

            return View(newBooking);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
    [Bind(Include = "BOOKING_ID,CUSTOMER_ID,PAYMENT_STATUS_ID,BOOKING_DATE,CHECKIN_DATE,CHECKOUT_DATE,NUM_ADULT,NUM_CHILDREN,SPECIAL_REQUESTS,BOOKING_SOURCE,BOOKING_AMOUNT")] BOOKING bOOKING,
    int[] selectedRooms,
    int[] selectedServices)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (selectedRooms == null || !selectedRooms.Any())
                    {
                        ModelState.AddModelError("", "Vui lòng chọn ít nhất một phòng");
                        throw new Exception("Chưa chọn phòng");
                    }

                    // Đặt giá trị mặc định nếu chưa được cung cấp
                    if (bOOKING.BOOKING_DATE == default)
                    {
                        bOOKING.BOOKING_DATE = DateTime.Now;
                    }

                    // Đặt giá trị mặc định
                    bOOKING.PAYMENT_STATUS_ID = 1;
                    bOOKING.BOOKING_SOURCE = "Trực tiếp";

                    // Validate ngày
                    if (bOOKING.CHECKIN_DATE >= bOOKING.CHECKOUT_DATE)
                    {
                        ModelState.AddModelError("", "Ngày trả phòng phải sau ngày nhận phòng");
                        throw new Exception("Lỗi ngày");
                    }

                    // Tính tổng tiền
                    decimal totalAmount = 0;

                    // Tính tiền phòng và cập nhật trạng thái nếu cần
                    foreach (var roomId in selectedRooms)
                    {
                        var room = db.ROOM.FirstOrDefault(r => r.ROOM_ID == roomId);
                        if (room != null)
                        {
                            // Kiểm tra nếu CHECKIN_DATE là ngày hiện tại
                            if (bOOKING.CHECKIN_DATE.Date == DateTime.Now.Date)
                            {
                                // Cập nhật trạng thái phòng từ Trống (id = 1) sang Đã đặt (id = 2)
                                if (room.ROOM_STATUS_ID == 1) // 1 là trạng thái Trống
                                {
                                    room.ROOM_STATUS_ID = 2; // 2 là trạng thái Đã đặt
                                }
                            }

                            // Tính tổng số tiền dựa trên số ngày lưu trú
                            var days = (bOOKING.CHECKOUT_DATE - bOOKING.CHECKIN_DATE).Days;
                            totalAmount += room.ROOMCLASS.BASE_PRICE * days;
                            bOOKING.ROOM.Add(room);
                        }
                    }

                    // Tính tiền dịch vụ
                    if (selectedServices != null)
                    {
                        foreach (var serviceId in selectedServices)
                        {
                            var service = db.SERVICE.Find(serviceId);
                            if (service != null)
                            {
                                totalAmount += service.PRICE;
                                bOOKING.SERVICE.Add(service);
                            }
                        }
                    }

                    bOOKING.BOOKING_AMOUNT = Math.Round(totalAmount);
                    db.BOOKING.Add(bOOKING);

                    // Lưu thay đổi vào cơ sở dữ liệu
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                // Ghi lại lỗi
                ModelState.AddModelError("", "Có lỗi xảy ra: " + ex.Message);
            }

            // Xử lý lại view nếu có lỗi
            PrepareViewDataForCreate(bOOKING);
            return View(bOOKING);
        }


        private void PrepareViewDataForCreate(BOOKING bOOKING)
        {
            var customers = db.CUSTOMER.Select(c => new
            {
                c.CUSTOMER_ID,
                FULL_NAME = c.LAST_NAME + " " + c.FIRST_NAME
            }).ToList();

            ViewBag.CUSTOMER_ID = new SelectList(customers, "CUSTOMER_ID", "FULL_NAME", bOOKING.CUSTOMER_ID);
            ViewBag.PAYMENT_STATUS_ID = new SelectList(db.PAYMENTSTATUS, "PAYMENT_STATUS_ID", "PAYMENT_STATUS_NAME", bOOKING.PAYMENT_STATUS_ID);
            ViewBag.ROOMS = new SelectList(db.ROOM, "ROOM_ID", "ROOM_NUMBER");
            ViewBag.SERVICES = new SelectList(db.SERVICE, "SERVICE_ID", "SERVICE_NAME");

            var rooms = db.ROOM.Select(r => new
            {
                r.ROOM_ID,
                r.ROOM_NUMBER,
                Price = r.ROOMCLASS.BASE_PRICE
            }).ToList();

            ViewBag.RoomPrices = rooms.ToDictionary(r => r.ROOM_ID.ToString(), r => r.Price);
            ViewBag.ServicePrices = db.SERVICE.ToDictionary(s => s.SERVICE_ID.ToString(), s => s.PRICE);
        }

        
        public ActionResult GetAvailableRooms(DateTime checkIn, DateTime checkOut, long? bookingId = null)
        {
            // Get all bookings that overlap with the selected date range, excluding the current booking
            var overlappingBookings = db.BOOKING.Where(b =>
                (b.CHECKIN_DATE <= checkOut && b.CHECKOUT_DATE >= checkIn) &&
                (bookingId == null || b.BOOKING_ID != bookingId));

            // Get the rooms that are booked during this period
            var bookedRoomIds = overlappingBookings.SelectMany(b => b.ROOM)
                .Select(r => r.ROOM_ID).Distinct().ToList();

            // If we're editing, get the currently booked rooms for this booking
            var currentlyBookedRooms = new List<long>();
            if (bookingId.HasValue)
            {
                currentlyBookedRooms = db.BOOKING
                    .Where(b => b.BOOKING_ID == bookingId)
                    .SelectMany(b => b.ROOM)
                    .Select(r => r.ROOM_ID)
                    .ToList();
            }

            // Get available rooms (rooms not in bookedRoomIds OR currently booked for this booking)
            var availableRooms = db.ROOM
                .Where(r => !bookedRoomIds.Contains(r.ROOM_ID) || currentlyBookedRooms.Contains(r.ROOM_ID))
                .Select(r => new
                {
                    r.ROOM_ID,
                    r.ROOM_NUMBER,
                    Price = r.ROOMCLASS.BASE_PRICE,
                    IsCurrentlyBooked = currentlyBookedRooms.Contains(r.ROOM_ID)
                })
                .ToList();

            // Create the select list with currently booked rooms marked
            var roomSelectList = availableRooms.Select(r => new SelectListItem
            {
                Value = r.ROOM_ID.ToString(),
                Text = r.ROOM_NUMBER + (r.IsCurrentlyBooked ? " (Đã đặt)" : ""),
                Selected = r.IsCurrentlyBooked
            });

            return Json(new
            {
                rooms = roomSelectList,
                prices = availableRooms.ToDictionary(r => r.ROOM_ID.ToString(), r => r.Price)
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            BOOKING booking = db.BOOKING
                .Include(b => b.ROOM)
                .Include(b => b.SERVICE)
                .Include(b => b.CUSTOMER)
                .FirstOrDefault(b => b.BOOKING_ID == id);

            if (booking == null)
            {
                return HttpNotFound();
            }

            // Tạo tên đầy đủ của khách hàng
            ViewBag.CustomerFullName = booking.CUSTOMER.LAST_NAME + " " + booking.CUSTOMER.FIRST_NAME;

            // Lấy danh sách thanh toán
            ViewBag.PAYMENT_STATUS_ID = new SelectList(db.PAYMENTSTATUS, "PAYMENT_STATUS_ID", "PAYMENT_STATUS_NAME", booking.PAYMENT_STATUS_ID);

            var rooms = db.ROOM.Select(r => new
            {
                r.ROOM_ID,
                r.ROOM_NUMBER,
                Price = r.ROOMCLASS.BASE_PRICE
            }).ToList();

            ViewBag.ROOMS = new SelectList(rooms, "ROOM_ID", "ROOM_NUMBER");
            ViewBag.SERVICES = new SelectList(db.SERVICE, "SERVICE_ID", "SERVICE_NAME");
            ViewBag.RoomPrices = rooms.ToDictionary(r => r.ROOM_ID.ToString(), r => r.Price);
            ViewBag.ServicePrices = db.SERVICE.ToDictionary(s => s.SERVICE_ID.ToString(), s => s.PRICE);
            ViewBag.SelectedRooms = booking.ROOM.Select(r => r.ROOM_ID).ToArray();
            ViewBag.SelectedServices = booking.SERVICE.Select(s => s.SERVICE_ID).ToArray();

            PrepareViewDataForEdit(booking);

            return View(booking);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BOOKING_ID,CUSTOMER_ID,PAYMENT_STATUS_ID,BOOKING_DATE,CHECKIN_DATE,CHECKOUT_DATE,NUM_ADULT,NUM_CHILDREN,SPECIAL_REQUESTS,BOOKING_SOURCE,BOOKING_AMOUNT")] BOOKING booking,
    int[] selectedRooms,
    int[] selectedServices)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (selectedRooms == null || !selectedRooms.Any())
                    {
                        ModelState.AddModelError("", "Vui lòng chọn ít nhất một phòng");
                        throw new Exception("Chưa chọn phòng");
                    }

                    // Validate ngày
                    if (booking.CHECKIN_DATE >= booking.CHECKOUT_DATE)
                    {
                        ModelState.AddModelError("", "Ngày trả phòng phải sau ngày nhận phòng");
                        throw new Exception("Lỗi ngày");
                    }

                    var existingBooking = db.BOOKING
                        .Include(b => b.ROOM)
                        .Include(b => b.SERVICE)
                        .FirstOrDefault(b => b.BOOKING_ID == booking.BOOKING_ID);

                    if (existingBooking != null)
                    {
                        // Reset room statuses for previously booked rooms if check-in date is today
                        if (existingBooking.CHECKIN_DATE.Date == DateTime.Now.Date)
                        {
                            foreach (var room in existingBooking.ROOM)
                            {
                                if (room.ROOM_STATUS_ID == 2) // Đã đặt
                                {
                                    room.ROOM_STATUS_ID = 1; // Chuyển về trạng thái Trống
                                }
                            }
                        }

                        // Cập nhật thông tin cơ bản
                        db.Entry(existingBooking).CurrentValues.SetValues(booking);

                        // Cập nhật danh sách phòng
                        existingBooking.ROOM.Clear();
                        foreach (var roomId in selectedRooms)
                        {
                            var room = db.ROOM.Find(roomId);
                            if (room != null)
                            {
                                // Update room status if check-in date is today
                                if (booking.CHECKIN_DATE.Date == DateTime.Now.Date)
                                {
                                    if (room.ROOM_STATUS_ID == 1) // Trống
                                    {
                                        room.ROOM_STATUS_ID = 2; // Chuyển sang Đã đặt
                                    }
                                }
                                existingBooking.ROOM.Add(room);
                            }
                        }

                        // Cập nhật danh sách dịch vụ
                        existingBooking.SERVICE.Clear();
                        if (selectedServices != null)
                        {
                            foreach (var serviceId in selectedServices)
                            {
                                var service = db.SERVICE.Find(serviceId);
                                if (service != null)
                                {
                                    existingBooking.SERVICE.Add(service);
                                }
                            }
                        }

                        // Tính lại tổng tiền
                        decimal totalAmount = 0;

                        // Tính tiền phòng
                        foreach (var room in existingBooking.ROOM)
                        {
                            var days = (booking.CHECKOUT_DATE - booking.CHECKIN_DATE).Days;
                            totalAmount += room.ROOMCLASS.BASE_PRICE * days;
                        }

                        // Tính tiền dịch vụ
                        foreach (var service in existingBooking.SERVICE)
                        {
                            totalAmount += service.PRICE;
                        }

                        existingBooking.BOOKING_AMOUNT = Math.Round(totalAmount);

                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra: " + ex.Message);
            }

            // Nếu có lỗi, chuẩn bị lại dữ liệu cho view
            PrepareViewDataForCreate(booking);
            return View(booking);
        }

        private void PrepareViewDataForEdit(BOOKING bOOKING)
        {
            var customers = db.CUSTOMER.Select(c => new
            {
                c.CUSTOMER_ID,
                FULL_NAME = c.LAST_NAME + " " + c.FIRST_NAME
            }).ToList();

            ViewBag.CUSTOMER_ID = new SelectList(customers, "CUSTOMER_ID", "FULL_NAME", bOOKING.CUSTOMER_ID);
            ViewBag.PAYMENT_STATUS_ID = new SelectList(db.PAYMENTSTATUS, "PAYMENT_STATUS_ID", "PAYMENT_STATUS_NAME", bOOKING.PAYMENT_STATUS_ID);

            // Get currently booked rooms
            var currentRoomIds = bOOKING.ROOM.Select(r => r.ROOM_ID).ToList();

            // Get all rooms
            var rooms = db.ROOM.Select(r => new
            {
                r.ROOM_ID,
                r.ROOM_NUMBER,
                Price = r.ROOMCLASS.BASE_PRICE,
                IsCurrentlyBooked = currentRoomIds.Contains(r.ROOM_ID)
            }).ToList();

            ViewBag.ROOMS = new MultiSelectList(rooms.Select(r => new SelectListItem
            {
                Value = r.ROOM_ID.ToString(),
                Text = r.ROOM_NUMBER + (r.IsCurrentlyBooked ? " (Đã đặt)" : ""),
                Selected = r.IsCurrentlyBooked
            }), "Value", "Text");

            ViewBag.SERVICES = new MultiSelectList(db.SERVICE, "SERVICE_ID", "SERVICE_NAME",
                bOOKING.SERVICE.Select(s => s.SERVICE_ID));

            ViewBag.RoomPrices = rooms.ToDictionary(r => r.ROOM_ID.ToString(), r => r.Price);
            ViewBag.ServicePrices = db.SERVICE.ToDictionary(s => s.SERVICE_ID.ToString(), s => s.PRICE);
        }


        // GET: Employee_64132675/BOOKINGs_64132675/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Lấy thông tin Booking kèm danh sách Rooms và Services
            var booking = db.BOOKING
                .Include(b => b.ROOM)
                .Include(b => b.SERVICE)
                .SingleOrDefault(b => b.BOOKING_ID == id);

            if (booking == null)
            {
                return HttpNotFound();
            }

            // Truyền dữ liệu qua ViewBag
            ViewBag.Rooms = booking.ROOM.ToList();
            ViewBag.Services = booking.SERVICE.ToList();

            return View(booking);
        }

        // POST: Employee_64132675/BOOKINGs_64132675/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            BOOKING bOOKING = db.BOOKING.Find(id);
            db.BOOKING.Remove(bOOKING);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
