using Microsoft.Extensions.Logging;
using MTCG.Models;
using System.Data;
using System.Numerics;
using System.Reflection;
using System.Text;

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

        private readonly UserManager _userManager = new();
        public List<string> BattleLog { get; private set; } = new();

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

        public List<string> Start()
        {
            Random rnd = new();
            GameStatus gameStatus = GameStatus.Playing;
            while (CurrentRound <= MaxRounds && gameStatus == GameStatus.Playing)
            {
                BattleLog.Add($"----------Round {CurrentRound}----------");
                PrintStatistics(CurrentRound);
                
                gameStatus = CheckGameOver();
                if (gameStatus != GameStatus.Playing) break;

                var leftPlayerCards = LeftPlayer.Deck.Cards;
                var rightPlayerCards = RightPlayer.Deck.Cards;

                Card leftPlayerCard = leftPlayerCards[rnd.Next(leftPlayerCards.Count)];
                Card rightPlayerCard = rightPlayerCards[rnd.Next(rightPlayerCards.Count)];

                BattleLog.Add($"Left Player uses {leftPlayerCard.Name} with {leftPlayerCard.Damage} damage.");
                BattleLog.Add($"Right Player uses {rightPlayerCard.Name} with {rightPlayerCard.Damage} damage.");

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
            {
                UpdateStats(RightPlayer, LeftPlayer);
                BattleLog.Add($"Left Player Lost - Remaining Cards: {LeftPlayer.CardsCount()}");
            }
            if (gameStatus == GameStatus.RightPlayerLost)
            {
                UpdateStats(LeftPlayer, RightPlayer);
                BattleLog.Add($"Right Player Lost - Remaining Cards: {RightPlayer.CardsCount()}");

            }
            else if(CurrentRound >= MaxRounds)
            {
                if(LeftPlayer.Elo < RightPlayer.Elo)
                    UpdateStats(RightPlayer, LeftPlayer);
                else if(RightPlayer.Elo < LeftPlayer.Elo)
                    UpdateStats(LeftPlayer, RightPlayer);
            }
            else
            {
                _userManager.AddDrawToUser(LeftPlayer);
                _userManager.AddDrawToUser(RightPlayer);
                BattleLog.Add("Draw...");
            }

            return BattleLog;
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
                UpdateRound(LeftPlayer, RightPlayer, rightPlayerCard);
            else if (leftPlayerDamage < rightPlayerDamage)
                UpdateRound(RightPlayer, LeftPlayer, leftPlayerCard);
        }

        private void UpdateRound(User winner, User loser, Card card)
        {
            _userManager.IncEloUser(winner, WinningPoints);
            _userManager.DecEloUser(loser, LosingPoints);
            _userManager.AddCardToStack(winner, card);
            RemoveCardFromPlayer(loser, card);
        }

        private void UpdateStats(User winner, User loser)
        {
            _userManager.AddWinToUser(winner);
            _userManager.AddLossesToUser(loser);
        }

        private void RemoveCardFromPlayer(User player, Card card)
        {
            _userManager.RemoveCardFromDeck(player, card);
            Card? randomCard = _userManager.PopRandomCardFromStack(player);
            if (randomCard != null)
                _userManager.AddCardToDeck(player, randomCard);
        }

        public GameStatus CheckGameOver()
        {
            if (LeftPlayer.NoCardsLeft()) return GameStatus.LeftPlayerLost;
            if (RightPlayer.NoCardsLeft()) return GameStatus.RightPlayerLost;
            return GameStatus.Playing;
        }
    }
}
