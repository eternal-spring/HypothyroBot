using HypothyroBot.Models.Alice_API;
using System.Threading.Tasks;

namespace HypothyroBot.Models.Session.Interfaces
{
    public interface IMode
    {
        public abstract Task<AliceResponse> HandleRequest(AliceRequest aliceRequest, ApplicationContext db);
    }
}