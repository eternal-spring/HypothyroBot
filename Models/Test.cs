using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HypothyroBot.Models
{
    public class Test
    {
        [Key]
        public string Id { get; set; }
        public double TshLevel { get; set; }
        [Column(TypeName = "date")]
        public DateTime TestDate { get; set; }
        public bool Actual { get; set; }
        public virtual User User { get; set; }
        public Test()
        {
            ;
        }
    }
}