using MTCG.Database;
using MTCG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Services
{
    public class LoginService
    {
        private static Dictionary<string, User> _tokens;
        public LoginService()
        {
            _tokens = new Dictionary<string, User>();
        }
        public string Login(string name, string password)
        {
            var user = MTCGData.getUser(name);
            string? existingUserToken = isLoggedIn(user);
            if(existingUserToken != null) return existingUserToken;
            if (user != null && BCrypt.Net.BCrypt.EnhancedVerify(password, user.Password))
            {
                string token = Guid.NewGuid().ToString();
                _tokens[token] = user;
                return token;
            }
            throw new UnauthorizedAccessException("Invalid username or password");
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
                if (u.Value == user) token= u.Key;
            return token;
        } 
    }
}
