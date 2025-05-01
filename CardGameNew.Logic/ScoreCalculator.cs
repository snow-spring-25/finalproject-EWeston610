using System.Linq;

namespace CardGameNew.Logic
{
    public static class ScoreCalculator
    {
        public static int CalculateCardScore(Deck deck)
        {
            return deck.Cards.Sum(card => card.Rank switch
            {
                Rank.Joker => 50,
                Rank.Two or Rank.Ace => 20,
                Rank.Eight or Rank.Nine or Rank.Ten or Rank.Jack or Rank.King or Rank.Queen => 10,
                Rank.Three or Rank.Four or Rank.Five or Rank.Six or Rank.Seven => 5,
                _ => 0
            });
        }

        public static int CalculateMeldScore(IMeld meld)
        {
            return meld.IsClean ? 500 : 300;
        }
    }
}