using Planning.LinqToDb.DbImport;

namespace Planning.LinqToDb.Models
{
   public class CourseXl
    {
       
        [DbColumn("Matière")]
        public string Name { get; set; }

       
        [DbColumn("Abréviation")]
        public string Code { get; set; }

    }

}
