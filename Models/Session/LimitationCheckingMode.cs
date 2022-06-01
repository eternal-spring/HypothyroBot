using HypothyroBot.Models.Alice_API;
using HypothyroBot.Models.Session.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HypothyroBot.Models.Session
{
    public class LimitationCheckingMode : IMode
    {
        private User User;
        public LimitationCheckingMode(User user)
        {
            User = user;
        }
        public async Task<AliceResponse> HandleRequest(AliceRequest aliceRequest, ApplicationContext db)
        {
            string text = "Сдавали ли вы после этого контрольный анализ на ТТГ?";
            string tts = text.Replace("ТТГ", "тэтэг+э");
            var buttons = new List<ButtonModel>();
            if (aliceRequest.State.Session.LastResponse == text)
            {
                if (aliceRequest.Request.Command.Contains("да"))
                {
                    return await new ResultsCollectingMode(User).HandleRequest(aliceRequest, db);
                }
                else if (aliceRequest.Request.Command.Contains("не"))
                {
                    text = "В срок 4-8 недель после операции на щитовидной железе следует определить уровень ТТГ для оценки необходимости ";
                    tts = "В срок 4-8 недель после операции на щитовидной железе следует определить уровень тиреоторопного гормона для оценки необходимости ";
                    if (User.PretreatmentDose == 0)
                    {
                        text += "назначения терапии.";
                        tts += "назначения терапии";
                    }
                    else
                    {
                        text += "коррекции терапии.";
                        tts += "коррекции терапии";
                    }
                }
            }
            else
            {
                tts = "Сдавали ли вы после этого контрольный анализ на тиреоторопный гормон?";
                buttons = new List<ButtonModel>() { new ButtonModel("Да", true), new ButtonModel("Нет", true) };
            }
            var response = new AliceResponse(aliceRequest, text, tts, buttons)
            {
                SessionState = new SessionState() {Authorised = true, LastResponse = text },
            };
            return response;
        }
    }
}