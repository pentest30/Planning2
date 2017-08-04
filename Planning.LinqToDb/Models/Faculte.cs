using LinqToDB.Mapping;

namespace Planning.LinqToDb.Models
{
  [Table("Facultes")]
    public class Faculte
    {
    [PrimaryKey , NotNull]
        public int Id { get; set; }
    [Column, NotNull]
        public string Libelle { get; set; }

    }
}
