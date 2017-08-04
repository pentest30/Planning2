using LinqToDB.Mapping;

namespace Planning.LinqToDb.Models
{
  [Table("Annees")]
    public class Annee
    {
    [PrimaryKey, NotNull]
        public int Id { get; set; }
    [Column , NotNull]
        public int Name { get; set; }
    }
}
