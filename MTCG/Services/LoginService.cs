using MTCG.Database;
using MTCG.Database.Repositories;
using MTCG.Database.Repositories.Interfaces;
using MTCG.Models;
using MTCG.Services.Interfaces;

namespace MTCG.Services
{
    public class LoginService : ILoginService
    {
        private readonly SessionService _sessionService;
        private readonly IUserRepository? _userRepository;

        public LoginService()
        {
            _userRepository = new UserRepository();
            _sessionService = new SessionService();
        }
        public LoginService(IUserRepository userRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public string CreateToken(User user)
        {
            throw new NotImplementedException();
        }

        public string Login(string name, string password)
        {
            var user = _userRepository?.Get(name.Trim());
            if (user == null || !BCrypt.Net.BCrypt.EnhancedVerify(password, user.Password))
                throw new UnauthorizedAccessException("Invalid username or password");
            return _sessionService.GetSessionToken(user.Id) ?? _sessionService.CreateSession(user.Id);
        }

        public void Logout(string token)
        {
            if (_sessionService.VerifyToken(token)) _sessionService.RevokeSession(token);
        }
    }
}
