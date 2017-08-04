using LinqToDB.Mapping;

namespace Planning.LinqToDb.Models
{
  [Table("AnneeScolaires")]
  public class AnneeScolaire
  {
    [PrimaryKey, NotNull]
    public int Id { get; set; }

    [Column, NotNull]
    public string Name { get; set; }
  }
}
