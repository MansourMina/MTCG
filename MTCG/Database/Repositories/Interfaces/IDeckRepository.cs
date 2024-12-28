namespace MTCG.Database.Repositories.Interfaces
{
    public interface IDeckRepository
    {
        void AddCard(string id, string card_id, string deck_id);
        string Create(string deck_id, string user_id);
    }
}