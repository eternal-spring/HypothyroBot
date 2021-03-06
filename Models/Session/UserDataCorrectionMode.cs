using HypothyroBot.Models.Alice_API;
using HypothyroBot.Models.Session.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace HypothyroBot.Models.Session
{
    public class UserDataCorrectionMode : IMode
    {
        private User User;
        public UserDataCorrectionMode(User user)
        {
            User = user;
        }
        public async Task<AliceResponse> HandleRequest(AliceRequest aliceRequest, ApplicationContext db)
        {
            string text = "";
            var buttons = new List<ButtonModel>();
            switch (User.Mode)
            {
                case ModeType.RelevanceAssessment:
                    {
                        text = "Скажите, что нужно исправить?";
                        buttons = new List<ButtonModel>() { new ButtonModel("Имя", true), new ButtonModel("Пол", true),
                                new ButtonModel("Вес", true), new ButtonModel("Дата рождения", true), new ButtonModel("Терапия до операции", true),
                                new ButtonModel("Дата операции", true), new ButtonModel("Что с щитовидкой", true),
                                new ButtonModel("Что пришло по гистологии", true), new ButtonModel("Терапия после операции", true), new ButtonModel("Всё верно",true)};
                        User.Mode = ModeType.UserDataCorrection;
                        db.Users.Update(User);
                        await db.SaveChangesAsync();
                        var response = new AliceResponse(aliceRequest, text, buttons)
                        {
                            SessionState = new SessionState() { Authorised = true, Id = User.Id, LastResponse = text, LastButtons = buttons },
                        };
                        return response;
                    }
                default:
                    {
                        if (User.Name == null || User.DateOfBirth == default || User.Gender == GenderType.None || User.Weight == 0 || User.PretreatmentDose == -2
                            || User.PretreatmentDose > 0 && User.PretreatmentDrug == DrugType.None || User.DateOfOperation == default || User.ThyroidCondition == ThyroidType.None
                            || User.Pathology == PathologyType.None || User.TreatmentDose == -2 || User.TreatmentDose > 0 && User.TreatmentDrug == DrugType.None)
                        {
                            if (User.Name == null)
                            {
                                User.Name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(aliceRequest.Request.OriginalUtterance.Trim().ToLower());
                                text = $"Ваше имя - {User.Name}.";
                            }
                            else if (User.DateOfBirth == default)
                            {
                                try
                                {
                                    var bd = (from date in aliceRequest.Request.Nlu.Entities
                                              where (aliceRequest.Request.Nlu.Entities.Any() && date as DateTimeModel != null)
                                              select (date as DateTimeModel).Value).First();
                                    if (bd != null)
                                    {
                                        if ((int)bd.Year < 30)
                                        {
                                            bd.Year += 2000;
                                        }
                                        else if ((int)bd.Year < 100)
                                        {
                                            bd.Year += 1900;
                                        }
                                        User.DateOfBirth = new DateTime((int)bd.Year, (int)bd.Month, (int)bd.Day);
                                    }
                                    if ((int.Parse(DateTime.Now.ToString("yyyyMMdd")) - int.Parse(User.DateOfBirth.ToString("yyyyMMdd"))) / 10000 < 18)
                                    {
                                        text = "Навык предназначен для совершеннолетних пользователей, Спасибо, до свидания.";
                                        return new AliceResponse(aliceRequest, text, true);
                                    }
                                    text = $"Ваша дата рождения - {User.DateOfBirth.ToString("d MMMM yyyy", CultureInfo.CreateSpecificCulture("ru-RU"))}.";
                                }
                                catch
                                {
                                    return new AliceResponse(aliceRequest, "Не поняла, повторите");
                                }
                            }
                            else if (User.Gender == GenderType.None)
                            {
                                if (aliceRequest.Request.Nlu.Tokens.First().Contains("муж"))
                                {
                                    User.Gender = GenderType.Male;
                                }
                                else if (aliceRequest.Request.Nlu.Tokens.First().Contains("жен"))
                                {
                                    User.Gender = GenderType.Female;
                                }
                                else if (aliceRequest.Request.Command.Contains("отказываюсь отвечать"))
                                {
                                    User.Gender = GenderType.Unknown;
                                }
                                else
                                {
                                    buttons = new List<ButtonModel>() { new ButtonModel("мужской", true), new ButtonModel("женский", true),
                                new ButtonModel("отказываюсь отвечать", true) };
                                    return new AliceResponse(aliceRequest, "Не поняла, повторите", buttons);
                                }
                                if (!(User.Gender == GenderType.Unknown))
                                {
                                    text = $"Ваш пол - { ((DescriptionAttribute)User.Gender.GetType().GetMember(User.Gender.ToString())[0].GetCustomAttributes(typeof(DescriptionAttribute), false)[0]).Description}. ";
                                }
                                else
                                {
                                    text = "Вы отказались называть свой пол";
                                }
                            }
                            else if (User.Weight == 0)
                            {
                                try
                                {
                                    var weight = (from w in aliceRequest.Request.Nlu.Entities
                                                  where (aliceRequest.Request.Nlu.Entities.Any()
                                                    && (w as NumberModel != null))
                                                  select (w as NumberModel).Value).First();
                                    User.Weight = (double)weight;
                                }
                                catch
                                {
                                    return new AliceResponse(aliceRequest, "Не поняла, повторите");
                                }
                                text = $"Ваш вес - {User.Weight}.";
                            }
                            else if (User.PretreatmentDose == -2)
                            {
                                try
                                {
                                    var dose = (from d in aliceRequest.Request.Nlu.Entities where (d as NumberModel != null) select (d as NumberModel).Value).First();
                                    User.PretreatmentDose = (double)dose;
                                    db.Users.Update(User);
                                    await db.SaveChangesAsync();
                                    text = "Вы не принимали тироксин до операции.";
                                    if (User.PretreatmentDose > 0)
                                    {
                                        buttons = new List<ButtonModel>() { new ButtonModel("эутирокс", true), new ButtonModel("L-тироксин", true),
                                                                                                                new ButtonModel("другой", true) };
                                        text = "Какой препарат?";
                                        var response = new AliceResponse(aliceRequest, text, buttons)
                                        {
                                            SessionState = new SessionState() { Authorised = true, Id = User.Id, LastResponse = text, LastButtons = buttons },
                                        };
                                        return response;
                                    }
                                }
                                catch
                                {
                                    return new AliceResponse(aliceRequest, "Не поняла, повторите");
                                }
                            }
                            else if (User.PretreatmentDose > 0 && User.PretreatmentDrug == DrugType.None)
                            {
                                var lt = new string[] { "l-тироксин", "элтироксин", "эл тироксин", "l тироксин", "л тироксин", "эль тироксин", "эльтироксин" };
                                if (aliceRequest.Request.Command.Contains("эутирокс"))
                                {
                                    User.PretreatmentDrug = DrugType.Eutirox;
                                }
                                else if (lt.Any(aliceRequest.Request.Command.Contains))
                                {
                                    User.PretreatmentDrug = DrugType.LThyroxine;
                                }
                                else if (aliceRequest.Request.Command.Contains("друг"))
                                {
                                    User.PretreatmentDrug = DrugType.Another;
                                }
                                else
                                {
                                    buttons = new List<ButtonModel>() { new ButtonModel("эутирокс", true), new ButtonModel("L-тироксин", true),
                                                                                                                new ButtonModel("другой", true) };
                                    return new AliceResponse(aliceRequest, "Не поняла, повторите", buttons);
                                }
                                text = $"Вы принимали {User.PretreatmentDose} мкг левотироксина до операции. ";
                                if (User.PretreatmentDrug != DrugType.Another)
                                {
                                    text += $"Препарат - { ((DescriptionAttribute)User.PretreatmentDrug.GetType().GetMember(User.PretreatmentDrug.ToString())[0].GetCustomAttributes(typeof(DescriptionAttribute), false)[0]).Description}. ";
                                }
                            }
                            else if (User.DateOfOperation == default)
                            {
                                try
                                {
                                    var bd = (from date in aliceRequest.Request.Nlu.Entities
                                              where (aliceRequest.Request.Nlu.Entities.Any() && date as DateTimeModel != null)
                                              select (date as DateTimeModel).Value).First();
                                    if (bd != null)
                                    {
                                        if ((int)bd.Year < 30)
                                        {
                                            bd.Year += 2000;
                                        }
                                        else if ((int)bd.Year < 100)
                                        {
                                            bd.Year += 1900;
                                        }
                                        User.DateOfOperation = new DateTime((int)bd.Year, (int)bd.Month, (int)bd.Day);
                                    }
                                    text = $"Дата вашей операции - {User.DateOfOperation.ToString("d MMMM yyyy", CultureInfo.CreateSpecificCulture("ru-RU"))}.";
                                }
                                catch
                                {
                                    return new AliceResponse(aliceRequest, "Не поняла, повторите");
                                }
                            }
                            else if (User.ThyroidCondition == ThyroidType.None)
                            {
                                var cr = new string[] { "да", "вся", "полностью", "целиком" };
                                var hr = new string[] { "половин" };
                                var ir = new string[] { "перешеек" };
                                var lrl = new string[] { "остав" };
                                var un = new string[] { "не знаю", "затрудняюсь" };
                                if (cr.Any(aliceRequest.Request.Command.Contains))
                                {
                                    User.ThyroidCondition = ThyroidType.CompletelyRemoved;
                                }
                                else if (hr.Any(aliceRequest.Request.Command.Contains))
                                {
                                    User.ThyroidCondition = ThyroidType.HalfRemoved;
                                }
                                else if (ir.Any(aliceRequest.Request.Command.Contains))
                                {
                                    User.ThyroidCondition = ThyroidType.IsthmusRemoved;
                                }
                                else if (lrl.Any(aliceRequest.Request.Command.Contains))
                                {
                                    User.ThyroidCondition = ThyroidType.LobeRemainderLeft;
                                }
                                else if (un.Any(aliceRequest.Request.Command.Contains))
                                {
                                    User.ThyroidCondition = ThyroidType.Unknown;
                                }
                                else
                                {
                                    var b = new List<ButtonModel>() { new ButtonModel("Да", true), new ButtonModel("Половина", true),
                                new ButtonModel("Перешеек", true), new ButtonModel("Оставлен небольшой остаток доли",true),
                                new ButtonModel("Затрудняюсь ответить",true) };
                                    return new AliceResponse(aliceRequest, "Не поняла, повторите", b);
                                }
                                if (User.ThyroidCondition == ThyroidType.CompletelyRemoved)
                                {
                                    text += "Щитовидка была удалена вся. ";
                                }
                                else if (User.ThyroidCondition == ThyroidType.HalfRemoved)
                                {
                                    text += "Щитовидка была удалена наполовину. ";
                                }
                                else if (User.ThyroidCondition == ThyroidType.IsthmusRemoved)
                                {
                                    text += "Был удалён перешеек щитовидки. ";
                                }
                                else if (User.ThyroidCondition == ThyroidType.LobeRemainderLeft)
                                {
                                    text += "Был оставлен небольшой остаток доли щитовидки. ";
                                }
                            }
                            else if (User.Pathology == PathologyType.None)
                            {
                                var nng = new string[] { "аденома", "узловой", "нетокс" };
                                var dtg = new string[] { "тиреоидит", "хашимото", "диффузный", "токс" };
                                var pfc = new string[] { "папиллярная", "фолликулярная карцинома" };
                                var mc = new string[] { "медуллярная" };
                                var a = new string[] { "не знаю", "друг" };
                                if (nng.Any(aliceRequest.Request.Command.Contains))
                                {
                                    User.Pathology = PathologyType.NodularNonToxicGoiter;
                                }
                                else if (dtg.Any(aliceRequest.Request.Command.Contains))
                                {
                                    User.Pathology = PathologyType.DiffuseToxicGoiter;
                                }
                                else if (pfc.Any(aliceRequest.Request.Command.Contains))
                                {
                                    User.Pathology = PathologyType.PapillaryOrFollicularCarcinoma;
                                    User.upTshLevel = 2;
                                }
                                else if (mc.Any(aliceRequest.Request.Command.Contains))
                                {
                                    User.Pathology = PathologyType.MedullaryCarcinoma;
                                }
                                else if (a.Any(aliceRequest.Request.Command.Contains))
                                {
                                    User.Pathology = PathologyType.Another;
                                }
                                else
                                {
                                    var b = new List<ButtonModel>() { new ButtonModel("Фолликулярная аденома/ узловой нетоксический зоб", true),
                                new ButtonModel("Тиреоидит Хашимото / диффузный токсический зоб", true),
                                new ButtonModel("Папиллярная / фолликулярная карцинома", true),
                                  new ButtonModel("Медуллярная карцинома",true), new ButtonModel("Другое",true) };
                                    return new AliceResponse(aliceRequest, "Не поняла, повторите", b);
                                }
                                text += $"По гистологии пришло: " +
                                    $"{ ((DescriptionAttribute)User.Pathology.GetType().GetMember(User.Pathology.ToString())[0].GetCustomAttributes(typeof(DescriptionAttribute), false)[0]).Description}. ";
                            }
                            else if (User.TreatmentDose == -2)
                            {
                                try
                                {
                                    var dose = (from d in aliceRequest.Request.Nlu.Entities where (d as NumberModel != null) select (d as NumberModel).Value).First();
                                    User.TreatmentDose = (double)dose;
                                    text = "Вам не был назначен тироксин после операции.";
                                    db.Users.Update(User);
                                    await db.SaveChangesAsync();
                                    if (User.TreatmentDose > 0)
                                    {
                                        buttons = new List<ButtonModel>() { new ButtonModel("эутирокс", true), new ButtonModel("L-тироксин", true),
                                                                                                                new ButtonModel("другой", true) };
                                        text = "Какой препарат?";
                                        var response = new AliceResponse(aliceRequest, text, buttons)
                                        {
                                            SessionState = new SessionState() { Authorised = true, Id = User.Id, LastResponse = text, LastButtons = buttons },
                                        };
                                        return response;

                                    }
                                }
                                catch
                                {
                                    return new AliceResponse(aliceRequest, "Не поняла, повторите");
                                }
                            }
                            else if (User.TreatmentDose > 0 && User.TreatmentDrug == DrugType.None)
                            {
                                var lt = new string[] { "l-тироксин", "элтироксин", "эл тироксин", "l тироксин", "л тироксин", "эль тироксин", "эльтироксин" };
                                if (aliceRequest.Request.Command.Contains("эутирокс"))
                                {
                                    User.TreatmentDrug = DrugType.Eutirox;
                                }
                                else if (lt.Any(aliceRequest.Request.Command.Contains))
                                {
                                    User.TreatmentDrug = DrugType.LThyroxine;
                                }
                                else if (aliceRequest.Request.Command.Contains("друг"))
                                {
                                    User.TreatmentDrug = DrugType.Another;
                                }
                                else
                                {
                                    buttons = new List<ButtonModel>() { new ButtonModel("эутирокс", true), new ButtonModel("L-тироксин", true),
                                                                                                                new ButtonModel("другой", true) };
                                    return new AliceResponse(aliceRequest, "Не поняла, повторите", buttons);
                                }
                                text = $"Теперь вы принимаете {User.TreatmentDose} мкг левотироксина.";
                                if (User.TreatmentDrug != DrugType.Another)
                                {
                                    text += $"Препарат - { ((DescriptionAttribute)User.TreatmentDrug.GetType().GetMember(User.TreatmentDrug.ToString())[0].GetCustomAttributes(typeof(DescriptionAttribute), false)[0]).Description}. ";
                                }
                            }
                            text += " Всё верно? ";
                            string tts = text.Replace("мкг", "микрограмм");
                            buttons = new List<ButtonModel> { new ButtonModel("да", true), new ButtonModel("нет", true) };
                            db.Users.Update(User);
                            await db.SaveChangesAsync();
                            return new AliceResponse(aliceRequest, text, tts, buttons);
                        }
                        else
                        {
                            if (aliceRequest.Request.Command.Contains("имя"))
                            {
                                text = "Назовите имя";
                                User.Name = null;
                            }
                            else if (aliceRequest.Request.Command.Contains("пол"))
                            {
                                text = "Скажите ваш пол";
                                buttons = new List<ButtonModel>() { new ButtonModel("мужской", true), new ButtonModel("женский", true),
                                new ButtonModel("отказываюсь отвечать", true) };
                                User.Gender = GenderType.None;
                            }
                            else if (aliceRequest.Request.Command.Contains("вес"))
                            {
                                text = "Скажите ваш вес в кг";
                                User.Weight = 0;
                            }
                            else if (aliceRequest.Request.Command.Contains("дата рожд"))
                            {
                                text = "Назовите дату рождения";
                                User.DateOfBirth = default;
                            }
                            else if (aliceRequest.Request.Command.Contains("терапия до опер"))
                            {
                                text = "Скажите, сколько мкг тироксина вы принимали до операции?";
                                User.PretreatmentDose = -2;
                                User.PretreatmentDrug = DrugType.None;
                            }
                            else if (aliceRequest.Request.Command.Contains("дата опер"))
                            {
                                text = "Назовите дату операции";
                                User.DateOfOperation = default;
                            }
                            else if (aliceRequest.Request.Command.Contains("щитовид"))
                            {
                                buttons = new List<ButtonModel>() { new ButtonModel("Да", true), new ButtonModel("Половина", true),
                            new ButtonModel("Перешеек", true), new ButtonModel("Оставлен небольшой остаток доли",true),
                            new ButtonModel("Затрудняюсь ответить",true) };
                                text = "Щитовидная железа была удалена вся?";
                                User.ThyroidCondition = ThyroidType.None;
                            }
                            else if (aliceRequest.Request.Command.Contains("гист"))
                            {
                                buttons = new List<ButtonModel>() { new ButtonModel("Фолликулярная аденома/ узловой нетоксический зоб", true),
                                new ButtonModel("Тиреоидит Хашимото / диффузный токсический зоб", true),
                                new ButtonModel("Папиллярная / фолликулярная карцинома", true),
                                new ButtonModel("Медуллярная карцинома",true), new ButtonModel("Другое",true) };
                                text = "Что пришло по гистологии?";
                                User.Pathology = PathologyType.None;
                            }
                            else if (aliceRequest.Request.Command.Contains("терапия после опер"))
                            {
                                text = "Скажите, сколько мкг тироксина вы принимаете после операции?";
                                User.TreatmentDose = -2;
                                User.TreatmentDrug = DrugType.None;
                            }
                            else if (aliceRequest.Request.Command.Contains("да"))
                            {
                                return await new RelevanceAssessmentMode(User).HandleRequest(aliceRequest, db);
                            }
                            else if (aliceRequest.Request.Command.Contains("не"))
                            {
                                User.Mode = ModeType.RelevanceAssessment;
                                return await new UserDataCorrectionMode(User).HandleRequest(aliceRequest, db);
                            }
                            db.Users.Update(User);
                            await db.SaveChangesAsync();
                            string tts = text.Replace("ТТГ", "тэтэ+гэ").Replace("мкМЕ/мл", "эмк+а эм е на миллилитр");
                            var response = new AliceResponse(aliceRequest, text, tts, buttons)
                            {
                                SessionState = new SessionState() { Authorised = true, Id = User.Id, LastResponse = text, LastButtons = buttons },
                            };
                            return response;
                        }
                    }
            }
        }
    }
}