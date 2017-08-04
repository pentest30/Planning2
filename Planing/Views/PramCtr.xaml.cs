using System.Linq;
using System.Windows;
using LinqToDB;
using Planning.LinqToDb.Models;

namespace Planing.Views
{
    /// <summary>
    /// Interaction logic for PramCtr.xaml
    /// </summary>
    public partial class PramCtr
    {
        public PramCtr()
        {
            InitializeComponent();
          using (var db = new Linq2DbModel())
          {
            var item = db.Parameters.FirstOrDefault();
            if (item != null) Grid.DataContext = item;

            else
            {
              var param = new Parameter();
              param.Temprature = 20;
              param.TimeSpan = 100;
              param.SoftCourseSuccessingBonus = 10;
              param.SoftCourseSuccessingPenalty = -1;
              param.SoftLastPeriodPenalty = -1;
              param.SoftLastPeriodeBonus = 10;
              param.SoftUnAvailableRoomBonus = 10;
              param.SoftUnAvailableRoomPenalty = -1;
              param.SoftUnAvailableRoomPenalty = -1;
              param.SoftUnAvailableRoomBonus = 10;
              param.SoftUnAvailableTeacherBonus = 10;
              param.SoftUnAvailableTeacherPenalty = -1;
              Grid.DataContext = param;
            }
          }
            


        }

        private void BtnSave_OnClick(object sender, RoutedEventArgs e)
        {
            var parm = Grid.DataContext as Parameter;
            using (var db  = new Linq2DbModel())
            {
                if (parm != null && parm.Id == 0) db.InsertWithIdentity(parm);
                else db.Update(parm);
                MessageBox.Show("Ok");
            }
        }
    }
}
