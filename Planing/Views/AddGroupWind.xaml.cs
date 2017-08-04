using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using LinqToDB;
using Planning.LinqToDb.Models;


namespace Planing.Views
{
  /// <summary>
  /// Interaction logic for AddGroupWind.xaml
  /// </summary>
  public partial class AddGroupWind
  {
    Linq2DbModel _db = new Linq2DbModel();

    public delegate void UpdateDg();

    public UpdateDg UpdateDataDg;

    public AddGroupWind()
    {
      InitializeComponent();
      Grid.DataContext = new Groupe();
      CbArticle.ItemsSource = _db.Sections.ToList();
    }

    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
      var item = Grid.DataContext as Groupe;
      var start = Convert.ToInt32(TxtStart.Text);
      var end = Convert.ToInt32(TxtEnd.Text);
      var rnd = new Random();
      if (end >= start)
      {
        for (int i = start; i <= end; i++)
        {
          if (item != null)
          {
            var firstOrDefault = _db.Sections.FirstOrDefault(x => x.Id == item.SectionId);
            if (firstOrDefault != null)
              item.Semestre = firstOrDefault.Semestre;
            item.Name = "Group " + i.ToString(CultureInfo.InvariantCulture);
            item.Code = "G" + i.ToString(CultureInfo.InvariantCulture);
            // item.Id = i;
            item.Nombre = rnd.Next(25, 40);
            _db.InsertWithIdentity(item);
           
          }
        }
        if (UpdateDataDg!= null)UpdateDataDg();

      }


    }
  }
}
