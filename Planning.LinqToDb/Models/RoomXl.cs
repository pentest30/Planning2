using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinqToDB.Mapping;
using Planning.LinqToDb.DbImport;

namespace Planning.LinqToDb.Models
{
    public class RoomXl
    {
        [DbColumn("Id")]
        //[PrimaryKey, NotNull]
        public int Id { get; set; }

        //[Column, NotNull]
         [DbColumn("Name")]
        public string Name { get; set; }

        [DbColumn("Code")]
        //[Column, NotNull]
        public string Code { get; set; }
    }

}
