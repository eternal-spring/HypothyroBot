using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HypothyroBot.Models
{
    public class User
    {
        [Key]
        public string Id { get; set; }
        public ModeType Mode { get; set; }
        public string Name { get; set; }
        [Column(TypeName = "date")]
        public DateTime DateOfBirth { get; set; }
        public GenderType Gender { get; set; }
        public double Weight { get; set; }
        public int isPregnant { get; set; } = -1;
        public double PretreatmentDose { get; set; } = -2;
        public DrugType PretreatmentDrug { get; set; }
        [Column(TypeName = "date")]
        public DateTime DateOfOperation { get; set; }
        public double TreatmentDose { get; set; } = -2;
        public DrugType TreatmentDrug { get; set; }
        public ThyroidType ThyroidCondition { get; set; }
        public PathologyType Pathology { get; set; }
        public double lowTshLevel { get; set; } = 0.35;
        public double upTshLevel { get; set; } = 4;
        public double checkinterval { get; set; } = 60;
        public virtual ICollection<Test> Tests { get; set; }
    }
}