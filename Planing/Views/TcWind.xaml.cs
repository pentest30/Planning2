using System.Collections.Generic;

using System.Linq;
using System.Windows;
using System.Windows.Controls;
using LinqToDB;
using Planning.LinqToDb.Models;


namespace Planing.Views
{
    /// <summary>
    /// Interaction logic for TcWind.xaml
    /// </summary>
    public partial class TcWind
    {
        readonly Linq2DbModel _db = new Linq2DbModel();
        public delegate void UpdateDg();
        public UpdateDg UpdateDataDg;
        private readonly int _semestre;
        private readonly int _anSc;

      public TcWind(Tc selectedItem, int fid, int anS, int s)
      {
        _semestre = s;
        _anSc = anS;
        InitializeComponent();

        Grid.DataContext = selectedItem;
        CbAnnee.ItemsSource = _db.Annees.ToList();
        //CbAnnee.SelectedIndex = -1;
        // CbCategorie.ItemsSource = _db.Specialites.Where(x => x.FaculteId == fid).ToList();
        CbOptions.ItemsSource = PeriodeOption();
        CbTypeCourse.ItemsSource = _db.ClassRoomTypes.ToList();
        CbEnseignant.ItemsSource = _db.Teachers.Where(x => x.FaculteId == fid).ToList();


      }

      public TcWind(int id, int fid, int anS, int s)
      {
        InitializeComponent();
        _semestre = s;
        _anSc = anS;
        CbAnnee.ItemsSource = _db.Annees.ToList();
        CbAnnee.SelectedIndex = -1;
        CbCategorie.ItemsSource = _db.Specialites.Where(x => x.FaculteId == fid).ToList();
        CbOptions.ItemsSource = PeriodeOption();
        CbTypeCourse.ItemsSource = _db.ClassRoomTypes.ToList();
        CbEnseignant.ItemsSource = _db.Teachers.Where(x => x.FaculteId == fid).ToList();
        Grid.DataContext =
          new Tc();

      }

      private Dictionary<int, string> PeriodeOption()
        {
            var dc = new Dictionary<int, string>();
            dc.Add(0, "Toute la journé");
            dc.Add(1, "1 ere demi journé");
            dc.Add(2, "2 eme demi journé");
            return dc;
        }

      private void CbCategorie_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
      {
        var annee = CbAnnee.SelectedItem as Annee;
         CbCours.SelectedIndex = -1;
        CbArticle.SelectedIndex = -1;
        var specialite = CbCategorie.SelectedItem as Specialite;
        if (annee != null && specialite != null)
        {
          CbCours.ItemsSource = _db.Courses
            .Where(x => x.AnneeId == annee.Id && x.SpecialiteId == specialite.Id && x.Semestre == _semestre).ToList();
          // CbCours.SelectionChanged += (o, args) => { };

          CbArticle.ItemsSource = _db.Sections.LoadWith(x=>x.AnneeScolaire).Where(x =>
            x.SpecialiteId == specialite.Id
            && x.Semestre == _semestre && x.AnneeScolaireId == _anSc
          ).ToList();
        }

      }

      private void CbArticle_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CbSousCategorie.SelectedIndex = -1;
            if (CbArticle.SelectedIndex != -1)
            {
                var item = CbArticle.SelectedItem as Section;
                CbSousCategorie.ItemsSource = _db.Groupes.Where(x => x.SectionId == item.Id).ToList();
            }
        }

      private void CbCours_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
      {
        //var item = CbCours.SelectedItem as Course;
       
      }

      private void CbAnnee_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CbAnnee.SelectedIndex != -1)
            {
              var annees = CbAnnee.ItemsSource as List<Annee>;
              if (annees != null)
              {
                var item = annees[CbAnnee.SelectedIndex];
                // var item2 = CbCategorie.SelectedItem as Specialite;
                if (item != null )
                {
                  CbCategorie.ItemsSource =
                    _db.Specialites.Where(x=>x.Name.Contains(item.Name.ToString())).ToList();
                  //CbCategorie.SelectionChanged += CbCategorie_OnSelectionChanged;

                }
                else
                {
                  MessageBox.Show("Selectionner l'année et la specialité");
                  //return;
                }
              }
            }
        }



      private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
      {
        var item = Grid.DataContext as Tc;
        //if (item1 != null)
        //{
        //  Tc item = new Tc()
        //  {
        //    Id = item1.Id,
        //    AnneeScolaireId = item1.AnneeScolaireId,
        //    ClassRoomTypeId = item1.ClassRoomTypeId,
        //    Periode = item1.Periode,
        //    TeacherId = item1.TeacherId,
        //    CourseId = item1.CourseId,
        //    SectionId = item1.SectionId,
        //    GroupeId = item1.GroupeId,
        //    ScheduleWieght = item1.ScheduleWieght,
        //    Semestre = item1.Semestre

        //  };
        //  if (item1 != null) item1.Id = 0;
        if (item != null && (item.AnneeScolaireId == 0 || item.ClassRoomTypeId == 0 || item.TeacherId == 0 ||
                             item.ClassRoomTypeId == 0 || item.SectionId == 0))
        {
          MessageBox.Show("NULL values not allowed");
          return;
        }
        var firstOrDefault = _db.Sections.FirstOrDefault(x => x.Id == item.SectionId);
          if (firstOrDefault != null)
          {
            if (item != null)
            {
              item.AnneeScolaireId = firstOrDefault.AnneeScolaireId;
              item.Semestre = firstOrDefault.Semestre;
            }
          }
          if (item != null && item.ScheduleWieght == 0)
          {
            MessageBox.Show("Nbr de seances doit etre superieur a zero");
            return;
          }

          if (item != null && item.Id == 0)
          {
            if (
              _db.Tcs.Any(
                x =>
                  x.AnneeScolaireId == item.AnneeScolaireId && x.CourseId == item.CourseId &&
                  x.TeacherId == item.TeacherId && x.Semestre == item.Semestre &&
                  x.SectionId == item.SectionId) && item.GroupeId == null)
            {
              MessageBox.Show("L'enregistrement existe deja dans la base de données ");
              return;
            }
            if (_db.Tcs.Any(
              x =>
                x.AnneeScolaireId == item.AnneeScolaireId && x.CourseId == item.CourseId &&
                x.TeacherId == item.TeacherId && x.Semestre == item.Semestre &&
                x.SectionId == item.SectionId && x.GroupeId == item.GroupeId))
            {
              MessageBox.Show("L'enregistrement existe deja dans la base de données ");
              return;
            }
            _db.InsertWithIdentity(item);

          }
          else
          {
            //    db.Tcs.Attach(item);
            _db.Update(item);
          }

        
        if (UpdateDataDg != null) UpdateDataDg();
      }
    }
}
