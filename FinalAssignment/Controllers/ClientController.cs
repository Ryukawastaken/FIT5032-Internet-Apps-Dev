using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FinalAssignment.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;

namespace FinalAssignment.Controllers
{
    public class ClientController : Controller
    {
        private PracticeContext db = new PracticeContext();

        // GET: Client
        [Authorize]
        public ActionResult Index()
        {
            if (!(User.IsInRole("Admin") || User.IsInRole("Reception")))
            {
                return HttpNotFound();
            }
            return View(db.Clients.ToList());
        }

        // GET: Client/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Client client = db.Clients.Find(id);
            if (client == null)
            {
                return HttpNotFound();
            }
            if (!(User.IsInRole("Admin") || User.IsInRole("Reception")))
            {
                return HttpNotFound();
            }
            return View(client);
        }

        // GET: Client/Create
        [Authorize]
        public ActionResult Create()
        {
            if (!(User.IsInRole("Admin") || User.IsInRole("Reception")))
            {
                return HttpNotFound();
            }
            return View();
        }

        // POST: Client/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Create([Bind(Include = "ClientID,FirstName,LastName,DOB,MedicareNo,MedicareUniqueID,Address,Suburb,Postcode,PhoneNumber,Email")] Client client)
        {
            if (ModelState.IsValid)
            {
                /*var temp = new ApplicationDbContext();
                //var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(HttpContext.GetOwinContext()));
                var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>());
                //var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>());
                var users = userManager.Users.ToList();
                for (int i = 0; i < users.Count; i++)
                {
                    if (users[i].Email == client.Email)
                    {
                        userManager.AddToRole(users[i].Id, "Client");
                        break;
                    }
                }*/
                db.Clients.Add(client);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(client);
        }

        // GET: Client/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Client client = db.Clients.Find(id);
            if (client == null)
            {
                return HttpNotFound();
            }
            if (!(User.IsInRole("Admin") || User.IsInRole("Reception")))
            {
                return HttpNotFound();
            }

            return View(client);
        }

        // POST: Client/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit([Bind(Include = "ClientID,FirstName,LastName,DOB,MedicareNo,MedicareUniqueID,Address,Suburb,Postcode,PhoneNumber,Email")] Client client)
        {
            if (ModelState.IsValid)
            {
                db.Entry(client).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(client);
        }

        // GET: Client/Delete/5
        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Client client = db.Clients.Find(id);
            if (client == null)
            {
                return HttpNotFound();
            }
            if (!(User.IsInRole("Admin") || User.IsInRole("Reception")))
            {
                return HttpNotFound();
            }
            return View(client);
        }

        // POST: Client/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!(User.IsInRole("Admin") || User.IsInRole("Reception")))
            {
                return HttpNotFound();
            }
            Client client = db.Clients.Find(id);
            db.Clients.Remove(client);
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
