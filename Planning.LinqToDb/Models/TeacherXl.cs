using Planning.LinqToDb.DbImport;

namespace Planning.LinqToDb.Models
{
    public class TeacherXl
    {
         [DbColumn("Nom")]
        public string Nom { get; set; }
    }
}
