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

namespace FinalAssignment.Controllers
{
    public class ImageController : Controller
    {
        private PracticeContext db = new PracticeContext();

        // GET: Image
        [Authorize]
        public ActionResult Index()
        {
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
            }
*/
            if (User.IsInRole("Admin") || User.IsInRole("Reception"))
            {
                return View(db.Images);
            }

            return HttpNotFound();
        }

        // GET: Image/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Image image = db.Images.Find(id);
            if (image == null)
            {
                return HttpNotFound();
            }

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
            else if (User.IsInRole("Admin") || User.IsInRole("Reception"))
            {
                return View(image);
            }
            return HttpNotFound();
        }

        // GET: Image/Create
        [Authorize]
        public ActionResult Create()
        {
            if (!(User.IsInRole("Admin") || User.IsInRole("Reception")))
            {
                return HttpNotFound();
            }
            return View();
        }

        // POST: Image/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Create([Bind(Include = "ImageID,Name")] Image image, HttpPostedFileBase postedFile)
        {
            ModelState.Clear();
            var myUniqueFileName = string.Format(@"{0}", Guid.NewGuid());
            image.Path = myUniqueFileName;
            TryValidateModel(image);

            if (ModelState.IsValid)
            {
                string serverPath = Server.MapPath("~/Uploads/");
                string fileExtension = Path.GetExtension(postedFile.FileName);
                string filePath = image.Path + fileExtension;
                image.Path = filePath;
                postedFile.SaveAs(serverPath + image.Path);

                db.Images.Add(image);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(image);
        }

        // GET: Image/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Image image = db.Images.Find(id);
            if (image == null)
            {
                return HttpNotFound();
            }
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
            else if (User.IsInRole("Admin") || User.IsInRole("Reception"))
            {
                return View(image);
            }
            return HttpNotFound();
        }

        // POST: Image/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit([Bind(Include = "ImageID,Path,Name")] Image image)
        {
            if (ModelState.IsValid)
            {
                db.Entry(image).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(image);
        }

        // GET: Image/Delete/5
        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Image image = db.Images.Find(id);
            if (image == null)
            {
                return HttpNotFound();
            }
            if (!(User.IsInRole("Admin") || User.IsInRole("Reception")))
            {
                return HttpNotFound();
            }
            return View(image);
        }

        // POST: Image/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!(User.IsInRole("Admin") || User.IsInRole("Reception")))
            {
                return HttpNotFound();
            }
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
