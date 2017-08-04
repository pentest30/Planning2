﻿using System.Collections.Generic;

using Planning.LinqToDb.Models;

namespace Planing.PL.Generator
{
    public class MultiGeneration
    {
        public double CountLateTime { get; set; }
        public int CountConflict { get; set; }
        public int TeacherLectures { get; set; }
        public List<Lecture> Lectures { get; set; }
    }
}
