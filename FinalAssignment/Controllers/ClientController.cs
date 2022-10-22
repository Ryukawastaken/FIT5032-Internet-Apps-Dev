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

//A single client
namespace FinalAssignment.Controllers
{
    public class ClientController : Controller
    {
        //Get the database
        private PracticeContext db = new PracticeContext();

        //Get all of the clients
        // GET: Client
        [Authorize]
        public ActionResult Index()
        {
            //Only get the clients if user is admin or reception, otherwise fail
            if (!(User.IsInRole("Admin") || User.IsInRole("Reception")))
            {
                return HttpNotFound();
            }
            return View(db.Clients.ToList());
        }

        //Get a single client's details
        // GET: Client/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            //Check if id is valid
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //Get the client from the database
            Client client = db.Clients.Find(id);

            //If no client, fail
            if (client == null)
            {
                return HttpNotFound();
            }
            //If user not admin or reception, fail
            if (!(User.IsInRole("Admin") || User.IsInRole("Reception")))
            {
                return HttpNotFound();
            }
            return View(client);
        }

        //Launch client create page
        // GET: Client/Create
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

        //Create a client with the details from the form
        // POST: Client/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Create([Bind(Include = "ClientID,FirstName,LastName,DOB,MedicareNo,MedicareUniqueID,Address,Suburb,Postcode,PhoneNumber,Email")] Client client)
        {
            //If all data received
            if (ModelState.IsValid)
            {

                //This is all trying to get the identity stuff to work so I could set each new client to be a new user, but couldn't get it working properly
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

                //Add client to database and save it
                db.Clients.Add(client);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(client);
        }

        //Try and launch the edit form from a client
        // GET: Client/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            //If id not valid, fail
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //Get client from id
            Client client = db.Clients.Find(id);
            
            //If client not found, fail
            if (client == null)
            {
                return HttpNotFound();
            }
            //If user not admin or reception, fail
            if (!(User.IsInRole("Admin") || User.IsInRole("Reception")))
            {
                return HttpNotFound();
            }

            return View(client);
        }

        //Actually update the client's details from the edit
        // POST: Client/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit([Bind(Include = "ClientID,FirstName,LastName,DOB,MedicareNo,MedicareUniqueID,Address,Suburb,Postcode,PhoneNumber,Email")] Client client)
        {
            //If all information received, update the entry in the database and save it
            if (ModelState.IsValid)
            {
                db.Entry(client).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(client);
        }

        //Launch delete screen from client id
        // GET: Client/Delete/5
        [Authorize]
        public ActionResult Delete(int? id)
        {
            //If id null, fail
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //Get client from id
            Client client = db.Clients.Find(id);
            //If client not found, fail
            if (client == null)
            {
                return HttpNotFound();
            }
            //if user not admin/reception, fail
            if (!(User.IsInRole("Admin") || User.IsInRole("Reception")))
            {
                return HttpNotFound();
            }
            return View(client);
        }

        //Actually delete the client
        // POST: Client/Delete/5
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
            //find the client, then remove it from the database and save it
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
