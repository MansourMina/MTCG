using MTCG.Database;
using MTCG.Database.Repositories;
using MTCG.Models;

namespace MTCG.Services
{
    public class RegisterService
    {
        private readonly LoginService _loginService = new LoginService();
        private readonly UserRepository? _dbUser = new UserRepository();

        public string Register(string name, string password)
        {
            var user = _dbUser?.GetByUsername(name.Trim());
            if (user != null) throw new InvalidOperationException("User already exists");
            if (string.IsNullOrWhiteSpace(name.Trim()) || string.IsNullOrWhiteSpace(password.Trim()))
                throw new ArgumentException("Username or password cannot be empty.");

            var newUser = new User(name, BCrypt.Net.BCrypt.EnhancedHashPassword(password));
            _dbUser?.Add(newUser);
            return _loginService.CreateToken(newUser);
        }

    }
}
