using System.ComponentModel;

namespace HypothyroBot.Models
{
    public enum GenderType
    {
        None,
        [Description("мужчина")]
        Male,
        [Description("женщина")]
        Female,
        Unknown
    }
}