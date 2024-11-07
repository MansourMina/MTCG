using MTCG.Models;

namespace MTCGTest.ModelsTest
{
    public class CardTests
    {
        [Test]
        public void Constructor_ShouldSetDamageAndElementTypeCorrectly()
        {
            // Arrange
            int expectedDamage = 50;
            ElementType expectedElementType = ElementType.Fire;

            // Act
            var monsterCard = new MonsterCard("TestMonster", expectedDamage, expectedElementType);
            var spellCard = new SpellCard("TestSpell", expectedDamage, expectedElementType);

            // Assert
            Assert.That(monsterCard.Damage, Is.EqualTo(expectedDamage));
            Assert.That(spellCard.Damage, Is.EqualTo(expectedDamage));
            Assert.That(monsterCard.ElementType, Is.EqualTo(expectedElementType));
            Assert.That(spellCard.ElementType, Is.EqualTo(expectedElementType));
        }
    }
}
