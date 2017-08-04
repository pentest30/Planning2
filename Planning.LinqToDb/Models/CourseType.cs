using LinqToDB.Mapping;

namespace Planning.LinqToDb.Models
{
  [Table("CourseTypes")]
    public class CourseType
    {
    [PrimaryKey, NotNull]
        public int Id { get; set; }
    [Column , NotNull]
        public string Name { get; set; }
    }
}
