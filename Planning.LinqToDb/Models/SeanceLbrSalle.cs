using LinqToDB.Mapping;

namespace Planning.LinqToDb.Models
{
    [Table("SeanceLbrSalles")]
    public class SeanceLbrSalle
    {
        [PrimaryKey, NotNull]
        [Column(IsIdentity = true, SkipOnInsert = true)]
        public int Id { get; set; }

        //[ForeignKey("AnneeScolaire")]
        [Column, NotNull]
        public int AnneeScolaireId { get; set; }
        [Column, NotNull]
        public int Number { get; set; }
        [Column, NotNull]
        public int Day { get; set; }
        [Column, NotNull]
        public int SalleId { get; set; }
        [Column, NotNull]
        public int Semestre { get; set; }
        [Association(ThisKey = "SalleId" , OtherKey = "Id")]
        public ClassRoom Salle { get; set; }
        [Association(ThisKey = "AnneeScolaireId", OtherKey = "Id")]
        public AnneeScolaire AnneeScolaire { get; set; }
    }
}
