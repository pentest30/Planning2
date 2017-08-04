using System.Collections.Generic;
using LinqToDB.Mapping;

namespace Planning.LinqToDb.Models
{
  [Table("Teachers")]
  public class Teacher
  {
    public Teacher()
    {
      Seances = new List<Seance>();
    }

    [PrimaryKey, NotNull]
    [Column(IsIdentity = true, SkipOnInsert = true)]
    public int Id { get; set; }

    [Column, NotNull]
    // [DbColumn("Nom")]
    public string Nom { get; set; }

    [Column, NotNull]
    public string Prenom { get; set; }

    [Column, NotNull]
    public int Numero { get; set; }

    [Column, NotNull]
    public int FaculteId { get; set; }
    [Association(ThisKey = "FaculteId", OtherKey = "Id")]
    public Faculte Faculte { get; set; }
    [Association(ThisKey = "Id", OtherKey = "TeacherId")]
    public List<Seance> Seances { get; set; }

    public string FullName
    {
      get { return Nom + " " + Prenom; }
    }
  }
}
