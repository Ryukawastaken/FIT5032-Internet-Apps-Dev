using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FIT5032Assignment.Models;

namespace FIT5032Assignment.DAL
{
    public class PracticeInitialiser : System.Data.Entity.DropCreateDatabaseIfModelChanges<PracticeContext>
    {
        protected override void Seed(PracticeContext context)
        {
            var clients = new List<Client>
            {
                new Client("Nicolas", "Pallant", DateTime.Today, "11111111", "1", "21 Braeburn Parade", "3178", "0422592902", "NicolasPallant@hotmail.com"),
                new Client("Jaimee", "Raper", DateTime.Today, "11111112", "1", "12 Braeburn Parade", "3178", "0422592901", "NicolasPallant@yahoo.com")
            };

            clients.ForEach(s => context.Clients.Add(s));
            context.SaveChanges();

            var appointments = new List<Appointment>
            {
                new Appointment(clients[0]),
                new Appointment(clients[1])
            };

            appointments.ForEach(s => context.Appointments.Add(s));
            context.SaveChanges();
        }
    }
}