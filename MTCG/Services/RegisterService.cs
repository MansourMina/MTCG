using MTCG.Database;
using MTCG.Database.Repositories;
using MTCG.Models;

namespace MTCG.Services
{
    public class RegisterService
    {
        private readonly LoginService _loginService;
        private readonly IUserRepository? _userRepository;

        public RegisterService(LoginService loginService, IUserRepository userRepository)
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
            var user = _userRepository?.Get(name.Trim());
            if (user != null) throw new InvalidOperationException("User already exists");
            if (string.IsNullOrWhiteSpace(name.Trim()) || string.IsNullOrWhiteSpace(password.Trim()))
                throw new ArgumentException("Username or password cannot be empty.");

            var newUser = new User(name, BCrypt.Net.BCrypt.EnhancedHashPassword(password));
            _userRepository?.Add(newUser);
        }

    }
}
