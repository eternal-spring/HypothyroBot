using HypothyroBot.Models.Alice_API;
using HypothyroBot.Models.Session.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
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
        public async Task<AliceResponse> HandleRequest(AliceRequest aliceRequest, ApplicationContext db)
        {
            string text;
            string tts = null;
            switch (User.Mode)
            {
                case ModeType.RelevanceAssessment:
                    {
                        if (aliceRequest.Request.Command.Contains("да"))
                        {
                            if ((DateTime.Now - User.DateOfOperation).TotalDays >= 56)
                            {
                                User.Mode = ModeType.LimitationChecking;
                                db.Users.Update(User);
                                await db.SaveChangesAsync();
                                return await new LimitationCheckingMode(User).HandleRequest(aliceRequest, db);
                            }
                            else
                            {
                                text = "Хотите ли вы получать напоминания о необходимости контролировать гормоны?";
                                var buttons = new List<ButtonModel>() { new ButtonModel("Да", true), new ButtonModel("Нет", true) };
                                User.Mode = ModeType.SetReminder;
                                db.Users.Update(User);
                                await db.SaveChangesAsync();
                                var response = new AliceResponse(aliceRequest, text, buttons)
                                {
                                    SessionState = new SessionState() { Authorised = true, Id = User.Id, LastResponse = text, LastButtons = buttons },
                                };
                                return response;
                            }
                        }
                        else if (aliceRequest.Request.Command.Contains("не"))
                        {
                            return await new UserDataCorrectionMode(User).HandleRequest(aliceRequest, db);
                        }
                        else
                        {
                            return new AliceResponse(aliceRequest, "Не поняла, повторите.");
                        }
                    }
                default:
                    {
                        if (User.ThyroidCondition == ThyroidType.CompletelyRemoved && User.TreatmentDose == 0)
                        {
                            text = "Пациенты без щитовидной железы должны получать заместительную терапию. " +
                                "Пожалуйста, обратитесь к своему врачу за разъяснениями.";
                            return new AliceResponse(aliceRequest, text, true);
                        }
                        else if (User.ThyroidCondition == ThyroidType.CompletelyRemoved && User.TreatmentDose < User.Weight * 1.6 - 25)
                        {
                            text = "Возможно эта дозировка не достаточная для вас. Пожалуйста, обратитесь к своему врачу за разъяснениями.";
                            return new AliceResponse(aliceRequest, text, true);
                        }
                        else
                        {
                            text = $"Итак, вас зовут {User.Name}, вы родились {User.DateOfBirth.ToString("D", CultureInfo.CreateSpecificCulture("ru-RU"))}, весите {User.Weight} кг, ";
                            if (User.Gender != GenderType.None)
                            {
                                text += $"{ ((DescriptionAttribute)User.Gender.GetType().GetMember(User.Gender.ToString())[0].GetCustomAttributes(typeof(DescriptionAttribute), false)[0]).Description}, ";
                            }
                            if (User.PretreatmentDose == 0)
                            {
                                text += "до операции не принимали левотироксин, ";
                            }
                            else if (User.PretreatmentDose > 0)
                            {
                                text += $"до операции принимали {User.PretreatmentDose} мкг ";
                                if (User.PretreatmentDrug == DrugType.Another)
                                {
                                    text += "тироксина, ";
                                }
                                else
                                {
                                    text += $"{ ((DescriptionAttribute)User.PretreatmentDrug.GetType().GetMember(User.PretreatmentDrug.ToString())[0].GetCustomAttributes(typeof(DescriptionAttribute), false)[0]).Description}а, ";
                                }
                            }
                            text += $"операция была {User.DateOfOperation.ToString("D", CultureInfo.CreateSpecificCulture("ru-RU"))}, ";
                            if (User.ThyroidCondition == ThyroidType.CompletelyRemoved)
                            {
                                text += "щитовидка была удалена вся, ";
                            }
                            else if (User.ThyroidCondition == ThyroidType.HalfRemoved)
                            {
                                text += "щитовидка была удалена наполовину, ";
                            }
                            else if (User.ThyroidCondition == ThyroidType.IsthmusRemoved)
                            {
                                text += "был удалён перешеек щитовидки, ";
                            }
                            else if (User.ThyroidCondition == ThyroidType.LobeRemainderLeft)
                            {
                                text += "оставлен небольшой остаток доли щитовидки, ";
                            }
                            if (User.Pathology != PathologyType.Another)
                            {
                                text += $"по гистологии: " +
                                    $"{ ((DescriptionAttribute)User.Pathology.GetType().GetMember(User.Pathology.ToString())[0].GetCustomAttributes(typeof(DescriptionAttribute), false)[0]).Description}, ";
                            }
                            if (User.TreatmentDose == 0)
                            {
                                text += "терапия левотироксином не была назначена. ";
                            }
                            else if (User.TreatmentDose > 0)
                            {
                                text += $"было назначено {User.TreatmentDose} мкг ";
                                if (User.TreatmentDrug == DrugType.Another)
                                {
                                    text += "тироксина, ";
                                }
                                else
                                {
                                    text += $"{ ((DescriptionAttribute)User.TreatmentDrug.GetType().GetMember(User.TreatmentDrug.ToString())[0].GetCustomAttributes(typeof(DescriptionAttribute), false)[0]).Description}а. ";
                                }
                            }
                            text += "Всё верно? ";
                            tts = text.Replace("мкг", "микрограмм");
                            var buttons = new List<ButtonModel> { new ButtonModel("да", true), new ButtonModel("нет", true) };
                            User.Mode = ModeType.RelevanceAssessment;
                            db.Users.Update(User);
                            await db.SaveChangesAsync();
                            return new AliceResponse(aliceRequest, text, tts, buttons);
                        }
                    }
            }
        }
    }
}