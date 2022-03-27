using HypothyroBot.Models.Alice_API;
using HypothyroBot.Models.Session.Interfaces;
using System;
using System.Collections.Generic;
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
                if ((DateTime.Now - User.DateOfOperation).TotalDays >= 28)
                {
                    text = "После назначения лекарственной терапии или изменении дозировки контроль обычно осуществляется через 4-8 недель, " +
                        "найдите время и сдайте ТТГ";
                    tts = "После назначения лекарственной терапии или изменении дозировки контроль обычно осуществляется через 4-8 недель, " +
                        "найдите время и сдайте тиреотропный гормон";
                }
                else if ((DateTime.Now - User.DateOfOperation).TotalDays >= 42)
                {
                    text = "Уже прошло 6 недель с момента смены терапии/операции. Вы сдали ТТГ?";
                    tts = "Уже прошло 6 недель с момента смены терапии/операции. Вы сдали тиреотропный гормон?";
                }
                else if ((DateTime.Now - User.DateOfOperation).TotalDays >= 49)
                {
                    text = "Если вы еще не сдали ТТГ, то поспешите, рекомендованный интервал заканчивается через неделю";
                    tts = "Если вы еще не сдали тиреотропный гормон, то поспешите, рекомендованный интервал заканчивается через неделю";
                }
                else
                {
                    text = "Вы всегда можете сообщить мне об изменениях в самочувствии, терапии, данных анализов.";
                }
            }
            else
            {
                if (aliceRequest.Request.Command.Contains("я сдал анализ"))
                {
                    return await new ResultsCollectingMode(User).HandleRequest(aliceRequest, db);
                }
                else if (aliceRequest.Request.Command.Contains("когда"))
                {
                    text = $"Вам нужно сдать анализы не позднее, чем ";
                    if (User.Tests?.Count == 0)
                        text += $"{User.DateOfOperation.AddDays(User.checkinterval):D}";
                    else
                        text += $"{User.Tests.Last().TestDate.AddDays(User.checkinterval):D}";
                }
                else if (aliceRequest.Request.Command.Contains("прошл"))
                {
                    if (User.Tests.Any())
                    {
                        text = $"Ваш последний анализ {User.Tests?.Last()?.TshLevel} мкМЕ/мл ";
                        text += $"Дата сдачи: {User.Tests?.Last()?.TestDate:D}";
                        text += $"Хотите узнать более ранние ТТГ?";
                        return new AliceResponse(aliceRequest, text, tts, new List<ButtonModel>() { new ButtonModel("да", true), new ButtonModel("нет", true) })
                        {
                            SessionState = new SessionState() { Authorised = true, Id = User.Id, LastResponse = text },
                        };
                    }
                    else
                    {
                        text = "Вы ещё не сдавали ТТГ";
                    }
                }
                else if (aliceRequest.Request.Command.Contains("да"))
                {
                    text = $"Ваши ТТГ: ";
                    foreach (var test in User.Tests)
                    {
                        text += $"{test.TestDate:D} - {test.TshLevel} мкМЕ/мл, ";
                    }
                    text += "Вы всегда можете сообщить мне об изменениях в самочувствии, терапии, данных анализов.";
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