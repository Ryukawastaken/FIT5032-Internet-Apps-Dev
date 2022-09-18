using System;
using System.Collections.Generic;
using FIT5032Assignment.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace FIT5032Assignment.DAL
{
    public class PracticeContext : DbContext
    {

        public PracticeContext() : base("PracticeContext")
        {

        }

        public DbSet<Client> Clients { get; set; }

        public DbSet<Appointment> Appointments { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}