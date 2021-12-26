using HypothyroBot.Models.Alice_API;
using HypothyroBot.Models.Session.Interfaces;
using System.Threading.Tasks;

namespace HypothyroBot.Models.Session
{
    public class SetReminderMode : IMode
    {
        private User User;
        public SetReminderMode(User user)
        {
            User = user;
        }
        public async Task<AliceResponse> HandleRequest(AliceRequest aliceRequest, UsersDataBaseContext db)
        {
            string text = "";
            if (aliceRequest.Request.Command.Contains("да"))
            {
                text = "Отлично. Вы всегда можете сообщить мне об изменениях в самочувствии, терапии, данных анализов.";
                User.Mode = ModeType.OnReminder;
            }
            db.Users.Update(User);
            await db.SaveChangesAsync();
            var response = new AliceResponse(aliceRequest, text, true)
            {
                SessionState = aliceRequest.State.Session,
            };
            return response;
        }
    }
}