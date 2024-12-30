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

            var rooms = db.ROOM.Select(r => new
            {
                r.ROOM_ID,
                r.ROOM_NUMBER,
                Price = r.ROOMCLASS.BASE_PRICE * r.NUM_BEDS
            }).ToList();

            ViewBag.CUSTOMER_ID = new SelectList(customers, "CUSTOMER_ID", "FULL_NAME");
            ViewBag.PAYMENT_STATUS_ID = new SelectList(db.PAYMENTSTATUS, "PAYMENT_STATUS_ID", "PAYMENT_STATUS_NAME", 1);
            ViewBag.ROOMS = new SelectList(rooms, "ROOM_ID", "ROOM_NUMBER");
            ViewBag.SERVICES = new SelectList(db.SERVICE, "SERVICE_ID", "SERVICE_NAME");

            ViewBag.RoomPrices = rooms.ToDictionary(r => r.ROOM_ID.ToString(), r => r.Price);
            ViewBag.ServicePrices = db.SERVICE.ToDictionary(s => s.SERVICE_ID.ToString(), s => s.PRICE);

            if (customerId.HasValue)
            {
                ViewBag.SelectedCustomerId = customerId.Value;
            }

            var newBooking = new BOOKING
            {
                BOOKING_DATE = DateTime.Now
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

                    if (bOOKING.BOOKING_DATE == default)
                    {
                        bOOKING.BOOKING_DATE = DateTime.Now;
                    }

                    bOOKING.PAYMENT_STATUS_ID = 1;
                    bOOKING.BOOKING_SOURCE = "Trực tiếp";

                    if (bOOKING.CHECKIN_DATE >= bOOKING.CHECKOUT_DATE)
                    {
                        ModelState.AddModelError("", "Ngày trả phòng phải sau ngày nhận phòng");
                        throw new Exception("Lỗi ngày");
                    }

                    decimal totalAmount = 0;

                    foreach (var roomId in selectedRooms)
                    {
                        var room = db.ROOM.FirstOrDefault(r => r.ROOM_ID == roomId);
                        if (room != null)
                        {
                            if (bOOKING.CHECKIN_DATE.Date == DateTime.Now.Date && room.ROOM_STATUS_ID == 1)
                            {
                                room.ROOM_STATUS_ID = 2;
                            }

                            var days = (bOOKING.CHECKOUT_DATE - bOOKING.CHECKIN_DATE).Days;
                            totalAmount += room.ROOMCLASS.BASE_PRICE * room.NUM_BEDS * days;
                            bOOKING.ROOM.Add(room);
                        }
                    }

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
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra: " + ex.Message);
            }

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
                Price = r.ROOMCLASS.BASE_PRICE * r.NUM_BEDS
            }).ToList();

            ViewBag.RoomPrices = rooms.ToDictionary(r => r.ROOM_ID.ToString(), r => r.Price);
            ViewBag.ServicePrices = db.SERVICE.ToDictionary(s => s.SERVICE_ID.ToString(), s => s.PRICE);
        }

        public ActionResult GetAvailableRooms(DateTime checkIn, DateTime checkOut, long? bookingId = null)
        {
            var overlappingBookings = db.BOOKING.Where(b =>
                (b.CHECKIN_DATE <= checkOut && b.CHECKOUT_DATE >= checkIn) &&
                (bookingId == null || b.BOOKING_ID != bookingId));

            var bookedRoomIds = overlappingBookings.SelectMany(b => b.ROOM)
                .Select(r => r.ROOM_ID).Distinct().ToList();

            var currentlyBookedRooms = new List<long>();
            if (bookingId.HasValue)
            {
                currentlyBookedRooms = db.BOOKING
                    .Where(b => b.BOOKING_ID == bookingId)
                    .SelectMany(b => b.ROOM)
                    .Select(r => r.ROOM_ID)
                    .ToList();
            }

            var availableRooms = db.ROOM
                .Where(r => !bookedRoomIds.Contains(r.ROOM_ID) || currentlyBookedRooms.Contains(r.ROOM_ID))
                .Select(r => new
                {
                    r.ROOM_ID,
                    r.ROOM_NUMBER,
                    Price = r.ROOMCLASS.BASE_PRICE * r.NUM_BEDS,
                    IsCurrentlyBooked = currentlyBookedRooms.Contains(r.ROOM_ID)
                })
                .ToList();

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
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            BOOKING booking = db.BOOKING
                .Include(b => b.ROOM)
                .Include(b => b.SERVICE)
                .Include(b => b.CUSTOMER)
                .FirstOrDefault(b => b.BOOKING_ID == id);

            if (booking == null)
                return HttpNotFound();

            ViewBag.CustomerFullName = booking.CUSTOMER.LAST_NAME + " " + booking.CUSTOMER.FIRST_NAME;
            ViewBag.PAYMENT_STATUS_ID = new SelectList(db.PAYMENTSTATUS, "PAYMENT_STATUS_ID", "PAYMENT_STATUS_NAME", booking.PAYMENT_STATUS_ID);

            var rooms = db.ROOM.Select(r => new
            {
                r.ROOM_ID,
                r.ROOM_NUMBER,
                Price = r.ROOMCLASS.BASE_PRICE * r.NUM_BEDS
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
                if (ModelState.IsValid && ValidateBooking(booking, selectedRooms))
                {
                    var existingBooking = db.BOOKING
                        .Include(b => b.ROOM)
                        .Include(b => b.SERVICE)
                        .FirstOrDefault(b => b.BOOKING_ID == booking.BOOKING_ID);

                    if (existingBooking != null)
                    {
                        UpdateRoomStatuses(existingBooking, booking.CHECKIN_DATE);
                        db.Entry(existingBooking).CurrentValues.SetValues(booking);
                        UpdateBookingRooms(existingBooking, selectedRooms, booking.CHECKIN_DATE);
                        UpdateBookingServices(existingBooking, selectedServices);
                        UpdateBookingAmount(existingBooking, booking.CHECKIN_DATE, booking.CHECKOUT_DATE);

                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra: " + ex.Message);
            }

            PrepareViewDataForCreate(booking);
            return View(booking);
        }

        private bool ValidateBooking(BOOKING booking, int[] selectedRooms)
        {
            if (selectedRooms == null || !selectedRooms.Any())
            {
                ModelState.AddModelError("", "Vui lòng chọn ít nhất một phòng");
                return false;
            }
            if (booking.CHECKIN_DATE >= booking.CHECKOUT_DATE)
            {
                ModelState.AddModelError("", "Ngày trả phòng phải sau ngày nhận phòng");
                return false;
            }
            return true;
        }

        private void UpdateRoomStatuses(BOOKING existingBooking, DateTime newCheckInDate)
        {
            if (existingBooking.CHECKIN_DATE.Date == DateTime.Now.Date)
            {
                foreach (var room in existingBooking.ROOM.Where(r => r.ROOM_STATUS_ID == 2))
                {
                    room.ROOM_STATUS_ID = 1;
                }
            }
        }

        private void UpdateBookingRooms(BOOKING booking, int[] selectedRooms, DateTime checkInDate)
        {
            booking.ROOM.Clear();
            foreach (var roomId in selectedRooms)
            {
                var room = db.ROOM.Find(roomId);
                if (room != null)
                {
                    if (checkInDate.Date == DateTime.Now.Date && room.ROOM_STATUS_ID == 1)
                    {
                        room.ROOM_STATUS_ID = 2;
                    }
                    booking.ROOM.Add(room);
                }
            }
        }

        private void UpdateBookingServices(BOOKING booking, int[] selectedServices)
        {
            booking.SERVICE.Clear();
            if (selectedServices != null)
            {
                foreach (var serviceId in selectedServices)
                {
                    var service = db.SERVICE.Find(serviceId);
                    if (service != null)
                    {
                        booking.SERVICE.Add(service);
                    }
                }
            }
        }

        private void UpdateBookingAmount(BOOKING booking, DateTime checkInDate, DateTime checkOutDate)
        {
            decimal totalAmount = 0;
            var days = (checkOutDate - checkInDate).Days;

            foreach (var room in booking.ROOM)
            {
                totalAmount += room.ROOMCLASS.BASE_PRICE * room.NUM_BEDS * days;
            }

            foreach (var service in booking.SERVICE)
            {
                totalAmount += service.PRICE;
            }

            booking.BOOKING_AMOUNT = Math.Round(totalAmount);
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

            var currentRoomIds = bOOKING.ROOM.Select(r => r.ROOM_ID).ToList();
            var rooms = db.ROOM.Select(r => new
            {
                r.ROOM_ID,
                r.ROOM_NUMBER,
                Price = r.ROOMCLASS.BASE_PRICE * r.NUM_BEDS,
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
