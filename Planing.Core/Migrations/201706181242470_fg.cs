namespace Planing.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fg : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Annees",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AnneeScolaires",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 4000),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ClassRooms",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 4000),
                        Code = c.String(maxLength: 4000),
                        Type = c.String(maxLength: 4000),
                        MinSize = c.Int(nullable: false),
                        MaxSize = c.Int(nullable: false),
                        FaculteId = c.Int(nullable: false),
                        ClassRoomTypeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ClassRoomTypes", t => t.ClassRoomTypeId, cascadeDelete: true)
                .ForeignKey("dbo.Facultes", t => t.FaculteId, cascadeDelete: true)
                .Index(t => t.FaculteId)
                .Index(t => t.ClassRoomTypeId);
            
            CreateTable(
                "dbo.ClassRoomTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 4000),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Facultes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Libelle = c.String(maxLength: 4000),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ClassSeances",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClassRoomId = c.Int(nullable: false),
                        Seance = c.Int(nullable: false),
                        TypeClass = c.String(maxLength: 4000),
                        Min = c.Int(nullable: false),
                        Max = c.Int(nullable: false),
                        ClassRoomTypeId = c.Int(nullable: false),
                        Semestre = c.Int(nullable: false),
                        AnneeScolaireId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AnneeScolaires", t => t.AnneeScolaireId, cascadeDelete: true)
                .ForeignKey("dbo.ClassRooms", t => t.ClassRoomId, cascadeDelete: true)
                .ForeignKey("dbo.ClassRoomTypes", t => t.ClassRoomTypeId, cascadeDelete: false)
                .Index(t => t.ClassRoomId)
                .Index(t => t.ClassRoomTypeId)
                .Index(t => t.AnneeScolaireId);
            
            CreateTable(
                "dbo.Courses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 4000),
                        Code = c.String(maxLength: 4000),
                        CourseTypeId = c.Int(),
                        SpecialiteId = c.Int(nullable: false),
                        AnneeId = c.Int(nullable: false),
                        Semestre = c.Int(nullable: false),
                        Periode = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Annees", t => t.AnneeId, cascadeDelete: true)
                .ForeignKey("dbo.CourseTypes", t => t.CourseTypeId)
                .ForeignKey("dbo.Specialites", t => t.SpecialiteId, cascadeDelete: true)
                .Index(t => t.CourseTypeId)
                .Index(t => t.SpecialiteId)
                .Index(t => t.AnneeId);
            
            CreateTable(
                "dbo.CourseTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 4000),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Specialites",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 4000),
                        Code = c.String(maxLength: 4000),
                        FilliereId = c.Int(),
                        DepartementId = c.Int(),
                        FaculteId = c.Int(nullable: false),
                        NiveauId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Departements", t => t.DepartementId)
                .ForeignKey("dbo.Facultes", t => t.FaculteId, cascadeDelete: true)
                .ForeignKey("dbo.Fillieres", t => t.FilliereId)
                .ForeignKey("dbo.Niveaux", t => t.NiveauId, cascadeDelete: true)
                .Index(t => t.FilliereId)
                .Index(t => t.DepartementId)
                .Index(t => t.FaculteId)
                .Index(t => t.NiveauId);
            
            CreateTable(
                "dbo.Departements",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Libelle = c.String(maxLength: 4000),
                        FaculteId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Facultes", t => t.FaculteId, cascadeDelete: true)
                .Index(t => t.FaculteId);
            
            CreateTable(
                "dbo.Fillieres",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Libelle = c.String(maxLength: 4000),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Niveaux",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Libelle = c.String(maxLength: 4000),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Groupes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 4000),
                        SectionId = c.Int(nullable: false),
                        Semestre = c.Int(nullable: false),
                        Nombre = c.Int(nullable: false),
                        Code = c.String(maxLength: 4000),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Sections", t => t.SectionId, cascadeDelete: true)
                .Index(t => t.SectionId);
            
            CreateTable(
                "dbo.SalleClasses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SectionId = c.Int(),
                        GroupeId = c.Int(),
                        ClassRoomId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ClassRooms", t => t.ClassRoomId, cascadeDelete: true)
                .ForeignKey("dbo.Groupes", t => t.GroupeId)
                .ForeignKey("dbo.Sections", t => t.SectionId)
                .Index(t => t.SectionId)
                .Index(t => t.GroupeId)
                .Index(t => t.ClassRoomId);
            
            CreateTable(
                "dbo.Sections",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 4000),
                        Code = c.String(maxLength: 4000),
                        AnneeId = c.Int(nullable: false),
                        SpecialiteId = c.Int(nullable: false),
                        AnneeScolaireId = c.Int(nullable: false),
                        Semestre = c.Int(nullable: false),
                        Nombre = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Annees", t => t.AnneeId, cascadeDelete: true)
                .ForeignKey("dbo.AnneeScolaires", t => t.AnneeScolaireId, cascadeDelete: true)
                .ForeignKey("dbo.Specialites", t => t.SpecialiteId, cascadeDelete: true)
                .Index(t => t.AnneeId)
                .Index(t => t.SpecialiteId)
                .Index(t => t.AnneeScolaireId);
            
            CreateTable(
                "dbo.Lectures",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        TeacherId = c.Int(nullable: false),
                        ClassRoomId = c.Int(),
                        CourseId = c.Int(nullable: false),
                        Seance = c.Int(nullable: false),
                        AnneeId = c.Int(nullable: false),
                        SectionId = c.Int(nullable: false),
                        GroupeId = c.Int(),
                        FaculteId = c.Int(nullable: false),
                        DepartementId = c.Int(),
                        SpecialiteId = c.Int(nullable: false),
                        Solved = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Annees", t => t.AnneeId, cascadeDelete: true)
                .ForeignKey("dbo.ClassRooms", t => t.ClassRoomId)
                .ForeignKey("dbo.Courses", t => t.CourseId, cascadeDelete: false)
                .ForeignKey("dbo.Departements", t => t.DepartementId)
                .ForeignKey("dbo.Facultes", t => t.FaculteId, cascadeDelete: true)
                .ForeignKey("dbo.Groupes", t => t.GroupeId)
                .ForeignKey("dbo.Sections", t => t.SectionId, cascadeDelete: false)
                .ForeignKey("dbo.Specialites", t => t.SpecialiteId, cascadeDelete: false)
                .ForeignKey("dbo.Teachers", t => t.TeacherId, cascadeDelete: true)
                .Index(t => t.TeacherId)
                .Index(t => t.ClassRoomId)
                .Index(t => t.CourseId)
                .Index(t => t.AnneeId)
                .Index(t => t.SectionId)
                .Index(t => t.GroupeId)
                .Index(t => t.FaculteId)
                .Index(t => t.DepartementId)
                .Index(t => t.SpecialiteId);
            
            CreateTable(
                "dbo.Teachers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nom = c.String(maxLength: 4000),
                        Prenom = c.String(maxLength: 4000),
                        Numero = c.Int(nullable: false),
                        FaculteId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Facultes", t => t.FaculteId, cascadeDelete: false)
                .Index(t => t.FaculteId);
            
            CreateTable(
                "dbo.Seances",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AnneeScolaireId = c.Int(nullable: false),
                        Number = c.Int(nullable: false),
                        Day = c.Int(nullable: false),
                        TeacherId = c.Int(nullable: false),
                        Semestre = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AnneeScolaires", t => t.AnneeScolaireId, cascadeDelete: true)
                .ForeignKey("dbo.Teachers", t => t.TeacherId, cascadeDelete: true)
                .Index(t => t.AnneeScolaireId)
                .Index(t => t.TeacherId);
            
            CreateTable(
                "dbo.SeanceLbrSalles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AnneeScolaireId = c.Int(nullable: false),
                        Number = c.Int(nullable: false),
                        Day = c.Int(nullable: false),
                        SalleId = c.Int(nullable: false),
                        Semestre = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AnneeScolaires", t => t.AnneeScolaireId, cascadeDelete: true)
                .ForeignKey("dbo.ClassRooms", t => t.SalleId, cascadeDelete: true)
                .Index(t => t.AnneeScolaireId)
                .Index(t => t.SalleId);
            
            CreateTable(
                "dbo.Tcs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TeacherId = c.Int(nullable: false),
                        CourseId = c.Int(nullable: false),
                        ScheduleWieght = c.Int(nullable: false),
                        AnneeScolaireId = c.Int(nullable: false),
                        Semestre = c.Int(nullable: false),
                        ClassRoomTypeId = c.Int(nullable: false),
                        Periode = c.Int(nullable: false),
                        SectionId = c.Int(nullable: false),
                        GroupeId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AnneeScolaires", t => t.AnneeScolaireId, cascadeDelete: true)
                .ForeignKey("dbo.ClassRoomTypes", t => t.ClassRoomTypeId, cascadeDelete: true)
                .ForeignKey("dbo.Courses", t => t.CourseId, cascadeDelete: true)
                .ForeignKey("dbo.Groupes", t => t.GroupeId)
                .ForeignKey("dbo.Sections", t => t.SectionId, cascadeDelete: false)
                .ForeignKey("dbo.Teachers", t => t.TeacherId, cascadeDelete: true)
                .Index(t => t.TeacherId)
                .Index(t => t.CourseId)
                .Index(t => t.AnneeScolaireId)
                .Index(t => t.ClassRoomTypeId)
                .Index(t => t.SectionId)
                .Index(t => t.GroupeId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tcs", "TeacherId", "dbo.Teachers");
            DropForeignKey("dbo.Tcs", "SectionId", "dbo.Sections");
            DropForeignKey("dbo.Tcs", "GroupeId", "dbo.Groupes");
            DropForeignKey("dbo.Tcs", "CourseId", "dbo.Courses");
            DropForeignKey("dbo.Tcs", "ClassRoomTypeId", "dbo.ClassRoomTypes");
            DropForeignKey("dbo.Tcs", "AnneeScolaireId", "dbo.AnneeScolaires");
            DropForeignKey("dbo.SeanceLbrSalles", "SalleId", "dbo.ClassRooms");
            DropForeignKey("dbo.SeanceLbrSalles", "AnneeScolaireId", "dbo.AnneeScolaires");
            DropForeignKey("dbo.Lectures", "TeacherId", "dbo.Teachers");
            DropForeignKey("dbo.Seances", "TeacherId", "dbo.Teachers");
            DropForeignKey("dbo.Seances", "AnneeScolaireId", "dbo.AnneeScolaires");
            DropForeignKey("dbo.Teachers", "FaculteId", "dbo.Facultes");
            DropForeignKey("dbo.Lectures", "SpecialiteId", "dbo.Specialites");
            DropForeignKey("dbo.Lectures", "SectionId", "dbo.Sections");
            DropForeignKey("dbo.Lectures", "GroupeId", "dbo.Groupes");
            DropForeignKey("dbo.Lectures", "FaculteId", "dbo.Facultes");
            DropForeignKey("dbo.Lectures", "DepartementId", "dbo.Departements");
            DropForeignKey("dbo.Lectures", "CourseId", "dbo.Courses");
            DropForeignKey("dbo.Lectures", "ClassRoomId", "dbo.ClassRooms");
            DropForeignKey("dbo.Lectures", "AnneeId", "dbo.Annees");
            DropForeignKey("dbo.Sections", "SpecialiteId", "dbo.Specialites");
            DropForeignKey("dbo.SalleClasses", "SectionId", "dbo.Sections");
            DropForeignKey("dbo.Groupes", "SectionId", "dbo.Sections");
            DropForeignKey("dbo.Sections", "AnneeScolaireId", "dbo.AnneeScolaires");
            DropForeignKey("dbo.Sections", "AnneeId", "dbo.Annees");
            DropForeignKey("dbo.SalleClasses", "GroupeId", "dbo.Groupes");
            DropForeignKey("dbo.SalleClasses", "ClassRoomId", "dbo.ClassRooms");
            DropForeignKey("dbo.Courses", "SpecialiteId", "dbo.Specialites");
            DropForeignKey("dbo.Specialites", "NiveauId", "dbo.Niveaux");
            DropForeignKey("dbo.Specialites", "FilliereId", "dbo.Fillieres");
            DropForeignKey("dbo.Specialites", "FaculteId", "dbo.Facultes");
            DropForeignKey("dbo.Specialites", "DepartementId", "dbo.Departements");
            DropForeignKey("dbo.Departements", "FaculteId", "dbo.Facultes");
            DropForeignKey("dbo.Courses", "CourseTypeId", "dbo.CourseTypes");
            DropForeignKey("dbo.Courses", "AnneeId", "dbo.Annees");
            DropForeignKey("dbo.ClassSeances", "ClassRoomTypeId", "dbo.ClassRoomTypes");
            DropForeignKey("dbo.ClassSeances", "ClassRoomId", "dbo.ClassRooms");
            DropForeignKey("dbo.ClassSeances", "AnneeScolaireId", "dbo.AnneeScolaires");
            DropForeignKey("dbo.ClassRooms", "FaculteId", "dbo.Facultes");
            DropForeignKey("dbo.ClassRooms", "ClassRoomTypeId", "dbo.ClassRoomTypes");
            DropIndex("dbo.Tcs", new[] { "GroupeId" });
            DropIndex("dbo.Tcs", new[] { "SectionId" });
            DropIndex("dbo.Tcs", new[] { "ClassRoomTypeId" });
            DropIndex("dbo.Tcs", new[] { "AnneeScolaireId" });
            DropIndex("dbo.Tcs", new[] { "CourseId" });
            DropIndex("dbo.Tcs", new[] { "TeacherId" });
            DropIndex("dbo.SeanceLbrSalles", new[] { "SalleId" });
            DropIndex("dbo.SeanceLbrSalles", new[] { "AnneeScolaireId" });
            DropIndex("dbo.Seances", new[] { "TeacherId" });
            DropIndex("dbo.Seances", new[] { "AnneeScolaireId" });
            DropIndex("dbo.Teachers", new[] { "FaculteId" });
            DropIndex("dbo.Lectures", new[] { "SpecialiteId" });
            DropIndex("dbo.Lectures", new[] { "DepartementId" });
            DropIndex("dbo.Lectures", new[] { "FaculteId" });
            DropIndex("dbo.Lectures", new[] { "GroupeId" });
            DropIndex("dbo.Lectures", new[] { "SectionId" });
            DropIndex("dbo.Lectures", new[] { "AnneeId" });
            DropIndex("dbo.Lectures", new[] { "CourseId" });
            DropIndex("dbo.Lectures", new[] { "ClassRoomId" });
            DropIndex("dbo.Lectures", new[] { "TeacherId" });
            DropIndex("dbo.Sections", new[] { "AnneeScolaireId" });
            DropIndex("dbo.Sections", new[] { "SpecialiteId" });
            DropIndex("dbo.Sections", new[] { "AnneeId" });
            DropIndex("dbo.SalleClasses", new[] { "ClassRoomId" });
            DropIndex("dbo.SalleClasses", new[] { "GroupeId" });
            DropIndex("dbo.SalleClasses", new[] { "SectionId" });
            DropIndex("dbo.Groupes", new[] { "SectionId" });
            DropIndex("dbo.Departements", new[] { "FaculteId" });
            DropIndex("dbo.Specialites", new[] { "NiveauId" });
            DropIndex("dbo.Specialites", new[] { "FaculteId" });
            DropIndex("dbo.Specialites", new[] { "DepartementId" });
            DropIndex("dbo.Specialites", new[] { "FilliereId" });
            DropIndex("dbo.Courses", new[] { "AnneeId" });
            DropIndex("dbo.Courses", new[] { "SpecialiteId" });
            DropIndex("dbo.Courses", new[] { "CourseTypeId" });
            DropIndex("dbo.ClassSeances", new[] { "AnneeScolaireId" });
            DropIndex("dbo.ClassSeances", new[] { "ClassRoomTypeId" });
            DropIndex("dbo.ClassSeances", new[] { "ClassRoomId" });
            DropIndex("dbo.ClassRooms", new[] { "ClassRoomTypeId" });
            DropIndex("dbo.ClassRooms", new[] { "FaculteId" });
            DropTable("dbo.Tcs");
            DropTable("dbo.SeanceLbrSalles");
            DropTable("dbo.Seances");
            DropTable("dbo.Teachers");
            DropTable("dbo.Lectures");
            DropTable("dbo.Sections");
            DropTable("dbo.SalleClasses");
            DropTable("dbo.Groupes");
            DropTable("dbo.Niveaux");
            DropTable("dbo.Fillieres");
            DropTable("dbo.Departements");
            DropTable("dbo.Specialites");
            DropTable("dbo.CourseTypes");
            DropTable("dbo.Courses");
            DropTable("dbo.ClassSeances");
            DropTable("dbo.Facultes");
            DropTable("dbo.ClassRoomTypes");
            DropTable("dbo.ClassRooms");
            DropTable("dbo.AnneeScolaires");
            DropTable("dbo.Annees");
        }
    }
}
