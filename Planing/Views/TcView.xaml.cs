using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using LinqToDB;
using Planing.ModelView;
using Planing.UI.Helpers;
using Planning.LinqToDb.DbImport;
using Planning.LinqToDb.Models;


namespace Planing.Views
{
  /// <summary>
  /// Interaction logic for TcView.xaml
  /// </summary>
  public partial class TcView
  {
    readonly Linq2DbModel _db = new Linq2DbModel();
    private int _semestre;
    private int _anS;
    private int _fId;

    public TcView()
    {
      InitializeComponent();


    }



    private void DeleteButton_OnClick(object sender, RoutedEventArgs e)
    {
      if (DataGrid.SelectedItem == null)
      {
        MessageBox.Show("Selectionner un champ", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
        return;
      }
      var result = MessageBox.Show("Est vous sure!", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);
      if (!result.ToString().Equals("Yes")) return;

      if (DataGrid.SelectedItems.Count > 0)
      {
        ProgressBar.Maximum = 0;
        ProgressBar.Maximum = DataGrid.SelectedItems.Count;
        PBar pBar = new PBar(ProgressBar);
        var tcViewModels = DataGrid.SelectedItems.Cast<Tc>();
        foreach (var tcViewModel in tcViewModels)
        {
          //var deleted = DataGrid.SelectedItem as TcViewModel;
          if (tcViewModel == null) return;
          var converted = _db.Tcs.FirstOrDefault(x => x.Id == tcViewModel.Id);
          _db.Delete(converted);
          pBar.IncPb();
        }
        GetDg();
        ProgressBar.Minimum = 0;
      }

    }

    private void GetDg()
    {
      var query = (from t in _db.Tcs
          .LoadWith(x => x.AnneeScolaire)
          .LoadWith(x => x.Course)
          .LoadWith(x => x.Section)
          .LoadWith(x => x.Teacher)
          .LoadWith(x => x.ClassRoomType)
          .LoadWith(x => x.Groupe)
        where t.AnneeScolaireId == _anS
              && t.Semestre == _semestre
              && t.Section.Specialite.FaculteId == _fId
        select t);
      DataGrid.ItemsSource = query.ToList();


    }



    private void AddButton_OnClick(object sender, RoutedEventArgs e)
    {
      //AddButton.Visibility = Visibility.Hidden;
      //var list = DataGrid.ItemsSource.OfType<TcViewModel>().ToList();
      //list.Add(new TcViewModel());
      //Grid.DataContext =  AutoMapper.Mapper.Map<TcViewModel, Tc>(list.Last()); 
      var frm = new TcWind(0, _fId, _anS, _semestre);

      frm.Show();
      frm.UpdateDataDg += GetDg;
    }

    private void UpdateButton_OnClick(object sender, RoutedEventArgs e)
    {
      var item = DataGrid.SelectedItem as Tc;
      if (item == null)
      {
        MessageBox.Show("Selectionner un champ", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
        return;
      }

      var frm = new TcWind(item, _fId, _anS, _semestre);

      frm.Show();
      frm.UpdateDataDg += GetDg;

    }

    private void SaveButton_OnClick(object sender, RoutedEventArgs e)
    {

      var item1 = Grid.DataContext as TcViewModel;
      Tc item = AutoMapper.Mapper.Map<TcViewModel, Tc>(item1);

      if (item.ScheduleWieght <= 0)
      {

        MessageBox.Show("désigner le nombre de séances  ");
        return;
      }




      using (var db = new Linq2DbModel())
      {
        if (item.Id <= 0)
        {
          db.Insert(item);

        }
        else
        {

          db.Update(item);
        }
        try
        {

          GetDg();

        }
        catch (Exception ex)
        {

          MessageBox.Show(ex.Message, "Erreurs pendant l'enregistrement", MessageBoxButton.OK,
            MessageBoxImage.Error);
          //((ObservableCollection<Article>)DataGrid.ItemsSource).Remove(item);
        }
      }

      AddButton.Visibility = Visibility.Visible;

      var binding = new Binding {ElementName = "DataGrid", Path = new PropertyPath("SelectedItem")};
      Grid.SetBinding(DataContextProperty, binding);
      //item.Id = 0;
      UpdateButton.Visibility = Visibility.Visible;
      DeleteButton.Visibility = Visibility.Visible;

    }



    private void BackButton_OnClick(object sender, RoutedEventArgs e)
    {
      AddButton.Visibility = Visibility.Visible;
      UpdateButton.Visibility = Visibility.Visible;
      DeleteButton.Visibility = Visibility.Visible;
      var binding = new Binding {ElementName = "DataGrid", Path = new PropertyPath("SelectedItem")};
      Grid.SetBinding(DataContextProperty, binding);
    }

    private void DupliquButton_OnClick(object sender, RoutedEventArgs e)
    {
      var ofd = new Microsoft.Win32.OpenFileDialog();
      var result = ofd.ShowDialog();
      if (result == false) return;
      string cheminExcel = ofd.FileName;
      if (!cheminExcel.Split('\\').Last().Contains(".xlsx"))
      {
        MessageBox.Show("Le fichier que vous avez selectioné ce n'est un fichier Excel");
        return;
      }
      //  var liste = DbAcceess.GetPlaning(cheminExcel, 3);
      var t = DbAcceess.DataExcel(cheminExcel, 3);
      var dictionary = new Dictionary<string, List<TcViewModel>>();
      var current = "";
      foreach (DataRow row in t.Rows)
      {

        if (row["Classe"] != DBNull.Value)
        {
          dictionary.Add(row["Classe"].ToString(), new List<TcViewModel>());
          current = row["Classe"].ToString();
        }
        else
        {
          try
          {
            if (row["Matière"] == DBNull.Value) continue;
            var groupe = row["Groupe"].ToString().Contains(",")
              ? row["Groupe"].ToString().Split(',')[0]
              : row["Groupe"].ToString();
            var gr = Regex.Match(groupe, @"\d+").Value;

            var tc = new TcViewModel();
            tc.Section = current;
            tc.Teacher = (row["Professeur"].ToString().Contains(","))
              ? row["Professeur"].ToString().Split(',')[0]
              : row["Professeur"].ToString();
            tc.Course = row["Matière"].ToString();
            if (!row["Groupe"].ToString().Contains("Classe complète")) tc.Groupe = "G" + gr;
            tc.ScheduleWieght = Convert.ToInt32(row["Nombre par semaine"].ToString());
            if (!string.IsNullOrEmpty(row["Salles de classe"].ToString()))
              tc.ClassRoomtype = row["Salles de classe"].ToString().Contains(",")
                ? row["Salles de classe"].ToString().Split(',')[0]
                : row["Salles de classe"].ToString();
            else
            {
              tc.ClassRoomtype = row["Salles de classe disponibles"].ToString().Contains(",")
                ? row["Salles de classe disponibles"].ToString().Split(',')[0]
                : row["Salles de classe disponibles"].ToString();
            }
            dictionary[current].Add(tc);
          }
          catch (Exception exception)
          {
            continue;
          }

        }
      }
      ProgressBar.Minimum = 0;
      foreach (KeyValuePair<string, List<TcViewModel>> keyValuePair in dictionary)
      {

        var firstOrDefault = _db.Sections.FirstOrDefault(x => x.Name.Equals(keyValuePair.Key));
        if (firstOrDefault != null)
        {

          ProgressBar.Maximum = keyValuePair.Value.Count();
          PBar pBar = new PBar(ProgressBar);
          foreach (var tcViewModel in keyValuePair.Value)
          {
            var tc = new Tc();
            tc.SectionId = firstOrDefault.Id;
            tc.TeacherId = _db.Teachers.FirstOrDefault(x => x.Nom.Equals(tcViewModel.Teacher)).Id;
            tc.CourseId = _db.Courses.FirstOrDefault(x => x.Code.Equals(tcViewModel.Course)).Id;
            if (tcViewModel.Groupe != null)
              tc.GroupeId = _db.Groupes
                .FirstOrDefault(x => x.SectionId == firstOrDefault.Id && x.Code.Equals(tcViewModel.Groupe)).Id;
            tc.ScheduleWieght = tcViewModel.ScheduleWieght;
            tc.AnneeScolaireId = firstOrDefault.AnneeScolaireId;
            tc.Semestre = firstOrDefault.Semestre;

            try
            {
              tc.ClassRoomTypeId =
                _db.ClassRooms.FirstOrDefault(x => x.Code.Equals(tcViewModel.ClassRoomtype)).ClassRoomTypeId;

              _db.InsertWithIdentity(tc);
              pBar.IncPb();
            }
            catch (Exception)
            {
              pBar.IncPb();
              continue;
            }
          }

        }
      }
      //SaveButton.Click += SaveButton_OnClick;
    }

    private void SemestreTxt_OnSelectionChanged(object sender, RoutedEventArgs e)
    {
      CbAs.SelectedIndex = -1;
    }

    private void CbAs_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      try
      {
        var f = CbFacutlte.SelectedItem as Faculte;
        var a = CbAs.SelectedItem as AnneeScolaire;
        _semestre = (SemestreTxt.Text != "") ? Convert.ToInt32(SemestreTxt.Text) : 0;
        if (f != null) _fId = f.Id;
        if (a != null) _anS = a.Id;
        Application.Current.Dispatcher.Invoke(() =>
        {
          Mouse.OverrideCursor = Cursors.Wait;

          GetDg();
          Mouse.OverrideCursor = null;
        });
      }
      catch (Exception exception)
      {
        MessageBox.Show(exception.Message);
        //  throw;
      }

    }


    private void BenchmarkBtn_OnClick(object sender, RoutedEventArgs e)
    {

      using (var db = new Linq2DbModel())
      {
        int j = 1;
        foreach (var course in db.Courses.ToList())
        {
          course.OptaCode = ("c" + j.ToString().PadLeft(4, '0')).Replace(" ", "");
          db.Update(course);
          j++;
        }
        j = 1;
        foreach (var teacher in db.Teachers.ToList())
        {
          teacher.Prenom = ("t" + j.ToString().PadLeft(4, '0')).Replace(" ", "");
          db.Update(teacher);
          j++;
        }
        j = 0;
        foreach (var classRoom in db.ClassRooms.ToList())
        {
          var code = ("R" + j.ToString().PadLeft(2, '0')).Replace(" ", "");
          db.ClassRooms.Where(x => x.Id == classRoom.Id).Set(x => x.OptaCode, code).Update();

          j++;
        }
        OptaPlannerBenhmarkgenerator(db);
        MessageBox.Show("benchmark generated");
      }
    }

    private void OptaPlannerBenhmarkgenerator(Linq2DbModel db)
    {
      //var f = CbFacutlte.SelectedItem as Faculte;
      var a = CbAs.SelectedItem as AnneeScolaire;
      _semestre = (SemestreTxt.Text != "") ? Convert.ToInt32(SemestreTxt.Text) : 0;
      var benchmarck = new StringBuilder();
      var rnd = new Random();
      var curricula = (from s in db.Courses
        orderby s.SpecialiteId
        where s.Semestre == _semestre
        select s).ToList();
      var rooms = (from r in db.ClassRooms select r).ToList();
      var courses = (from c in db.Tcs
        join teacher in db.Teachers on c.TeacherId equals teacher.Id
        join m in db.Courses on c.CourseId equals m.Id
        //  join classRoom in db. on EXPR1 equals EXPR2
        let nombreEtudiant = rnd.Next(25, 100)
        let minDays = 2
        let doubleLecture = 0
        let techerName = teacher.Prenom
        where c.Semestre == _semestre && c.AnneeScolaireId == a.Id
        select new
        {
          m.OptaCode,
          techerName,
          c.ScheduleWieght,
          nombreEtudiant,
          minDays,
          doubleLecture,
          c.ClassRoomTypeId
        });
      var benchConfig = @"Name: univ_timeTable" + Environment.NewLine +
                        "Courses: " + courses.Count() + Environment.NewLine +
                        "Rooms: " + db.ClassRooms.Count() + Environment.NewLine +
                        "Days: 6" + Environment.NewLine +
                        "Periods_per_day: 6" + Environment.NewLine +
                        "Curricula:" + curricula.GroupBy(x => x.SpecialiteId).Count() + Environment.NewLine +
                        "Constraints: 0" + Environment.NewLine;
      //  +"RoomConstraints: 9990" + Environment.NewLine;
      benchmarck.Append(benchConfig + Environment.NewLine);
      Debug.WriteLine(courses.ToString());
      int k = 0;
      var curriculums = "CURRICULA:" + Environment.NewLine;
      var courseList = Environment.NewLine + "COURSES:" + Environment.NewLine;
      var roomList = Environment.NewLine + "ROOMS:" + Environment.NewLine;
      foreach (var course in courses)
      {
        //var r = courses.Count(x => x.Code == course.Code && x.techerName == course.techerName);
        courseList = string.Format("{0}{1} {2} {3} {4} {5} {6}\n",
          courseList,
          course.OptaCode,
          course.techerName.Replace(" ", ""),
          course.ScheduleWieght * 1,
          course.minDays,
          course.nombreEtudiant,
          course.ClassRoomTypeId
        );
      }
      benchmarck.Append(courseList.TrimStart());
      foreach (var course in curricula.GroupBy(c => c.SpecialiteId))
      {
        var body = "";
        int count = 0;
        foreach (var course1 in course)
        {
          if (courses.Any(x => x.OptaCode == course1.OptaCode))
          {
            body = string.Format("{0} {1}  ", body, course1.OptaCode.Trim());
            count++;
          }
        }
        curriculums = curriculums + ("q" + k.ToString().PadLeft(2, '0')).Replace(" ", "") + " " + count + body +
                      Environment.NewLine;

        k++;
      }

      k = 0;
      foreach (var item in rooms)
      {
        roomList = string.Format("{0}{1} {2} {3} {4}", roomList, item.OptaCode, item.MaxSize, item.ClassRoomTypeId,
          Environment.NewLine);
        k++;
      }

      benchmarck.Append(roomList + Environment.NewLine);
      benchmarck.Append(curriculums + Environment.NewLine);
      benchmarck.Append("UNAVAILABILITY_CONSTRAINTS:\n" + Environment.NewLine);
      //  benchmarck.Append(roomConstaraints + Environment.NewLine);
      benchmarck.Append("END.");
      string path = Environment.CurrentDirectory + @"\comp_univ.ctt";
      using (StreamWriter sw = new StreamWriter(path))
      {
        sw.WriteLine(benchmarck);
      }
    }

    private void SolBtn_OnClick(object sender, RoutedEventArgs e)
    {
      var ofd = new Microsoft.Win32.OpenFileDialog();
      var result = ofd.ShowDialog();
      if (result == false) return;
      string cheminExcel = ofd.FileName;
      if (!cheminExcel.Split('\\').Last().Contains(".sol"))
      {
        MessageBox.Show("Le fichier que vous avez selectioné  n'est pas un fichier Opta planner");
        return;
      }
      string[] lines = System.IO.File.ReadAllLines(cheminExcel);
      var list = new List<OptaSolution>();
      var tcs = _db.Tcs.LoadWith(x => x.Course).LoadWith(x => x.Section).ToList();
      var rooms = _db.ClassRooms.ToList();
      foreach (var line in lines)
      {
        var l = new OptaSolution();
        l.CourseCode = line.Split(' ')[0];
        l.RoomCode = line.Split(' ')[1].Replace("r", "");
        l.Day = Int32.Parse(line.Split(' ')[2]);
        l.Periode = Int32.Parse(line.Split(' ')[3]);
        list.Add(l);
      }
      ProgressBar.Minimum = 0;
      ProgressBar.Maximum = list.Count();
      PBar pBar = new PBar(ProgressBar);

      foreach (var optaSolution in list)
      {
        var r = rooms.FirstOrDefault(x => x.OptaCode == optaSolution.RoomCode);
        var tc =
          tcs.FirstOrDefault(
            x => x.Course.OptaCode == optaSolution.CourseCode && x.ClassRoomTypeId == r.ClassRoomTypeId);
        var pl = new Lecture();
        pl.TeacherId = tc.TeacherId;
        pl.CourseId = tc.CourseId;
        pl.ClassRoomId = r.Id;
        pl.ClassRoomTypeId = r.ClassRoomTypeId;
        pl.Seance = optaSolution.Periode + (optaSolution.Day * 6) + 1;
        pl.SectionId = Convert.ToInt32(tc.SectionId);
        pl.GroupeId = tc.GroupeId;
        pl.SpecialiteId = tc.Section.SpecialiteId;
        pl.FaculteId = 1;
        pl.Groupe = tc.Groupe;
        pl.AnneeId = tc.Section.AnneeId;
        pl.Periode = tc.Periode;
        pl.Teacher = tc.Teacher;
        _db.InsertWithIdentity(pl);
        pBar.IncPb();
        //pl.Teacher.Seances.AddRange(tc.Teacher.Seances);
      }

    }

    private void TcView_OnLoaded(object sender, RoutedEventArgs e)
    {
      Application.Current.Dispatcher.Invoke(() =>
      {
        CbFacutlte.ItemsSource = _db.Facultes.ToList();
        CbAs.ItemsSource = _db.AnneeScolaires.ToList();
      });
       
    }
  }
}

