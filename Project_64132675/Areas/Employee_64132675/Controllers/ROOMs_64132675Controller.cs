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
    public class ROOMs_64132675Controller : Adminbase_64132675Controller
    {
        private readonly Model_64132675 db = new Model_64132675();

        // GET: Employee_64132675/ROOMs_64132675
        public ActionResult Index()
        {
            var rOOM = db.ROOM.Include(r => r.FLOOR).Include(r => r.ROOMCLASS).Include(r => r.ROOMSTATUS);
            return View(rOOM.ToList());
        }

        // GET: Employee_64132675/ROOMs_64132675/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ROOM rOOM = db.ROOM.Find(id);
            if (rOOM == null)
            {
                return HttpNotFound();
            }
            return View(rOOM);
        }

        // GET: Employee_64132675/ROOMs_64132675/Create
        public ActionResult Create()
        {
            ViewBag.FLOOR_ID = new SelectList(db.FLOOR, "FLOOR_ID", "FLOOR_ID");
            ViewBag.ROOM_CLASS_ID = new SelectList(db.ROOMCLASS, "ROOM_CLASS_ID", "CLASS_NAME");
            ViewBag.ROOM_STATUS_ID = new SelectList(db.ROOMSTATUS, "ROOM_STATUS_ID", "ROOM_STATUS_NAME");
            return View();
        }

        // POST: Employee_64132675/ROOMs_64132675/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ROOM_ID,FLOOR_ID,ROOM_CLASS_ID,ROOM_STATUS_ID,ROOM_NUMBER,BED_TYPE,NUM_BEDS")] ROOM rOOM)
        {
            if (ModelState.IsValid)
            {
                db.ROOM.Add(rOOM);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.FLOOR_ID = new SelectList(db.FLOOR, "FLOOR_ID", "FLOOR_ID", rOOM.FLOOR_ID);
            ViewBag.ROOM_CLASS_ID = new SelectList(db.ROOMCLASS, "ROOM_CLASS_ID", "CLASS_NAME", rOOM.ROOM_CLASS_ID);
            ViewBag.ROOM_STATUS_ID = new SelectList(db.ROOMSTATUS, "ROOM_STATUS_ID", "ROOM_STATUS_NAME", rOOM.ROOM_STATUS_ID);
            return View(rOOM);
        }

        // GET: Employee_64132675/ROOMs_64132675/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ROOM rOOM = db.ROOM.Find(id);
            if (rOOM == null)
            {
                return HttpNotFound();
            }
            ViewBag.FLOOR_ID = new SelectList(db.FLOOR, "FLOOR_ID", "FLOOR_ID", rOOM.FLOOR_ID);
            ViewBag.ROOM_CLASS_ID = new SelectList(db.ROOMCLASS, "ROOM_CLASS_ID", "CLASS_NAME", rOOM.ROOM_CLASS_ID);
            ViewBag.ROOM_STATUS_ID = new SelectList(db.ROOMSTATUS, "ROOM_STATUS_ID", "ROOM_STATUS_NAME", rOOM.ROOM_STATUS_ID);
            return View(rOOM);
        }

        // POST: Employee_64132675/ROOMs_64132675/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ROOM_ID,FLOOR_ID,ROOM_CLASS_ID,ROOM_STATUS_ID,ROOM_NUMBER,BED_TYPE,NUM_BEDS")] ROOM rOOM)
        {
            if (ModelState.IsValid)
            {
                db.Entry(rOOM).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.FLOOR_ID = new SelectList(db.FLOOR, "FLOOR_ID", "FLOOR_ID", rOOM.FLOOR_ID);
            ViewBag.ROOM_CLASS_ID = new SelectList(db.ROOMCLASS, "ROOM_CLASS_ID", "CLASS_NAME", rOOM.ROOM_CLASS_ID);
            ViewBag.ROOM_STATUS_ID = new SelectList(db.ROOMSTATUS, "ROOM_STATUS_ID", "ROOM_STATUS_NAME", rOOM.ROOM_STATUS_ID);
            return View(rOOM);
        }

        // GET: Employee_64132675/ROOMs_64132675/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ROOM rOOM = db.ROOM.Find(id);
            if (rOOM == null)
            {
                return HttpNotFound();
            }
            return View(rOOM);
        }

        // POST: Employee_64132675/ROOMs_64132675/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            ROOM rOOM = db.ROOM.Find(id);
            db.ROOM.Remove(rOOM);
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
