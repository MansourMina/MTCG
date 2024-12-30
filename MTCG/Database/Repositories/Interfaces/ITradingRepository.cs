using MTCG.Models;

namespace MTCG.Database.Repositories.Interfaces
{
    public interface ITradingRepository
    {
        string Create(string id, string card_to_trade, string status, string created_by_id, string required_card_type, int min_damage);
        List<Trade> GetAll();
    }
}