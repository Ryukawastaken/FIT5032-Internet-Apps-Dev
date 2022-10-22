using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FinalAssignment.Models;

//Represents an image, usually doctor's certificates
namespace FinalAssignment.Controllers
{
    public class ImageController : Controller
    {
        //Get database
        private PracticeContext db = new PracticeContext();

        //Get all images
        // GET: Image
        [Authorize]
        public ActionResult Index()
        {
            //Tried to get all of a doctor's and client's images so they could see them all in one place, but it didn't end up working

            /*if (User.IsInRole("Doctor"))
            {
                var appointments  = db.Appointments.Where(a => string.Equals(a.Doctor.Email, User.Identity.Name)).Include(c => c.Client).Include(d => d.Doctor).Include(l => l.Location).Include(i => i.Image).ToList();
                List<Image> images = new List<Image>();
                for (int i = 0; i < appointments.Count; i++)
                {
                    images.Append(appointments[i].Image);
                }
                return View(images);
            }
            if (User.IsInRole("Client"))
            {
                var appointments = db.Appointments.Where(a => string.Equals(a.Client.Email, User.Identity.Name)).Include(c => c.Client).Include(d => d.Doctor).Include(l => l.Location).Include(i => i.Image).ToList().Where(i => i.Image.ImageID == images.ForEach());
                List<Image> images = new List<Image>();
                for (int i = 0; i < appointments.Count; i++)
                {
                    images.Append(appointments[i].Image);
                }
            }*/

            //If user reception/admin, return all images, otherwise fail
            if (User.IsInRole("Admin") || User.IsInRole("Reception"))
            {
                return View(db.Images);
            }

            return HttpNotFound();
        }

        //Get image details from id
        // GET: Image/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            //if id null, fail
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //get image from id
            Image image = db.Images.Find(id);
            //If image null, fail
            if (image == null)
            {
                return HttpNotFound();
            }

            //Tried to see if a user owned an image, they could access it, but it failed

            /*if (User.IsInRole("Client"))
            {
                var appointments = db.Appointments.Where(a => string.Equals(a.Client.Email, User.Identity.Name)).Include(c => c.Client).Include(d => d.Doctor).Include(l => l.Location).Include(i => i.Image).ToList();
                for (int i = 0; i < appointments.Count; i++)
                {
                    if (image == appointments[i].Image)
                    {
                        return View(image);
                    }
                }
            }
            else if (User.IsInRole("Doctor"))
            {
                var appointments = db.Appointments.Where(a => string.Equals(a.Doctor.Email, User.Identity.Name)).Include(c => c.Client).Include(d => d.Doctor).Include(l => l.Location).Include(i => i.Image).ToList();
                for (int i = 0; i < appointments.Count; i++)
                {
                    if (image == appointments[i].Image)
                    {
                        return View(image);
                    }
                }
            }*/

            //if user admin/reception, return image, otherwise, fail
            else if (User.IsInRole("Admin") || User.IsInRole("Reception"))
            {
                return View(image);
            }
            return HttpNotFound();
        }

        //Launch create image form
        // GET: Image/Create
        [Authorize]
        public ActionResult Create()
        {
            //if user not admin or reception, fail
            if (!(User.IsInRole("Admin") || User.IsInRole("Reception")))
            {
                return HttpNotFound();
            }
            return View();
        }

        //Create image from information and file received from form
        // POST: Image/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Create([Bind(Include = "ImageID,Name")] Image image, HttpPostedFileBase postedFile)
        {
            //Set the image's path and infromation, then see if that worked
            ModelState.Clear();
            var myUniqueFileName = string.Format(@"{0}", Guid.NewGuid());
            image.Path = myUniqueFileName;
            TryValidateModel(image);

            //If data valid
            if (ModelState.IsValid)
            {
                //Get path in server, then save it in the server
                string serverPath = Server.MapPath("~/Uploads/");
                string fileExtension = Path.GetExtension(postedFile.FileName);
                string filePath = image.Path + fileExtension;
                image.Path = filePath;
                postedFile.SaveAs(serverPath + image.Path);

                //Add image to database and save it
                db.Images.Add(image);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(image);
        }

        //Launch edit form for image from id
        // GET: Image/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            //if id null, fail
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //get image from id
            Image image = db.Images.Find(id);
            //if image null, fail
            if (image == null)
            {
                return HttpNotFound();
            }

            //Tried to let user's access images that belong to them, but didn't end up working

            /*if (User.IsInRole("Client"))
            {
                var appointments = db.Appointments.Where(a => string.Equals(a.Client.Email, User.Identity.Name)).Include(c => c.Client).Include(d => d.Doctor).Include(l => l.Location).Include(i => i.Image).ToList();
                for (int i = 0; i < appointments.Count; i++)
                {
                    if (image == appointments[i].Image)
                    {
                        return View(image);
                    }
                }
            }
            else if (User.IsInRole("Doctor"))
            {
                var appointments = db.Appointments.Where(a => string.Equals(a.Doctor.Email, User.Identity.Name)).Include(c => c.Client).Include(d => d.Doctor).Include(l => l.Location).Include(i => i.Image).ToList();
                for (int i = 0; i < appointments.Count; i++)
                {
                    if (image == appointments[i].Image)
                    {
                        return View(image);
                    }
                }
            }*/

            //if user admin/reception, return image, if not, fail
            else if (User.IsInRole("Admin") || User.IsInRole("Reception"))
            {
                return View(image);
            }
            return HttpNotFound();
        }

        //Edit image information
        // POST: Image/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit([Bind(Include = "ImageID,Path,Name")] Image image)
        {
            //If all data valid, alter image entry in database and save that
            if (ModelState.IsValid)
            {
                db.Entry(image).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(image);
        }

        //Launch delete view for image from id
        // GET: Image/Delete/5
        [Authorize]
        public ActionResult Delete(int? id)
        {
            //if id null, fail
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //get image from id
            Image image = db.Images.Find(id);
            //if image null, fail
            if (image == null)
            {
                return HttpNotFound();
            }
            //if user not admin/reception, fail
            if (!(User.IsInRole("Admin") || User.IsInRole("Reception")))
            {
                return HttpNotFound();
            }
            return View(image);
        }

        //Actually delete the image
        // POST: Image/Delete/5
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
            //get the image from the id and delete it from the database, then save it.
            Image image = db.Images.Find(id);
            db.Images.Remove(image);
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
