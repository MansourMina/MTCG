using MTCG.Models;

namespace MTCG.Database.Repositories.Interfaces
{
    public interface IStackRepository
    {
        string Create(string stack_id, string user_id);
        void AddCards(string stack_id, List<Card> cards);
        void AddCard(string stack_id, Card card);
        int Remove(string card_id, string stack_id);


    }
}