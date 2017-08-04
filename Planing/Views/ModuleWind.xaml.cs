using System.Linq;
using System.Windows;
using System.Windows.Controls;
using LinqToDB;

using Planning.LinqToDb.Models;

namespace Planing.Views
{
    /// <summary>
    /// Interaction logic for ModuleWind.xaml
    /// </summary>
    public partial class ModuleWind
    {
        readonly Linq2DbModel _db = new Linq2DbModel();
        public delegate void UpdateDg();
        public AddGroupWind.UpdateDg UpdateDataDg;
        public ModuleWind()
        {
            InitializeComponent();
            CbCategorie.ItemsSource = _db.Specialites.ToList();
            CbSousCategorie.ItemsSource = _db.Annees.ToList();
            CbTypeCourse.ItemsSource = _db.CourseTypes.ToList();
            //GetDg();
            Grid.DataContext = new Course();
        }

        private void CbCategorie_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var item = (Course) Grid.DataContext;
            _db.InsertWithIdentity(item);
            
            if (UpdateDataDg != null && item != null) UpdateDataDg();
        }
    }
}
