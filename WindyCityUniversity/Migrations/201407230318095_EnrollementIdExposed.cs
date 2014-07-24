namespace WindyCityUniversity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EnrollementIdExposed : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Enrollment", "Student_Id", "dbo.Student");
            DropIndex("dbo.Enrollment", new[] { "Student_Id" });
            RenameColumn(table: "dbo.Enrollment", name: "Student_Id", newName: "StudentId");
            AlterColumn("dbo.Enrollment", "StudentId", c => c.Guid(nullable: false));
            CreateIndex("dbo.Enrollment", "StudentId");
            AddForeignKey("dbo.Enrollment", "StudentId", "dbo.Student", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Enrollment", "StudentId", "dbo.Student");
            DropIndex("dbo.Enrollment", new[] { "StudentId" });
            AlterColumn("dbo.Enrollment", "StudentId", c => c.Guid());
            RenameColumn(table: "dbo.Enrollment", name: "StudentId", newName: "Student_Id");
            CreateIndex("dbo.Enrollment", "Student_Id");
            AddForeignKey("dbo.Enrollment", "Student_Id", "dbo.Student", "Id");
        }
    }
}
