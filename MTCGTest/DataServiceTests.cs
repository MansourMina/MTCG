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
        private UserRepository _mockedUserRepository;

        [SetUp]
        public void Setup()
        {
            _mockedUserRepository = Substitute.For<UserRepository>();
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

            _mockedLoginService.CreateToken(Arg.Any<User>()).Returns(expectedToken);

            // Act
            string token = _registerService.Register(username, password);


            // Assert
            Assert.That(token, Is.Not.Null);
            Assert.That(token, Is.EqualTo(expectedToken));
            _mockedUserRepository.Received(1).Add(Arg.Is<User>(u => u.Username == username));
        }

       
    }
}
