namespace MTCG.Database.Repositories.Interfaces
{
    public interface ISessionRepository
    {
        void Add(string userId, string token, DateTime expires_at);
        void Delete(string token);
        void DeleteByUserId(string userId);
        string? GetUserId(string token);
        string? GetUserToken(string userId);
        bool VerifyToken(string token);
    }
}