using MTCG.Database.Repositories.Interfaces;
using MTCG.Models;
using MTCG.Services;
using MTCG.Services.Interfaces;
using NSubstitute;

namespace MTCGTest.Services
{
    public class RegisterServiceTests
    {
        private ILoginService _mockedLoginService;
        private UserManager _registerService;
        private IUserRepository _mockedUserRepository;

        [SetUp]
        public void Setup()
        {
            _mockedUserRepository = Substitute.For<IUserRepository>();
            _mockedLoginService = Substitute.For<ILoginService>();
            _registerService = new RegisterService(_mockedLoginService, _mockedUserRepository);
        }

        [Test]
        public void Register_User_ShouldReturnTokenAndVerifySuccessfully()
        {
            // Arrange
            string username = "testUser";
            string password = "testPassword";
            string expectedToken = "";
            bool expectedVerification = true;

            _mockedLoginService.Login(username, password).Returns(expectedToken);
            _mockedLoginService.VerifyToken(expectedToken).Returns(expectedVerification);

            // Act
            _registerService.Register(username, password);
            string token = _mockedLoginService.Login(username, password);
            bool verifiedToken = _mockedLoginService.VerifyToken(token);

            // Assert
            Assert.That(token, Is.EqualTo(expectedToken));
            Assert.IsTrue(verifiedToken);
        }

        [Test]
        public void Register_User_ShouldReturnError_WhenUserAlreadyExists()
        {
            // Arrange
            string username = "testUser";
            string password = "testPassword";
            _mockedUserRepository.GetByName(username).Returns(new User(username, password));

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => _registerService.Register(username, password));
            Assert.That(exception.Message, Is.EqualTo("User already exists"));
        }

        [Test]
        public void Register_User_ShouldReturnMessage_WhenUsernameOrPasswordIsEmpty()
        {
            // Arrange
            string username = "";
            string password = "testPassword";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => _registerService.Register(username, password));
            Assert.That(exception.Message, Is.EqualTo("Username or password cannot be empty."));
        }

        [Test]
        public void RegisterService_ShouldInitializeLoginServiceAndUserRepository()
        {
            // Act
            var registerService = new UserManager();

            // Assert
            Assert.That(registerService.GetLoginService(), Is.Not.Null);
            Assert.That(registerService.GetUserRepository(), Is.Not.Null);

        }

    }
}
