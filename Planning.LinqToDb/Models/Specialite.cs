using LinqToDB.Mapping;

namespace Planning.LinqToDb.Models
{
  [Table("Specialites")]
  public class Specialite
  {
    [PrimaryKey, NotNull]
    public int Id { get; set; }

    [Column, NotNull]
    // [DbColumn("Classe")]
    public string Name { get; set; }

    [Column, NotNull]
    // [DbColumn("Abréviation")]
    public string Code { get; set; }

    [Column, Nullable]
    public int? FilliereId { get; set; }

    [Column, Nullable]
    //[ForeignKey("Departement")]
    public int? DepartementId { get; set; }

    [Column, NotNull]
    public int FaculteId { get; set; }

    [Column, NotNull]
    //public int AnneeId { get; set; }
    public int NiveauId { get; set; }

    [Association(ThisKey = "NiveauId" ,OtherKey = "Id")]
    public Niveau Niveau { get; set; }

    [Association(ThisKey = "DepartementId", OtherKey = "Id")]
    public Departement Departement { get; set; }

    [Association(ThisKey = "FilliereId", OtherKey = "Id")]
    public Filliere Filliere { get; set; }

    [Association(ThisKey = "FaculteId", OtherKey = "Id")]
    //public Annee Annee { get; set; }
    public Faculte Faculte { get; set; }
  }
}
