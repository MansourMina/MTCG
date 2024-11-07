using MTCG.Database.Repositories.Interfaces;
using MTCG.Models;
using MTCG.Services;
using MTCG.Services.Interfaces;
using Newtonsoft.Json.Linq;
using NSubstitute;

namespace MTCGTest.Services
{
    public class LoginServiceTests
    {
        private LoginService _loginService;
        private IUserRepository _mockedUserRepository;

        [SetUp]
        public void Setup()
        {
            _mockedUserRepository = Substitute.For<IUserRepository>();
            _loginService = new LoginService(_mockedUserRepository);
        }

        [Test]
        public void Login_User_ShouldReturnToken_And_VerifySuccessfully()
        {
            // Arrange
            string username = "testUser";
            string password = "testPassword";

            _mockedUserRepository.Get(username).Returns(new User(username, BCrypt.Net.BCrypt.EnhancedHashPassword(password)));

            // Act
            string token = _loginService.Login(username, password);
            bool verifiedToken = _loginService.VerifyToken(token);

            // Assert
            Assert.That(token, Is.Not.Null);
            Assert.That(verifiedToken, Is.True);   

        }

        [Test]
        public void Login_User_ShouldReturnInvalidCredentials_WhenUsernameOrPasswordIsIncorrect()
        {
            // Arrange
            string username = "testUser";
            string password = "testPassword";
            string wrongPassword = "Wrong";

            _mockedUserRepository.Get(username).Returns(new User(username, BCrypt.Net.BCrypt.EnhancedHashPassword(password)));

            // Act & Assert
            var exception = Assert.Throws<UnauthorizedAccessException>(() => _loginService.Login(username, wrongPassword));
            Assert.That(exception.Message, Is.EqualTo("Invalid username or password"));
        }

        [Test]
        public void GetSessionToken_User_ShouldReturnToken_WhenUserIsLoggedIn()
        {
            // Arrange
            string username = "testUser";
            string password = "testPassword";
            var user = new User(username, BCrypt.Net.BCrypt.EnhancedHashPassword(password));

            _mockedUserRepository.Get(username).Returns(user);

            // Act
            string token = _loginService.Login(username, password);
            var loggedUserToken = LoginService.GetSessionToken(user);

            // Assert
            Assert.That(token, Is.EqualTo(loggedUserToken));
        }

        [Test]
        public void GetSessionToken_User_ShouldReturnNull_WhenUserIsNotLoggedIn()
        {
            // Arrange
            string username = "testUser";
            string password = "testPassword";
            var user = new User(username, BCrypt.Net.BCrypt.EnhancedHashPassword(password));

            _mockedUserRepository.Get(username).Returns(user);

            // Act
            var loggedUserToken = LoginService.GetSessionToken(user);

            // Assert
            Assert.That(loggedUserToken, Is.Null);
        }

        [Test]
        public void Logout_ShouldRemoveToken_WhenTokenIsValid()
        {
            // Arrange
            string username = "testUser";
            string password = "testPassword";
            var user = new User(username, BCrypt.Net.BCrypt.EnhancedHashPassword(password));

            _mockedUserRepository.Get(username).Returns(user);

            // Act
            var token = _loginService.Login(username, password);
            _loginService.Logout(token);
            bool verifiedToken = _loginService.VerifyToken(token);
            var isLoggedIn = LoginService.GetSessionToken(user);

            // Assert
            Assert.That(isLoggedIn, Is.Null);
            Assert.That(verifiedToken, Is.False);
        }

        [Test]
        public void GetSessionToken_ShouldReturnCorrectToken_ForMultipleUsers()
        {
            // Arrange
            var user1 = new User("user1", BCrypt.Net.BCrypt.EnhancedHashPassword("password1"));
            var user2 = new User("user2", BCrypt.Net.BCrypt.EnhancedHashPassword("password2"));

            _mockedUserRepository.Get("user1").Returns(user1);
            _mockedUserRepository.Get("user2").Returns(user2);

            // Act
            string token1 = _loginService.Login("user1", "password1");
            string token2 = _loginService.Login("user2", "password2");

            var sessionToken1 = LoginService.GetSessionToken(user1);
            var sessionToken2 = LoginService.GetSessionToken(user2);

            // Assert
            Assert.That(sessionToken1, Is.EqualTo(token1));
            Assert.That(sessionToken2, Is.EqualTo(token2));
        }

    }
}
