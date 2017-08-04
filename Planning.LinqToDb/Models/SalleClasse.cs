using LinqToDB.Mapping;

namespace Planning.LinqToDb.Models
{
  [Table("SalleClasses")]
  public class SalleClasse
  {
    [PrimaryKey, NotNull]
    public int Id { get; set; }

    [Column, Nullable]
    public int? SectionId { get; set; }

    [Column, Nullable]
    public int? GroupeId { get; set; }

    [Column, NotNull]
    public int ClassRoomId { get; set; }
    [Association(ThisKey = "SectionId" , OtherKey = "Id")]
    public Section Section { get; set; }
    [Association(ThisKey = "GroupeId", OtherKey = "Id")]
    public Groupe Groupe { get; set; }
    [Association(ThisKey = "ClassRoomId", OtherKey = "Id")]
    public ClassRoom ClassRoom { get; set; }
  }
}
