using MTCG.Models;

namespace MTCG.Services.Interfaces
{
    public interface ILoginService
    {
        string CreateToken(User user);
        string Login(string name, string password);
        void Logout(string token);
    }
}