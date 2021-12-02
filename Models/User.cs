using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HypothyroBot.Models
{
    public class User
    {
        [Key]
        public string Id { get; set; }
        [Column(TypeName = "nvarchar(24)")]
        public ModeType Mode { get; set; }
        public string Name { get; set; }
        [Column(TypeName = "Date")]
        public DateTime BirthDate { get; set; }
        [Column(TypeName = "nvarchar(max)")]
        public GenderType Gender { get; set; }
        public double Weight { get; set; }
        public double PretreatmentDose { get; set; } = -2;
        [Column(TypeName = "nvarchar(24)")]
        public DrugType PretreatmentDrug { get; set; }
        [Column(TypeName = "Date")]
        public DateTime OperationDate { get; set; }
        public double TreatmentDose { get; set; } = -2;
        [Column(TypeName = "nvarchar(24)")]
        public DrugType TreatmentDrug { get; set; }
        [Column(TypeName = "nvarchar(32)")]
        public ThyroidType ThyroidCondition { get; set; }
        [Column(TypeName = "nvarchar(32)")]
        public PathologyType Pathology { get; set; }
        public double lowpthslev { get; set; } = 0.35;
        public double uppthslev { get; set; } = 4;
        public TimeSpan checkinterval { get; set; } = new TimeSpan(60,0,0,0);
        public double TSH { get; set; }
        [Column(TypeName = "Date")]
        public DateTime TestDate { get; set; }

    }
}