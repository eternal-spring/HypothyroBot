using HypothyroBot.Models.Alice_API;
using HypothyroBot.Models.Session.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace HypothyroBot.Models.Session
{
    public class RelevanceAssessmentMode : IMode
    {
        private User User;
        public RelevanceAssessmentMode(User user)
        {
            User = user;
        }
        public async Task<AliceResponse> HandleRequest(AliceRequest aliceRequest, DataBaseContext db)
        {
            string text = "";
            //User user = DataBaseContext.Me.Users.FirstOrDefault(u => u.Id == aliceRequest.Session.UserId);
            if (User.ThyroidCondition == ThyroidType.CompletelyRemoved && User.TreatmentDose == 0)
            {
                text = "Пациенты без щитовидной железы должны получать заместительную терапию. " +
                    "Пожалуйста, обратитесь к своему врачу за разъяснениями.";
            }
            else if (User.ThyroidCondition == ThyroidType.CompletelyRemoved && User.TreatmentDose < User.Weight * 1.6 - 25)
            {
                text = "Возможно эта дозировка не достаточная для вас. Пожалуйста, обратитесь к своему врачу за разъяснениями.";
            }
            text += "Хотите ли вы получать напоминания о необходимости контролировать гормоны?";
            var buttons = new List<ButtonModel>() { new ButtonModel("Да", true), new ButtonModel("Нет", true) };
            User.Mode = ModeType.SetReminder;
            db.Users.Update(User);
            await db.SaveChangesAsync();
            var response = new AliceResponse(aliceRequest, text, buttons)
            {
                SessionState = new SessionState() {Authorised = true, LastResponse = text, LastButtons = buttons },
                //UserStateUpdate = user,
            };
            return response;
        }
    }
}