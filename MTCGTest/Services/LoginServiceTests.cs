using MTCG.Database.Repositories;
using MTCG.Database.Repositories.Interfaces;
using MTCG.Models;
using MTCG.Services;
using MTCG.Services.Interfaces;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MTCGTest.Services
{
    public class LoginServiceTests
    {
        private LoginService _loginService;
        private IRegisterService _mockedRegisterService;
        private IUserRepository _mockedUserRepository;

        [SetUp]
        public void Setup()
        {
            _mockedUserRepository = Substitute.For<IUserRepository>();
            _mockedRegisterService = Substitute.For<IRegisterService>();
            _loginService = new LoginService(_mockedUserRepository);
        }

    }
}
