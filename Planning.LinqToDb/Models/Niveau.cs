using LinqToDB.Mapping;

namespace Planning.LinqToDb.Models
{
  [Table("Niveaux")]
  public class Niveau
  {
    [PrimaryKey, NotNull]
    public int Id { get; set; }

    [Column, NotNull]
    public string Libelle { get; set; }

  }
}
