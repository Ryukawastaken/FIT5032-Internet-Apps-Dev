using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using FinalAssignment.Models;
using SendGrid;
using System.Threading.Tasks;
using System.Web.WebPages;
using EllipticCurve.Utils;
using SendGrid.Helpers.Mail;
using Image = System.Drawing.Image;

namespace FinalAssignment.Controllers
{
    public class AppointmentController : Controller
    {
        private PracticeContext db = new PracticeContext();

        // GET: Appointment
        public ActionResult Index()
        {
            if (User.IsInRole("Doctor"))
            {
                var appointments = db.Appointments.Where(a => string.Equals(a.Doctor.Email, User.Identity.Name)).Include(c => c.Client).Include(d => d.Doctor);
                return View(appointments);
            }
            if (User.IsInRole("Client"))
            {
                var appointments = db.Appointments.Where(a => string.Equals(a.Client.Email, User.Identity.Name)).Include(c => c.Client).Include(d => d.Doctor);
                return View(appointments);
            }

            var appointmentss = db.Appointments.Include(c => c.Client).Include(d => d.Doctor);
            return View(appointmentss);
        }

        // GET: Appointment/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Appointment appointment = db.Appointments.Find(id);
            if (appointment == null)
            {
                return HttpNotFound();
            }
            return View(appointment);
        }

        // GET: Appointment/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Appointment/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AppointmentID,DateAndTime, ClientString, DoctorString, Client, Doctor")] Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                appointment.Client = db.Clients.Find(appointment.ClientString.Split('-')[0].Trim().AsInt());
                appointment.Doctor = db.Doctors.Find(appointment.DoctorString.Split('-')[0].Trim().AsInt());
                //appointment.Image = db.Images.ToList()[0];
                db.Appointments.Add(appointment);
                db.SaveChanges();
                SendEmail(appointment.Client);
                return RedirectToAction("Index");
            }

            return View(appointment);
        }

        // GET: Appointment/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Appointment appointment = db.Appointments.Find(id);
            if (appointment == null)
            {
                return HttpNotFound();
            }
            return View(appointment);
        }

        // POST: Appointment/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AppointmentID,DateAndTime")] Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(appointment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(appointment);
        }

        // GET: Appointment/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Appointment appointment = db.Appointments.Find(id);
            if (appointment == null)
            {
                return HttpNotFound();
            }
            return View(appointment);
        }

        // POST: Appointment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Appointment appointment = db.Appointments.Find(id);
            db.Appointments.Remove(appointment);
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

        private async Task SendEmail(Client sendee)
        {
            var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
            var client = new SendGridClient(apiKey);
            string serverPath = Server.MapPath("~/Uploads/");
            string filePath = db.Images.ToList()[0].Path;
            string fullPath = serverPath + filePath;
            var newEmail = new SendGridMessage() { From = new EmailAddress("nicolaspallant@hotmail.com") };
            newEmail.AddTo(sendee.Email);
            newEmail.Subject = "Doctor's certificate";
            newEmail.HtmlContent = "This is your doctor's certificate for the appointment from today";
            newEmail.PlainTextContent = "Testing";
            if (System.IO.File.Exists(fullPath))
            {
                //attachment.Content = Convert.ToBase64String(System.IO.File.OpenRead(fullPath));
                //Convert.ToBase64String(Image.FromFile(fullPath));
                MemoryStream m = new MemoryStream();
                Image.FromFile(fullPath).Save(m, Image.FromFile(fullPath).RawFormat);
                byte[] imageBytes = m.ToArray();
                string base64String = Convert.ToBase64String(imageBytes);
                newEmail.AddAttachment("DoctorsCertificate" + Path.GetExtension(filePath), base64String);
            }

            var response = await client.SendEmailAsync(newEmail);
        }
    }
}
