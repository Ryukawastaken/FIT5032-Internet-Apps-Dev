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

//Controller for General Practice Appointments
namespace FinalAssignment.Controllers
{
    public class AppointmentController : Controller
    {
        //Get the Database
        private PracticeContext db = new PracticeContext();

        //Gets all appointments, and makes sure clients/doctors can't see any appointments that aren't theirs
        // GET: Appointment
        [Authorize]
        public ActionResult Index()
        {
            //If User is a doctor, get all the appointments that they are a part of
            if (User.IsInRole("Doctor"))
            {
                var appointments = db.Appointments.Where(a => string.Equals(a.Doctor.Email, User.Identity.Name)).Include(c => c.Client).Include(d => d.Doctor).Include(l => l.Location).Include(i => i.Image);
                return View(appointments);
            }
            //If User is a client, get all the appointments that they are a part of
            if (User.IsInRole("Client"))
            {
                var appointments = db.Appointments.Where(a => string.Equals(a.Client.Email, User.Identity.Name)).Include(c => c.Client).Include(d => d.Doctor).Include(l => l.Location).Include(i => i.Image);
                return View(appointments);
            }
            //If user is Admin or Reception, get all appointments
            if (User.IsInRole("Admin") || User.IsInRole("Reception"))
            {
                var allAppointments = db.Appointments.Include(c => c.Client).Include(d => d.Doctor).Include(l => l.Location).Include(i => i.Image);
                return View(allAppointments);
            }
            //If user not logged in, or has no appointments, fail request
            return HttpNotFound();
        }

        //Get a singular appointment's details
        // GET: Appointment/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            //Check if id is valid
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //Get all appointments
            var appointments = db.Appointments.Include(c => c.Client).Include(d => d.Doctor).Include(s => s.Schedule).Include(i=>i.Image).Include(l=>l.Location).ToList();
            Appointment appointment = new Appointment();

            //If they appointment is found, set it
            for (int i = 0; i < appointments.Count; i++)
            {
                if (appointments[i].AppointmentID == id)
                {
                    appointment = appointments[i];
                }
            }
            //If no appointment found, fail
            if (appointment == null)
            {
                return HttpNotFound();
            }
            //If client/doctor tries to view appointment that not theirs, fail
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

        //Create an appointment
        // POST: Appointment/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Create([Bind(Include = "AppointmentID,DateAndTime, ClientString, DoctorString, Client, Doctor, LocationString, Location, Duration, AppointmentType, Schedule, Image")] Appointment appointment)
        {
            //If got all relevant information
            if (ModelState.IsValid)
            {
                //Set client, doctor, and location from the dropdown list string by splitting it
                appointment.Client = db.Clients.Find(appointment.ClientString.Split('-')[0].Trim().AsInt());
                appointment.Doctor = db.Doctors.Find(appointment.DoctorString.Split('-')[0].Trim().AsInt());
                appointment.Location = db.Locations.Find(appointment.LocationString.Split('-')[0].Trim().AsInt());
                //Get all of the client's other appointments, then loop through them and check to see the the new appointment overlaps with another appointment as a booking constraint.
                var clientAppointments = db.Appointments.Where(a => appointment.Client.ClientID == a.Client.ClientID).ToList();
                for (int i = 0; i < clientAppointments.Count; i++)
                {
                    //If overlap, fail and say that it failed
                    if ((appointment.DateAndTime + TimeSpan.FromMinutes(appointment.Duration) >
                        clientAppointments[i].DateAndTime &&
                        clientAppointments[i].DateAndTime > appointment.DateAndTime) || (clientAppointments[i].DateAndTime + TimeSpan.FromMinutes(clientAppointments[i].Duration) > appointment.DateAndTime && appointment.DateAndTime > clientAppointments[i].DateAndTime))
                    {
                        var tempView = View(appointment);
                        tempView.ViewBag.Title = appointment.Client.DropDownList + " already has a booking during this time!";
                        return tempView;
                    }
                }
                //Get all of the doctor's other appointments, then loop through them and check to see the the new appointment overlaps with another appointment as a booking constraint.

                var doctorAppointments = db.Appointments.Where(a => appointment.Doctor.DoctorID == a.Doctor.DoctorID).ToList();
                for (int i = 0; i < doctorAppointments.Count; i++)
                {
                    //If overlap, fail and say that it failed

                    if ((appointment.DateAndTime + TimeSpan.FromMinutes(appointment.Duration) >
                         doctorAppointments[i].DateAndTime &&
                         doctorAppointments[i].DateAndTime > appointment.DateAndTime) || (doctorAppointments[i].DateAndTime + TimeSpan.FromMinutes(doctorAppointments[i].Duration) > appointment.DateAndTime && appointment.DateAndTime > doctorAppointments[i].DateAndTime))
                    {
                        var tempView = View(appointment);
                        tempView.ViewBag.Title = appointment.Doctor.DropDownList + " already has a booking during this time!";
                        return tempView;
                    }
                }
                //Create a new schedule which is used for sending an email
                Schedule newSchedule = new Schedule();
                newSchedule.Name = appointment.AppointmentType.ToString() + " between " + appointment.ClientString + " and " + appointment.DoctorString;
                newSchedule.StartTime = appointment.DateAndTime;
                newSchedule.EndTime = appointment.DateAndTime.Add(TimeSpan.FromMinutes(appointment.Duration));
                appointment.Schedule = newSchedule;

                //Add that new appointment to the database, save the changes, and then send an email saying the client has an appointment
                db.Appointments.Add(appointment);
                db.SaveChanges();
                SendEmail(appointment);
                return RedirectToAction("Index");
            }

            return View(appointment);
        }

        //Request an existing appointment to edit
        // GET: Appointment/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            //Check if id is valid
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //Get all appointments
            var appointments = db.Appointments.Include(c => c.Client).Include(d => d.Doctor).ToList();
            Appointment appointment = new Appointment();
            //Loop through all appointments to check if appointment matches ID requested
            for (int i = 0; i < appointments.Count; i++)
            {
                if (appointments[i].AppointmentID == id)
                {
                    appointment = appointments[i];
                }
            }
            //If appointment null, fail
            if (appointment == null)
            {
                return HttpNotFound();
            }
            //If client/doctor tries to view appointment that not theirs, fail
            else if (!(string.Equals(appointment.Client.Email, User.Identity.Name) ||
                       string.Equals(appointment.Doctor.Email, User.Identity.Name)) && !(User.IsInRole("Admin") || User.IsInRole("Reception")))
            {
                return HttpNotFound();
            }
            //Return the appointment
            return View(appointment);
        }

        //Actually edit the requested appointment
        // POST: Appointment/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit([Bind(Include = "AppointmentID, ClientString, DoctorString, Client, Doctor, LocationString, Location, Duration, AppointmentType, Schedule, Image, DoctorRating, AppointmentRating, DateAndTime")] Appointment appointment, HttpPostedFileBase postedFile)
        {
            //If got all relevant information
            if(ModelState.IsValid)
            {
                //Set client, doctor, and location from the dropdown list string by splitting it
                appointment.Client = db.Clients.Find(appointment.ClientString.Split('-')[0].Trim().AsInt());
                appointment.Doctor = db.Doctors.Find(appointment.DoctorString.Split('-')[0].Trim().AsInt());
                appointment.Location = db.Locations.Find(appointment.LocationString.Split('-')[0].Trim().AsInt());
                //Get all of the client's other appointments, then loop through them and check to see the the new appointment overlaps with another appointment as a booking constraint.
                var clientAppointments = db.Appointments.Where(a => appointment.Client.ClientID == a.Client.ClientID).ToList();
                for (int i = 0; i < clientAppointments.Count; i++)
                {
                    //If overlap, fail and say that it failed
                    if ((appointment.DateAndTime + TimeSpan.FromMinutes(appointment.Duration) >
                         clientAppointments[i].DateAndTime &&
                         clientAppointments[i].DateAndTime > appointment.DateAndTime) || (clientAppointments[i].DateAndTime + TimeSpan.FromMinutes(clientAppointments[i].Duration) > appointment.DateAndTime && appointment.DateAndTime > clientAppointments[i].DateAndTime) && appointment.DateAndTime + TimeSpan.FromMinutes(appointment.Duration) < DateTime.Now)
                    {
                        var tempView = View(appointment);
                        tempView.ViewBag.Title = appointment.Client.DropDownList + " already has a booking during this time!";
                        return tempView;
                    }
                }
                //Get all of the doctors's other appointments, then loop through them and check to see the the new appointment overlaps with another appointment as a booking constraint.

                var doctorAppointments = db.Appointments.Where(a => appointment.Doctor.DoctorID == a.Doctor.DoctorID).ToList();
                for (int i = 0; i < doctorAppointments.Count; i++)
                {
                    //If overlap, fail and say that it failed
                    if ((appointment.DateAndTime + TimeSpan.FromMinutes(appointment.Duration) >
                         doctorAppointments[i].DateAndTime &&
                         doctorAppointments[i].DateAndTime > appointment.DateAndTime) || (doctorAppointments[i].DateAndTime + TimeSpan.FromMinutes(doctorAppointments[i].Duration) > appointment.DateAndTime && appointment.DateAndTime > doctorAppointments[i].DateAndTime) && appointment.DateAndTime + TimeSpan.FromMinutes(appointment.Duration) < DateTime.Now)
                    {
                        var tempView = View(appointment);
                        tempView.ViewBag.Title = appointment.Doctor.DropDownList + " already has a booking during this time!";
                        return tempView;
                    }
                }
                
                //Create a new schedule used for emailing appointments and assign it to the appointment
                Schedule newSchedule = new Schedule();
                newSchedule.Name = appointment.AppointmentType.ToString() + " between " + appointment.ClientString + " and " + appointment.DoctorString;
                newSchedule.StartTime = appointment.DateAndTime;
                newSchedule.EndTime = appointment.DateAndTime.Add(TimeSpan.FromMinutes(appointment.Duration));
                appointment.Schedule = newSchedule;

                //Check if user uploaded file attachment
                if (postedFile != null)
                {
                    //Create an image with the information requested
                    var myUniqueFileName = string.Format(@"{0}", Guid.NewGuid());
                    Models.Image newImage = new Models.Image();
                    newImage.Path = myUniqueFileName;
                    newImage.Name = "Doctor's Certificate";
                    appointment.Image = newImage;

                    //Get the filepath of the image
                    string serverPath = Server.MapPath("~/Uploads/");
                    string fileExtension = Path.GetExtension(postedFile.FileName);
                    string filePath = appointment.Image.Path + fileExtension;

                    //Set the appointment's image, then save the image to the server and add it to the database
                    appointment.Image.Path = filePath;
                    postedFile.SaveAs(serverPath + appointment.Image.Path);
                    db.Images.Add(newImage);

                    //Send an email to the client with the new image attached to it
                    SendEmailWithAttachment(appointment);
                }

                //Update the appointment and save those changes in the database
                db.Set<Appointment>().AddOrUpdate(appointment);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(appointment);
        }

        //Get the appointment to delete
        // GET: Appointment/Delete/5
        [Authorize]
        public ActionResult Delete(int? id)
        {
            //Check if Id valid, if not, fail
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //Get all appointments
            var appointments = db.Appointments.Include(c => c.Client).Include(d => d.Doctor).ToList();
            Appointment appointment = new Appointment();

            //Find the appointment from its id
            for (int i = 0; i < appointments.Count; i++)
            {
                if (appointments[i].AppointmentID == id)
                {
                    appointment = appointments[i];
                }
            }
            //If appointment null, fail
            if (appointment == null)
            {
                return HttpNotFound();
            }

            //If client/doctor tries to view appointment that not theirs, fail
            else if (!(string.Equals(appointment.Client.Email, User.Identity.Name) ||
                       string.Equals(appointment.Doctor.Email, User.Identity.Name)) && !(User.IsInRole("Admin") || User.IsInRole("Reception")))
            {
                return HttpNotFound();
            }

            return View(appointment);
        }

        //Actually delete the requested delete appointment
        // POST: Appointment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult DeleteConfirmed(int id)
        {
            //Remove it from the database
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

        //Function for sending the client an email with their appointment attached as an .icms file
        private async Task SendEmail(Appointment appointment)
        {
            //Get sendgrid api key, and make a new instance
            var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
            var client = new SendGridClient(apiKey);
            //Create a new email from my sendgrid email address
            var newEmail = new SendGridMessage() { From = new EmailAddress("nicolaspallant@hotmail.com","Tamrul General Practice Innovations") };
            
            //Set email information with client's information and appointment's information
            newEmail.AddTo(appointment.Client.Email);
            newEmail.Subject = appointment.Schedule.Name;
            newEmail.HtmlContent = "Please find below the details of your new appointment <br> <br>";
            newEmail.HtmlContent += "Client Name: " + appointment.Client.FirstName + " " + appointment.Client.LastName + "<br>";
            newEmail.HtmlContent += "Doctor Name: " + appointment.Doctor.FirstName + " " + appointment.Doctor.LastName + "<br>";
            newEmail.HtmlContent += "Appointment Type: " + appointment.AppointmentType + "<br>";
            newEmail.HtmlContent += "Date and Time: " + appointment.DateAndTime + "<br>";
            newEmail.HtmlContent += "Duration: " + appointment.Duration + " minutes <br> <br>";
            newEmail.HtmlContent += "Thank you for choosing Healthcare Services General Practice";

            //Create a calandar file as a string, then turn it into a base64 string to attach it to the email
            string CalendarItem = CreateCalendarFile(appointment);
            byte[] byteArray = System.Text.ASCIIEncoding.ASCII.GetBytes(CalendarItem);
            string base64String = Convert.ToBase64String(byteArray);
            newEmail.AddAttachment("Appointment.ics", base64String);

            //Send the email
            var response = await client.SendEmailAsync(newEmail);

        }
        //Method for sending an email to the client with the doctor's certificate/referal attached
        private async Task SendEmailWithAttachment(Appointment appointment)
        {
            //Get Sendgrid key and create an instance
            var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
            var client = new SendGridClient(apiKey);

            //Get the image's path from the server
            string serverPath = Server.MapPath("~/Uploads/");
            string filePath = appointment.Image.Path;
            string fullPath = serverPath + filePath;

            //Create a new email from my sendgrid email
            var newEmail = new SendGridMessage() { From = new EmailAddress("nicolaspallant@hotmail.com","Tamrul General Practice Innovations") };
            newEmail.AddTo(appointment.Client.Email);
            newEmail.Subject = "Doctor's certificate";
            newEmail.HtmlContent = "This is your doctor's certificate for the appointment from today";
            newEmail.PlainTextContent = "Testing";

            //If there was actually an attachment and it was found
            if (System.IO.File.Exists(fullPath))
            {
                //Get the information from the server, then convert it to a base64 string, and attach it to the email
                MemoryStream m = new MemoryStream();
                Image.FromFile(fullPath).Save(m, Image.FromFile(fullPath).RawFormat);
                byte[] imageBytes = m.ToArray();
                string base64String = Convert.ToBase64String(imageBytes);
                newEmail.AddAttachment("DoctorsCertificate" + Path.GetExtension(filePath), base64String);
            }
            //Send email
            var response = await client.SendEmailAsync(newEmail);
        }

        //Method for sending emails to everyone with an appointment today
        private async Task SendBulkEmail()
        {
            //Get sendgrid API key and creating sendgrid instance
            var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
            var client = new SendGridClient(apiKey);

            //Get all the appointments, and make a list of people to send to
            var appointments = db.Appointments.Include(c => c.Client).ToList();
            var tos = new List<EmailAddress>();

            //Find all appointments with the date of today, and add that client's email to the to list
            for (int i = 0; i < appointments.Count; i++)
            {
                if (DateTime.Now.Date == appointments[i].DateAndTime.Date)
                {
                    var clientEmail = new EmailAddress(appointments[i].Client.Email, appointments[i].Client.FirstName + " " + appointments[i].Client.LastName);
                    tos.Add(clientEmail);
                }
            }

            //Set the email's information
            var from = new EmailAddress("nicolaspallant@hotmail.com", "Healthcare Services General Practice");
            var subject = "Reminder About Your Appointment Today";
            var plainTextContent = "";
            var htmlContent = "Just a reminder that you have a booking today at one of our clinics. Please find attached a map with directions highlighted between our two clinics for your convenience.";
            var showAllRecipients = false;

            //Create the message for multiple recipients
            var msg = MailHelper.CreateSingleEmailToMultipleRecipients(from, tos, subject, plainTextContent, htmlContent, showAllRecipients);

            //Get the first image from the server, which is directions between the two practices
            string serverPath = Server.MapPath("~/Uploads/");
            string filePath = db.Images.ToList()[0].Path;
            string fullPath = serverPath + filePath;

            //If that image exists, convert it to a base64 string and attach it to the email
            if (System.IO.File.Exists(fullPath))
            {
                MemoryStream m = new MemoryStream();
                Image.FromFile(fullPath).Save(m, Image.FromFile(fullPath).RawFormat);
                byte[] imageBytes = m.ToArray();
                string base64String = Convert.ToBase64String(imageBytes);
                msg.AddAttachment("Directions Between Clinics" + Path.GetExtension(filePath), base64String);
            }
            //Send the email
            var response = await client.SendEmailAsync(msg);
            
        }

        //Method for creating a calandar file from an appointment's details
        private string CreateCalendarFile(Appointment appointment)
        {

            //Create a new calandar event from Ical.net
            var newEvent = new CalendarEvent()
           {
               //Set the calandar event's details with the appointment
               Start = new CalDateTime(appointment.DateAndTime),
               End = new CalDateTime(appointment.DateAndTime + TimeSpan.FromMinutes(appointment.Duration)),
               Location = appointment.LocationString,
               Summary = appointment.Schedule.Name,
               Description = "Your Booking with Healthcare Services General Practice"
           };

            //Create the calandar, then serialise the event to a string and return it
           var calendar = new Calendar();
           calendar.Events.Add(newEvent);
           var serialiser = new CalendarSerializer();
           return serialiser.SerializeToString(calendar);
        }

        //Method for when the button on the appointment index page is clicked to send a bulk email
        public ActionResult OnButton()
        {
            SendBulkEmail();
            return new JsonResult();
        }
    }

}
