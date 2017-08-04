using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using LinqToDB.Data;
using Planing.ModelView;
using Planing.PL.Generator;
using Planning.LinqToDb.Models;

namespace Planing.Views
{
    /// <summary>
    /// Interaction logic for GenePlan.xaml
    /// </summary>
    public partial class GenePlan
    {
        readonly Linq2DbModel _db = new Linq2DbModel();
        private ObservableCollection<LectureModelView> _bindingList;
        ObservableCollection<Lecture> _solution = new ObservableCollection<Lecture>();
        Thread t;
        public GenePlan()
        {
            InitializeComponent();
            CbCategorie.ItemsSource = _db.Facultes.ToList();
            CbAs.ItemsSource = _db.AnneeScolaires.ToList();
            //CbArticle.ItemsSource = _db.Specialites.ToList();
            //CbSection.ItemsSource =
            //    _db.Sections.ToList();
        }

      
        public SumilatedAnnealing s { get; set; }
        private void GenBtn_OnClick(object sender, RoutedEventArgs e)
        {
            var fid = (CbCategorie.SelectedItem as Faculte);
            var asid = (CbAs.SelectedItem as AnneeScolaire);
            //  var secid = (CbArticle.SelectedItem as Specialite).Id;

            if (fid==null || asid==null)return;

            var _s = (!string.IsNullOrEmpty(SemestreTxt.Text)) ? Convert.ToInt32(SemestreTxt.Text) : 0;
            var f = 0;
            var initial = PlaningGenerator.GeneratingPlanings(fid.Id, asid.Id, _s, ProgressBar, out f);
            s = new SumilatedAnnealing(initial, f);
            t = new Thread(() =>
            {


                s.UpdateDataDg += () =>
                {
                    var sx = new SchedulGenerator();
                    _bindingList = new ObservableCollection<LectureModelView>(sx.PopulateList(s.Solution));

                    Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate
                    {
                        Scheduler.Storage.RefreshData();
                        Scheduler.Storage.AppointmentStorage.DataSource = _bindingList;
                        var result = " softs: " + s.Fitness + " /hards: " + f;
                        TextBlock.Text =result;
                        TextBlock.Foreground = f == 0 ? Brushes.Green : Brushes.OrangeRed;

                    }));



                };
                _solution = s.HillClimbing(200);
                _solution = s.Run(20);
              //  
                Abort();
            });
            t.Start();
        }

        private void Abort()
        {
            var result = MessageBox.Show("la génération du planing est termninée! n voulez vous savegarder cette _solution ?",
               "Warning", MessageBoxButton.YesNo,
               MessageBoxImage.Warning);
            if (!result.ToString().Equals("Yes")) return;

            using (var db = new Linq2DbModel())
            {
                foreach (var lecture in _solution)
                {
                    if (lecture.GroupeId == 0) lecture.GroupeId = null;

                    var s = db.Sections.FirstOrDefault(x => x.Id == lecture.SectionId);
                    lecture.SpecialiteId = s.SpecialiteId;
                    lecture.AnneeId = s.AnneeId;

                }
                db.BulkCopy(_solution);
                //   db.Lectures.BulkCopy(_solution);

            }
            MessageBox.Show("Enregistrement terminé");
        }

        private void BtnStop_OnClick(object sender, RoutedEventArgs e)
        {
            if (t == null) return;
            t.Abort();
            s.Stop = true;
            _solution = s.Solution;
            Abort();
        }

        private void Scheduler_Drop(object sender, DragEventArgs e)
        {
            
        }


      private void GenePlan_OnLoaded(object sender, RoutedEventArgs e)
      {
        Application.Current.Dispatcher.Invoke(() =>
        {
          InitializeComponent();
          CbCategorie.ItemsSource = _db.Facultes.ToList();
          CbAs.ItemsSource = _db.AnneeScolaires.ToList();
        });
      }
    }
}
