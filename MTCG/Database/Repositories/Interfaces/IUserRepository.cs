using MTCG.Models;

namespace MTCG.Database.Repositories.Interfaces
{
    public interface IUserRepository
    {
        string Create(User user);
        int Delete(string username);
        User? GetByName(string username);
        List<User> GetAll();
        void UpdateUserCreds(string username, User user);
    }
}