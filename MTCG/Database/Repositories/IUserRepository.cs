using MTCG.Models;

namespace MTCG.Database.Repositories
{
    public interface IUserRepository
    {
        void Add(User user);
        int Delete(string username);
        User? Get(string username);
        List<User> GetAll();
        void Update(string username, User user);
    }
}