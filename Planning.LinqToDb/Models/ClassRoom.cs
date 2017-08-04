using System.Collections.Generic;
using LinqToDB.Mapping;
using Planning.LinqToDb.DbImport;

namespace Planning.LinqToDb.Models
{
    [Table("ClassRooms")]
    public class ClassRoom
    {
        public ClassRoom()
        {
            SeanceLbrSalles = new List<SeanceLbrSalle>();
        }

        [DbColumn("Id")]
        [PrimaryKey, NotNull]
        [Column(IsIdentity = true, SkipOnInsert = true)]
        public int Id { get; set; }

        [Column, NotNull]
        [DbColumn("Name")]
        public string Name { get; set; }

        [Column, NotNull]
        public string OptaCode { get; set; }

        [DbColumn("Code")]
        [Column, NotNull]
        public string Code { get; set; }

        public string Type { get; set; }

        [Column, NotNull]
        public int MinSize { get; set; }

        [Column, NotNull]
        public int MaxSize { get; set; }

        [Column, NotNull]
        public int FaculteId { get; set; }

        [Column, NotNull]
        // [DbColumn("ClassRoomTypeId")]
        public int ClassRoomTypeId { get; set; }

        [Association(ThisKey = "FaculteId", OtherKey = "Id")]
        public Faculte Faculte { get; set; }

        [Association(ThisKey = "ClassRoomTypeId", OtherKey = "Id")]
        public ClassRoomType ClassRoomType { get; set; }

        [Association(ThisKey = "Id", OtherKey = "ClassRoomId")]
        public List<SeanceLbrSalle> SeanceLbrSalles { get; set; }
    }
}
