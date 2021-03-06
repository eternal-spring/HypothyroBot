using HypothyroBot.Models.Alice_API;
using HypothyroBot.Models.Session.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
        public async Task<AliceResponse> HandleRequest(AliceRequest aliceRequest, ApplicationContext db)
        {
            string text = "";
            string tts = "";
            var buttons = new List<ButtonModel>();
            switch (User.Mode)
            {
                case ModeType.Control:
                    {
                        if (User.TreatmentDose == 0 && User.Tests?.Last()?.TshLevel > User.upTshLevel)
                        {
                            if (aliceRequest.Request.Nlu.Tokens.First().StartsWith("да"))
                            {
                                text = "Вам нужно начинать заместительную терапию. Обычно эндокринологи " +
                                    "начинают с дозы в 50 мкг левотироксина. Контроль ТТГ через 2 месяца.";
                            }
                            else if (aliceRequest.Request.Nlu.Tokens.First().StartsWith("не"))
                            {
                                text = "В случаях, когда нет клинических симптомов гипотиреоза, " +
                                    "решение о заместительной терапии можно отложить на 2 месяца. " +
                                    "Давайте проконтролируем ТТГ.";
                            }
                            else
                            {
                                return new AliceResponse(aliceRequest, "Не поняла, повторите.");
                            }
                            User.checkinterval = 60; 
                            User.Mode = ModeType.OnReminder;
                        }
                        else if (User.TreatmentDose > 0 && User.Tests?.Last()?.TshLevel < User.lowTshLevel)
                        {
                            if (aliceRequest.Request.Nlu.Tokens.First().StartsWith("да"))
                            {
                                text = "Важным условием супрессивной терапии является безопасность. " +
                                    "Есть ли у вас сердцебиения, потливость, быстрая утомляемость, то имеет смысл снизить дозу на ";
                                text += (User.DateOfBirth.CompareTo(DateTime.Now.AddYears(-70)) < 0 || User.Weight < 55) ? "12,5 мкг." :
                                    "25 мкг. ";
                                text += "Затем следует проконтролировать ТТГ через 2 месяца.";
                                User.checkinterval = 60;
                            }
                            else if (aliceRequest.Request.Nlu.Tokens.First().StartsWith("не"))
                            {
                                text = "Контроль через 3 месяца.";
                                User.checkinterval = 90; 
                            }
                            else
                            {
                                return new AliceResponse(aliceRequest, "Не поняла, повторите.");
                            }
                            User.Mode = ModeType.OnReminder;
                        }
                        tts = text.Replace("ТТГ", "тэтэ+гэ");
                        db.Users.Update(User);
                        await db.SaveChangesAsync();
                        var response = new AliceResponse(aliceRequest, text, tts, buttons)
                        {
                            SessionState = aliceRequest.State.Session,
                        };
                        return response;
                    }
                default:
                    {
                        if (User.TreatmentDose == 0)
                        {
                            if (User.Tests?.Last()?.TshLevel < User.lowTshLevel)
                            {
                                text = "Есть риск, что у вас тиреотоксикоз. Если вы ранее не сталкивались " +
                                    "с этим, нужно сдать кровь на свободные фракции T3 и Т4 и антитела к " +
                                    "рецептору ТТГ и обратиться к эндокринологу.";
                                User.Mode = ModeType.OnReminder;
                            }
                            else if (User.Tests?.Last()?.TshLevel > User.upTshLevel)
                            {
                                text = "Вероятно, ваша железа не справляется с обеспечением вас гормонами. ";
                                if (User.Tests?.Last()?.TshLevel < 6)
                                {
                                    text += "Повышение ТТГ означает, что щитовидная железа (то, что от нее осталось " +
                                        "после операции) вырабатывает меньше гормонов, чем нужно, однако разница " +
                                        "не настолько значима. Если вы себя нормально чувствуете, можно не спешить " +
                                        "с назначением гормонов. Имеются ли у Вас жалобы на сонливость, редкий пульс?";
                                    buttons = new List<ButtonModel>() { new ButtonModel("да", true), new ButtonModel("нет", true), };
                                    User.Mode = ModeType.Control;
                                }
                                else if (User.Tests?.Last()?.TshLevel > 6)
                                {
                                    text += "У вас недостаток гормонов. Вам нужно начинать заместительную терапию. " +
                                        "Обычно эндокринологи начинают с дозы в 50 мкг левотироксина. Контроль ТТГ через ";
                                    if (User.Tests?.Last()?.TshLevel > 10)
                                    {
                                        text += "1,5 месяца.";
                                        User.checkinterval = 45;
                                    }
                                    else
                                    {
                                        text += "2 месяца.";
                                        User.checkinterval = 60;
                                    }
                                    User.Mode = ModeType.OnReminder;
                                }
                            }
                            else
                            {
                                text = "У вас отличный уровень ТТГ. Следующий раз контроль через 6 месяцев.";
                                User.Mode = ModeType.OnReminder;
                            }
                        }
                        else if (User.TreatmentDose > 0)
                        {
                            if (User.Tests?.Last()?.TshLevel < User.lowTshLevel)
                            {
                                text = "Мне кажется, что доза для Вас велика. Если мы наблюдаем снижение ТТГ, " +
                                    "то это значит, что гипофиз относится к уровню ваших гормонов как к избыточным.";
                                if (User.Pathology == PathologyType.PapillaryOrFollicularCarcinoma)
                                {
                                    text += "Это может быть нормальным для супрессивной терапии для пациентов высокой и умеренной " +
                                        "группы риска. У вас были множественные метастазы, операции по поводу рецидивов, " +
                                        "планируется терапия радиоактивным йодом, имеются высокие уровни тиреоглобуллина?";
                                    buttons = new List<ButtonModel>() { new ButtonModel("да", true), new ButtonModel("нет", true), };
                                    User.Mode = ModeType.Control;
                                }
                                else
                                {
                                    text += "Вам, вероятно, нужно снизить дозу на ";
                                    text += (User.DateOfBirth.CompareTo(DateTime.Now.AddYears(-70)) < 0 || User.Weight < 55) ? "12,5 мкг." :
                                        "25 мкг. ";
                                    text += "Затем следует проконтролировать ТТГ через 2 месяца.";
                                    User.checkinterval = 60;
                                    User.Mode = ModeType.OnReminder;
                                }
                            }
                            else if (User.Tests?.Last()?.TshLevel > User.upTshLevel)
                            {
                                if (User.Tests?.Last()?.TshLevel >= 6 && User.Tests?.Last()?.TshLevel <= 10)
                                {
                                    text = "Вам нужно увеличить дозу на ";
                                    text += (User.DateOfBirth.CompareTo(DateTime.Now.AddYears(-70)) < 0 || User.Weight < 55) ? "12,5 мкг." :
                                        "25 мкг.";
                                    text += "Контроль ТТГ через 2 месяца.";
                                    User.checkinterval = 60;
                                }
                                else if (User.Tests?.Last()?.TshLevel > 10)
                                {
                                    text = "У вас выраженный недостаток гормонов. Вам нужно срочно увеличить дозу " +
                                        "на 25 мкг. Контроль ТТГ через 1,5 месяца.";
                                    User.checkinterval = 45;
                                }
                                else
                                {
                                    text = $"У вас отличный уровень ТТГ. Продолжайте принимать " +
                                    $"{((DescriptionAttribute)User.TreatmentDrug.GetType().GetMember(User.TreatmentDrug.ToString())[0].GetCustomAttributes(typeof(DescriptionAttribute), false)[0]).Description} " +
                                    $"по {User.TreatmentDose} мкг. Следующий раз контроль через ";
                                    if (User.checkinterval >= 180)
                                    {
                                        text += "год.";
                                        User.checkinterval = 365;
                                    }
                                    else
                                    {
                                        text += "6 месяцев.";
                                        User.checkinterval = 180;
                                    }
                                }
                                User.Mode = ModeType.OnReminder;
                            }
                            else
                            {
                                text = $"У вас отличный уровень ТТГ. Продолжайте принимать " +
                                $"{((DescriptionAttribute)User.TreatmentDrug.GetType().GetMember(User.TreatmentDrug.ToString())[0].GetCustomAttributes(typeof(DescriptionAttribute), false)[0]).Description} " +
                                $"по {User.TreatmentDose} мкг. Следующий раз контроль через ";
                                if (User.checkinterval >= 180)
                                {
                                    text += "год.";
                                    User.checkinterval = 365;
                                }
                                else
                                {
                                    text += "6 месяцев.";
                                    User.checkinterval = 180;
                                }
                                User.Mode = ModeType.OnReminder;
                            }
                        }
                        db.Users.Update(User);
                        await db.SaveChangesAsync();
                        tts = text.Replace("ТТГ", "тэтэг+э").Replace("мкМЕ/мл", "эмк+а эм е на миллилитр");
                        if (User.Mode == ModeType.OnReminder)
                        {
                            buttons = new List<ButtonModel>() { new ButtonModel("Я сдал анализы", true), new ButtonModel("Когда мне сдавать анализы?", true),
                        new ButtonModel("Мои прошлые ТТГ?", true), new ButtonModel("У меня другая доза лекарства", true) };

                        }
                        var response = new AliceResponse(aliceRequest, text, tts, buttons)
                        {
                            SessionState = aliceRequest.State.Session,
                        };
                        return response;
                    }
            }
        }
    }
}