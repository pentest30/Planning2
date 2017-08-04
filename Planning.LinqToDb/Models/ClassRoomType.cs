using LinqToDB.Mapping;

namespace Planning.LinqToDb.Models
{
  [Table("ClassRoomTypes")]
  public class ClassRoomType
  {
    [PrimaryKey, NotNull]
    public int Id { get; set; }

    [Column, NotNull]
    public string Name { get; set; }
  }
}
