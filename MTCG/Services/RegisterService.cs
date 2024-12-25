using MTCG.Database;
using MTCG.Database.Repositories;
using MTCG.Database.Repositories.Interfaces;
using MTCG.Models;
using MTCG.Services.Interfaces;

namespace MTCG.Services
{
    public class RegisterService : IRegisterService
    {
        private readonly ILoginService _loginService;
        private readonly IUserRepository _userRepository;

        public RegisterService(ILoginService loginService, IUserRepository userRepository)
        {
            _loginService = loginService ?? throw new ArgumentNullException(nameof(loginService));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public RegisterService()
        {
            _loginService = new LoginService();
            _userRepository = new UserRepository();
        }

        public void Register(string name, string password)
        {
            var user = _userRepository?.GetByName(name.Trim());
            if (user != null) throw new InvalidOperationException("User already exists");
            if (string.IsNullOrWhiteSpace(name.Trim()) || string.IsNullOrWhiteSpace(password.Trim()))
                throw new ArgumentException("Username or password cannot be empty.");

            var newUser = new User(name, BCrypt.Net.BCrypt.EnhancedHashPassword(password));
            _userRepository?.Add(newUser);
        }

        public ILoginService GetLoginService() => _loginService;
        public IUserRepository GetUserRepository() => _userRepository;

    }
}
