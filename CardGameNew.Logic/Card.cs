namespace CardGameNew.Logic;
using System.Text.Json.Serialization;

public enum Rank
    {
        Ace = 0,
        Two = 1,
        Three = 2,
        Four = 3,
        Five = 4,
        Six = 5,
        Seven = 6,
        Eight = 7,
        Nine = 8,
        Ten = 9,
        Jack = 10,
        King = 11,
        Queen = 12,
        Joker = 13
    }

    public class Card
    {
        [JsonInclude]
        public Rank Rank { get; private set; }

        [JsonIgnore]
        public bool IsWild => Rank == Rank.Two || Rank == Rank.Joker;

        public Card() { }
        public Card(Rank rank) => Rank = rank;
    }


