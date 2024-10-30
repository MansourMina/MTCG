using Moq;
using MTCG.Database.Repositories;
using MTCG.Models;
using MTCG.Services;
using NSubstitute;

namespace MTCGTest
{
    public class UserTests
    {

        LoginService mockedLoginService;
        RegisterService mockedRegisterService;
        UserRepository mockedUserRepository;

        [SetUp]
        public void Setup()
        {
            mockedLoginService = Substitute.For<LoginService>();
            mockedRegisterService = Substitute.For<RegisterService>();
            mockedUserRepository = Substitute.For<UserRepository>();
        }

        [Test]
        public void Register_User_ShouldReturnTokenAndVerifySuccessfully()
        {
            // Arrange
            var username = "testUser";
            var password = "testPassword";
            var expectedToken = "";

            mockedRegisterService.Register(username, password).Returns(expectedToken);

            // Act
            var token = mockedRegisterService.Register(username, password);

            // Assert
            Assert.IsNotNull(token);
            Assert.AreEqual(expectedToken, token);

            mockedLoginService.VerifyToken(token).Returns(true);
            Assert.IsTrue(mockedLoginService.VerifyToken(token));
        }
    }
}
