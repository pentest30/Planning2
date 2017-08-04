using LinqToDB.Mapping;
using Planning.LinqToDb.DbImport;

namespace Planning.LinqToDb.Models
{
  [Table("Courses")]
  public class Course
  {
      [PrimaryKey, NotNull]
      [Column(IsIdentity = true, SkipOnInsert = true)]
    public int Id { get; set; }
    
    [Column, NotNull]

    [DbColumn("Matière")]
    public string Name { get; set; }
    
    [Column, NotNull]
    [DbColumn("Abréviation")]
    public string Code { get; set; }

    [Column, NotNull]
    public string OptaCode { get; set; }

    [Column, Nullable]
    public int? CourseTypeId { get; set; }

    [Column, NotNull]
    public int SpecialiteId { get; set; }

    [Column, NotNull]
    public int AnneeId { get; set; }

    [Column, NotNull]
    public int Semestre { get; set; }
    [Association(ThisKey = "AnneeId", OtherKey = "Id")]
    public Annee Annee { get; set; }
    [Association(ThisKey = "SpecialiteId", OtherKey = "Id")]
    public Specialite Specialite { get; set; }
    [Association(ThisKey = "CourseTypeId", OtherKey = "Id")]
    public CourseType CourseType { get; set; }

    [Column, NotNull]
    public int Periode { get; set; }

  }

  public enum Periode
    {
        FullDay,
        FirstHalf,
        LastHalf
    };
}
