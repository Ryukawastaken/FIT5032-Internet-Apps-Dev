using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FinalAssignment.Models;

//Represents a location in the world
namespace FinalAssignment.Controllers
{
    public class LocationController : Controller
    {
        //Get database
        private PracticeContext db = new PracticeContext();

        //Get all locations
        // GET: Location
        public ActionResult Index()
        {
            return View(db.Locations.ToList());
        }

        //Get a location's details
        // GET: Location/Details/5
        public ActionResult Details(int? id)
        {
            //if id null, fail
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //get location from id
            Location location = db.Locations.Find(id);
            //if location null, fail
            if (location == null)
            {
                return HttpNotFound();
            }
            return View(location);
        }

        //Launch create form for location
        // GET: Location/Create
        [Authorize]
        public ActionResult Create()
        {
            //if user not admin/reception, fail
            if (!(User.IsInRole("Admin") || User.IsInRole("Reception")))
            {
                return HttpNotFound();
            }
            return View();
        }

        //Actually create the location from the form data
        // POST: Location/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Create([Bind(Include = "LocationID,Name,Description,Latitude,Longitude")] Location location)
        {
            //If all data, proceed
            if (ModelState.IsValid)
            {
                //Add location to the database and save it
                db.Locations.Add(location);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(location);
        }

        //Launch edit form for id of location
        // GET: Location/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            //if id null, fail
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //get location from id
            Location location = db.Locations.Find(id);
            //if location null, fail
            if (location == null)
            {
                return HttpNotFound();
            }
            //if user not admin/reception, fail
            if (!(User.IsInRole("Admin") || User.IsInRole("Reception")))
            {
                return HttpNotFound();
            }
            return View(location);
        }

        //Actually edit the location based on edit form information
        // POST: Location/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit([Bind(Include = "LocationID,Name,Description,Latitude,Longitude")] Location location)
        {
            //If all data valid, continue
            if (ModelState.IsValid)
            {
                //Change the entry in the databse and then save it
                db.Entry(location).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(location);
        }

        //Open delete view from location id
        // GET: Location/Delete/5
        [Authorize]
        public ActionResult Delete(int? id)
        {
            //if id null, fail
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //get location from id
            Location location = db.Locations.Find(id);
            //if location null, fail
            if (location == null)
            {
                return HttpNotFound();
            }
            //if user not admin/reception, fail
            if (!(User.IsInRole("Admin") || User.IsInRole("Reception")))
            {
                return HttpNotFound();
            }
            return View(location);
        }

        //Actually delete the location
        // POST: Location/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult DeleteConfirmed(int id)
        {
            //if user not admin/reception, fail
            if (!(User.IsInRole("Admin") || User.IsInRole("Reception")))
            {
                return HttpNotFound();
            }
            //Get the location, then delete it from the database and save it
            Location location = db.Locations.Find(id);
            db.Locations.Remove(location);
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
