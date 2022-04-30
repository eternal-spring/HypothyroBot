using HypothyroBot.Models.Alice_API;
using HypothyroBot.Models.Session.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace HypothyroBot.Models.Session
{
    public class AddingUserMode : IMode
    {
        private User User;
        public AddingUserMode(User user)
        {
            User = user;
        }
        public async Task<AliceResponse> HandleRequest(AliceRequest aliceRequest, ApplicationContext db)
        {
            string text = "";
            var buttons = new List<ButtonModel>();
            if (User?.Id == null)
            {
                User = new User
                {
                    Id = aliceRequest.Session.UserId,
                };
                db.Users.Add(User);
                await db.SaveChangesAsync();
                text = "Привет, я навык для наблюдения за функцией щитовидной железы у оперированных пациентов " +
                                                      "или контроля заместительной терапии. Давайте знакомиться! Представьтесь";
                return new AliceResponse(aliceRequest, text, buttons)
                {
                    SessionState = new SessionState() { Authorised = true, Id = User.Id, LastResponse = text, LastButtons = buttons },
                };
            }
            else if (aliceRequest.Session.New)
            {
                
                text = aliceRequest.State.Session.LastResponse;
                buttons = aliceRequest.State.Session.LastButtons;
            }
            else if (User.Name == null)
            {
                User.Name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(aliceRequest.Request.OriginalUtterance.Trim().ToLower());
                text = "Скажите дату рождения";
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
                }
                catch
                {
                    return new AliceResponse(aliceRequest, "Не поняла, повторите.");
                }
                buttons = new List<ButtonModel>() { new ButtonModel("мужской", true), new ButtonModel("женский", true),
                    new ButtonModel("отказываюсь отвечать", true) };
                text = "Ваш пол?";
            }
            else if (User.Gender == GenderType.None)
            {
                if (aliceRequest.Request.Nlu.Tokens.First().Contains("муж"))
                {
                    User.Gender = GenderType.Male;
                    User.isPregnant = 0;
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
                    return new AliceResponse(aliceRequest, "Не поняла, повторите.", buttons);
                }
                text = "Сколько вы весите?";
            }
            else if (User.Weight == 0)
            {
                try
                {
                    var weight = (from w in aliceRequest.Request.Nlu.Entities
                                  where aliceRequest.Request.Nlu.Entities.Any()
                                    && (w as NumberModel != null)
                                  select (w as NumberModel).Value).First();
                    User.Weight = (double)weight;
                }
                catch
                {
                    return new AliceResponse(aliceRequest, "Не поняла, повторите.");
                }
                if ((User.Gender == GenderType.Female || User.Gender == GenderType.Unknown) && 18 < 
                    DateTime.Now.Subtract(User.DateOfBirth).TotalDays / 365.2425 && DateTime.Now.Subtract(User.DateOfBirth).TotalDays / 365.2425 < 45)
                {
                    buttons = new List<ButtonModel>() { new ButtonModel("да", true), new ButtonModel("нет", true), };
                    text = "Вы беременны?";
                }
                else
                {
                    buttons = new List<ButtonModel>() { new ButtonModel("да", true), new ButtonModel("нет", true), };
                    text = "До операции приходилось ли принимать тироксин?";
                }
            }
            else if ((User.Gender == GenderType.Female || User.Gender == GenderType.Unknown) && 18 < 
                DateTime.Now.Subtract(User.DateOfBirth).TotalDays / 365.2425 && DateTime.Now.Subtract(User.DateOfBirth).TotalDays / 365.2425 < 45 && User.isPregnant == -1)
            {
                if (aliceRequest.Request.Nlu.Tokens.First().StartsWith("да"))
                {
                    User.isPregnant = 1;
                    db.Users.Update(User);
                    await db.SaveChangesAsync();
                    text = "На данный момент ведение беременности не поддерживается, Спасибо, до свидания.";
                    return new AliceResponse(aliceRequest, text, true)
                    {
                        SessionState = new SessionState() {Authorised = true, Id = User.Id, LastResponse = text },
                    };
                }
                if (aliceRequest.Request.Nlu.Tokens.First().StartsWith("не"))
                {
                    User.isPregnant = 0;
                    buttons = new List<ButtonModel>() { new ButtonModel("да", true), new ButtonModel("нет", true), };
                    text = "До операции приходилось ли принимать тироксин?";
                }
                else
                {
                    buttons = new List<ButtonModel>() { new ButtonModel("да", true), new ButtonModel("нет", true), };
                    return new AliceResponse(aliceRequest, "Не поняла, повторите.", buttons);
                }
            }
            else if (User.PretreatmentDose == -2)
            {
                if (aliceRequest.Request.Nlu.Tokens.First().StartsWith("да"))
                {
                    User.PretreatmentDose = -1;
                    text = "Сколько мкг?";
                }
                else if (aliceRequest.Request.Nlu.Tokens.First().StartsWith("не"))
                {
                    User.PretreatmentDose = 0;
                    text = "Когда была операция?";
                }
                else
                {
                    buttons = new List<ButtonModel>() { new ButtonModel("да", true), new ButtonModel("нет", true), };
                    return new AliceResponse(aliceRequest, "Не поняла, повторите.", buttons);
                }
            }
            else if (User.PretreatmentDose == -1)
            {
                try
                {
                    var dose = (from d in aliceRequest.Request.Nlu.Entities where (d as NumberModel != null) select (d as NumberModel).Value).First();
                    User.PretreatmentDose = (double)dose;
                    buttons = new List<ButtonModel>() { new ButtonModel("эутирокс", true), new ButtonModel("L-тироксин", true),
                                                                                                                new ButtonModel("другой", true) };
                    text = "Какой препарат?";
                }
                catch
                {
                    return new AliceResponse(aliceRequest, "Не поняла, повторите.");
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
                    return new AliceResponse(aliceRequest, "Не поняла, повторите.", buttons);
                }
                text = "Когда была операция?";
            }
            else if (User.DateOfOperation == default)
            {
                try
                {
                    var od = (from date in aliceRequest.Request.Nlu.Entities
                               where (date as DateTimeModel != null)
                               select (date as DateTimeModel).Value).First();
                    if (od != null)
                    {
                        if ((int)od.Year < 30)
                        {
                            od.Year += 2000;
                        }
                        else if ((int)od.Year < 100)
                        {
                            od.Year += 1900;
                        }
                        User.DateOfOperation = new DateTime((int)od.Year, (int)od.Month, (int)od.Day);
                        buttons = new List<ButtonModel>() { new ButtonModel("Да", true), new ButtonModel("Половина", true),
                            new ButtonModel("Перешеек", true), new ButtonModel("Оставлен небольшой остаток доли",true), 
                            new ButtonModel("Затрудняюсь ответить",true) };
                        text = "Щитовидная железа была удалена вся?";
                    }
                }
                catch
                {
                    return new AliceResponse(aliceRequest, "Не поняла, повторите.");
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
                    return new AliceResponse(aliceRequest, "Не поняла, повторите.", b);
                }
                buttons = new List<ButtonModel>() { new ButtonModel("Фолликулярная аденома/ узловой нетоксический зоб", true),
                        new ButtonModel("Тиреоидит Хашимото / диффузный токсический зоб", true),
                    new ButtonModel("Папиллярная / фолликулярная карцинома", true),
                                      new ButtonModel("Медуллярная карцинома",true), new ButtonModel("Другое",true) };
                text = "Что пришло по гистологии?";
            }
            else if (User.Pathology == PathologyType.None)
            {
                var nng = new string[] { "фолликулярная аденома", "узловой", "нетокс" };
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
                    return new AliceResponse(aliceRequest, "Не поняла, повторите.", b);
                }
                if (User.PretreatmentDose == 0)
                {
                    buttons = new List<ButtonModel>() { new ButtonModel("Да", true), new ButtonModel("Нет", true) };
                    text = "Была ли назначена терапия левотироксином?";
                }
                else
                {
                    text = "Если терапия проводилась ранее, то какая после? (Сколько мкг?)";
                }
            }
            else if (User.PretreatmentDose == 0 && User.TreatmentDose == -2)
            {
                if (aliceRequest.Request.Command.Contains("да"))
                {
                    User.TreatmentDose = -1;
                    text = "Сколько мкг?";
                }
                else if (aliceRequest.Request.Command.Contains("не"))
                {
                    User.TreatmentDose = 0;
                    db.Users.Update(User);
                    await db.SaveChangesAsync();
                    return await new RelevanceAssessmentMode(User).HandleRequest(aliceRequest, db);
                }
                else
                {
                    buttons = new List<ButtonModel>() { new ButtonModel("Да", true), new ButtonModel("Нет", true) };
                    return new AliceResponse(aliceRequest, "Не поняла, повторите.", buttons);
                }
            }
            else if (User.PretreatmentDose == 0 && User.TreatmentDose == -1)
            {
                try
                {
                    var dose = (from d in aliceRequest.Request.Nlu.Entities where (d as NumberModel != null) select (d as NumberModel)?.Value)?.First();
                    if (dose != null)
                    {
                        User.TreatmentDose = (double)dose;
                        buttons = new List<ButtonModel>() { new ButtonModel("эутирокс", true), new ButtonModel("L-тироксин", true),
                                                                                                                new ButtonModel("другой", true) };
                        text = "Какой препарат?";
                    }
                }
                catch
                {
                    return new AliceResponse(aliceRequest, "Не поняла, повторите.");
                }
            }
            else if (User.PretreatmentDose == 0 && User.TreatmentDose > 0 && User.TreatmentDrug == DrugType.None)
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
                    return new AliceResponse(aliceRequest, "Не поняла, повторите.", buttons);
                }
                db.Users.Update(User);
                await db.SaveChangesAsync();
                return await new RelevanceAssessmentMode(User).HandleRequest(aliceRequest, db);
            }
            else if (User.TreatmentDose == -2)
            {
                try
                {
                    var dose = (from d in aliceRequest.Request.Nlu.Entities where (d as NumberModel != null) select (d as NumberModel).Value).First();
                    User.TreatmentDose = (double)dose;
                    if (User.TreatmentDose > 0)
                    {
                        buttons = new List<ButtonModel>() { new ButtonModel("Да", true), new ButtonModel("Нет", true) };
                        text = "Препарат прежний?";
                    }
                    else if (User.TreatmentDose == 0)
                    {
                        db.Users.Update(User);
                        await db.SaveChangesAsync();
                        return await new RelevanceAssessmentMode(User).HandleRequest(aliceRequest, db);
                    }
                }
                catch
                {
                    return new AliceResponse(aliceRequest, "Не поняла, повторите.");
                }
            }
            else if (User.TreatmentDose > 0 && User.TreatmentDrug == DrugType.None)
            {
                var lt = new string[] { "l-тироксин", "элтироксин", "эл тироксин" , "l тироксин", "л тироксин", "эль тироксин", "эльтироксин" };
                if (aliceRequest.Request.Command.Contains("да"))
                {
                    User.TreatmentDrug = User.PretreatmentDrug;
                }
                else if (aliceRequest.Request.Command.Contains("не"))
                {
                    buttons = new List<ButtonModel>() { new ButtonModel("эутирокс", true), new ButtonModel("L-тироксин", true),
                                                                                                                new ButtonModel("другой", true) };
                    text = "А какой?";
                    return new AliceResponse(aliceRequest, text, buttons)
                    {
                        SessionState = new SessionState() { Authorised = true, LastResponse = text, LastButtons = buttons },
                    };
                }
                else
                {
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
                        return new AliceResponse(aliceRequest, "Не поняла, повторите.");
                    }
                }
                db.Users.Update(User);
                await db.SaveChangesAsync();
                return await new RelevanceAssessmentMode(User).HandleRequest(aliceRequest, db);
            }
            db.Users.Update(User);
            await db.SaveChangesAsync();
            string tts = text.Replace("ТТГ", "тэтэг+э").Replace("мкМЕ/мл", "эмк+а эм е на миллилитр");
            var response = new AliceResponse(aliceRequest, text, tts, buttons)
            {
                SessionState = new SessionState() { Authorised = true, Id = User.Id, LastResponse = text, LastButtons = buttons },
            };
            return response;
        }
    }
}