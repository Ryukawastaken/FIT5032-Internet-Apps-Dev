namespace FIT5032_CodeFirstModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initialMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Students",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FirstName = c.String(),
                        LastName = c.String(),
                        PhoneNumber = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Units",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Descriton = c.String(),
                        UnitCode = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UnitStudents",
                c => new
                    {
                        Unit_Id = c.Int(nullable: false),
                        Student_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Unit_Id, t.Student_Id })
                .ForeignKey("dbo.Units", t => t.Unit_Id, cascadeDelete: true)
                .ForeignKey("dbo.Students", t => t.Student_Id, cascadeDelete: true)
                .Index(t => t.Unit_Id)
                .Index(t => t.Student_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UnitStudents", "Student_Id", "dbo.Students");
            DropForeignKey("dbo.UnitStudents", "Unit_Id", "dbo.Units");
            DropIndex("dbo.UnitStudents", new[] { "Student_Id" });
            DropIndex("dbo.UnitStudents", new[] { "Unit_Id" });
            DropTable("dbo.UnitStudents");
            DropTable("dbo.Units");
            DropTable("dbo.Students");
        }
    }
}
