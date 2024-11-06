using MTCG.Database.Repositories;
using MTCG.Models;
using MTCG.Services;
using NSubstitute;

namespace MTCGTest
{
    public class DataServiceTests
    {
        private LoginService _mockedLoginService;
        private RegisterService _registerService;
        private IUserRepository _mockedUserRepository;

        [SetUp]
        public void Setup()
        {
            _mockedUserRepository = Substitute.For<IUserRepository>();
            _mockedLoginService = Substitute.For<LoginService>(_mockedUserRepository);
            _registerService = new RegisterService(_mockedLoginService, _mockedUserRepository);
        }

        [Test]
        public void Register_User_ShouldReturnTokenAndVerifySuccessfully()
        {
            // Arrange
            string username = "testUser";
            string password = "testPassword";
            string expectedToken = "";

            _mockedLoginService.Login(username, password).Returns(expectedToken);

            // Act
            _registerService.Register(username, password);
            string token = _mockedLoginService.Login(username, password);

            // Assert
            Assert.That(token, Is.Not.Null);
            Assert.That(token, Is.EqualTo(expectedToken));
            Assert.IsTrue(_mockedLoginService.VerifyToken(token));
        }
    }
}
