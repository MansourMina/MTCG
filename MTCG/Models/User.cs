using MTCG.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Models
{

    public class User
    {
        public string Name { get; private set; }
        public string Password { get; private set; }
        public int Coins { get; private set; } = 20;
        public Stack Stack { get; private set; } = new Stack();
        public Deck deck { get; private set; } = new Deck();
        public int Elo { get; private set; } = 100;
        public Statistic statistic { get; private set; } = new Statistic();

        
        public User(string name, string password)
        {
            _setCredentials(name, password);
        }
        public User(string name, string password, Stack stack)
        {
            _setCredentials(name, password);
            Stack = stack;
        }

        
        private void _setCredentials(string name, string password)
        {
            Name = name;
            Password= password;
        }

        public void setDeck(List<Card> cards)
        {
            int deckSpace= deck.Cards.Capacity - deck.Cards.Count;
            for (int card = 0; card < deckSpace && card < cards.Count; card++)
                deck.AddCard(cards[card]);
        }

        public void printDeck()
        {
            for(int card = 0; card < deck.Cards.Count; card++)
                Console.WriteLine(deck.Cards[card].Name);
        }

        public void addWin()
        {
            Elo += Battle.WinningPoints;
            statistic.addWin();
        }

        public void addLosses()
        {
            Elo -= Battle.LosingPoints - Elo < 0 ? 0:Battle.LosingPoints;
            statistic.addLosses();
        }

        public void addDraw()
        {
            statistic.addDraw();
        }


    }
}
