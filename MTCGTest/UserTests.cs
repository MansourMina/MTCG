using Moq;
using MTCG.Database.Repositories;
using MTCG.Models;
using MTCG.Services;
using NSubstitute;
using NUnit.Framework;

namespace MTCGTest
{
    public class UserTests
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
        public void User_Initialization_ShouldSetUsernameAndPassword()
        {
            // Arrange
            string username = "testUser";
            string password = "testPassword";

            // Act
            User user = new User(username, password);

            // Assert
            Assert.That(user.Username, Is.EqualTo(username));
            Assert.That(user.Password, Is.EqualTo(password));
        }

        [Test]
        public void User_Initialization_ShouldSetDefaultEloAndCoins()
        {
            // Arrange
            string username = "testUser";
            string password = "testPassword";

            // Act
            User user = new User(username, password);

            // Assert
            Assert.That(user.Elo, Is.EqualTo(100));
            Assert.That(user.Coins, Is.EqualTo(20));
        }


        [Test]
        public void AddWin_ShouldIncreaseEloAndCoins()
        {
            // Arrange
            string username = "testUser";
            string password = "testPassword";
            User user = new User(username, password);
            int elo = 100;
            int coins = 200;

            // Act
            user.SetElo(elo);
            user.SetCoins(coins);

            // Assert
            Assert.That(user.Elo, Is.EqualTo(elo)); 
            Assert.That(user.Coins, Is.EqualTo(coins));

        }

        [Test]
        public void AddWin_ShouldIncreaseEloAndWinCount()
        {
            // Arrange
            string username = "testUser";
            string password = "testPassword";
            User user = new User(username, password);
            user.SetElo(100);
            int pointsToAdd = 10;

            // Act
            user.AddWin(pointsToAdd);

            // Assert
            Assert.That(user.Elo, Is.EqualTo(110)); 
            Assert.That(user.statistic.Wins, Is.EqualTo(1));
        }


        [Test]
        public void AddLoss_ShouldDecreaseEloAndIncreaseLossCount()
        {
            // Arrange
            string username = "testUser";
            string password = "testPassword";
            User user = new User(username, password);
            user.SetElo(10);
            int pointsToRemove = 5;

            // Act
            user.AddLosses(pointsToRemove);

            // Assert
            Assert.That(user.Elo, Is.EqualTo(5));
            Assert.That(user.statistic.Losses, Is.EqualTo(1));

        }

        [Test]
        public void AddLoss_ShouldNotDecreaseEloBelowZero()
        {
            // Arrange
            string username = "testUser";
            string password = "testPassword";
            User user = new User(username, password);
            user.SetElo(5);
            int pointsToRemove = 10;

            // Act
            user.AddLosses(pointsToRemove);

            // Assert
            Assert.That(user.Elo, Is.EqualTo(0));
            Assert.That(user.statistic.Losses, Is.EqualTo(1));
        }


        [Test]
        public void AddDraw_ShouldIncreasDrawCount()
        {
            // Arrange
            string username = "testUser";
            string password = "testPassword";
            User user = new User(username, password);

            // Act
            user.AddDraw();

            // Assert
            Assert.That(user.statistic.Draws, Is.EqualTo(1));
        }


        [Test]
        public void ChangePassword_ShouldUpdatePassword_WhenPasswordIsHashed()
        {
            // Arrange
            string username = "testUser";
            string password = "$2a$12$hashedpassword";
            User user = new User(username, password);
            string newPassword = "$2a$12$newhashedpassword"; 

            // Act
            user.ChangePassword(newPassword);

            // Assert
            Assert.That(user.Password, Is.EqualTo(newPassword)); 
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
