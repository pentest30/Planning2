
using System.Linq;
using System.Windows;
using LinqToDB;


using Planning.LinqToDb.Models;

namespace Planing.Views
{
    /// <summary>
    /// Interaction logic for DepartementView.xaml
    /// </summary>
    public partial class DepartementView
    {
        private readonly Linq2DbModel _db = new Linq2DbModel();
        public DepartementView()
        {
            InitializeComponent();
            CbArticle.ItemsSource = _db.Facultes.ToList();
            DataGrid.ItemsSource = _db.Departements.LoadWith(x=>x.Faculte).ToList();
        }

        private void DeleteButton_OnClick(object sender, RoutedEventArgs e)
        {
            
        }

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            
        }

        private void UpdateButton_OnClick(object sender, RoutedEventArgs e)
        {
            
        }

        private void AddButton_OnClick(object sender, RoutedEventArgs e)
        {
            
        }

        private void BackButton_OnClick(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
