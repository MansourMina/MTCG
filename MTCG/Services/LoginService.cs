using MTCG.Database;
using MTCG.Database.Repositories;
using MTCG.Models;

namespace MTCG.Services
{
    public class LoginService
    {
        private static Dictionary<string, string> _tokens = new Dictionary<string, string>();
        private readonly UserRepository? _dbUser = new UserRepository();

        public string Login(string name, string password)
        {
            var user = _dbUser?.Get(name.Trim());
            if (user == null || !BCrypt.Net.BCrypt.EnhancedVerify(password, user.Password))
                throw new UnauthorizedAccessException("Invalid username or password");
            return isLoggedIn(user) ?? CreateToken(user);
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

        private string? isLoggedIn(User user)
        {
            if (user == null) return null;
            string? token = null;
            foreach (var u in _tokens)
            {
                if (u.Value == user.Username) token = u.Key;
            }
            return token;
        }
    }
}
