using System;
using System.Collections.Generic;

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
    /// Interaction logic for SectionView.xaml
    /// </summary>
    public partial class SectionView
    {
        readonly Linq2DbModel _db = new Linq2DbModel();

      public SectionView()
      {
        InitializeComponent();
        CbCategorie.ItemsSource = _db.Specialites.ToList();
        CbSousCategorie.ItemsSource = _db.Annees.ToList();
        CbAnneeScolaire.ItemsSource = _db.AnneeScolaires.ToList();
        DataGrid.ItemsSource = _db.Sections.LoadWith(x => x.Specialite).LoadWith(x => x.Annee)
          .LoadWith(x => x.AnneeScolaire).ToList();
      }

      private void CbCategorie_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //var item = CbCategorie.SelectedItem as Specialite;
            //CbEns.ItemsSource = _db.ClassRooms.Where(c=>c.FaculteId == item.FaculteId).ToList();
        }

        private void AddButton_OnClick(object sender, RoutedEventArgs e)
        {
            AddButton.Visibility = Visibility.Hidden;
            var list = DataGrid.ItemsSource as List<Section>;
            if (list != null)
            {
                list.Add(new Section());
                Grid.DataContext = list.Last();
            }
        }

        private void UpdateButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedItem== null)
            {
                MessageBox.Show("Selectionner un champ", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            AddButton.Visibility = Visibility.Hidden;
            UpdateButton.Visibility = Visibility.Hidden;
            DeleteButton.Visibility = Visibility.Hidden;
        }

        private void BackButton_OnClick(object sender, RoutedEventArgs e)
        {
            AddButton.Visibility = Visibility.Visible;
            UpdateButton.Visibility = Visibility.Visible;
            DeleteButton.Visibility = Visibility.Visible;
            var binding = new Binding {ElementName = "DataGrid", Path = new PropertyPath("SelectedItem")};
            Grid.SetBinding(DataContextProperty, binding);
            
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
        var deleted = DataGrid.SelectedItem as Specialite;
        if (deleted == null) return;
        _db.Delete(deleted);

        GetDg();
      }

      private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            var item = (Section)Grid.DataContext;
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
            DataGrid.ItemsSource = _db.Sections.LoadWith(x=>x.Specialite).LoadWith(x=>x.Annee).LoadWith(x=>x.AnneeScolaire).ToList();
            var binding = new Binding { ElementName = "DataGrid", Path = new PropertyPath("SelectedItem") };
            Grid.SetBinding(DataContextProperty, binding);
            UpdateButton.Visibility = Visibility.Visible;
            DeleteButton.Visibility = Visibility.Visible;
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
            var liste = DbAcceess.GetSpecialites(cheminExcel, 0);

            if (liste != null)
            {
                var specialites = liste as IList<Section> ?? liste.ToList().Distinct();
                ProgressBar.Minimum = 0;
                var enumerable = specialites as Section[] ?? specialites.Where(x=>!string.IsNullOrEmpty(x.Name)).ToArray();
                ProgressBar.Maximum = enumerable.Count();
                PBar pBar = new PBar(ProgressBar);
              var ls = _db.Specialites.ToList();
              int k = 1;

                foreach (var specialite in enumerable)
                {
                 
                    
                    if (specialite != null && !string.IsNullOrEmpty(specialite.Name))
                    {
                        var item = new Section();
                      item.Id = k;
                      k++;
                       item.Semestre = 1;
                        item.Code = specialite.Code;
                        var n = specialite.Name.Split(' ')[0];
                        item.Name = specialite.Name;
                        Specialite firstOrDefault = ls.FirstOrDefault(x => specialite.Name.Contains(x.Name));
                        item.AnneeScolaireId = 1;
                        if (firstOrDefault != null) item.SpecialiteId= firstOrDefault.Id;
                        switch (n)
                        {
                            case "L1":
                                item.AnneeId = 1;break;
                            case "L2":
                                item.AnneeId = 2; break;
                            case "L3":
                                item.AnneeId = 3; break;
                            case "M1":
                                item.AnneeId = 1; break;
                            case "M2":
                                item.AnneeId = 2; break;

                        }
                      try
                      {
                        _db.InsertWithIdentity(item);
                        
                        pBar.IncPb();

            }
                      catch (Exception exception)
                      {
                        pBar.IncPb();
                        continue;
                      }
                      

                    }
                    
                }
                GetDg();
            }
        }

      private void GetDg()
      {
        DataGrid.ItemsSource = _db.Sections.LoadWith(x => x.Specialite).LoadWith(x => x.Annee)
          .LoadWith(x => x.AnneeScolaire).ToList();
      }

      private void GetDgSalle(int id)
        {
            DataGridSalle.ItemsSource = _db.SalleClasses.LoadWith(x=>x.Section).LoadWith(x=>x.ClassRoom).LoadWith(x=> x.ClassRoom.ClassRoomType).Where(x => x.SectionId == id).ToList();
        }


        private void LBonAddBtn_OnClick(object sender, RoutedEventArgs e)
        {
            var item = DataGrid.SelectedItem as Section;
            if (item == null)
            {
                MessageBox.Show("Sélectioner un champ");
                return;
            }
            var frm = new AddClasseView(item,null);
            frm.UpdateDataDg += GetDgSalle;
            frm.Show();
        }

       

        private void DataGrid_OnSelectedItemChanged(object sender, SelectedItemChangedEventArgs e)
        {
            var item = DataGrid.SelectedItem as Section;
            if (item != null) GetDgSalle(item.Id);
        }
    }
}
