using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using DevExpress.Xpf.Grid;
using LinqToDB;
using Planing.UI.Helpers;
using Planning.LinqToDb.DbImport;
using Planning.LinqToDb.Models;

namespace Planing.Views
{
    /// <summary>
    /// Interaction logic for SalleView.xaml
    /// </summary>
    /// readonly Linq2DbModel _db = new Linq2DbModel();
    public partial class SalleView
    {
        readonly Linq2DbModel _db = new Linq2DbModel();

      public SalleView()
      {
        InitializeComponent();

      }

      private void BackButton_OnClick(object sender, RoutedEventArgs e)
        {
            AddButton.Visibility = Visibility.Visible;
            UpdateButton.Visibility = Visibility.Visible;
            DeleteButton.Visibility = Visibility.Visible;
            var binding = new Binding { ElementName = "DataGrid", Path = new PropertyPath("SelectedItem") };
            Grid.SetBinding(DataContextProperty, binding);
        }

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            var item = (ClassRoom)Grid.DataContext;
            if (item.Id <= 0)
            {
                _db.InsertWithIdentity(item);
                // ((ObservableCollection<Article>)DataGrid.ItemsSource).Add(item);
            }
            else
            {
                _db.Update(item);
            }
            try
            {
                
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "Erreurs pendant l'enregistrement", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                //((ObservableCollection<Article>)DataGrid.ItemsSource).Remove(item);
            }

            AddButton.Visibility = Visibility.Visible;
            DataGrid.ItemsSource = new ObservableCollection<ClassRoom>(_db.ClassRooms.ToList());
            var binding = new Binding { ElementName = "DataGrid", Path = new PropertyPath("SelectedItem") };
            Grid.SetBinding(DataContextProperty, binding);
            UpdateButton.Visibility = Visibility.Visible;
            DeleteButton.Visibility = Visibility.Visible;
        }

        private void UpdateButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedItem == null)
            {
                MessageBox.Show("Selectionner un champ", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            AddButton.Visibility = Visibility.Hidden;
            UpdateButton.Visibility = Visibility.Hidden;
            DeleteButton.Visibility = Visibility.Hidden;
        }

        private void AddButton_OnClick(object sender, RoutedEventArgs e)
        {
            AddButton.Visibility = Visibility.Hidden;
            var list = DataGrid.ItemsSource as List<ClassRoom>;
            if (list != null)
            {
                list.Add(new ClassRoom());
                Grid.DataContext = list.Last();
            }
        }

        private void DeleteButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedItem == null)
            {
                MessageBox.Show("Selectionner un champ", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var result = MessageBox.Show("Est vous sure!", "Warning", MessageBoxButton.YesNo,
                MessageBoxImage.Warning);
            if (!result.ToString().Equals("Yes")) return;
            var deleted = DataGrid.SelectedItem as ClassRoom;
            if (deleted == null) return;
           _db.Delete(deleted);
            
            DataGrid.ItemsSource = _db.ClassRooms.LoadWith(x=>x.Faculte).LoadWith(x=>x.ClassRoomType).ToList();
        }

        private void CbCategorie_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void ImportButton_OnClick(object sender, RoutedEventArgs e)
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
            var liste = DbAcceess.GetEnumerable<RoomXl>(cheminExcel, 0);

            if (liste != null)
            {
                var list = liste.ToList();
                var specialites = liste as IList<RoomXl> ?? list.ToList().Distinct();
                ProgressBar.Minimum = 0;
                var enumerable = specialites as RoomXl[] ?? specialites.Where(x =>!string.IsNullOrEmpty( x.Name)).ToArray();
                ProgressBar.Maximum = enumerable.Count();
                PBar pBar = new PBar(ProgressBar);
                var typeClasses = _db.ClassRoomTypes.ToList();
                foreach (var classRoom in enumerable)
                {
                    if (classRoom != null && !string.IsNullOrEmpty(classRoom.Name))
                    {
                        var item = new ClassRoom();
                        item.Name = classRoom.Name;
                        item.Code = classRoom.Code;
                        item.MaxSize = 0;
                        item.FaculteId = 1;
                        var firstOrDefault = typeClasses.LastOrDefault(x => classRoom.Name.Contains(x.Name));
                        if (firstOrDefault != null)
                            item.ClassRoomTypeId =
                                firstOrDefault.Id;
                        try
                        {
                            _db.InsertWithIdentity(item);
                            
                        }
                        catch (Exception)
                        {
                            continue;
                        }
                    }
                    pBar.IncPb();
                }
                GetDg();
            }
        }

        private void GetDg()
        {
            DataGrid.ItemsSource = _db.ClassRooms.LoadWith(x=>x.Faculte).LoadWith(x=>x.ClassRoomType).ToList();
        }

        private void LBonAddBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (CheckSelectedItem()) return;
            var classRoom = DataGrid.SelectedItem as ClassRoom;
            if (classRoom == null || classRoom.Id == 0) return;
            var frm = new HrLbrSalleView(classRoom.Id);
           // frm.UpdateDataDg += UpdateDg;
            frm.ShowDialog();
        }private bool CheckSelectedItem()
        {
            if (DataGrid.SelectedItem == null)
            {
                MessageBox.Show("Selectionner un champ", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return true;
            }
            return false;
        }

        private void UpdateDg(int id)
        {
            DataGridLignes.ItemsSource = _db.SeanceLbrSalles.LoadWith(x=>x.Salle).LoadWith(x=>x.AnneeScolaire).Where(x => x.SalleId == id).ToList();
        }

        private void DeleteBeLignesButton_OnClick(object sender, RoutedEventArgs e)
        {
            var item = DataGridLignes.SelectedItem as SeanceLbrSalle;
            if (item == null)
            {
                MessageBox.Show("Selectionner un champ!");
            }
            else
            {
                var result = MessageBox.Show("Est vous sure!", "Warning", MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);
                if (!result.ToString().Equals("Yes")) return;
                _db.Delete(item);
                
                DataGridLignes.ItemsSource =
                    _db.SeanceLbrSalles
                        .LoadWith(x=>x.Salle)
                        .LoadWith(x=>x.AnneeScolaire)
                        .Where(x => x.SalleId == item.SalleId)
                        .ToList();
            }
        }


      

      

        private void DataGrid_OnSelectedItemChanged(object sender, SelectedItemChangedEventArgs e)
        {
            try
            {
                var item = DataGrid.SelectedItem as ClassRoom;
                DataGridLignes.ItemsSource = _db.SeanceLbrSalles.
                    LoadWith(x=>x.Salle).
                    LoadWith(w=>w.AnneeScolaire)
                    .Where(x => x.SalleId == item.Id).ToList();
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch (Exception)
            {

                //throw;
            }
        }

      private void SalleView_OnLoaded(object sender, RoutedEventArgs e)
      {
        CbCategorie.ItemsSource = _db.ClassRoomTypes.ToList();
        CbSousCategorie.ItemsSource = _db.Facultes.ToList();
        //CbTypeCourse.ItemsSource = _db.CourseTypes.ToList();
        DataGrid.ItemsSource = _db.ClassRooms.LoadWith(x => x.Faculte).LoadWith(x => x.ClassRoomType).ToList();

      }
    }
}
