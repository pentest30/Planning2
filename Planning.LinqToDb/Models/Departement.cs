using LinqToDB.Mapping;

namespace Planning.LinqToDb.Models
{
  [Table("Departements")]
  public class Departement
  {
    [PrimaryKey, NotNull]
    public int Id { get; set; }

    [Column, NotNull]
    public string Libelle { get; set; }

    [Column, NotNull]
    public int FaculteId { get; set; }

    [Association(ThisKey = "FaculteId", OtherKey = "Id")]
    public Faculte Faculte { get; set; }
  }
}
