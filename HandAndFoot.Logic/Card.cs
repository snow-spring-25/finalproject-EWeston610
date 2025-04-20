using System;

namespace HandAndFoot.Logic
{
    public enum Suit { Clubs, Diamonds, Hearts, Spades, Joker }
    public enum Rank
    {
        Three = 3,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        Ten,
        Jack,
        Queen,
        King,
        Ace = 14,
        Deuce = 15,
        Joker = 16
    }

    public class Card
    {
        public Suit Suit { get; }
        public Rank Rank { get; }
        public bool IsWild => Rank == Rank.Deuce || Rank == Rank.Joker;
        public bool IsRedThree => Rank == Rank.Three && (Suit == Suit.Diamonds || Suit == Suit.Hearts);
        public bool IsBlackThree => Rank == Rank.Three && (Suit == Suit.Clubs || Suit == Suit.Spades);
        public int PointValue => Rank switch
        {
            Rank.Three or Rank.Four or Rank.Five or Rank.Six or Rank.Seven => 5,
            Rank.Eight or Rank.Nine or Rank.Ten or Rank.Jack or Rank.Queen or Rank.King => 10,
            Rank.Ace or Rank.Deuce => 20,
            Rank.Joker => 50,
            _ => throw new InvalidOperationException($"Unknown card rank: {Rank}")
        };

        public Card(Suit suit, Rank rank)
        {
            Suit = suit;
            Rank = rank;
        }

        public override string ToString() => $"{Rank} of {Suit}";
    }
}