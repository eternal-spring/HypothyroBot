using System.ComponentModel;

namespace HypothyroBot.Models
{
    public enum DrugType
    {
        None,
        [Description("эутирокс")]
        Eutirox,
        [Description("l-тироксин")]
        LThyroxine,
        [Description("препарат")]
        Another
    }
}