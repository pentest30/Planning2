using Planning.LinqToDb.DbImport;

namespace Planning.LinqToDb.Models
{
    public class Module
    {
        public string Nom { get; set; }
        [DbColumn("Matière")]
        public string Code { get; set; }
        [DbColumn("Classe")]
        public string Specialite { get; set; }
    }
}
