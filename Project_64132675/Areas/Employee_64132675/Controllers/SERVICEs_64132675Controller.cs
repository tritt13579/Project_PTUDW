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
    public class SERVICEs_64132675Controller : Adminbase_64132675Controller
    {
        private readonly Model_64132675 db = new Model_64132675();

        // GET: Employee_64132675/SERVICEs_64132675
        public ActionResult Index()
        {
            return View(db.SERVICE.ToList());
        }

        // GET: Employee_64132675/SERVICEs_64132675/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SERVICE sERVICE = db.SERVICE.Find(id);
            if (sERVICE == null)
            {
                return HttpNotFound();
            }
            return View(sERVICE);
        }

        // GET: Employee_64132675/SERVICEs_64132675/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Employee_64132675/SERVICEs_64132675/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SERVICE_ID,SERVICE_NAME,PRICE")] SERVICE sERVICE)
        {
            if (ModelState.IsValid)
            {
                db.SERVICE.Add(sERVICE);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(sERVICE);
        }

        // GET: Employee_64132675/SERVICEs_64132675/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SERVICE sERVICE = db.SERVICE.Find(id);
            if (sERVICE == null)
            {
                return HttpNotFound();
            }
            return View(sERVICE);
        }

        // POST: Employee_64132675/SERVICEs_64132675/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SERVICE_ID,SERVICE_NAME,PRICE")] SERVICE sERVICE)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sERVICE).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(sERVICE);
        }

        // GET: Employee_64132675/SERVICEs_64132675/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SERVICE sERVICE = db.SERVICE.Find(id);
            if (sERVICE == null)
            {
                return HttpNotFound();
            }
            return View(sERVICE);
        }

        // POST: Employee_64132675/SERVICEs_64132675/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            SERVICE sERVICE = db.SERVICE.Find(id);
            db.SERVICE.Remove(sERVICE);
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
