using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Models
{
    public class Package
    {
        public string Id {  get;}
        public List<Card> Cards { get; }

        public const int Capacity = 4;
        public Package(List<Card> cards, string id)
        {
            Cards = cards.Take(Capacity).ToList();
            Id = id;
        }

        public void AddCard(Card card)
        {
            if (Cards.Count >= Capacity) return;
            Cards.Add(card);
        }
    }
}
