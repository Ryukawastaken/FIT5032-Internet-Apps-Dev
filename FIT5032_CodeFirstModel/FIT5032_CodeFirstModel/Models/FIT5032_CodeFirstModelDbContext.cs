using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace FIT5032_CodeFirstModel.Models
{
    public class FIT5032_CodeFirstModelDbContext : DbContext
    {
        // Your context has been configured to use a 'FIT5032_CodeFirstModel' connection string from your application's 
        // configuration file (App.config or Web.config). By default, this connection string targets the 
        // 'FIT5032_CodeFirstModel.Models.FIT5032_CodeFirstModel' database on your LocalDb instance. 
        // 
        // If you wish to target a different database and/or database provider, modify the 'FIT5032_CodeFirstModel' 
        // connection string in the application configuration file.

        public DbSet<Student> Students { get; set; }

        public DbSet<Unit> Units { get; set; }

        public FIT5032_CodeFirstModelDbContext()
            : base("name=FIT5032_CodeFirstModel")
        {
            
        }

        // Add a DbSet for each entity type that you want to include in your model. For more information 
        // on configuring and using a Code First model, see http://go.microsoft.com/fwlink/?LinkId=390109.

        // public virtual DbSet<MyEntity> MyEntities { get; set; }
    }

    public class Student
    {
        public int Id { get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string PhoneNumber { get; set; }

        public virtual List<Unit> Units { get; set; }
    }

    public class Unit
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string Descriton { get; set; }

        public string UnitCode { get; set; }

        public virtual List<Student> Students {get; set;}
    }
}