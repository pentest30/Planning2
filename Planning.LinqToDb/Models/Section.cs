using System.Collections.Generic;
using LinqToDB.Mapping;
using Planning.LinqToDb.DbImport;

namespace Planning.LinqToDb.Models
{
  [Table("Sections")]
  public class Section
  {
    public Section()
    {
      Groupes = new List<Groupe>();
      SalleClasses = new List<SalleClasse>();
    }

    [PrimaryKey, NotNull]
    public int Id { get; set; }
    [DbColumn("Nom")]
    [Column, NotNull]
    public string Name { get; set; }
    [DbColumn("Abréviation")]
    [Column, NotNull]
    public string Code { get; set; }

    [Column, NotNull]
    public int AnneeId { get; set; }

    [Column, NotNull]
    public int SpecialiteId { get; set; }

    [Column, NotNull]
    public int AnneeScolaireId { get; set; }

    [Column, NotNull]
    public int Semestre { get; set; }

    [Column, NotNull]
    public int Nombre { get; set; }

    //public int? ClassRoomId { get; set; }
    //public ClassRoom ClassRoom { get; set; }
    public List<Groupe> Groupes { get; set; }

    [Association(ThisKey = "AnneeId", OtherKey = "Id")]
    public Annee Annee { get; set; }

    [Association(ThisKey = "SpecialiteId", OtherKey = "Id")]
    public Specialite Specialite { get; set; }
    [Association(ThisKey = "AnneeScolaireId", OtherKey = "Id")]
    public AnneeScolaire AnneeScolaire { get; set; }
    [Association(ThisKey = "Id", OtherKey = "SectionId")]
    public List<SalleClasse> SalleClasses { get; set; }
  }
}
