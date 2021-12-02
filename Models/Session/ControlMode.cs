using HypothyroBot.Models.Alice_API;
using HypothyroBot.Models.Session.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HypothyroBot.Models.Session
{
    public class ControlMode : IMode
    {
        private User User;
        public ControlMode(User user)
        {
            User = user;
        }
        public async Task<AliceResponse> HandleRequest(AliceRequest aliceRequest, DataBaseContext db)
        {
            string text = "";
            string tts = null;
            var buttons = new List<ButtonModel>();
            if (User.TreatmentDose == 0)
            {
                if (User.TSH < User.lowpthslev)
                {
                    text = "Есть риск что у вас тиреотоксикоз. Если вы ранее не сталкивались " +
                        "с этим нужно сдать кровь на свободные фракции T3 и Т4 и Антитела к " +
                        "рецептору ТТГ и обратится к эндокринологу.";
                }
                else if (User.TSH > User.uppthslev)
                {
                    text = "Вероятно, ваша железа не справляется с обеспечением вас гормонами. ";
                    if (User.TSH < 6)
                    {
                        text += "Повышение ТТГ означает что щитовидная железа (то, что от нее осталось " +
                            "после операции) вырабатывает меньше гормонов, чем нужно, однако разница " +
                            "не настолько значима. Если вы себя нормально чувствуете можно не спешить " +
                            "с назначением гормонов. Имеются ли у Вас жалобы на сонливость, редкий пульс?";
                        buttons = new List<ButtonModel>() { new ButtonModel("да", true), new ButtonModel("нет", true), };
                    }
                    else if (User.TSH > 6)
                    {
                        text += "У вас недостаток гормонов. Вам нужно начинать заместительную терапию. " +
                            "Обычно эндокринологи начинают с дозы в 50 мкг левотироксина. Контроль ТТГ через 2 месяца.";
                        User.checkinterval = User.TSH > 10 ? new TimeSpan(60, 0, 0, 0) : new TimeSpan(45, 0, 0, 0);
                    }
                }
                else
                {
                    text = "У вас отличный уровень ТТГ. Следующий раз контроль через 6 месяцев";
                }
            }
            else if (User.TreatmentDose > 0)
            {
                if (User.TSH < User.lowpthslev)
                {
                    text = "Мне кажется, что доза для Вас велика. Если мы наблюдаем снижение ТТГ, " +
                        "то это значит, что гипофиз относится к уровню ваших гормонов как к избыточным.";
                    if(User.Pathology == PathologyType.PapillaryOrFollicularCarcinoma)
                    {
                        text += "Это может быть нормальным для супрессивной терапии для пациентов высокой и умеренной " +
                            "группы риска. У вас были множественные метастазы, операции по поводу рецидивов, " +
                            "планируется терапия радиоактивным йодом, имеются высокие уровни тиреоглобуллина?";
                        buttons = new List<ButtonModel>() { new ButtonModel("да", true), new ButtonModel("нет", true), };

                    }
                }
                else if (User.TSH > User.uppthslev)
                {
                    if (User.uppthslev > 6 && User.uppthslev < 10)
                    {
                        text = "Вам нужно увеличить дозу на ";
                        text += (User.BirthDate.CompareTo(DateTime.Now.AddYears(-70)) < 0 || User.Weight < 55) ? "12,5 мкг." :
                            "25 мкг.";
                        text += "Контроль ТТГ через 2 месяца.";
                        User.checkinterval = new TimeSpan(60, 0, 0, 0);
                    }
                    else if (User.uppthslev > 10)
                    {
                        text = "У вас выраженный недостаток гормонов. Вам нужно срочно увеличить дозу " +
                            "на 25 мкг. Контроль ТТГ через 1,5 месяца.";
                        User.checkinterval = new TimeSpan(45, 0, 0, 0);
                    }
                }
                else
                {
                    text = $"У вас отличный уровень ТТГ. Продолжайте принимать " +
                    $"{((DescriptionAttribute)User.TreatmentDrug.GetType().GetMember(User.TreatmentDrug.ToString())[0].GetCustomAttributes(typeof(DescriptionAttribute), false)[0]).Description} " +
                    $"по {User.TreatmentDose} мкг. Следующий раз контроль через ";
                    if (User.checkinterval.Days >= 180)
                    {
                        text += "год";
                        User.checkinterval = new TimeSpan(360, 0, 0, 0);
                    }
                    else
                    {
                        text += "6 месяцев";
                        User.checkinterval = new TimeSpan(180, 0, 0, 0);
                    }
                }
            }
        }
    }
}