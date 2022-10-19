using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FinalAssignment.Models
{
    public class PracticeContext : DbContext
    {

        public PracticeContext() : base("PracticeDB"){}
        public DbSet<Client> Clients { get; set; }

        public DbSet<Doctor> Doctors { get; set; }
        
        public DbSet<Appointment> Appointments { get; set; }

        public DbSet<Image> Images { get; set; }

        public DbSet<Location> Locations { get; set; }

        public System.Data.Entity.DbSet<FinalAssignment.Models.Event> Events { get; set; }
    }
}