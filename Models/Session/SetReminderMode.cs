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

        public async Task<AliceResponse> HandleRequest(AliceRequest aliceRequest, DataBaseContext db)
        {
            string text = "";
            //User user = DataBaseContext.Me.Users.FirstOrDefault(u => u.Id == aliceRequest.Session.UserId);
            if (aliceRequest.Request.Command.Contains("да"))
            {
                text = "Отлично";
                User.Mode = ModeType.OnReminder;
            }
            db.Users.Update(User);
            await db.SaveChangesAsync();
            var response = new AliceResponse(aliceRequest, text, true)
            {
                //SessionState = new SessionState() { Mode = User.Mode },
                //UserStateUpdate = user,
            };
            return response;
        }
    }
}