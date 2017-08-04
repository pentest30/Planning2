using LinqToDB.Mapping;

namespace Planning.LinqToDb.Models
{
    [Table("Parameters")]
    public class Parameter
    {

        [PrimaryKey, NotNull]
        [Column(IsIdentity = true, SkipOnInsert = true)]
        public int Id { get; set; }
        [Column, NotNull]
        public int Temprature { get; set; }
        [Column, NotNull]
        public int TimeSpan { get; set; }
        [Column, NotNull]
        public int SoftCourseSuccessingPenalty { get; set; }
        [Column, NotNull]
        public int SoftCourseSuccessingBonus { get; set; }
        [Column, NotNull]
        public int SoftLastPeriodPenalty { get; set; }
        [Column, NotNull]
        public int SoftLastPeriodeBonus { get; set; }
        [Column, NotNull]
        public int SoftUnAvailableTeacherPenalty { get; set; }
        [Column, NotNull]
        public int SoftUnAvailableTeacherBonus { get; set; }
        [Column, NotNull]
        public int SoftUnAvailableRoomPenalty { get; set; }
        [Column, NotNull]
        public int SoftUnAvailableRoomBonus { get; set; }


    }
}
