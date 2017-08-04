using LinqToDB.Mapping;
using Planning.LinqToDb.DbImport;

namespace Planning.LinqToDb.Models
{
  [Table("Tcs")]
  public class Tc
  {
    //[ReadOnly(true)]
    //[IgnoreMap]
    [PrimaryKey]
    [Column(IsIdentity = true, SkipOnInsert = true)]
    public int Id { get; set; }
    [DbColumn("TeacherId")]
    [Column, NotNull]
    
    public int TeacherId { get; set; }

    [DbColumn("CourseId")]
    [Column, NotNull]

    public int CourseId { get; set; }
    [DbColumn("ScheduleWieght")]

    [Column, NotNull]
    public int ScheduleWieght { get; set; }

    [Column, NotNull]
    public int AnneeScolaireId { get; set; }

    [Column, NotNull]

    public int Semestre { get; set; }

    [Column, NotNull]

    public int ClassRoomTypeId { get; set; }

    [Column, NotNull]

    public int Periode { get; set; }

    //    [IgnoreMap]
    [Association(ThisKey = "AnneeScolaireId", OtherKey = "Id")]
    public AnneeScolaire AnneeScolaire { get; set; }

    //  [IgnoreMap]
    [Association(ThisKey = "TeacherId", OtherKey = "Id")]
    public Teacher Teacher { get; set; }

    //  [IgnoreMap]
    [Association(ThisKey = "CourseId", OtherKey = "Id")]
    public Course Course { get; set; }

    [Column, NotNull]


    public int SectionId { get; set; }

    public int SeanceSum { get; set; }

    [Column, Nullable]


    public int? GroupeId { get; set; }

    //    [IgnoreMap]
    [Association(ThisKey = "SectionId", OtherKey = "Id")]
    public Section Section { get; set; }

    //  [IgnoreMap]
    [Association(ThisKey = "GroupeId", OtherKey = "Id")]
    public Groupe Groupe { get; set; }

    //   [IgnoreMap]
    [Association(ThisKey = "ClassRoomTypeId", OtherKey = "Id")]
    public ClassRoomType ClassRoomType { get; set; }
  }
}
