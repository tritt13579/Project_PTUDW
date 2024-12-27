using Project_64132675.Models;
using System;
using System.Collections.Generic;
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
        public ActionResult BookingPartial()
        {
            // Return your existing booking form
            return PartialView();
        }

        [ChildActionOnly]
        public ActionResult ServicesPartial()
        {
            // Get and return your services data
            var services =
        return PartialView();
        }

        [ChildActionOnly]
        public ActionResult BookingSummaryPartial()
        {
            // Get booking summary data
            var summary = 
        return PartialView();
        }

        // Action to update booking summary
        [HttpPost]
        public JsonResult UpdateBookingSummary(BookingSummaryViewModel model)
        {
            try
            {
                // Update your booking summary logic here
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}