using HypothyroBot.Models.Alice_API;
using HypothyroBot.Models.Session.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HypothyroBot.Models.Session
{
    public class ResultsCollectingMode : IMode
    {
        private User User;
        public ResultsCollectingMode(User user)
        {
            User = user;
        }
        public async Task<AliceResponse> HandleRequest(AliceRequest aliceRequest, ApplicationContext db)
        {
            string text = "";
            string tts = null;
            var buttons = new List<ButtonModel>();
            if (User.Mode != ModeType.ResultsCollecting)
            {
                User.Tests?.Add(new Test {TshLevel = -2, Id = (User.Tests?.Count + 1).ToString() });
                text = "Скажите значение ТТГ (в мкМЕ/мл)";
                User.Mode = ModeType.ResultsCollecting;
            }
            else
            {
                if (User.Tests?.Last()?.TshLevel == -2)
                {
                    try
                    {
                        var tsh = (from w in aliceRequest.Request.Nlu.Entities
                                   where (aliceRequest.Request.Nlu.Entities.Any()
                                     && (w as NumberModel != null))
                                   select (w as NumberModel).Value).First();
                        User.Tests.Last().TshLevel = (double)tsh;
                    }
                    catch
                    {
                        return new AliceResponse(aliceRequest, "Не поняла, повторите");
                    }
                    text = "Назовите дату сдачи анализа";
                    db.Users.Update(User);
                    await db.SaveChangesAsync();
                    return new AliceResponse(aliceRequest, text, tts, buttons)
                    {
                        SessionState = new SessionState() { Authorised = true, LastResponse = text },
                    };

                }
                else if (User.Tests?.Last().TestDate == default(DateTime))
                {
                    try
                    {
                        var td = (from date in aliceRequest.Request.Nlu.Entities
                                  where (aliceRequest.Request.Nlu.Entities.Any() && date as DateTimeModel != null)
                                  select (date as DateTimeModel).Value).First();
                        if (td != null)
                        {
                            if ((int)td.Year < 2000)
                            {
                                td.Year += 2000;
                            }
                            User.Tests.Last().TestDate = new DateTime((int)td.Year, (int)td.Month, (int)td.Day);
                        }
                        if (User.Tests?.Last().TestDate == default(DateTime))
                        {
                            return new AliceResponse(aliceRequest, "Не поняла, повторите");
                        }
                        if (User.Tests?.Last()?.TestDate < DateTime.Now.AddDays(-User.checkinterval))
                        {
                            User.Tests.Last().Actual = false;
                            text = $"Данный анализ не актуален. Прошло более {User.checkinterval} дней с момента сдачи. " +
                                $"Рекомендуется повторить.";
                            User.Mode = ModeType.OnReminder;
                            buttons = new List<ButtonModel>() { new ButtonModel("Я сдал анализы", true), new ButtonModel("Когда мне сдавать анализы?", true),
                        new ButtonModel("Мои прошлые ТТГ?", true), new ButtonModel("У меня другая доза лекарства", true) };
                        }
                        else
                        {
                            db.Users.Update(User);
                            await db.SaveChangesAsync();
                            return await new ControlMode(User).HandleRequest(aliceRequest, db);
                        }
                    }
                    catch
                    {
                        return new AliceResponse(aliceRequest, "Не поняла, повторите");
                    }
                }
            }
            db.Users.Update(User);
            await db.SaveChangesAsync();
            var response = new AliceResponse(aliceRequest, text, tts, buttons)
            {
                SessionState = new SessionState() { Authorised = true, LastResponse = text },
            };
            return response;
        }
    }
}