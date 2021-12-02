using HypothyroBot.Models.Alice_API;
using HypothyroBot.Models.Session.Interfaces;
using System;
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

        public async Task<AliceResponse> HandleRequest(AliceRequest aliceRequest, DataBaseContext db)
        {
            string text = "";
            string tts = "";
            //User user = DataBaseContext.Me.Users.FirstOrDefault(u => u.Id == aliceRequest.Session.UserId);
            if ((DateTime.Now - User.OperationDate).TotalDays >= 28)
            {
                text = "После назначения лекарственной терапии или изменении дозировки контроль обычно осуществляется через 4-8 недель, " +
                    "найдите время и сдайте ТТГ";
                tts = "После назначения лекарственной терапии или изменении дозировки контроль обычно осуществляется через 4-8 недель, " +
                    "найдите время и сдайте тиреоторопный гормон";

            }
            else if ((DateTime.Now - User.OperationDate).TotalDays >= 42)
            {
                text = "Уже прошло 6 недель с момента смены терапии/операции. Вы сдали ТТГ?";
                tts = "Уже прошло 6 недель с момента смены терапии/операции. Вы сдали тиреоторопный гормон?";
            }
            else if ((DateTime.Now - User.OperationDate).TotalDays >= 49)
            {
                text = "Если вы еще не сдали ТТГ, то поспешите, рекомендованный интервал заканчивается через неделю";
                tts = "Если вы еще не сдали тиреоторопный гормон, то поспешите, рекомендованный интервал заканчивается через неделю";
            }
            var response = new AliceResponse(aliceRequest, text, tts)
            {
                SessionState = new SessionState() {Authorised = true, LastResponse = text },
                //UserStateUpdate = user,
            };
            return response;
        }
    }
}