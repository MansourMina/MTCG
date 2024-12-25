using MTCG.Models;

namespace MTCG.Database.Repositories.Interfaces
{
    public interface IStackRepository
    {
        string Add(string stack_id);
        void AddCards(string stack_id, List<Card> cards);
  
    }
}