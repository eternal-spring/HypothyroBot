using HypothyroBot.Models.Alice_API;
using HypothyroBot.Models.Session;
using System.Threading.Tasks;

namespace HypothyroBot.Models
{
    public class Response
    {
        public Response()
        {

        }
        public async Task<AliceResponse> AliceResponse(AliceRequest aliceRequest, DataBaseContext db)
        {

            User user = await db.Users.FindAsync(aliceRequest.State?.Session?.Id ?? aliceRequest.Session.UserId);
            switch (user?.Mode)
            {
                default:
                    {
                        return await new AddingUserMode(user).HandleRequest(aliceRequest, db);
                    }
                case ModeType.RelevanceAssessment:
                    {
                        return await new RelevanceAssessmentMode(user).HandleRequest(aliceRequest, db);
                    }
                case ModeType.SetReminder:
                    {
                        return await new SetReminderMode(user).HandleRequest(aliceRequest, db);
                    }
                case ModeType.OnReminder:
                    {
                        return await new OnReminderMode(user).HandleRequest(aliceRequest, db);
                    }
                case ModeType.LimitationChecking:
                    {
                        return await new LimitationCheckingMode(user).HandleRequest(aliceRequest, db);
                    }
            }
        }
    }
}