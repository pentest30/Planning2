using System;
using System.Collections.Generic;
using LinqToDB.Mapping;

namespace Planning.LinqToDb.Models
{
    [Table("ClassSeances")]
  public class ClassSeance
  {
    [PrimaryKey, NotNull]
    [Column(IsIdentity = true, SkipOnInsert = true)]
    public int Id { get; set; }

    [Column, NotNull]
    public int ClassRoomId { get; set; }

    [Association(ThisKey = "ClassRoomId", OtherKey = "Id")]
    public ClassRoom ClassRoom { get; set; }

    [Association(ThisKey = "ClassRoomTypeId", OtherKey = "Id")]
    public ClassRoomType ClassRoomType { get; set; }

    [Association(ThisKey = "AnneeScolaireId", OtherKey = "Id")]
    public AnneeScolaire AnneeScolaire { get; set; }

    [Column, NotNull]
    public int Seance { get; set; }
    public string TypeClass { get; set; }
    public int Min { get; set; }
    public int Max { get; set; }

    public string Jour { get; set; }

    public string Time { get; set; }

    [Column, NotNull]
    public int ClassRoomTypeId { get; set; }

    [Column, NotNull]
    public int Semestre { get; set; }

    [Column, NotNull]
    public int AnneeScolaireId { get; set; }


    public static List<ClassSeance> GenerateSeances(List<ClassRoom> classRooms, int anneeScolaire, int semestre)
    {
      var result = new List<ClassSeance>();
      // List<SeanceLbrSalle> ss;
      //    var db = new DbModel();

      foreach (var classRoom in classRooms)
      {

        for (int i = 1; i < 37; i++)
        {
          var item = new ClassSeance
          {
            ClassRoomId = classRoom.Id,
            TypeClass = classRoom.ClassRoomType.Name,
            ClassRoomTypeId = Convert.ToInt32(classRoom.ClassRoomTypeId),
            Seance = i,
            Min = classRoom.MinSize,
            Max = classRoom.MaxSize,
            AnneeScolaireId = anneeScolaire,
            Semestre = semestre
          };
          // if (item.TypeClass=="Amphi"&& item.Seance==36 ) continue;
          result.Add(item);
        }


      }
      //var items = new List<SeanceLbrSalle>();
      //foreach (var classSeance in result.Select(w => w.ClassRoomId).Distinct())
      //{
      //    int seance = classSeance;
      //    items .AddRange(
      //        db.SeanceLbrSalles.Where(
      //            w => w.SalleId == seance && w.AnneeScolaireId == anneeScolaire && w.Semestre == semestre)
      //            .ToList());

      //}
      //if (items.Count != 0)
      //{
      //    foreach (var classSeance in items)
      //    {
      //        var item = result.FirstOrDefault(w => w.ClassRoomId == classSeance.SalleId&&w.Seance ==classSeance.Number);
      //        result.Remove(item);
      //    }
      //}
      return result;
    }
  }
}