using MTCG.Models;
using Newtonsoft.Json.Bson;
using System.Diagnostics;

namespace MTCG.Database.Repositories.Interfaces
{
    public interface ITradingRepository
    {
        string Create(string id, string card_to_trade, string status, string created_by_id, string required_card_type, int min_damage);
        List<Trade> GetAll();

        Trade? GetTradeFromCardId(string cardID);
        Trade? GetTradeFromCardIdOfUser(string cardID, string userId);
        void Traded(string tradeId, TradeStatus status);

        int Delete(string tradeId, string userId);
    }
}