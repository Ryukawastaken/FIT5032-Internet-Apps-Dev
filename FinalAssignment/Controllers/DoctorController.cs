using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FinalAssignment.Models;

//Representing a doctor
namespace FinalAssignment.Controllers
{
    public class DoctorController : Controller
    {
        //Get database
        private PracticeContext db = new PracticeContext();

        //Get all doctors
        // GET: Doctor
        [Authorize]
        public ActionResult Index()
        {
            //If user not admin/reception, fail
            if (!(User.IsInRole("Admin") || User.IsInRole("Reception")))
            {
                return HttpNotFound();
            }
            //Return all doctors
            return View(db.Doctors.ToList());
        }

        //Get doctor's details from id
        // GET: Doctor/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            //if id null, fail
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //get doctor from id
            Doctor doctor = db.Doctors.Find(id);
            //if not doctor for id, fail
            if (doctor == null)
            {
                return HttpNotFound();
            }
            //if user not admin/reception, fail
            if (!(User.IsInRole("Admin") || User.IsInRole("Reception")))
            {
                return HttpNotFound();
            }
            return View(doctor);
        }

        //Get create screen for creating a doctor
        // GET: Doctor/Create
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

        //Create doctor from information provided in form
        // POST: Doctor/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Create([Bind(Include = "DoctorID,FirstName,LastName,DOB,Address,Suburb,Postcode,PhoneNumber,Email")] Doctor doctor)
        {
            //if all data provided
            if (ModelState.IsValid)
            {
                //add doctor to database and save it
                db.Doctors.Add(doctor);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(doctor);
        }

        //Launch edit screen for doctor from id
        // GET: Doctor/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            //if id null, fail
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //get doctor from id
            Doctor doctor = db.Doctors.Find(id);
            //if doctor null, fail
            if (doctor == null)
            {
                return HttpNotFound();
            }
            //if user not admin/reception, fail
            if (!(User.IsInRole("Admin") || User.IsInRole("Reception")))
            {
                return HttpNotFound();
            }
            return View(doctor);
        }

        //Actually update the doctor with edit form information
        // POST: Doctor/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit([Bind(Include = "DoctorID,FirstName,LastName,DOB,Address,Suburb,Postcode,PhoneNumber,Email")] Doctor doctor)
        {
            //if all information received
            if (ModelState.IsValid)
            {
                //update the entry in the database and save it
                db.Entry(doctor).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(doctor);
        }

        //Launch delete view for doctor from id
        // GET: Doctor/Delete/5
        [Authorize]
        public ActionResult Delete(int? id)
        {
            //if id null, fail
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //get doctor from id
            Doctor doctor = db.Doctors.Find(id);
            //if doctor null, fail
            if (doctor == null)
            {
                return HttpNotFound();
            }
            //if user not admin/reception, fail
            if (!(User.IsInRole("Admin") || User.IsInRole("Reception")))
            {
                return HttpNotFound();
            }
            return View(doctor);
        }

        //Actually delete the doctor from the database
        // POST: Doctor/Delete/5
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
            //Remove doctor from the database and then save it
            Doctor doctor = db.Doctors.Find(id);
            db.Doctors.Remove(doctor);
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
