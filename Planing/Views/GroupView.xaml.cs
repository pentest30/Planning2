using System;
using System.Collections.Generic;

using System.Linq;
using System.Windows;
using System.Windows.Data;
using DevExpress.Xpf.Grid;
using LinqToDB;
using Planning.LinqToDb.Models;


namespace Planing.Views
{
    /// <summary>
    /// Interaction logic for GroupView.xaml
    /// </summary>
    public partial class GroupView
    {
        private readonly Linq2DbModel _db = new Linq2DbModel();

        public GroupView()
        {
            InitializeComponent();
            CbArticle.ItemsSource = _db.Sections.LoadWith(x=>x.Annee).LoadWith(x=>x.AnneeScolaire).LoadWith(x=>x.Specialite).ToList();
            UpdateDg();
        }

        private void UpdateDg()
        {
            DataGrid.ItemsSource = _db.Groupes.LoadWith(x=>x.Section).LoadWith(x=>x.Section.Specialite).ToList();
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
            var deleted = DataGrid.SelectedItem as Groupe;
            if (deleted == null) return;
           _db.Delete(deleted);
            
            UpdateDg();
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
            var item = (Groupe)Grid.DataContext;
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
            UpdateDg();
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
            var list = DataGrid.ItemsSource as List<Groupe>;
            if (list != null)
            {
                list.Add(new Groupe());
                Grid.DataContext = list.Last();
            }
        }

        private void AddGroupButton_OnClick(object sender, RoutedEventArgs e)
        {
            var frm = new AddGroupWind();
            frm.UpdateDataDg += UpdateDg;
            frm.ShowDialog();
        }
        private void GetDgSalle(int id)
        {
            DataGridSalle.ItemsSource = _db.SalleClasses.LoadWith(x => x.Section).LoadWith(x => x.Groupe).LoadWith(x => x.ClassRoom).LoadWith(x => x.ClassRoom.ClassRoomType).LoadWith(x=>x.Section).Where(x => x.GroupeId == id).ToList();
        }


        private void LBonAddBtn_OnClick(object sender, RoutedEventArgs e)
        {
            var item = DataGrid.SelectedItem as Groupe;
            if (item == null)
            {
                MessageBox.Show("Sélectioner un champ");
                return;
            }
            var frm = new AddClasseView(null,item);
            frm.UpdateDataDg += GetDgSalle;
            frm.Show();
        }

       

        private void DataGrid_OnSelectionChanged(object sender, GridSelectionChangedEventArgs e)
        {
            var item = DataGrid.SelectedItem as Groupe;
            if (item != null) GetDgSalle(item.Id);
        }
    }
}
