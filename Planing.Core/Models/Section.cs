﻿using System.Collections.Generic;
using Planing.Core.DbImport;
using Planing.Models;

namespace Planing.Core.Models
{
    public class Section
    {
        public Section()
        {
            Groupes = new List<Groupe>();
            SalleClasses = new List<SalleClasse>();
        }
        public int Id { get; set; }
        [DbColumn("Nom")]
        public string Name { get; set; }
        [DbColumn("Abréviation")]
        public string Code { get; set; }
        public int AnneeId { get; set; }
        public int SpecialiteId { get; set; }
        public int AnneeScolaireId { get; set; }
        public int Semestre { get; set; }
        public int Nombre { get; set; }
        //public int? ClassRoomId { get; set; }
        //public ClassRoom ClassRoom { get; set; }
        public List<Groupe> Groupes { get; set; }

        public  Annee Annee { get; set; }
        public  Specialite Specialite { get; set; }
        public  AnneeScolaire AnneeScolaire { get; set; }
        public List<SalleClasse> SalleClasses { get; set; }
    }
}
