using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CardGameNew.Logic
{
    public class Deck
    {
        private static readonly Random _rng = new Random();

        [JsonInclude]
        public List<Card> Cards { get; set; }

        public Deck()
        {
            Cards = new List<Card>();
        }

        // Initializes a new deck
        public Deck(IEnumerable<Card> cards)
        {
            Cards = cards.ToList();
        }

        public void Shuffle()
        {
            Cards.Sort((a, b) => _rng.Next(-1, 2));
        }

        public Card Draw()
        {
            if (Cards.Count == 0)
                throw new InvalidOperationException("Cannot draw from an empty deck.");
            var c = Cards[0];
            Cards.RemoveAt(0);
            return c;
        }

        public void Add(Card card) => Cards.Add(card);
        public bool Remove(Card card) => Cards.Remove(card);
    }
}