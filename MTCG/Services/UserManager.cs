using MTCG.Database;
using MTCG.Database.Repositories;
using MTCG.Database.Repositories.Interfaces;
using MTCG.Models;
using MTCG.Services.Interfaces;

namespace MTCG.Services
{
    public class UserManager : IUserManager
    {
        private readonly ILoginService _loginService;
        private readonly IUserRepository _userRepository;
        private readonly IDeckRepository _deckRepository;
        private readonly IStackRepository _stackRepository;
        private readonly IStatisticRepository _statisticRepository;

        public UserManager(ILoginService loginService, IUserRepository userRepository, IDeckRepository deckRepository, IStackRepository stackRepository, IStatisticRepository statisticRepository)
        {
            _loginService = loginService ?? throw new ArgumentNullException(nameof(loginService));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _deckRepository = deckRepository ?? throw new ArgumentNullException(nameof(deckRepository));
            _stackRepository = stackRepository ?? throw new ArgumentNullException(nameof(stackRepository));
            _statisticRepository = statisticRepository ?? throw new ArgumentNullException(nameof(statisticRepository));
        }

        public UserManager()
        {
            _loginService = new LoginService();
            _userRepository = new UserRepository();
            _deckRepository = new DeckRepository();
            _stackRepository = new StackRepository();
            _statisticRepository = new StatisticRepository();
        }

        public void Register(string name, string password)
        {
            var user = _userRepository?.GetByName(name.Trim());
            if (user != null) throw new InvalidOperationException("User already exists");
            if (string.IsNullOrWhiteSpace(name.Trim()) || string.IsNullOrWhiteSpace(password.Trim()))
                throw new ArgumentException("Username or password cannot be empty.");

            // Create Stack for User
            string new_stack_id = Guid.NewGuid().ToString();
            _stackRepository.Add(new_stack_id);

            // Create Deck for User
            string new_deck_id = Guid.NewGuid().ToString();
            _deckRepository.Add(new_deck_id);

            // Create Statistic for User
            string new_statistic_id = Guid.NewGuid().ToString();
            _statisticRepository.Add(new_statistic_id);

            var newUser = new User(name, BCrypt.Net.BCrypt.EnhancedHashPassword(password), new_stack_id, new_deck_id, new_statistic_id);
            _userRepository?.Add(newUser);
        }

        public ILoginService GetLoginService() => _loginService;
        public IUserRepository GetUserRepository() => _userRepository;

    }
}
