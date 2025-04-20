namespace HandAndFoot.Tests;
using HandAndFoot;

using NUnit.Framework;
using HandAndFoot.Logic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public class GameInitializationTests
{
    [Test]
    public void NewDeckHasExpectedCount()
    {
        var deck = new Deck(decks: 5);
        Assert.AreEqual(5 * 54, deck.Count);
    }

    [Test]
    public void ShuffleDoesNotChangeCount()
    {
        var deck = new Deck();
        var original = deck.Count;
        deck.Shuffle();
        Assert.AreEqual(original, deck.Count);
    }
}

public class MeldValidationTests
{
    [Test]
    public void ValidCleanMeld()
    {
        var meld = new Meld(Rank.Nine);
        Enumerable.Range(0, 3).ToList()
                  .ForEach(_ => meld.Add(new Card(Suit.Clubs, Rank.Nine)));
        Assert.IsTrue(meld.IsValidInitial());
        Assert.IsTrue(meld.IsClean);
    }

    [Test]
    public void InvalidDirtyMeldFails()
    {
        var meld = new Meld(Rank.King);
        meld.Add(new Card(Suit.Spades, Rank.King));
        meld.Add(new Card(Suit.Joker, Rank.Joker));
        meld.Add(new Card(Suit.Joker, Rank.Deuce));
        Assert.IsFalse(meld.IsValidInitial());
    }
}

public class PersistenceTests
{
    private const string Path = "gamestate.json";

    [Test]
    public async Task SaveAndLoad_RestoresGame()
    {
        var game = new HandAndFootGame(numPlayers: 2);
        var repo = new JsonGameStateRepository();
        await repo.SaveAsync(game, Path);

        Assert.IsTrue(File.Exists(Path));
        var loaded = await repo.LoadAsync(Path);
        Assert.IsNotNull(loaded);
        File.Delete(Path);
    }
}
public class DeckTests
{
    [Test]
    public void Draw_AllCards_ThenThrows()
    {
        var deck = new Deck(decks: 1);
        int count = deck.Count;

        for (int i = 0; i < count; i++)
            Assert.DoesNotThrow(() => _ = deck.Draw());

        Assert.Throws<InvalidOperationException>(() => deck.Draw());
    }
}

public class MeldRuleTests
{
    [Test]
    public void CannotAddThreeToMeld()
    {
        var meld = new Meld(Rank.Five);
        var three = new Card(Suit.Diamonds, Rank.Three);
        Assert.Throws<InvalidOperationException>(() => meld.Add(three));
    }

    [Test]
    public void CleanMeldIsValidAndClean()
    {
        var meld = new Meld(Rank.Nine);
        for (int i = 0; i < 3; i++)
            meld.Add(new Card(Suit.Clubs, Rank.Nine));

        Assert.IsTrue(meld.IsValidInitial());
        Assert.IsTrue(meld.IsClean);
    }

    [Test]
    public void DirtyMeld()
    {
        var meld = new Meld(Rank.Eight);
        for (int i = 0; i < 4; i++)
            meld.Add(new Card(Suit.Hearts, Rank.Eight));
        meld.Add(new Card(Suit.Joker, Rank.Joker));

        Assert.IsTrue(meld.IsValidInitial());
        Assert.IsFalse(meld.IsClean);
    }

    [Test]
    public void AllWildMeldIsInvalid()
    {
        var meld = new Meld(Rank.King);
        for (int i = 0; i < 3; i++)
            meld.Add(new Card(Suit.Joker, Rank.Joker));

        Assert.IsFalse(meld.IsValidInitial());
    }

    [Test]
    public void MeldBecomesClosedAtSeven()
    {
        var meld = new Meld(Rank.Six);
        for (int i = 0; i < 7; i++)
            meld.Add(new Card(Suit.Spades, Rank.Six));

        Assert.IsTrue(meld.IsClosed);
        Assert.Throws<InvalidOperationException>(() => meld.Add(new Card(Suit.Spades, Rank.Six)));
    }
}

