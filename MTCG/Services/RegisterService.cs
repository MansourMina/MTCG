using MTCG.Database;
using MTCG.Models;

namespace MTCG.Services
{
    public class RegisterService
    {
        private readonly LoginService _loginService = new LoginService();
        public string Register(string name, string password)
        {
            var user = MTCGData.getUser(name.Trim());
            if (user != null) throw new InvalidOperationException("User already exists");
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Username or password cannot be empty.");

            var newUser = new User(name, BCrypt.Net.BCrypt.EnhancedHashPassword(password));
            MTCGData.addUser(newUser);
            return _loginService.CreateToken(newUser);
        }

    }
}
