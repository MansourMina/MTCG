using MTCG.Database.Repositories.Interfaces;

namespace MTCG.Services.Interfaces
{
    public interface IRegisterService
    {
        ILoginService GetLoginService();
        IUserRepository GetUserRepository();
        void Register(string name, string password);
    }
}