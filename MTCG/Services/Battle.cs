using MTCG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Services
{
    public class Battle
    {
        public User LeftPlayer { get; private set; }
        public User RightPlayer { get; private set; }
        public int CurrentRound { get; private set; } = 1;

        private const int MaxRounds = 100;

        public const int WinningPoints = 3;

        public const int LosingPoints = 3;
        enum Effectiveness
        {
            isEffective,
            notEffective,
            noEffective
        }
        public Battle(User leftPlayer, User rightPlayer)
        {
            LeftPlayer = leftPlayer; 
            RightPlayer = rightPlayer;
        }

        public void start()
        {
            Random rnd = new Random();
            while(CurrentRound <= MaxRounds)
            {
                Card leftPlayerCard = LeftPlayer.deck.Cards[rnd.Next(4)];
                Card rightPlayerCard = RightPlayer.deck.Cards[rnd.Next(4)];

                int leftPlayerDamage = leftPlayerCard.Damage;
                int rightPlayerDamage = rightPlayerCard.Damage;

                if (_isSpellCard(leftPlayerCard) || _isSpellCard(rightPlayerCard))
                {
                    leftPlayerDamage = reCalcDamage(leftPlayerCard, rightPlayerCard);
                    rightPlayerDamage = reCalcDamage(rightPlayerCard, leftPlayerCard);
                }

                updateGame(leftPlayerCard, rightPlayerCard);
                printStatistics(CurrentRound);
                CurrentRound++;
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
            switch(getEffectiveness(leftPlayerCard, rightPlayerCard))
            {
                case Effectiveness.isEffective:
                    damage *= 2;
                    break;
                case Effectiveness.notEffective:
                    damage /= 2;
                    break;
            }
            return damage;
        }
        private Effectiveness getEffectiveness(Card leftPlayerCard, Card rightPlayerCard)
        {
            ElementType leftE = leftPlayerCard.ElementType;
            ElementType rightE = rightPlayerCard.ElementType;
            Effectiveness effectiveness = Effectiveness.noEffective;

            // Not Effective
            if (leftE == ElementType.Fire && rightE == ElementType.Water || 
             leftE == ElementType.Normal && rightE == ElementType.Fire || 
            leftE == ElementType.Water && rightE == ElementType.Normal) effectiveness= Effectiveness.notEffective;

            // Effective
            else if (leftE == ElementType.Water && rightE == ElementType.Fire || 
            leftE == ElementType.Fire && rightE == ElementType.Normal || 
            leftE == ElementType.Normal && rightE == ElementType.Water) effectiveness = Effectiveness.isEffective;

            // No Effect
            return effectiveness;
        }


        private void updateGame(Card leftPlayerCard, Card rightPlayerCard)
        {
            if (leftPlayerCard.Damage > rightPlayerCard.Damage)
            {
                LeftPlayer.addWin();
                RightPlayer.addLosses();
                RightPlayer.deck.RemoveCard(rightPlayerCard);
                LeftPlayer.deck.AddCard(rightPlayerCard);
            }
            else if (leftPlayerCard.Damage < rightPlayerCard.Damage)
            {
                LeftPlayer.addLosses();
                RightPlayer.addWin();
                LeftPlayer.deck.RemoveCard(leftPlayerCard);
                RightPlayer.deck.AddCard(leftPlayerCard);
            }
            else
            {
                LeftPlayer.addDraw();
                RightPlayer.addDraw();
            }
        }
    }
}
