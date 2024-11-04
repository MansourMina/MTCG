using MTCG.Models;

namespace MTCG.Services
{
    public class BattleService
    {
        public User LeftPlayer { get; private set; }
        public User RightPlayer { get; private set; }
        public int CurrentRound { get; private set; } = 1;

        private const int MaxRounds = 500;

        public const int WinningPoints = 3;

        public const int LosingPoints = 3;

        public const int DeckSize = 4;
        enum Effectiveness
        {
            isEffective,
            notEffective,
            noEffect
        }

        public enum GameStatus
        {
            LeftPlayerLost,
            RightPlayerLost,
            Playing
        }
        public BattleService(User leftPlayer, User rightPlayer)
        {
            LeftPlayer = leftPlayer;
            RightPlayer = rightPlayer;
        }

        public void start()
        {
            Random rnd = new Random();
            GameStatus gameStatus = GameStatus.Playing;
            while (CurrentRound <= MaxRounds && gameStatus == GameStatus.Playing)
            {
                var leftPlayerCards = LeftPlayer.Deck.Cards;
                var rightPlayerCards = RightPlayer.Deck.Cards;

                Card leftPlayerCard = leftPlayerCards[rnd.Next(leftPlayerCards.Count)];
                Card rightPlayerCard = rightPlayerCards[rnd.Next(rightPlayerCards.Count)];

                int leftPlayerDamage = leftPlayerCard.Damage;
                int rightPlayerDamage = rightPlayerCard.Damage;

                if (_isSpellCard(leftPlayerCard) || _isSpellCard(rightPlayerCard))
                {
                    leftPlayerDamage = reCalcDamage(leftPlayerCard, rightPlayerCard);
                    rightPlayerDamage = reCalcDamage(rightPlayerCard, leftPlayerCard);
                }
                printStatistics(CurrentRound);
                updateGame(leftPlayerDamage, leftPlayerCard, rightPlayerDamage, rightPlayerCard);
                gameStatus = checkGameOver();
                CurrentRound++;
            }

            if (gameStatus == GameStatus.LeftPlayerLost)
                Console.WriteLine($"Left Player Lost - he has {LeftPlayer.CardsCount()} Cards left");
            if (gameStatus == GameStatus.RightPlayerLost)
                Console.WriteLine($"Right Player Lost - he has {RightPlayer.CardsCount()} Cards left");
            else
            {
                Console.WriteLine($"{(RightPlayer.statistic.Wins > LeftPlayer.statistic.Wins ? "Right" : "Left")} WON");
                Console.WriteLine(LeftPlayer.Stack.Cards.Count);
                Console.WriteLine(RightPlayer.Stack.Cards.Count);
            }


        }

        private void printStatistics(int round)
        {
            Console.WriteLine($"----------Round {round}----------");
            Console.WriteLine($"Left Player: {LeftPlayer.Elo}");
            Console.WriteLine($"Right Player: {RightPlayer.Elo}");
            Console.WriteLine("\n");
        }

        private bool _isSpellCard(Card card)
        {
            return card.GetType().Name == CardType.SpellCard.ToString();
        }

        private int reCalcDamage(Card leftPlayerCard, Card rightPlayerCard)
        {
            int damage = leftPlayerCard.Damage;
            switch (getEffectiveness(leftPlayerCard, rightPlayerCard))
            {
                case Effectiveness.isEffective:
                    damage *= 2;
                    break;
                case Effectiveness.notEffective:
                    damage /= 2;
                    break;
                default: break;
            }
            return damage;
        }
        private Effectiveness getEffectiveness(Card leftPlayerCard, Card rightPlayerCard)
        {
            ElementType leftE = leftPlayerCard.ElementType;
            ElementType rightE = rightPlayerCard.ElementType;
            Effectiveness effectiveness = Effectiveness.noEffect;

            // Not Effective
            if (leftE == ElementType.Fire && rightE == ElementType.Water ||
             leftE == ElementType.Normal && rightE == ElementType.Fire ||
            leftE == ElementType.Water && rightE == ElementType.Normal) effectiveness = Effectiveness.notEffective;

            // Effective
            else if (leftE == ElementType.Water && rightE == ElementType.Fire ||
            leftE == ElementType.Fire && rightE == ElementType.Normal ||
            leftE == ElementType.Normal && rightE == ElementType.Water) effectiveness = Effectiveness.isEffective;

            // No Effect
            return effectiveness;
        }


        private void updateGame(int leftPlayerDamage, Card leftPlayerCard, int rightPlayerDamage, Card rightPlayerCard)
        {
            if (leftPlayerDamage > rightPlayerDamage)
            {
                LeftPlayer.AddWin(WinningPoints);
                RightPlayer.AddLosses(LosingPoints);
                LeftPlayer.Deck.addCard(rightPlayerCard);
                removeCardFromPlayer(RightPlayer, rightPlayerCard);
            }
            else if (rightPlayerDamage < leftPlayerDamage)
            {
                LeftPlayer.AddLosses(LosingPoints);
                RightPlayer.AddWin(WinningPoints);
                RightPlayer.Deck.addCard(leftPlayerCard);
                removeCardFromPlayer(LeftPlayer, leftPlayerCard);
            }
            else
            {
                LeftPlayer.AddDraw();
                RightPlayer.AddDraw();
            }
        }

        private void removeCardFromPlayer(User player, Card card)
        {
            player.Deck.removeCard(card);
            Card? randomCard = player.Stack.popRandomCard();
            if (randomCard != null)
                player.Deck.addCard(randomCard);
        }

        public GameStatus checkGameOver()
        {
            if (LeftPlayer.NoCardsLeft()) return GameStatus.LeftPlayerLost;
            if (RightPlayer.NoCardsLeft()) return GameStatus.RightPlayerLost;
            return GameStatus.Playing;
        }
    }
}
