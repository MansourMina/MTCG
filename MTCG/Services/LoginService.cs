using MTCG.Database;
using MTCG.Database.Repositories;
using MTCG.Models;

namespace MTCG.Services
{
    public class LoginService
    {
        private static Dictionary<string, User> _tokens = new Dictionary<string, User>();
        private readonly UserRepository? _dbUser = new UserRepository();

        public string Login(string name, string password)
        {
            var user = _dbUser?.Get(name.Trim());
            string? existingUserToken = isLoggedIn(user);
            if (existingUserToken != null) return existingUserToken;
            if (user != null && BCrypt.Net.BCrypt.EnhancedVerify(password, user.Password))
                return CreateToken(user);
            throw new UnauthorizedAccessException("Invalid username or password");
        }

        public string CreateToken(User user)
        {
            string token = Guid.NewGuid().ToString();
            _tokens[token] = user;
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
            string token = null;
            foreach (var u in _tokens)
                if (u.Value == user) token = u.Key;
            return token;
        }
    }
}
