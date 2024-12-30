using MTCG.Database.Repositories.Interfaces;

namespace MTCG.Services.Interfaces
{
    public interface IUserManager
    {
        IUserRepository GetUserRepository();
        void Register(string name, string password, string role);
    }
}