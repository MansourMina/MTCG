using Moq;
using MTCG.Database.Repositories;
using MTCG.Models;
using MTCG.Services;

namespace MTCGTest
{
    public class UserTests
    {

        private LoginService _loginService;
        private RegisterService _registerService;
        private Mock<UserRepository> _mockUserRepository;

        [SetUp]
        public void Setup()
        {
            _mockUserRepository = new Mock<UserRepository>();
            _loginService = new LoginService();
            _registerService = new RegisterService();
        }

        [Test]
        public void Register_User()
        {
            //// Arrange
            //var username = "testUser";
            //var password = "testPassword";
            //var user = new User(username, BCrypt.Net.BCrypt.HashPassword(password));

            //_mockUserRepository.Setup(repo => repo.Get(username.Trim())).Returns(user);

            //// Act
            //var token = _loginService.Login(username, password);

            //// Assert
            //Assert.IsNotNull(token);
            //Assert.IsTrue(_loginService.VerifyToken(token));
        }
    }
}
