using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Controls;
using LinqToDB;
using Planing.ModelView;
using Planing.UI.Helpers;
using Planning.LinqToDb.Models;


namespace Planing.PL.Generator
{
    public static class PlaningGenerator
    {
        #region Properties
        public delegate void UpdateDg(string mesage);
        public delegate void UpdateStatusEvt(int fitness, TimeSpan span );

        public static UpdateStatusEvt EveStatusEvt;
        public static UpdateDg UpdateDataDg;
        private static Linq2DbModel _db;
        private static List<ClassSeance> _classSeances = new List<ClassSeance>();
        private static List<TeacherSeance> _techerSeances = new List<TeacherSeance>();
        private  static  List<Seance> _teacherUnavailablePeriods = new List<Seance>(); 
        private  static  List<ClassRoom> _rooms = new List<ClassRoom>(); 
     private static List<SeanceLbrSalle> _roomsUnavailablePeriods = new List<SeanceLbrSalle>(); 
        private static List<Lecture> _planing;
        public static string Status = "";
        private static List<Lecture> _noSolutions;
        private static readonly List<List<ClassSeance>> ListOfClaassLecture = new List<List<ClassSeance>>();
        private static List<Tc> _tcs;
          #endregion
        #region Functions
        private static List<int> GetListTechersIds(int fId)
        {
            using (_db = new Linq2DbModel())
            {
               
                return _db.Teachers.Where(x=>x.FaculteId == fId).Select(x => x.Id).ToList();

            }
        }

        public static IEnumerable<ClassSeance> GenerateClassSeances(int fId, int anneeScoliare, int semestre)
        {
            using (_db = new Linq2DbModel())
            {
                var classes = _db.ClassRooms.
                    LoadWith(x=>x.ClassRoomType)
                    .Where(x => x.FaculteId == fId).
                    ToList();
                return ClassSeance.GenerateSeances(classes, anneeScoliare, semestre);
            }
        }

        public static List<Lecture> GeneratingPlanings(int fid, int semestre, int anneeScolaire,
            ProgressBar progressBar, out int fitness)
        {
           
            _noSolutions = new List<Lecture>();
            var teacherIds = GetListTechersIds(fid);
            _planing = new List<Lecture>();
            _techerSeances = TeacherSeance.GenerateTeacherSeances(teacherIds, semestre, anneeScolaire);
            _classSeances = GenerateClassSeances(fid, anneeScolaire, semestre).ToList();
            progressBar.Minimum = 0;
            progressBar.Maximum = teacherIds.Count();
            var pbar = new PBar(progressBar);
            using (_db = new Linq2DbModel())
            {
                _rooms = _db.ClassRooms.Where(x => x.FaculteId == fid).ToList();
                _teacherUnavailablePeriods = _db.Seances.Where(x => x.AnneeScolaireId == anneeScolaire&&x.Semestre == semestre).ToList();
                _roomsUnavailablePeriods = _db.SeanceLbrSalles.Where(x => x.AnneeScolaireId == anneeScolaire && x.Semestre == semestre).ToList();
                _tcs = _db.Tcs.LoadWith(x => x.Section)
                    .LoadWith(x => x.Section.Specialite)
                    .LoadWith(x => x.Section.Annee)
                    .LoadWith(x => x.Groupe)
                    .LoadWith(x => x.Course)
                    .LoadWith(x => x.ClassRoomType)
                    .LoadWith(x => x.Teacher)
                    .OrderBy(x => x.ClassRoomTypeId)
                    .ThenByDescending(x => x.Periode)
                    .ThenBy(x => x.GroupeId)
                    .Where(
                        x => x.Section.Specialite.FaculteId == fid &&
                             x.Semestre == semestre
                             && x.Section.AnneeScolaireId == anneeScolaire).ToList();
            }
            progressBar.Maximum = _tcs.Count;
            foreach (var tc in _tcs.ToList())
            {
                var firstOrDefault = _techerSeances.FirstOrDefault(x => x.TeacherId == tc.TeacherId);
                RunFirstFit(fid, tc, firstOrDefault, pbar);
            }

            ListOfClaassLecture.Add(_classSeances);
            // CrossOver(_noSolutions);
            var result = new MultiGeneration()
            {
                Lectures = _planing
            };
            Fitness(result);
            fitness = result.CountConflict;
            return _planing;
        }

        private static void RunFirstFit(int fid, Tc tc1, TeacherSeance firstOrDefault, PBar pbar)
        {
          

            if (firstOrDefault != null)
            {

                var sc = firstOrDefault.Seances.OrderBy(p => p).ToList();
                var temp = new List<int>();
                
              
                temp.AddRange(sc);
                for (int i = 0; i < tc1.ScheduleWieght; i++)
                {
                    var rnd = new Random();
                    var rnd2 = new Random();
                    var item = 0;
                    int r1=-1;
                    var exist = false;
                    do
                    {
                        if (temp.Count == 0)
                            break;
                        r1 = rnd.Next(0, temp.Count);
                        item = temp[r1];
                        if (item==0)continue;
                        exist = CheckIfLectureExist(item, tc1);
                     //   periodes.Add(item);
                        temp.Remove(temp[r1]);
                    } while (!exist);
                    if (exist)
                    {
                        AddSolution(fid, tc1, pbar, item, rnd2);
                       

                    }
                    else
                    {
                      AddNoSolution(fid, tc1, item,pbar);
                   
                    }
                }

            }
        
        }

        private static void AddNoSolution(int fid, Tc tc1,int periode, PBar pbar)
        {
            //if (tc1.Periode == 1 || tc1.Periode == 2)
            //{
            //    tc1.Periode = 0;
            //    return;
            //}
            var nos = new Lecture();
            nos.TeacherId = tc1.TeacherId;
            nos.CourseId = tc1.CourseId;
          nos.Teacher = tc1.Teacher;
            //sc.RemoveAt(r1);
            nos.ClassRoomTypeId = tc1.ClassRoomTypeId;
            nos.SectionId = Convert.ToInt32(tc1.SectionId);
            nos.GroupeId = tc1.GroupeId;
            nos.SpecialiteId = tc1.Section.SpecialiteId;
            nos.FaculteId = fid;
            nos.AnneeId = tc1.Section.AnneeId;
            nos.Solved = false;
            if (
                !_noSolutions.Any(
                    x =>
                        x.TeacherId == nos.TeacherId && x.ClassRoomTypeId == nos.ClassRoomTypeId &&
                        x.CourseId == nos.CourseId && x.SectionId == nos.SectionId && x.GroupeId == nos.GroupeId))
                _noSolutions.Add(nos);
            pbar.IncPb();
            // _tcs.Remove(tc1);
        }

        private static void AddSolution(int fid, Tc tc1, PBar pbar, int item, Random rnd2)
        {
            Tc tc2 = tc1;
            var rest = SelectRoomLecture(item, tc2);
            var r2 = rnd2.Next(0, rest.Count());
            var  s = rest[r2];
        //   
            var lecture = s;
            if (lecture != null)
            {
                _classSeances.Remove(s);
                rest.Remove(s);
                
                //sc.RemoveAt(r1);
                foreach (var teacherSeance in _techerSeances)
                {
                    if (teacherSeance.TeacherId == tc1.TeacherId)
                        teacherSeance.Seances.Remove(lecture.Seance);
                }
                var pl = new Lecture();
                pl.TeacherId = tc1.TeacherId;
                pl.CourseId = tc1.CourseId;
                pl.ClassRoomId = lecture.ClassRoomId;
                pl.ClassRoomTypeId = lecture.ClassRoomTypeId;
                pl.Seance = lecture.Seance;
                pl.SectionId = Convert.ToInt32(tc1.SectionId);
                pl.GroupeId = tc1.GroupeId;
                pl.SpecialiteId = tc1.Section.SpecialiteId;
                pl.FaculteId = fid;
                pl.Groupe = tc1.Groupe;
                pl.AnneeId = tc1.Section.AnneeId;
                pl.Periode = tc1.Periode;
                pl.Teacher = tc1.Teacher;
                pl.Teacher.Seances = _teacherUnavailablePeriods.Where(x => x.AnneeScolaireId == tc1.AnneeScolaireId && x.Semestre == tc1.Semestre &&x.TeacherId == tc1.TeacherId).ToList();
                pl.ClassRoom = _rooms.FirstOrDefault(x => x.Id == pl.ClassRoomId);
                if (pl.ClassRoom != null)
                    pl.ClassRoom.SeanceLbrSalles =
                        _roomsUnavailablePeriods.Where(
                            x =>
                                x.AnneeScolaireId == tc1.AnneeScolaireId && x.Semestre == tc1.Semestre &&
                                x.SalleId == pl.ClassRoomId).ToList();
                pl.Solved = true;
                _planing.Add(pl);
                _tcs.Remove(tc1);
                pbar.IncPb();
                //list.Remove(tc1);
            }
        }

       
      

     

        public static int GeneratePopulations(int fId, int semestre, int anneeScolaire, ProgressBar progressBar , UpdateStatus updateStatus)
        {
            

            if (UpdateDataDg != null) UpdateDataDg("Calcul heuristiques ...");

            using (_db = new Linq2DbModel())
            {
                var l = _db.Tcs.Where(x => x.AnneeScolaireId == anneeScolaire && x.Semestre == semestre);
                var h =CalculHeuristics(l, fId,semestre, anneeScolaire);
                if (h == false)
                {
                    if (UpdateDataDg != null) UpdateDataDg("Sorry heuristics show that there is  a Solution with some conflicts (some hard constraints can not be satisfied)");
                   // updateStatus.EtatHeuristique = EtatProgression.FirstFitAlgorithm;
                   // return 0;

                }
                updateStatus.EtatHeuristique = EtatProgression.FirstFitAlgorithm;
            }
            var result = new List<MultiGeneration>();
            string progress = "Generation de planing ";
            var start = DateTime.Now;
            PBar pBar = new PBar(progressBar);
            progressBar.Minimum = 0;
            progressBar.Maximum = 10;
            progress = progress + "++";
            if (UpdateDataDg != null) UpdateDataDg(progress);
            _planing = new List<Lecture>();
            var f = 0;
            var item = new MultiGeneration
            {
                CountLateTime = 0,
                Lectures = GeneratingPlanings(fId, semestre, anneeScolaire, progressBar , out f)
            };
           
           // updateStatus.TempSpan = watch.Elapsed;
            //MessageBox.Show(t.ToString(CultureInfo.InvariantCulture));
            result.Add(item);
            if (UpdateDataDg != null) UpdateDataDg("Calcul de fitness.. ");
            Fitness(item);
            updateStatus.Fitness = item.CountConflict;
            pBar.IncPb();
           
           
            _planing = new List<Lecture>();
            var solution =result.FirstOrDefault();
            if (solution != null)
            {
                if (solution.CountConflict == 0 && solution.TeacherLectures == 0)
                {
                    _planing = solution.Lectures;
                    SumilatedAnnealing s  = new SumilatedAnnealing(_planing ,solution.CountConflict ) ;
                    s.UpdateStatusDgEvent += (span, i) =>
                    {
                        if (EveStatusEvt != null) EveStatusEvt(i, span);
                    };
                    s.HillClimbing(1000  );
                    updateStatus.EtatHeuristique = EtatProgression.SimulatedAnnealing;
                    s.Run(20000);
                    if (UpdateDataDg != null) UpdateDataDg("wait please until the program saves the founded solotion ");
                    //using (_db = new Linq2DbModel())
                    //{
                    //    var index = result.FindIndex(x => x == solution);
                    //    _db.Lectures.BulkCopy(_planing);
                    //    _db.ClassSeances.BulkCopy(ListOfClaassLecture[index]);

                    //    if (UpdateDataDg != null) UpdateDataDg("Done ! ");
                    //}

                }
               

            }
          

            return _planing != null ? _planing.Count : 0;
        }
        private static void Fitness(MultiGeneration generation)
        {
            int qnt = 0;
            
            foreach (var tc in generation.Lectures.Where(x => x.GroupeId != null).GroupBy(x => x.GroupeId))
            {
                for (int i = 1; i <= 36; i++)
                {
                    var f = tc.Count(x => x.Seance == i);
                    if (f > 1)
                    {
                        generation.CountConflict += 1;
                        //var ky = tc.Key;
                        //conflictPosition = tc.ToList();
                    } 
                }

            }
            foreach (var tc in generation.Lectures.GroupBy(x => x.SectionId))
            {
               
                for (int i = 1; i <= 36; i++)
                {
                    var f = tc.Count(x => x.Seance == i );
                    //var l = tc.SelectRoomLecture(x => x.Seance == i);
                    var f1 = tc.Count(w => w.GroupeId != 0 && w.Seance == i);
                    //var f3 = tc.Count(x => x.Seance == i);
                    if (f1 > 0 && f > f1)
                    {
                        generation.CountConflict = generation.CountConflict + 1;
                    }
                    else if (f1 == 0 && f > 1)
                    {
                        generation.CountConflict = generation.CountConflict + 1;
                    }
                   
                }
            }
            foreach (var lecture in generation.Lectures.GroupBy(x => x.TeacherId))
            {
                for (int i = 0; i < 36; i++)
                {
                    if (lecture.Count(w => w.Seance == i) > 1) 
                        generation.TeacherLectures ++;
                }
            }
            foreach (var lecture in generation.Lectures.GroupBy(x => x.ClassRoomId))
            {
                for (int i = 1; i <= 36; i++)
                {
                    if (lecture.Count(w => w.Seance == i) > 1) 
                        generation.TeacherLectures++;
                }
            }

        }
        public static T AnyOrDefault<T>( IList<T> e, Func<T, double> weightSelector)
        {
            if (e.Count < 1)
                return default(T);
            if (e.Count == 1)
                return e[0];
            var weights = e.Select(o => Math.Max(weightSelector(o), 0)).ToArray();
            var sum = weights.Sum(d => d);

            var rnd = new Random().NextDouble();
            for (int i = 0; i < weights.Length; i++)
            {
                //Normalize weight
                var w = sum == 0
                    ? 1 / (double)e.Count
                    : weights[i] / sum;
                if (rnd < w)
                    return e[i];
                rnd -= w;
            }
            throw new Exception("Should not happen");
        }
        private static List<ClassSeance> SelectRoomLecture(int item, Tc tc1)
        {
            var q1 = (from c in _classSeances.OrderBy(w => w.Seance)
                where c.Seance == item && c.ClassRoomTypeId.Equals(tc1.ClassRoomTypeId)
                select c).ToList();

            return q1;
        }

    
        private static bool CalculHeuristics(IEnumerable<Tc> tcs, int fId, int semestre, int annee)
        {
            var b = false;
            var mins = GenerateClassSeances(fId, annee, semestre);
            var classSeances = mins as ClassSeance[] ?? mins.ToArray();
            using (_db = new Linq2DbModel())
            {

                foreach (var tc in tcs.GroupBy(w => w.ClassRoomTypeId))
                {

                    var item = tc.FirstOrDefault();
                    var sum = tc.Sum(w => w.ScheduleWieght);
                    var q = classSeances.Count(w => item != null && w.ClassRoomTypeId == item.ClassRoomTypeId);
                    b = sum <= q;
                       
                    if (!b) return false;
                }
                return b;
            }

        }

         public static IEnumerable<T> Duplicates<T>
         (this IEnumerable<T> source, bool distinct = true)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            // select the elements that are repeated
            IEnumerable<T> result = source.GroupBy(a => a).SelectMany(a => a.Skip(1));

            // distinct?
            if (distinct == true)
            {
                // deferred execution helps us here
                result = result.Distinct();
            }

            return result;
        }

        private static bool CheckIfLectureExist(int item, Tc tc)
        {
            if (tc.GroupeId == null || tc.GroupeId == 0)
            {
                if (_planing.Any(lecture => lecture.SectionId == tc.SectionId && lecture.Seance == item))
                {
                    return false;
                }
                var q = (from c in _classSeances.OrderBy(w => w.Seance)
                         where c.Seance == item && c.ClassRoomTypeId.Equals(tc.ClassRoomTypeId)
                              // && c.Max >= tc.Section.Nombre
                              // && c.Min <= tc.Section.Nombre
                         select c).Any();

                return q;
            }
            if (tc.GroupeId != null)
            {
                if (
                    _planing.Any(
                        lecture =>
                            (lecture.SectionId == tc.SectionId && lecture.GroupeId == null) && lecture.Seance == item))
                {
                    return false;
                }
                if (_planing.Any(lecture => (lecture.GroupeId == tc.GroupeId) && lecture.Seance == item))
                {
                    return false;
                }

            }
            var q1 = (from c in _classSeances.OrderBy(w => w.Seance)
                      where c.Seance == item && c.ClassRoomTypeId.Equals(tc.ClassRoomTypeId)
                      //&& c.Max >= tc.Groupe.Nombre
                      //&& c.Min <= tc.Groupe.Nombre
                      select c).Any();

            return q1;
        }





        #endregion

    }
}
