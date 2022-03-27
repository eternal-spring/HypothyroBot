using System.ComponentModel;

namespace HypothyroBot.Models
{
    public enum PathologyType
    {
        None,
        [Description("фолликулярная аденома или узловой нетоксический зоб")]
        NodularNonToxicGoiter,
        [Description("тиреоидит Хашимото или диффузный токсический зоб")]
        DiffuseToxicGoiter, 
        [Description("папиллярная или фолликулярная карцинома")]
        PapillaryOrFollicularCarcinoma,
        [Description("медуллярная карцинома")]
        MedullaryCarcinoma,
        [Description("другое")]
        Another
    }
}