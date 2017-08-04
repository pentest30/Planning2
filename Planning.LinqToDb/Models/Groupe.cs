using System.Collections.Generic;
using LinqToDB.Mapping;

namespace Planning.LinqToDb.Models
{
  [Table("Groupes")]
  public class Groupe
  {
    public Groupe()
    {
      SalleClasses = new List<SalleClasse>();
    }

    [PrimaryKey]
    [Column(IsIdentity = true, SkipOnInsert = true)]
    public int Id { get; set; }

    [Column, NotNull]
    public string Name { get; set; }

    [Column, NotNull]
    public int SectionId { get; set; }

    [Column, NotNull]
    public int Semestre { get; set; }

    [Column, NotNull]
    public int Nombre { get; set; }

    [Association(ThisKey = "SectionId", OtherKey = "Id")]
    public Section Section { get; set; }
    [Column, NotNull]
    public string Code { get; set; }
    [Association(ThisKey = "Id", OtherKey = "GroupeId")]
    public List<SalleClasse> SalleClasses { get; set; }
  }
}
