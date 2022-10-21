using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using FinalAssignment.Models;
using SendGrid;
using System.Web.WebPages;
using EllipticCurve.Utils;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Ical.Net.Serialization;
using SendGrid.Helpers.Mail;
using Calendar = Ical.Net.Calendar;
using Image = System.Drawing.Image;
using System.Diagnostics;

namespace FinalAssignment.Controllers
{
    public class AppointmentController : Controller
    {
        private PracticeContext db = new PracticeContext();

        // GET: Appointment
        [Authorize]
        public ActionResult Index()
        {
            if (User.IsInRole("Doctor"))
            {
                var appointments = db.Appointments.Where(a => string.Equals(a.Doctor.Email, User.Identity.Name)).Include(c => c.Client).Include(d => d.Doctor).Include(l => l.Location).Include(i => i.Image);
                return View(appointments);
            }
            if (User.IsInRole("Client"))
            {
                var appointments = db.Appointments.Where(a => string.Equals(a.Client.Email, User.Identity.Name)).Include(c => c.Client).Include(d => d.Doctor).Include(l => l.Location).Include(i => i.Image);
                return View(appointments);
            }

            if (User.IsInRole("Admin") || User.IsInRole("Reception"))
            {
                var allAppointments = db.Appointments.Include(c => c.Client).Include(d => d.Doctor).Include(l => l.Location).Include(i => i.Image);
                return View(allAppointments);
            }

            return HttpNotFound();
        }

        // GET: Appointment/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var appointments = db.Appointments.Include(c => c.Client).Include(d => d.Doctor).Include(s => s.Schedule).Include(i=>i.Image).Include(l=>l.Location).ToList();
            Appointment appointment = new Appointment();
            for (int i = 0; i < appointments.Count; i++)
            {
                if (appointments[i].AppointmentID == id)
                {
                    appointment = appointments[i];
                }
            }
            if (appointment == null)
            {
                return HttpNotFound();
            }
            else if (!(string.Equals(appointment.Client.Email, User.Identity.Name) ||
                       string.Equals(appointment.Doctor.Email, User.Identity.Name)) && !(User.IsInRole("Admin") || User.IsInRole("Reception")))
            {
                return HttpNotFound();
            }
            return View(appointment);
        }

        // GET: Appointment/Create
        [Authorize]
        public ActionResult Create()
        {
            if (User.IsInRole("Admin") || User.IsInRole("Reception"))
            {
                return View();
            }

            return HttpNotFound();
        }

        // POST: Appointment/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Create([Bind(Include = "AppointmentID,DateAndTime, ClientString, DoctorString, Client, Doctor, LocationString, Location, Duration, AppointmentType, Schedule, Image")] Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                appointment.Client = db.Clients.Find(appointment.ClientString.Split('-')[0].Trim().AsInt());
                appointment.Doctor = db.Doctors.Find(appointment.DoctorString.Split('-')[0].Trim().AsInt());
                appointment.Location = db.Locations.Find(appointment.LocationString.Split('-')[0].Trim().AsInt());
                var clientAppointments = db.Appointments.Where(a => appointment.Client.ClientID == a.Client.ClientID).ToList();
                for (int i = 0; i < clientAppointments.Count; i++)
                {
                    if ((appointment.DateAndTime + TimeSpan.FromMinutes(appointment.Duration) >
                        clientAppointments[i].DateAndTime &&
                        clientAppointments[i].DateAndTime > appointment.DateAndTime) || (clientAppointments[i].DateAndTime + TimeSpan.FromMinutes(clientAppointments[i].Duration) > appointment.DateAndTime && appointment.DateAndTime > clientAppointments[i].DateAndTime))
                    {
                        var tempView = View(appointment);
                        tempView.ViewBag.Title = appointment.Client.DropDownList + " already has a booking during this time!";
                        return tempView;
                    }
                }
                var doctorAppointments = db.Appointments.Where(a => appointment.Doctor.DoctorID == a.Doctor.DoctorID).ToList();
                for (int i = 0; i < doctorAppointments.Count; i++)
                {
                    if ((appointment.DateAndTime + TimeSpan.FromMinutes(appointment.Duration) >
                         doctorAppointments[i].DateAndTime &&
                         doctorAppointments[i].DateAndTime > appointment.DateAndTime) || (doctorAppointments[i].DateAndTime + TimeSpan.FromMinutes(doctorAppointments[i].Duration) > appointment.DateAndTime && appointment.DateAndTime > doctorAppointments[i].DateAndTime))
                    {
                        var tempView = View(appointment);
                        tempView.ViewBag.Title = appointment.Doctor.DropDownList + " already has a booking during this time!";
                        return tempView;
                    }
                }
                Schedule newSchedule = new Schedule();
                newSchedule.Name = appointment.AppointmentType.ToString() + " between " + appointment.ClientString + " and " + appointment.DoctorString;
                newSchedule.StartTime = appointment.DateAndTime;
                newSchedule.EndTime = appointment.DateAndTime.Add(TimeSpan.FromMinutes(appointment.Duration));
                appointment.Schedule = newSchedule;


                db.Appointments.Add(appointment);
                db.SaveChanges();
                SendEmail(appointment);
                return RedirectToAction("Index");
            }

            return View(appointment);
        }

        // GET: Appointment/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var appointments = db.Appointments.Include(c => c.Client).Include(d => d.Doctor).ToList();
            Appointment appointment = new Appointment();
            for (int i = 0; i < appointments.Count; i++)
            {
                if (appointments[i].AppointmentID == id)
                {
                    appointment = appointments[i];
                }
            }
            if (appointment == null)
            {
                return HttpNotFound();
            }
            else if (!(string.Equals(appointment.Client.Email, User.Identity.Name) ||
                       string.Equals(appointment.Doctor.Email, User.Identity.Name)) && !(User.IsInRole("Admin") || User.IsInRole("Reception")))
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
        [Authorize]
        public ActionResult Edit([Bind(Include = "AppointmentID, ClientString, DoctorString, Client, Doctor, LocationString, Location, Duration, AppointmentType, Schedule, Image, DoctorRating, AppointmentRating, DateAndTime")] Appointment appointment, HttpPostedFileBase postedFile)
        {
            
            if(ModelState.IsValid)
            {
                appointment.Client = db.Clients.Find(appointment.ClientString.Split('-')[0].Trim().AsInt());
                appointment.Doctor = db.Doctors.Find(appointment.DoctorString.Split('-')[0].Trim().AsInt());
                appointment.Location = db.Locations.Find(appointment.LocationString.Split('-')[0].Trim().AsInt());
                var clientAppointments = db.Appointments.Where(a => appointment.Client.ClientID == a.Client.ClientID).ToList();
                for (int i = 0; i < clientAppointments.Count; i++)
                {
                    if ((appointment.DateAndTime + TimeSpan.FromMinutes(appointment.Duration) >
                        clientAppointments[i].DateAndTime &&
                        clientAppointments[i].DateAndTime > appointment.DateAndTime) || (clientAppointments[i].DateAndTime + TimeSpan.FromMinutes(clientAppointments[i].Duration) > appointment.DateAndTime && appointment.DateAndTime > clientAppointments[i].DateAndTime) && appointment.DateAndTime + TimeSpan.FromMinutes(appointment.Duration) < DateTime.Now)
                    {
                        var tempView = View(appointment);
                        tempView.ViewBag.Title = appointment.Client.DropDownList + " already has a booking during this time!";
                        return tempView;
                    }
                }
                var doctorAppointments = db.Appointments.Where(a => appointment.Doctor.DoctorID == a.Doctor.DoctorID).ToList();
                for (int i = 0; i < doctorAppointments.Count; i++)
                {
                    if ((appointment.DateAndTime + TimeSpan.FromMinutes(appointment.Duration) >
                         doctorAppointments[i].DateAndTime &&
                         doctorAppointments[i].DateAndTime > appointment.DateAndTime) || (doctorAppointments[i].DateAndTime + TimeSpan.FromMinutes(doctorAppointments[i].Duration) > appointment.DateAndTime && appointment.DateAndTime > doctorAppointments[i].DateAndTime) && appointment.DateAndTime + TimeSpan.FromMinutes(appointment.Duration) < DateTime.Now)
                    {
                        var tempView = View(appointment);
                        tempView.ViewBag.Title = appointment.Doctor.DropDownList + " already has a booking during this time!";
                        return tempView;
                    }
                }
                Schedule newSchedule = new Schedule();
                newSchedule.Name = appointment.AppointmentType.ToString() + " between " + appointment.ClientString + " and " + appointment.DoctorString;
                newSchedule.StartTime = appointment.DateAndTime;
                newSchedule.EndTime = appointment.DateAndTime.Add(TimeSpan.FromMinutes(appointment.Duration));
                appointment.Schedule = newSchedule;


                if (postedFile != null)
                {
                    var myUniqueFileName = string.Format(@"{0}", Guid.NewGuid());
                    Models.Image newImage = new Models.Image();
                    newImage.Path = myUniqueFileName;
                    newImage.Name = "Doctor's Certificate";
                    appointment.Image = newImage;

                    string serverPath = Server.MapPath("~/Uploads/");
                    string fileExtension = Path.GetExtension(postedFile.FileName);
                    string filePath = appointment.Image.Path + fileExtension;
                    appointment.Image.Path = filePath;
                    postedFile.SaveAs(serverPath + appointment.Image.Path);
                    db.Images.Add(newImage);
                    SendEmailWithAttachment(appointment);
                }
                db.Set<Appointment>().AddOrUpdate(appointment);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(appointment);
        }

        // GET: Appointment/Delete/5
        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var appointments = db.Appointments.Include(c => c.Client).Include(d => d.Doctor).ToList();
            Appointment appointment = new Appointment();
            for (int i = 0; i < appointments.Count; i++)
            {
                if (appointments[i].AppointmentID == id)
                {
                    appointment = appointments[i];
                }
            }
            if (appointment == null)
            {
                return HttpNotFound();
            }
            else if (!(string.Equals(appointment.Client.Email, User.Identity.Name) ||
                       string.Equals(appointment.Doctor.Email, User.Identity.Name)) && !(User.IsInRole("Admin") || User.IsInRole("Reception")))
            {
                return HttpNotFound();
            }

            return View(appointment);
        }

        // POST: Appointment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
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

        private async Task SendEmail(Appointment appointment)
        {
            var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
            var client = new SendGridClient(apiKey);
            var newEmail = new SendGridMessage() { From = new EmailAddress("nicolaspallant@hotmail.com") };
            newEmail.AddTo(appointment.Client.Email);
            newEmail.Subject = appointment.Schedule.Name;
            newEmail.HtmlContent = "Please find below the details of your new appointment <br> <br>";
            newEmail.HtmlContent += "Client Name: " + appointment.Client.FirstName + " " + appointment.Client.LastName + "<br>";
            newEmail.HtmlContent += "Doctor Name: " + appointment.Doctor.FirstName + " " + appointment.Doctor.LastName + "<br>";
            newEmail.HtmlContent += "Appointment Type: " + appointment.AppointmentType + "<br>";
            newEmail.HtmlContent += "Date and Time: " + appointment.DateAndTime + "<br>";
            newEmail.HtmlContent += "Duration: " + appointment.Duration + " minutes <br> <br>";
            newEmail.HtmlContent += "Thank you for choosing Healthcare Services General Practice";

            string CalendarItem = CreateCalendarFile(appointment);
            byte[] byteArray = System.Text.ASCIIEncoding.ASCII.GetBytes(CalendarItem);
            string base64String = Convert.ToBase64String(byteArray);
            newEmail.AddAttachment("Appointment.ics", base64String);
            var response = await client.SendEmailAsync(newEmail);

        }
        private async Task SendEmailWithAttachment(Appointment appointment)
        {
            var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
            var client = new SendGridClient(apiKey);
            string serverPath = Server.MapPath("~/Uploads/");
            string filePath = appointment.Image.Path;
            string fullPath = serverPath + filePath;
            var newEmail = new SendGridMessage() { From = new EmailAddress("nicolaspallant@hotmail.com") };
            newEmail.AddTo(appointment.Client.Email);
            newEmail.Subject = "Doctor's certificate";
            newEmail.HtmlContent = "This is your doctor's certificate for the appointment from today";
            newEmail.PlainTextContent = "Testing";
            if (System.IO.File.Exists(fullPath))
            {
                MemoryStream m = new MemoryStream();
                Image.FromFile(fullPath).Save(m, Image.FromFile(fullPath).RawFormat);
                byte[] imageBytes = m.ToArray();
                string base64String = Convert.ToBase64String(imageBytes);
                newEmail.AddAttachment("DoctorsCertificate" + Path.GetExtension(filePath), base64String);
            }

            var response = await client.SendEmailAsync(newEmail);
        }

        private async Task SendBulkEmail()
        {
            var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
            var client = new SendGridClient(apiKey);

            var appointments = db.Appointments.Include(c => c.Client).ToList();
            var tos = new List<EmailAddress>();

            for (int i = 0; i < appointments.Count; i++)
            {
                if (DateTime.Now.Date == appointments[i].DateAndTime.Date)
                {
                    var clientEmail = new EmailAddress(appointments[i].Client.Email, appointments[i].Client.FirstName + " " + appointments[i].Client.LastName);
                    tos.Add(clientEmail);
                }
            }

            var from = new EmailAddress("nicolaspallant@hotmail.com", "Healthcare Services General Practice");
            var subject = "Reminder About Your Appointment Today";
            var plainTextContent = "";
            var htmlContent = "Just a reminder that you have a booking today at one of our clinics. Please find attached a map with directions highlighted between our two clinics for your convenience.";
            var showAllRecipients = false;

            var msg = MailHelper.CreateSingleEmailToMultipleRecipients(from, tos, subject, plainTextContent, htmlContent, showAllRecipients);

            string serverPath = Server.MapPath("~/Uploads/");
            string filePath = db.Images.ToList()[0].Path;
            string fullPath = serverPath + filePath;

            if (System.IO.File.Exists(fullPath))
            {
                MemoryStream m = new MemoryStream();
                Image.FromFile(fullPath).Save(m, Image.FromFile(fullPath).RawFormat);
                byte[] imageBytes = m.ToArray();
                string base64String = Convert.ToBase64String(imageBytes);
                msg.AddAttachment("Directions Between Clinics" + Path.GetExtension(filePath), base64String);
            }

            var response = await client.SendEmailAsync(msg);
            
        }

        private string CreateCalendarFile(Appointment appointment)
        {

            var newEvent = new CalendarEvent()
           {
               Start = new CalDateTime(appointment.DateAndTime),
               End = new CalDateTime(appointment.DateAndTime + TimeSpan.FromMinutes(appointment.Duration)),
               Location = appointment.LocationString,
               Summary = appointment.Schedule.Name,
               Description = "Your Booking with Healthcare Services General Practice"
           };

           var calendar = new Calendar();
           calendar.Events.Add(newEvent);
           var serialiser = new CalendarSerializer();
           return serialiser.SerializeToString(calendar);
        }

        public ActionResult OnButton()
        {
            SendBulkEmail();
            return new JsonResult();
        }
    }

}
