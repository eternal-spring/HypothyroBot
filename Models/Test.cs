using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace HypothyroBot.Models
{
    public class Test
    {
        [Key]
        public string Id { get; set; }
        public double TshLevel { get; set; }
        [Column(TypeName = "date")]
        public DateTime TestDate { get; set; }
        public virtual User User { get; set; }
    }
}