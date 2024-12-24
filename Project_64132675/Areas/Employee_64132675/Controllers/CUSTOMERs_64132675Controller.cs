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
    public class CUSTOMERs_64132675Controller : Employeebase_64132675Controller
    {
        private readonly Model_64132675 db = new Model_64132675();

        // GET: Employee_64132675/CUSTOMERs_64132675
        public ActionResult Index()
        {
            return View(db.CUSTOMER.ToList());
        }

        // GET: Employee_64132675/CUSTOMERs_64132675/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CUSTOMER cUSTOMER = db.CUSTOMER.Find(id);
            if (cUSTOMER == null)
            {
                return HttpNotFound();
            }
            return View(cUSTOMER);
        }

        // GET: Employee_64132675/CUSTOMERs_64132675/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Employee_64132675/CUSTOMERs_64132675/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CUSTOMER_ID,FIRST_NAME,LAST_NAME,GENDER,DATE_OF_BIRTH,EMAIL,PHONE_NUMBER,PASSWORD")] CUSTOMER cUSTOMER, string redirectTo)
        {
            if (ModelState.IsValid)
            {
                // Mã hóa mật khẩu bằng BCrypt
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(cUSTOMER.PASSWORD);
                cUSTOMER.PASSWORD = hashedPassword;

                db.CUSTOMER.Add(cUSTOMER);
                db.SaveChanges();

                if (!string.IsNullOrEmpty(redirectTo) && redirectTo == "CreateBooking")
                {
                    return RedirectToAction("Create", "BOOKINGs_64132675", new { customerId = cUSTOMER.CUSTOMER_ID });
                }

                return RedirectToAction("Index");
            }

            return View(cUSTOMER);
        }

        // GET: Employee_64132675/CUSTOMERs_64132675/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CUSTOMER cUSTOMER = db.CUSTOMER.Find(id);
            if (cUSTOMER == null)
            {
                return HttpNotFound();
            }
            return View(cUSTOMER);
        }

        // POST: Employee_64132675/CUSTOMERs_64132675/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CUSTOMER_ID,FIRST_NAME,LAST_NAME,GENDER,DATE_OF_BIRTH,EMAIL,PHONE_NUMBER,PASSWORD")] CUSTOMER cUSTOMER)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cUSTOMER).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(cUSTOMER);
        }

        // GET: Employee_64132675/CUSTOMERs_64132675/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CUSTOMER cUSTOMER = db.CUSTOMER.Find(id);
            if (cUSTOMER == null)
            {
                return HttpNotFound();
            }
            return View(cUSTOMER);
        }

        // POST: Employee_64132675/CUSTOMERs_64132675/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            CUSTOMER cUSTOMER = db.CUSTOMER.Find(id);
            db.CUSTOMER.Remove(cUSTOMER);
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
