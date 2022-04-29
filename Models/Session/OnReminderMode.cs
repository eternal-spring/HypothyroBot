using HypothyroBot.Models.Alice_API;
using HypothyroBot.Models.Session.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace HypothyroBot.Models.Session
{
    public class OnReminderMode : IMode
    {
        private User User; 
        public OnReminderMode(User user)
        {
            User = user;
        }
        public async Task<AliceResponse> HandleRequest(AliceRequest aliceRequest, ApplicationContext db)
        {
            string text = "";
            string tts = "";
            if (User.TreatmentDose == -2 || (User.TreatmentDose > 0 && User.TreatmentDrug == DrugType.None))
            {
                return await new UserDataCorrectionMode(User).HandleRequest(aliceRequest, db);
            }
            if (aliceRequest.Session.New)
            {
                text = $"Добрый день, {User.Name}. ";
                var checkDate = User.Tests.Any() ? User.Tests.Where(t => t.Actual).Last().TestDate : User.DateOfOperation;
                var daysPassed = (DateTime.Now - checkDate).TotalDays;
                if (daysPassed < User.checkinterval)
                {
                    if ((User.checkinterval - daysPassed) < 10)
                    {
                        text += "Если вы еще не сдали ТТГ, то поспешите, рекомендованный интервал заканчивается через неделю. ";
                        text += "Вы сдали ТТГ?";
                        tts = text.Replace("ТТГ", "тэтэг+э");
                        return new AliceResponse(aliceRequest, text, new List<ButtonModel>() { new ButtonModel("да", true), new ButtonModel("нет", true) })
                        {
                            SessionState = new SessionState() { Authorised = true, Id = User.Id, LastResponse = text },
                        };
                    }
                    else if (daysPassed >= 42)
                    {
                        text += $"Уже прошло {Math.Truncate(daysPassed / 7)} недель с момента ";
                        text += User.Tests.Any() ? "смены терапии. " : "операции. ";
                        text += "Вы сдали ТТГ? ";
                        tts = text.Replace("ТТГ", "тэтэг+э");
                        return new AliceResponse(aliceRequest, text, new List<ButtonModel>() { new ButtonModel("да", true), new ButtonModel("нет", true) })
                        {
                            SessionState = new SessionState() { Authorised = true, Id = User.Id, LastResponse = text },
                        };
                    }
                    else if (daysPassed >= 28)
                    {

                        text += User.Tests.Any() ? "После изменения дозировки " : "После назначения лекарственной терапии ";
                        text += "контроль обычно осуществляется через 4-8 недель, найдите время и сдайте ТТГ.";
                    }
                    else
                    {
                        text += "Вы всегда можете сообщить мне об изменениях в самочувствии, терапии, данных анализов.";
                    }
                }
                else if (daysPassed >= User.checkinterval)
                {
                    text += "Пришло время контроля ТТГ.  Вы сдали ТТГ?";
                    tts = text.Replace("ТТГ", "тэтэг+э");
                    return new AliceResponse(aliceRequest, text, new List<ButtonModel>() { new ButtonModel("да", true), new ButtonModel("нет", true) })
                    {
                        SessionState = new SessionState() { Authorised = true, Id = User.Id, LastResponse = text },
                    };
                }
            }
            else
            {
                if (aliceRequest.Request.Command.Contains("сдал"))
                {
                    return await new ResultsCollectingMode(User).HandleRequest(aliceRequest, db);
                }
                else if (aliceRequest.Request.Command.Contains("когда"))
                {
                    text = $"Вам нужно сдать анализы не позднее, чем ";
                    if (User.Tests?.Where(t => t.Actual).Count() == 0)
                        text += $"{User.DateOfOperation.AddDays(User.checkinterval).ToString("d MMMM yyyy", CultureInfo.CreateSpecificCulture("ru-RU"))}.";
                    else
                        text += $"{User.Tests.Where(t => t.Actual).Last().TestDate.AddDays(User.checkinterval).ToString("d MMMM yyyy", CultureInfo.CreateSpecificCulture("ru-RU"))}.";
                }
                else if (aliceRequest.Request.Command.Contains("прошл"))
                {
                    if (User.Tests.Any())
                    {
                        var lastTest = User.Tests?.OrderBy(t => t.TestDate).Last();
                        text = $"Ваш последний анализ: {lastTest.TshLevel} мкМЕ/мл, ";
                        text += $"дата сдачи: {lastTest.TestDate.ToString("d MMMM yyyy", CultureInfo.CreateSpecificCulture("ru-RU"))}. ";
                        text += $"Хотите узнать более ранние ТТГ?";
                        tts = text.Replace("ТТГ", "тэтэг+э").Replace("мкМЕ/мл", "эмк+а эм е на миллилитр");
                        return new AliceResponse(aliceRequest, text, tts, new List<ButtonModel>() { new ButtonModel("да", true), new ButtonModel("нет", true) })
                        {
                            SessionState = new SessionState() { Authorised = true, Id = User.Id, LastResponse = text },
                        };
                    }
                    else
                    {
                        text = "Вы ещё не сдавали ТТГ.";
                    }
                }
                else if (aliceRequest.Request.Command.Contains("да"))
                {
                    if (aliceRequest.State.Session.LastResponse.Contains("Вы сдали ТТГ?"))
                    {
                        return await new ResultsCollectingMode(User).HandleRequest(aliceRequest, db);
                    }
                    else
                    {
                        text = $"Ваши ТТГ: ";
                        foreach (var test in User.Tests.OrderBy(t => t.TestDate))
                        {
                            text += $"{test.TestDate.ToString("d MMMM yyyy", CultureInfo.CreateSpecificCulture("ru-RU"))}, ";
                            text += $" {test.TshLevel} мкМЕ/мл; ";
                        }
                        text += "вы всегда можете сообщить мне об изменениях в самочувствии, терапии, данных анализов.";
                    }
                }
                else if (aliceRequest.Request.Command.Contains("нет"))
                {
                    text = "Вы всегда можете сообщить мне об изменениях в самочувствии, терапии, данных анализов.";
                }
                else if (aliceRequest.Request.Command.Contains("другая доза"))
                {
                    return await new UserDataCorrectionMode(User).HandleRequest(aliceRequest, db);
                }
            }
            tts = text.Replace("ТТГ", "тэтэг+э").Replace("мкМЕ/мл","эмк+а эм е на миллилитр");
            var buttons = new List<ButtonModel>() { new ButtonModel("Я сдал анализы", true), new ButtonModel("Когда мне сдавать анализы?", true),
                        new ButtonModel("Мои прошлые ТТГ?", true), new ButtonModel("У меня другая доза лекарства", true) };
            var response = new AliceResponse(aliceRequest, text, tts, buttons)
            {
                SessionState = new SessionState() {Authorised = true, Id = User.Id, LastResponse = text },
            };
            return response;
        }
    }
}