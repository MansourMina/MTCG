using MTCG.Models;

namespace MTCG.Services
{
    public class BattleService(User leftPlayer, User rightPlayer)
    {
        public User LeftPlayer { get; private set; } = leftPlayer;
        public User RightPlayer { get; private set; } = rightPlayer;
        public int CurrentRound { get; private set; } = 1;

        private const int MaxRounds = 100;

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

        public void Start()
        {
            Random rnd = new();
            GameStatus gameStatus = GameStatus.Playing;
            while (CurrentRound <= MaxRounds && gameStatus == GameStatus.Playing)
            {
                PrintStatistics(CurrentRound);
                
                gameStatus = CheckGameOver();
                if (gameStatus != GameStatus.Playing) break;

                var leftPlayerCards = LeftPlayer.Deck.Cards;
                var rightPlayerCards = RightPlayer.Deck.Cards;

                Card leftPlayerCard = leftPlayerCards[rnd.Next(leftPlayerCards.Count)];
                Card rightPlayerCard = rightPlayerCards[rnd.Next(rightPlayerCards.Count)];

                int leftPlayerDamage = leftPlayerCard.Damage;
                int rightPlayerDamage = rightPlayerCard.Damage;

                if (IsSpellCard(leftPlayerCard) || IsSpellCard(rightPlayerCard))
                {
                    leftPlayerDamage = ReCalcDamage(leftPlayerCard, rightPlayerCard);
                    rightPlayerDamage = ReCalcDamage(rightPlayerCard, leftPlayerCard);
                }
                UpdateGame(leftPlayerDamage, leftPlayerCard, rightPlayerDamage, rightPlayerCard);
                CurrentRound++;
            }

            if (gameStatus == GameStatus.LeftPlayerLost)
                Console.WriteLine($"Left Player Lost - he has {LeftPlayer.CardsCount()} Cards left");
            if (gameStatus == GameStatus.RightPlayerLost)
                Console.WriteLine($"Right Player Lost - he has {RightPlayer.CardsCount()} Cards left");
            else
            {
                Console.WriteLine($"{(RightPlayer.Statistic.Wins > LeftPlayer.Statistic.Wins ? "Right" : "Left")} WON");
            }
            Console.WriteLine("Left Player Deck: " + LeftPlayer.Deck.Cards.Count);
            Console.WriteLine("Left Player Stack: " + LeftPlayer.Stack.Cards.Count);
            Console.WriteLine("Right Player Deck: " + RightPlayer.Deck.Cards.Count);
            Console.WriteLine("Right Player Stack: " + RightPlayer.Stack.Cards.Count);


        }

        private void PrintStatistics(int round)
        {
            Console.WriteLine($"----------Round {round}----------");
            Console.WriteLine($"Left Player: {LeftPlayer.Elo}");
            Console.WriteLine($"Right Player: {RightPlayer.Elo}");
            Console.WriteLine("\n");
        }

        private static bool IsSpellCard(Card card)
        {
            return card.GetType().Name == CardType.Spell.ToString();
        }

        private static int ReCalcDamage(Card leftPlayerCard, Card rightPlayerCard)
        {
            int damage = leftPlayerCard.Damage;
            switch (GetEffectiveness(leftPlayerCard, rightPlayerCard))
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
        private static Effectiveness GetEffectiveness(Card leftPlayerCard, Card rightPlayerCard)
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


        private void UpdateGame(int leftPlayerDamage, Card leftPlayerCard, int rightPlayerDamage, Card rightPlayerCard)
        {
            if (leftPlayerDamage > rightPlayerDamage)
            {
                LeftPlayer.AddWin(WinningPoints);
                RightPlayer.AddLosses(LosingPoints);
                LeftPlayer.Deck.addCard(rightPlayerCard);
                RemoveCardFromPlayer(RightPlayer, rightPlayerCard);
            }
            else if (leftPlayerDamage < rightPlayerDamage)
            {
                RightPlayer.AddWin(WinningPoints);
                LeftPlayer.AddLosses(LosingPoints);
                RightPlayer.Deck.addCard(leftPlayerCard);
                RemoveCardFromPlayer(LeftPlayer, leftPlayerCard);
            }
            else
            {
                LeftPlayer.AddDraw();
                RightPlayer.AddDraw();
            }
        }

        private static void RemoveCardFromPlayer(User player, Card card)
        {
            player.Deck.removeCard(card);
            Card? randomCard = player.Stack.popRandomCard();
            if (randomCard != null)
                player.Deck.addCard(randomCard);
        }

        public GameStatus CheckGameOver()
        {
            if (LeftPlayer.NoCardsLeft()) return GameStatus.LeftPlayerLost;
            if (RightPlayer.NoCardsLeft()) return GameStatus.RightPlayerLost;
            return GameStatus.Playing;
        }
    }
}
