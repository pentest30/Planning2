using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using Planning.LinqToDb.Models;


namespace Planing.PL.Generator
{
    public class SumilatedAnnealing
    {
        public delegate void UpdateDg();
        public delegate void UpdateStatusDg( TimeSpan span , int f);
        public UpdateDg UpdateDataDg;
        public UpdateStatusDg UpdateStatusDgEvent;
        //private List<Tavu> tabu ;
        private List<Lecture> _initial;
        private readonly int _initialHardContrainsScore;
        private readonly List<ClassRoom> _rooms;
        private Parameter _param;
        private double _temperature;
        readonly int[] _fIrstHafls = { 1, 2, 3, 4, 5, 7, 8, 9, 10, 11, 13, 14, 15, 16, 17, 19, 20, 21, 22, 23, 25, 26, 27, 28, 29, 31, 32, 33, 34, 35 };
        readonly int[] _dayPereiods = { 1, 2, 3, 7, 8, 9, 13, 14, 15, 19, 20, 21, 25, 26, 27, 31, 32, 33 };
        public ObservableCollection<Lecture> Solution { get; set; }
        public int Fitness { get; set; }
        public bool Stop { get; set; }

        //public bool _stop;
        //  public Sa Sa { get; set; }

        public SumilatedAnnealing(List<Lecture> initial, int initialHardContrainsScore)
        {

            //  Sa = new Sa();
            Stop = false;
            Solution = new ObservableCollection<Lecture>();
            _initial = initial;
            _initialHardContrainsScore = initialHardContrainsScore;
            using (var fb = new Linq2DbModel())
            {
                _rooms = fb.ClassRooms.ToList();
                _param = fb.Parameters.FirstOrDefault();
            }
           
        }

      public ObservableCollection<Lecture> HillClimbing(int seconds)
      {
        var stopWatch = new Stopwatch();
        int oldValue;
        int newValue;
        Solution = new ObservableCollection<Lecture>();
        foreach (var lecture in _initial)
        {
          Solution.Add(lecture);

        }
        oldValue = CalculFitness(Solution) + CalculSoftFitness(Solution);
        if (UpdateDataDg != null) UpdateDataDg();
        bool stop = false;
        stopWatch.Start();
        while (!stop)
        {
          var clone = new ObservableCollection<Lecture>();
          GetClone(clone);
          var newSolution = new ObservableCollection<Lecture>();
          Application.Current.Dispatcher.BeginInvoke(
            DispatcherPriority.Background,
            new Action(() =>
            {
              newSolution = Perturbation1(clone);
              newSolution = Perturbation2(newSolution);
            }));
          int s = CalculFitness(newSolution);
          if (s > _initialHardContrainsScore) continue;
          newValue = s + CalculSoftFitness(Solution);
          if (newValue >= oldValue)
          {
            Solution = clone;
            oldValue = newValue;
            Fitness = newValue;
            if (UpdateDataDg != null) UpdateDataDg();
            if (UpdateStatusDgEvent != null) UpdateStatusDgEvent(stopWatch.Elapsed, Fitness);
          }
          // sum +=(int) stopWatch.Elapsed.TotalSeconds;
          if (seconds <= stopWatch.Elapsed.TotalSeconds) stop = true;
        }
        _initial = new List<Lecture>();
        foreach (var lecture in Solution)
        {
          _initial.Add(lecture);
        }
        return Solution;
      }

      private void GetClone(ObservableCollection<Lecture> clone)
        {
            foreach (var lecture in Solution)
            {
                clone.Add(lecture);
            }
        }

      public ObservableCollection<Lecture> Run(double temperature)
      {
        _temperature = temperature;

        double coolingRate = 0.003;

        //_stop = false;
        Solution = new ObservableCollection<Lecture>();
        int oldValue;
        int newValue;
        foreach (var lecture in _initial)
        {
          Solution.Add(lecture);

        }
        oldValue = CalculFitness(Solution) + CalculSoftFitness(Solution);
        if (UpdateDataDg != null) UpdateDataDg();
        //   stopWatch.Start();
        while ((int) _temperature > 0)
        {
          var clone = new ObservableCollection<Lecture>();
          GetClone(clone);
          var newSolution = new ObservableCollection<Lecture>();
          Application.Current.Dispatcher.BeginInvoke(
            DispatcherPriority.Background,
            new Action(() =>
            {
              newSolution = Perturbation1(clone);
              newSolution = Perturbation2(newSolution);
            }));

          int s = CalculFitness(newSolution);
          if (s > _initialHardContrainsScore) continue;
          newValue = s + CalculSoftFitness(Solution);
          var acceptanceRate = P(newValue, newValue);
          if (newValue >= oldValue || !acceptanceRate)
          {
            Solution = clone;
            oldValue = newValue;
            Fitness = newValue;
            if (UpdateDataDg != null) UpdateDataDg();
            //if (UpdateStatusDgEvent != null) UpdateStatusDgEvent(stopWatch.Elapsed, Fitness);
          }
          _temperature *= Convert.ToDouble(1 - coolingRate);

        }
        _initial = new List<Lecture>();
        foreach (var lecture in Solution)
        {
          _initial.Add(lecture);
        }
        return Solution;
      }

      private int CalculSoftFitness(ObservableCollection<Lecture> solution)
        {
            int score = 0;
            foreach (var lecture in solution)
            {
                if (CheckIfExistInFirstPeriode(lecture.Seance) && lecture.Periode != 2) score += _param.SoftLastPeriodeBonus;
                if (CheckIfExistForSecondPeriode(lecture.Seance) && lecture.Periode!=2 ) score -= _param.SoftLastPeriodPenalty;
            }



            foreach (var t in solution.GroupBy(x => x.TeacherId))
            {

                foreach (var lecture in t)
                {
                    if (lecture.Teacher.Seances.Count > 0)
                    {
                        if (lecture.Teacher.Seances.Any(x => x.Number == lecture.Seance)) score -= _param.SoftUnAvailableTeacherPenalty;
                        else score += _param.SoftUnAvailableTeacherBonus;

                    }

                }
            }

            foreach (var t in solution.GroupBy(x => x.ClassRoomId))
            {
               
                foreach (var lecture in t)
                {
                    if (lecture.ClassRoom.SeanceLbrSalles.Count > 0)
                    {
                        if (lecture.ClassRoom.SeanceLbrSalles.Any(x => x.Number == lecture.Seance)) score -= _param.SoftUnAvailableRoomPenalty;
                        else score += _param.SoftUnAvailableRoomBonus;

                    }
                    
                }
                var group = t.ToList();
                int i = 0;
                if (t.Count() == 1) continue;
                foreach (Lecture lecture1 in group)
                {
                    if ((i + 1) < group.Count)
                    {
                        if ((lecture1.Seance%6==0)&& (lecture1.Seance - group[i + 1].Periode > 1)) score -= _param.SoftCourseSuccessingPenalty; ;
                        if (i > 0 && (lecture1.Seance - group[i - 1].Periode > 1)) score -= _param.SoftCourseSuccessingPenalty;
                    }
               
                    else
                    {
                        score += _param.SoftCourseSuccessingBonus; ;  
                    }

                }
             
            }
                  foreach (var lecture in solution)
            {
                if (lecture.Periode == 1 && CheckIfExistInFirstPeriode2(lecture.Seance)) score += 10;
                if (lecture.Periode == 2 && CheckIfExistForSecondPeriode2(lecture.Seance)) score += 10;
                if (lecture.Periode == 2 && CheckIfExistInFirstPeriode2(lecture.Seance)) score -= 10;
                if (lecture.Periode == 1 && CheckIfExistForSecondPeriode2(lecture.Seance)) score -= 10;
            }
            return score;
        }
        private static bool CheckIfExistForSecondPeriode(int s)
        {
            var secondeHafls = new[] { 6, 12, 18, 24, 30, 36 };
            return secondeHafls.Any(x => x == (s));
        }



        private static bool CheckIfExistInFirstPeriode(int s)
        {
            var firstHafls = new[] { 1, 2, 3, 4, 5, 7, 8, 9, 10, 11, 13, 14, 15, 16, 17, 19, 20, 21, 22, 23, 25, 26, 27, 28, 29, 31, 32, 33, 34, 35 };
            return firstHafls.Any(x => x == (s));

        }
        private static bool CheckIfExistForSecondPeriode2(int s)
        {
            var secondeHafls = new[] { 4, 5, 6, 10, 11, 12, 16, 17, 18, 22, 23, 24, 25, 28, 29, 30, 34, 35, 36 };
            return secondeHafls.Any(x => x == (s));
        }

        private static bool CheckIfExistInFirstPeriode2(int s)
        {
            var firstHafls = new[] { 1, 2, 3, 7, 8, 9, 13, 14, 15, 19, 20, 21, 25, 26, 27, 31, 32, 33 };
            return firstHafls.Any(x => x == (s));

        }
        private ObservableCollection<Lecture> Perturbation1(ObservableCollection<Lecture> solution)
        {
            bool f = false;
            int p=-1;
            int index;
            List<int> peridos = new List<int>();
            for (int i = 1; i < 37; i++)
            {
                peridos.Add(i);
            }
            Lecture selected;
            var rnd2 = new Random();

            do
            {
                //bool exist2;

                var s0 = CalculFitness(solution);
                var f0 = s0;
                //Lecture lastSelected = new Lecture();
                //selected = new Lecture();
                //if (lastSelected.Id > 0 && lastSelected == selected) continue; 
                selected = AnyOrDefault(solution, xx => rnd2.NextDouble());
              //  lastSelected = selected;
                //   selected.Teacher.Seances = seances.Where(x => x.TeacherId == selected.TeacherId).ToList();
                if (selected.Teacher.Seances.Count == 0)
                {
                    p = (selected.Periode == 0 || selected.Periode == 2)
                        ? AnyOrDefault(_dayPereiods, i => 0.5)
                        : AnyOrDefault(_fIrstHafls, ss => rnd2.NextDouble());
                }

                else
                {
                    if (selected.Periode == 0 || selected.Periode == 2)
                    {
                        var selected1 = selected;
                        var tmp = peridos.Where(x => selected1.Teacher.Seances.All(y => y.Number != x)).ToList();
                        p = rnd2.Next(0, tmp.Count() - 1);
                        p = tmp[p];
                    }
                    else
                    {
                        var selected1 = selected;
                        var tmp = _fIrstHafls.Where(x => selected1.Teacher.Seances.All(y => y.Number != x)).ToList();
                        p = rnd2.Next(0, tmp.Count() - 1);
                        p = tmp[p];
                    }
                }
                var ff = selected.Seance;
               // var chance = rnd2.Next(1, 36);
                
                ////var index1 = index;

                selected.Seance = p;
                
                var s1 = CalculFitness(solution);
                var f1 = s1;
                //if (f1==0) break;
                f = f1 > f0 && f1 > 0 && s1 > _initialHardContrainsScore;
                selected.Seance = ff;
                //selected.ClassRoomId = rr;

            } while (f);
            index = solution.IndexOf(selected);
            solution[index].Seance = p;
            return solution;
        }

        private ObservableCollection<Lecture> Perturbation2(ObservableCollection<Lecture> solution)
        {
            bool f;
            Lecture selected;
            var rnd2 = new Random();
            selected = AnyOrDefault(solution, xx => rnd2.NextDouble());
            var lastROom = selected.ClassRoomId;
            var s0 = CalculFitness(solution);
            var rnd = new Random(); 
            do
            {
                //bool exist2;
               
                selected.ClassRoomId = lastROom;
               
                var f0 = s0;

                selected.ClassRoomId = AnyOrDefault(_rooms.Where(x=>x.ClassRoomTypeId == selected.ClassRoomTypeId).ToList() , room =>rnd.NextDouble() ).Id;

                var s1 = CalculFitness(solution);
                var f1 = s1;
                //if (f1==0) break;
                f = f1 > f0 && f1 > 0 && s1 > _initialHardContrainsScore;

            } while (f);

            return solution;
        }


        private bool P(int newValue, int oldValue)
        {
            var rnd = new Random();
            var ac = Math.Exp((newValue - oldValue) / _temperature);
            return rnd.NextDouble() > ac;

        }

        static T AnyOrDefault<T>(IList<T> e, Func<T, double> weightSelector)
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
                var w = Math.Abs(sum) <= 0
                    ? 1 / (double)e.Count
                    : weights[i] / sum;
                if (rnd < w)
                    return e[i];
                rnd -= w;
            }
            throw new Exception("Should not happen");
        }

        private int CalculFitness(ObservableCollection<Lecture> solution)
        {
            int final = 0;
            foreach (var tc in solution.Where(x => x.GroupeId != null).GroupBy(x => x.GroupeId))
            {
                for (int i = 1; i <= 36; i++)
                {
                    var f = tc.Count(x => x.Seance == i);
                    if (f > 1) final++;
                    
                }

            }
            foreach (var tc in solution.GroupBy(x => x.SectionId))
            {
                for (int i = 1; i <= 36; i++)
                {
                    var f = tc.Count(x => x.Seance == i);
                    //var l = tc.SelectRoomLecture(x => x.Seance == i);
                    var f1 = tc.Count(w => w.GroupeId != null && w.Seance == i);
                    //var f3 = tc.Count(x => x.Seance == i);
                    if (f1 > 0 && f > f1)
                    {
                        final++;
                    }
                    if (f1 == 0 && f > 1)
                    {
                        final++;
                    }

                }

            }
            foreach (var lecture in solution.GroupBy(x => x.TeacherId))
            {
                for (int i = 1; i <= 36; i++)
                {
                    if (lecture.Count(w => w.Seance == i) > 1) final++;
                }
            }
            foreach (var lecture in solution.GroupBy(x => x.ClassRoomId))
            {
                for (int i = 1; i <= 36; i++)
                {
                    if (lecture.Count(w => w.Seance == i) > 1) final++;
                }
            }

            return final;
        }

    }
}
