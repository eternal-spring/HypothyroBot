using HypothyroBot.Models.Alice_API;
using HypothyroBot.Models.Session.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public async Task<AliceResponse> HandleRequest(AliceRequest aliceRequest, DataBaseContext db)
        {
            string text = "";
            string tts = null;
            var buttons = new List<ButtonModel>();
            if (User.Mode != ModeType.ResultsCollecting)
            {            
                if (User.TSH == 0)
                {
                    text = "Скажите значение ТТГ (в мкМЕ/мл)";
                    User.Mode = ModeType.ResultsCollecting;
                }
            }
            else
            {
                if (User.TSH == 0)
                {
                    try
                    {
                        var tsh = (from w in aliceRequest.Request.Nlu.Entities
                                   where (aliceRequest.Request.Nlu.Entities.Count() > 0
                                     && (w as NumberModel != null))
                                   select (w as NumberModel).Value).First();
                        User.TSH = (double)tsh;
                    }
                    catch
                    {
                        return new AliceResponse(aliceRequest, "Не поняла, повторите");
                    }
                    text = "Назовите дату сдачи анализа";
                }
                else if (User.TestDate == default)
                {
                    try
                    {
                        var td = (from date in aliceRequest.Request.Nlu.Entities
                                  where (aliceRequest.Request.Nlu.Entities.Count() > 0 && date as DateTimeModel != null)
                                  select (date as DateTimeModel).Value).First();
                        if (td != null)
                        {
                            User.TestDate = new DateTime((int)td.Year, (int)td.Month, (int)td.Day);
                        }
                        if (User.TestDate > User.OperationDate.Add(User.checkinterval))
                        {
                            text = $"Данный анализ не актуален. Прошло более {User.checkinterval} дней с момента сдачи. " +
                                $"Рекомендуется повторить.";
                        }
                        else
                        {
                            return await new ControlMode(User).HandleRequest(aliceRequest, db);
                        }
                    }
                    catch
                    {
                        return new AliceResponse(aliceRequest, "Не поняла, повторите");
                    }
                }
            }
            throw new NotImplementedException();
        }
    }
}