using LinqToDB.Mapping;

namespace Planning.LinqToDb.Models
{
  [Table("Fillieres")]


  public class Filliere
  {
    [PrimaryKey, NotNull]
    public int Id { get; set; }

    [Column, NotNull]
    public string Libelle { get; set; }

  }
}
