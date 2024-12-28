using MTCG.Models;

namespace MTCG.Database.Repositories.Interfaces
{
    public interface ICardRepository
    {
        List<Card> GetDeckCards(string deck_id);
        List<Card> GetStackCards(string stack_id);
    }
}