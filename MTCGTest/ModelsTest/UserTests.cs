using MTCG.Models;

namespace MTCGTest.Models
{
    public class UserTests
    {


        [SetUp]
        public void Setup()
        {

        }


        [Test]
        public void Constructor_ShouldInitializeProperties_WhenValidArgumentsProvided()
        {
            // Arrange
            string name = "testUser";
            string password = "testPassword";
            var stack = new Stack();

            // Act
            var user = new User(name, password, stack);

            // Assert
            Assert.That(user.Username, Is.EqualTo(name));
            Assert.That(user.Password, Is.EqualTo(password));
            Assert.That(user.Stack, Is.EqualTo(stack));
        }

        [Test]
        public void PrivateConstructor_ShouldInitializeUser()
        {
            // Act
            var user = (User)Activator.CreateInstance(typeof(User), true);

            // Assert
            Assert.That(user, Is.Not.Null);

        }

        [Test]
        public void User_Initialization_ShouldSetUsernameAndPassword()
        {
            // Arrange
            string username = "testUser";
            string password = "testPassword";

            // Act
            User user = new(username, password);

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
            User user = new(username, password);

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
            User user = new(username, password);
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
            User user = new(username, password);
            user.SetElo(100);
            int pointsToAdd = 10;

            // Act
            user.AddWin(pointsToAdd);

            // Assert
            Assert.That(user.Elo, Is.EqualTo(110));
            Assert.That(user.Statistic.Wins, Is.EqualTo(1));
        }


        [Test]
        public void AddLoss_ShouldDecreaseEloAndIncreaseLossCount()
        {
            // Arrange
            string username = "testUser";
            string password = "testPassword";
            User user = new(username, password);
            user.SetElo(10);
            int pointsToRemove = 5;

            // Act
            user.AddLosses(pointsToRemove);

            // Assert
            Assert.That(user.Elo, Is.EqualTo(5));
            Assert.That(user.Statistic.Losses, Is.EqualTo(1));

        }

        [Test]
        public void AddLoss_ShouldNotDecreaseEloBelowZero()
        {
            // Arrange
            string username = "testUser";
            string password = "testPassword";
            User user = new(username, password);
            user.SetElo(5);
            int pointsToRemove = 10;

            // Act
            user.AddLosses(pointsToRemove);

            // Assert
            Assert.That(user.Elo, Is.EqualTo(0));
            Assert.That(user.Statistic.Losses, Is.EqualTo(1));
        }


        [Test]
        public void AddDraw_ShouldIncreasDrawCount()
        {
            // Arrange
            string username = "testUser";
            string password = "testPassword";
            User user = new(username, password);

            // Act
            user.AddDraw();

            // Assert
            Assert.That(user.Statistic.Draws, Is.EqualTo(1));
        }


        [Test]
        public void ChangePassword_ShouldUpdatePassword()
        {
            // Arrange
            string username = "testUser";
            string password = "$2a$12$hashedpassword";
            User user = new(username, password);
            string newPassword = "$2a$12$newhashedpassword";

            // Act
            user.ChangePassword(newPassword);

            // Assert
            Assert.That(user.Password, Is.EqualTo(newPassword));
        }

        [Test]
        public void ChangeUsername_ShouldUpdateUsername()
        {
            // Arrange
            string username = "testUser";
            string password = "$2a$12$hashedpassword";
            User user = new(username, password);
            string newUsername = "testUser2";

            // Act
            user.ChangeUsername(newUsername);

            // Assert
            Assert.That(user.Username, Is.EqualTo(newUsername));
        }

        [Test]
        public void NoCardsLeft_ShouldReturnTrue_WhenCardsCountIsZero()
        {
            // Arrange
            string username = "testUser";
            string password = "$2a$12$hashedpassword";
            User user = new(username, password);

            // Act
            bool noCards = user.NoCardsLeft();

            // Assert
            Assert.IsTrue(noCards);
        }

        [Test]
        public void NoCardsLeft_ShouldReturnFalse_WhenCardsCountIsNotZero()
        {
            // Arrange
            string username = "testUser";
            string password = "$2a$12$hashedpassword";
            User user = new(username, password);
            user.AddCardToDeck(new MonsterCard("TestMonster", 100, ElementType.Normal));

            // Act
            bool noCards = user.NoCardsLeft();

            // Assert
            Assert.That(noCards, Is.False);
        }


        [Test]
        public void SetToken_ShouldSetToken_WhenTokenIsValidGuid()
        {
            // Arrange
            string username = "testUser";
            string password = "$2a$12$hashedpassword";
            User user = new(username, password);
            var validToken = Guid.NewGuid().ToString();

            // Act
            user.SetToken(validToken);

            // Assert
            Assert.That(user.Token, Is.EqualTo(validToken));
        }

        [Test]
        public void SetToken_ShouldNotSetToken_WhenTokenIsInvalidGuid()
        {
            // Arrange
            string username = "testUser";
            string password = "$2a$12$hashedpassword";
            User user = new(username, password);
            var invalidToken = "InvalidToken123";

            // Act
            user.SetToken(invalidToken);

            // Assert
            Assert.That(user.Token, Is.Not.EqualTo(invalidToken));
        }
    }
}
