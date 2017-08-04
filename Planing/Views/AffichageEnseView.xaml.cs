using System;
using System.Collections.Generic;

using System.Linq;
using System.Windows;
using System.Windows.Controls;
using LinqToDB;
using Planing.Reporting;
using Planning.LinqToDb.DbImport;
using Planning.LinqToDb.Models;

namespace Planing.Views
{
    /// <summary>
    /// Interaction logic for AffichageEnseView.xaml
    /// </summary>
    public partial class AffichageEnseView
    {
        private readonly Linq2DbModel _db = new Linq2DbModel();

        public AffichageEnseView()
        {
            InitializeComponent();
            CbCategorie.ItemsSource = _db.Facultes.ToList();
            CbAs.ItemsSource = _db.AnneeScolaires.ToList();
        }

        private void CbAnnee_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = (Faculte)CbCategorie.SelectedItem;
            var asid = (AnneeScolaire)CbAs.SelectedItem;
            var s = Convert.ToInt32(SemestreTxt.Text);
            var an = (Teacher)CbEns.SelectedItem;
            try
            {
                var dictionary = Dictionary(item.Id, an.Id, s, asid.Id);
                dg.ItemsSource = dictionary;
            }
            catch (Exception)
            {

                //throw;
            }
            //var affichageDic = new Dictionary<string, Dictionary<string, List<string>>>();
            //var sections = _db.Sections.LoadWith(x=>x.Specialite").LoadWith(x=>x.Groupes").Where(x => x.AnneeScolaireId == asid.Id && x.Semestre == s&&x.Specialite.FaculteId ==item.Id);
            //foreach (var section in sections)
            //{
            //    if (section.Groupes.Count > 0)
            //    {
            //        foreach (var group in section.Groupes)
            //        {
            //            try
            //            {
            //                affichageDic.Add(section.Name + "\n" + group.Name, Dictionary(item.Id, section.Id, s, asid.Id, group.Id));
            //            }
            //            catch (Exception)
            //            {
                            
            //                continue;
            //            }
            //        }

            //    }
            //    else
            //    {
            //        affichageDic.Add(section.Name, Dictionary(item.Id, section.Id, s, asid.Id, null));
            //    }
            //}
            //dg.ItemsSource = affichageDic;
        }

        private void AddButton_OnClick(object sender, RoutedEventArgs e)
        {
            var fid = (CbCategorie.SelectedItem as Faculte);
            var asid = (CbAs.SelectedItem as AnneeScolaire);
        //    var secid = (CbArticle.SelectedItem as Specialite);
            var s = !string.IsNullOrEmpty(SemestreTxt.Text) ? Convert.ToInt32(SemestreTxt.Text) : 0;
            var teacher = (CbEns.SelectedItem as Teacher);
            //var sectionId = (CbSection.SelectedItem as Section);
            //var gId = (CbSousCategorie.SelectedItem as Groupe);
            var r = (Dictionary<string, List<string>>)dg.ItemsSource;

            if (asid == null) return;
            if (fid == null) return;
            try
            {
                System.Data.DataTable table =
                    DbAcceess.ToDictionary(r,  s,  fid.Libelle , teacher.Nom , asid.Name);
                var frmReport = new AffReportWind(table , ReportType.EnseignantReport);
                frmReport.Show();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }
        private Dictionary<string, List<string>> Dictionary(int fid,  int sectionId, int s, int asid)
        {
            var result =
                _db.Lectures.
                    LoadWith(x=>x.Teacher).
                    LoadWith(x=>x.Course).
                    LoadWith(x=>x.ClassRoom).
                    LoadWith(x=>x.Section).
                    LoadWith(x=>x.Groupe).
                    Where(x => x.FaculteId == fid
                               && x.TeacherId == sectionId
                               && x.Section.Semestre == s
                               && x.Section.AnneeScolaireId == asid).ToList();
            var dictionary = new Dictionary<string, List<string>>();
            foreach (var lecture in result)
            {
                var g = (lecture.Groupe != null) ? lecture.Groupe.Code + "\n" + lecture.Section.Code : lecture.Section.Code;
                lecture.Display = lecture.Course.Code +
                                  Environment.NewLine + lecture.ClassRoom.Code +
                                  Environment.NewLine + g;
            }
            //var q = result.Where(w => w.Seance <= 6);
            dictionary.Add("Samedi", ConstructList(result.Where(w => w.Seance <= 6).ToList(), 1));
            dictionary.Add("Dimanche", ConstructList(result.Where(w => w.Seance >= 7 && w.Seance < 13).ToList(), 7));
            dictionary.Add("Lundi", ConstructList(result.Where(w => w.Seance >= 13 && w.Seance < 19).ToList(), 13));
            dictionary.Add("Mardi", ConstructList(result.Where(w => w.Seance >= 19 && w.Seance < 25).ToList(), 19));
            dictionary.Add("Mercredi", ConstructList(result.Where(w => w.Seance >= 25 && w.Seance < 31).ToList(), 25));
            dictionary.Add("Jeudi", ConstructList(result.Where(w => w.Seance >= 31).ToList(), 31));
            return dictionary;
        }
        private List<String> ConstructList(List<Lecture> lectures, int j)
        {
            var result = new List<string>();
            for (int i = 0; i < 6; i++)
            {
                var item = lectures.FirstOrDefault(w => w.Seance == j);
                result.Add(item != null ? item.Display : "");
                j++;
            }
            return result;

        }
        private void CbCategorie_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CbCategorie.SelectedIndex != -1)
            {
                var item = (CbCategorie.SelectedItem as Faculte);
                CbEns.ItemsSource = _db.Teachers.Where(x => x.FaculteId == item.Id).ToList();
            }
        }
    }
}
