using MTCG.Database;
using MTCG.Database.Repositories;
using MTCG.Database.Repositories.Interfaces;
using MTCG.Models;
using MTCG.Services.Interfaces;

namespace MTCG.Services
{
    public class LoginService : ILoginService
    {
        private readonly static Dictionary<string, string> _tokens = [];
        private readonly IUserRepository? _userRepository;

        public LoginService()
        {
            _userRepository = new UserRepository();
        }
        public LoginService(IUserRepository userRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }
        public string Login(string name, string password)
        {
            var user = _userRepository?.Get(name.Trim());
            if (user == null || !BCrypt.Net.BCrypt.EnhancedVerify(password, user.Password))
                throw new UnauthorizedAccessException("Invalid username or password");
            return IsLoggedIn(user) ?? CreateToken(user);
        }

        public string CreateToken(User user)
        {
            string token = Guid.NewGuid().ToString();
            _tokens[token] = user.Username;
            return token;
        }
        public bool VerifyToken(string token)
        {
            return _tokens.ContainsKey(token);
        }

        public void Logout(string token)
        {
            if (VerifyToken(token)) _tokens.Remove(token);
        }

        private static string? IsLoggedIn(User user)
        {
            if (user == null) return null;
            string? token = null;
            foreach (var u in _tokens)
            {
                if (u.Value == user.Username)
                    return u.Key;
            }
            return token;
        }
    }
}
