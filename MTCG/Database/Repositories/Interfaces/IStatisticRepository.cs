using MTCG.Models;

namespace MTCG.Database.Repositories.Interfaces
{
    public interface IStatisticRepository
    {
        string Create(string stack_id, string user_id);
        void Update(Statistic statistic);
        Statistic? GetByUserId(string userId);
    }
}