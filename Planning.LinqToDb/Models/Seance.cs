using LinqToDB.Mapping;

namespace Planning.LinqToDb.Models
{
  [Table("Seances")]
  public class Seance
  {
    [PrimaryKey, NotNull]
    [Column(IsIdentity = true, SkipOnInsert = true)]
    public int Id { get; set; }

    [Column, NotNull]
    //[ForeignKey("AnneeScolaire")]
    public int AnneeScolaireId { get; set; }

    [Column, NotNull]
    public int Number { get; set; }

    [Column, NotNull]
    public int Day { get; set; }

    [Column, NotNull]
    public int TeacherId { get; set; }

    [Column, NotNull]
    public int Semestre { get; set; }

    [Association(ThisKey = "TeacherId", OtherKey = "Id")]
    public Teacher Teacher { get; set; }

    [Association(ThisKey = "AnneeScolaireId", OtherKey = "Id")]
    public AnneeScolaire AnneeScolaire { get; set; }

  }
}
