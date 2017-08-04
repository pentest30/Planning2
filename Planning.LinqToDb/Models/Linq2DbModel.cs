using LinqToDB;
using LinqToDB.Data;

namespace Planning.LinqToDb.Models
{
  public class Linq2DbModel : DataConnection
  {
    public Linq2DbModel(string configuration):base(configuration)
    {
     
    }
    public Linq2DbModel()
    {
      
    }

    public ITable<Specialite> Specialites { get { return GetTable<Specialite>(); } }

    public ITable<Groupe> Groupes
    {
      get { return GetTable<Groupe>(); }
    }

    public ITable<ClassRoomType> ClassRoomTypes { get { return GetTable<ClassRoomType>(); } }
    public ITable<Teacher> Teachers { get { return GetTable<Teacher>(); } }

    public ITable<Section> Sections { get { return GetTable<Section>(); } }
    public ITable<Tc> Tcs
    {
      get { return GetTable<Tc>(); }
    }

    public ITable<Annee> Annees
    {
      get { return GetTable<Annee>(); }
    }

    public ITable<AnneeScolaire> AnneeScolaires
    {
      get { return GetTable<AnneeScolaire>(); }
    }
    public ITable<Faculte> Facultes
    {
      get { return GetTable<Faculte>(); }
    }

    public ITable<Course> Courses { get { return GetTable<Course>(); } }
    public ITable<CourseType> CourseTypes { get { return GetTable<CourseType>(); } }
    public ITable<Lecture> Lectures { get { return GetTable<Lecture>(); } }
    public ITable<ClassSeance> ClassSeances { get { return GetTable<ClassSeance>(); } }
    public ITable<ClassRoom> ClassRooms{ get { return GetTable<ClassRoom>(); } }
    public ITable<Niveau> Niveaus { get { return GetTable<Niveau>(); } }
    public ITable<Filliere> Fillieres { get { return GetTable<Filliere>(); } }
    public ITable<Departement> Departements { get { return GetTable<Departement>(); } }
    public ITable<SalleClasse> SalleClasses { get { return GetTable<SalleClasse>(); } }
    public ITable<SeanceLbrSalle> SeanceLbrSalles { get { return GetTable<SeanceLbrSalle>(); } }
    public ITable<Seance> Seances { get { return GetTable<Seance>(); } }
      public ITable<Parameter> Parameters {
          get { return GetTable<Parameter>(); }
      }
  }
}
