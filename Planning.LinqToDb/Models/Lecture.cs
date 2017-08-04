using System;
using LinqToDB.Mapping;

namespace Planning.LinqToDb.Models
{
  [Table("Lectures")]
  public class Lecture
  {
    [PrimaryKey, NotNull]
    [Column(IsIdentity = true, SkipOnInsert = true)]
    public Int64 Id { get; set; }
    [Column, NotNull]
    public int TeacherId { get; set; }
    //[Column, NotNull]
    
    public int ClassRoomTypeId { get; set; }
   // [Column, NotNull]
    public int Periode { get; set; }
    [Column, Nullable]
    public int? ClassRoomId { get; set; }
    [Column, NotNull]
    public int CourseId { get; set; }
    [Column, NotNull]
    public int Seance { get; set; }
    [Column, NotNull]
    public int AnneeId { get; set; }
    [Column, NotNull]
    public int SectionId { get; set; }
    [Column, Nullable]
    public int? GroupeId { get; set; }
    [Column, Nullable]
    public int FaculteId { get; set; }
    [Column, Nullable]
    public int? DepartementId { get; set; }
    [Column, NotNull]
    public int SpecialiteId { get; set; }
    [Column, NotNull]
    public bool Solved { get; set; }

    [Association(ThisKey = "TeacherId", OtherKey = "Id")]
    public Teacher Teacher { get; set; }

    [Association(ThisKey = "AnneeId", OtherKey = "Id")]
    public Annee Annee { get; set; }

    [Association(ThisKey = "ClassRoomId", OtherKey = "Id")]
    public ClassRoom ClassRoom { get; set; }

    [Association(ThisKey = "FaculteId", OtherKey = "Id")]
    public Faculte Faculte { get; set; }
    [Association(ThisKey = "DepartementId", OtherKey = "Id")]
    public Departement Departement { get; set; }
    [Association(ThisKey = "SectionId", OtherKey = "Id")]
    public Section Section { get; set; }
    [Association(ThisKey = "GroupeId", OtherKey = "Id")]
    public Groupe Groupe { get; set; }
    [Association(ThisKey = "SpecialiteId", OtherKey = "Id")]
    public Specialite Specialite { get; set; }
    [Association(ThisKey = "CourseId", OtherKey = "Id")]
    public Course Course { get; set; }
   // [Column, NotNull]
    public string Display { get; set; }
    //[Column, NotNull]
    public string Jour { get; set; }
    //[Column, NotNull]
    public string Time { get; set; }

  }
}
