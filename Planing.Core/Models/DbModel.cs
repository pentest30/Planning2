﻿using System.Data.Entity;
using Planing.Models;

namespace Planing.Core.Models
{
    public class DbModel : DbContext
    {
        public DbModel()
            : base("Name=PlaningDbContext")
        {

        }
        #region Tables

        public DbSet<ClassSeance> ClassSeances { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<ClassRoom> ClassRooms { get; set; }
        public DbSet<Specialite> Specialites { get; set; }
        public DbSet<Annee> Annees { get; set; }
        public DbSet<Faculte> Facultes { get; set; }
        public DbSet<Seance> Seances { get; set; }
        public DbSet<Tc> Tcs { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseType> CourseTypes { get; set; }
        public DbSet<ClassRoomType> ClassRoomTypes { get; set; }
        public DbSet<AnneeScolaire> AnneeScolaires { get; set; }
        public DbSet<Section> Sections { get; set; }
        public DbSet<Groupe> Groupes { get; set; }
        public DbSet<Niveau> Niveaus { get; set; }
        public DbSet<Filliere> Fillieres { get; set; }
        public DbSet<Departement> Departements { get; set; }
        public DbSet<Lecture> Lectures { get; set; }
        public DbSet<SeanceLbrSalle> SeanceLbrSalles { get; set; }
        public DbSet<SalleClasse> SalleClasses { get; set; }
        #endregion




    }
}
