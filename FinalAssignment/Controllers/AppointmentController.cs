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
        public ActionResult Index()
        {
            if (User.IsInRole("Doctor"))
            {
                var appointments = db.Appointments.Where(a => string.Equals(a.Doctor.Email, User.Identity.Name)).Include(c => c.Client).Include(d => d.Doctor).Include(l => l.Location);
                return View(appointments);
            }
            if (User.IsInRole("Client"))
            {
                var appointments = db.Appointments.Where(a => string.Equals(a.Client.Email, User.Identity.Name)).Include(c => c.Client).Include(d => d.Doctor).Include(l => l.Location);
                return View(appointments);
            }

            var allAppointments = db.Appointments.Include(c => c.Client).Include(d => d.Doctor).Include(l => l.Location);
            return View(allAppointments);
        }

        // GET: Appointment/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Appointment appointment = db.Appointments.Include(c => c.Client).Include(d => d.Doctor).Include(s => s.Schedule).ToList()[(int)id];
            if (appointment == null)
            {
                return HttpNotFound();
            }
            SendBulkEmail();
            //SendEmail(appointment);
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
        public ActionResult Create([Bind(Include = "AppointmentID,DateAndTime, ClientString, DoctorString, Client, Doctor, LocationString, Location, Duration, AppointmentType, Schedule")] Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                appointment.Client = db.Clients.Find(appointment.ClientString.Split('-')[0].Trim().AsInt());
                appointment.Doctor = db.Doctors.Find(appointment.DoctorString.Split('-')[0].Trim().AsInt());
                appointment.Location = db.Locations.Find(appointment.LocationString.Split('-')[0].Trim().AsInt());
                appointment.OldDateAndTime = appointment.DateAndTime;
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
                for (int i = 0; i < clientAppointments.Count; i++)
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

                //appointment.Image = db.Images.ToList()[0];
                db.Appointments.Add(appointment);
                db.SaveChanges();
                SendEmail(appointment);
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

            var newView = View(appointment);
            newView.ViewBag.oldDate = appointment.DateAndTime.ToString("YYYY-MM-DDTHH:II");
            return newView;
        }

        // POST: Appointment/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AppointmentID,DateAndTime, ClientString, DoctorString, Client, Doctor, LocationString, Location, Duration, AppointmentType, Schedule, Image, DoctorRating, AppointmentRating")] Appointment appointment, HttpPostedFileBase postedFile)
        {
            if (appointment.DateAndTime == null)
            {
                appointment.DateAndTime = appointment.OldDateAndTime;
            }

            appointment.OldDateAndTime = appointment.DateAndTime;
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
                        clientAppointments[i].DateAndTime > appointment.DateAndTime) || (clientAppointments[i].DateAndTime + TimeSpan.FromMinutes(clientAppointments[i].Duration) > appointment.DateAndTime && appointment.DateAndTime > clientAppointments[i].DateAndTime))
                    {
                        var tempView = View(appointment);
                        tempView.ViewBag.Title = appointment.Client.DropDownList + " already has a booking during this time!";
                        return tempView;
                    }
                }
                var doctorAppointments = db.Appointments.Where(a => appointment.Doctor.DoctorID == a.Doctor.DoctorID).ToList();
                for (int i = 0; i < clientAppointments.Count; i++)
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
                //db.Entry(appointment).State = EntityState.Modified;
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
            Debug.WriteLine(response.IsSuccessStatusCode ? "Email Queued Succesffuly!" : "Not Successful! D:");


            /* string CalendarItem = CreateCalendarFile(appointment);
             byte[] byteArray = Convert.FromBase64String(CalendarItem);
             string base64String = Convert.ToBase64String(byteArray);

             Response.ClearHeaders();
             Response.Clear();
             Response.Buffer = true;
             Response.ContentType = "text/calendar";
             Response.AddHeader("content-length", CalendarItem.Length.ToString());
             Response.AddHeader("content-disposition", "attachment; filename=\"" + "Booking" + ".ics\"");
             Response.Write(CalendarItem);
             Response.Flush();

             newEmail.PlainTextContent = "Testing";*/

            //var response = await client.SendEmailAsync(newEmail);
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

        private async Task SendBulkEmail()
        {
            var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
            var client = new SendGridClient(apiKey);
            var personalisation = new Personalization();
            personalisation.Bccs = new List<EmailAddress>();
            var appointments = db.Appointments.Include(c => c.Client).ToList();
            var tos = new List<EmailAddress>();
            for (int i = 0; i < appointments.Count; i++)
            {
                var clientEmail = new EmailAddress(appointments[i].Client.Email, appointments[i].Client.FirstName);
                tos.Add(clientEmail);
            }

            var from = new EmailAddress("nicolaspallant@hotmail.com", "Healthcare Services General Practice");
            var subject = "Sending with Twilio SendGrid is Fun";
            var plainTextContent = "and easy to do anywhere, even with C#";
            var htmlContent = "<strong>and easy to do anywhere, even with C#</strong>";
            var showAllRecipients = false; // Set to true if you want the recipients to see each others email addresses

            var msg = MailHelper.CreateSingleEmailToMultipleRecipients(from,
                                                                       tos,
                                                                       subject,
                                                                       plainTextContent,
                                                                       htmlContent,
                                                                       showAllRecipients
                                                                       );
            var response = await client.SendEmailAsync(msg);
            

            /*string serverPath = Server.MapPath("~/Uploads/");
            string filePath = db.Images.ToList()[0].Path;
            string fullPath = serverPath + filePath;*//*
            newEmail.From = new EmailAddress("NicolasPallant.hotmail.com", "Healthcare Services General Practice");
            newEmail.Subject = "Appointment Reminder & Directions";
            //newEmail.AddTos(emails);
            newEmail.AddTo("nic.pallant@monash.edu");
            newEmail.AddTo("npal0002@student.monash.edu");
            //var newEmail = new SendGridMessage();
            newEmail.HtmlContent = "This is reminder about your appointment booked for today";
            newEmail.PlainTextContent = "Testing";
            *//*if (System.IO.File.Exists(fullPath))
            {
               
                MemoryStream m = new MemoryStream();
                Image.FromFile(fullPath).Save(m, Image.FromFile(fullPath).RawFormat);
                byte[] imageBytes = m.ToArray();
                string base64String = Convert.ToBase64String(imageBytes);
                newEmail.AddAttachment("DoctorsCertificate" + Path.GetExtension(filePath), base64String);
            }*//*
            Debug.WriteLine("Test");
            var response = await client.SendEmailAsync(newEmail);
            Debug.WriteLine("Finished!");*/
            //Debug.WriteLine(response.IsSuccessStatusCode ? "Email Queued Succesffuly!" : "Not Successful! D:");
        }

        private string CreateCalendarFile(Appointment appointment)
        {
           /* DateTime DateStart = appointment.DateAndTime;
            DateTime DateEnd = appointment.DateAndTime + TimeSpan.FromMinutes(appointment.Duration);
            string Summary = appointment.Schedule.Name;
            string Location = appointment.LocationString;
            string Description = "Your Booking with Healthcare Services General Practice";
            string FileName = "Booking";

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("BEGIN:VCALENDAR");
            sb.AppendLine("VERSION:2.0");
            sb.AppendLine("PRODID:stackoverflow.com");
            sb.AppendLine("CALSCALE:GREGORIAN");
            sb.AppendLine("METHOD:PUBLISH");

            sb.AppendLine("BEGIN:VTIMEZONE");
            sb.AppendLine("TZID:Australia/Sydney");
            sb.AppendLine("END:VTIMEZONE");

            sb.AppendLine("BEGIN:VEVENT");

            sb.AppendLine("DTSTART:" + DateStart.ToString("yyyyMMddTHHmm00"));
            sb.AppendLine("DTEND:" + DateEnd.ToString("yyyyMMddTHHmm00"));

            sb.AppendLine("SUMMARY:" + Summary + "");
            sb.AppendLine("LOCATION:" + Location + "");
            sb.AppendLine("DESCRIPTION:" + Description + "");
            sb.AppendLine("PRIORITY:3");
            sb.AppendLine("END:VEVENT");


            sb.AppendLine("END:VCALENDAR");

            string CalandarItem = sb.ToString();

            return CalandarItem;*/

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
    }

}
