using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using CardGameNew.Logic;
using CardGameNew.Persistence;

namespace CardGameNew.Tests
{
    [TestFixture]
    public class AllTests
    {
        private string _tempFile;

        [SetUp]
        public void Setup()
        {
            _tempFile = Path.GetTempFileName();
        }

        [TearDown]
        public void TearDown()
        {
            if (File.Exists(_tempFile))
                File.Delete(_tempFile);
        }

        [Test]
        public void Deck_Draw_Remove_Shuffle_Behavior()
        {
            var deck = new Deck(new[] { new Card(Rank.Ace), new Card(Rank.King) });
            var first = deck.Draw(); // REQ#1.1.1
            Assert.That(first.Rank, Is.EqualTo(Rank.Ace));
            Assert.That(deck.Cards.Count, Is.EqualTo(1));
            Assert.That(deck.Cards.First().Rank, Is.EqualTo(Rank.King));
            Assert.Throws<InvalidOperationException>(() => new Deck().Draw()); // REQ#1.1.2

            var c = new Card(Rank.Two);
            deck.Add(c);
            Assert.That(deck.Remove(c), Is.True); // REQ#1.2.1
            Assert.That(deck.Remove(new Card(Rank.Queen)), Is.False); // REQ#1.2.2

            var cards = Enumerable.Range(0, 10).Select(i => new Card((Rank)(i % 14))).ToList();
            deck = new Deck(cards);
            var before = deck.Cards.Select(x => x.Rank).ToArray();
            deck.Shuffle(); // REQ#1.3.1
            var after = deck.Cards.Select(x => x.Rank).ToArray();
            Assert.That(after.Length, Is.EqualTo(before.Length));
            Assert.That(after, Is.Not.EqualTo(before));
            Assert.That(after, Is.EquivalentTo(before));
        }

        [Test]
        public void Meld_Clean_And_Dirty_Rules()
        {
            var clean = new CleanMeld(Rank.Five); // REQ#1.4.1
            Assert.That(clean.CanAdd(new Card(Rank.Two)), Is.False);
            Assert.That(clean.CanAdd(new Card(Rank.Five)), Is.True);

            var dirty = new DirtyMeld(Rank.Three); // REQ#1.4.2
            Assert.That(dirty.CanAdd(new Card(Rank.Two)), Is.True);
            for (int i = 0; i < 7; i++) dirty.Add(new Card(Rank.Three));
            Assert.That(dirty.CanAdd(new Card(Rank.Three)), Is.False);
        }

        [Test]
        public void ScoreCalculator_Card_And_Meld_Scores()
        {
            var deck = new Deck();
            foreach (Rank r in Enum.GetValues(typeof(Rank)))
                deck.Add(new Card(r));
            int expected = 50 + 20 + 20 + 10 * 6 + 5 * 5;
            Assert.That(ScoreCalculator.CalculateCardScore(deck), Is.EqualTo(expected)); // REQ#1.5.1

            Assert.That(ScoreCalculator.CalculateMeldScore(new CleanMeld(Rank.Ace)), Is.EqualTo(500)); // REQ#1.5.2
            Assert.That(ScoreCalculator.CalculateMeldScore(new DirtyMeld(Rank.Ace)), Is.EqualTo(300)); 
        }

        [Test]
        public void Game_Turns_And_Scoring()
        {
            var game = new Game("A", "B");
            Assert.That(game.CurrentPlayer.Name, Is.EqualTo("A"));

            game.NextTurn(); // REQ#1.6.1
            Assert.That(game.CurrentPlayer.Name, Is.EqualTo("B"));

            Assert.That(game.CalculateScore(game.CurrentPlayer), Is.Zero); // REQ#1.6.2
        }

        [Test]
        public void Player_Discard_And_CreateMeld()
        {
            var player = new Player("X");
            var card = new Card(Rank.King);
            player.Hand.Add(card);
            player.Discard(card); // REQ#1.7.1
            Assert.That(player.Hand.Cards.Count, Is.Zero);

            var cards = new[] { new Card(Rank.Four), new Card(Rank.Four), new Card(Rank.Four) };
            foreach (var c in cards) player.Hand.Add(c);
            player.CreateMeld(Rank.Four, true, cards); // REQ#1.7.2
            Assert.That(player.Melds.Count, Is.EqualTo(1));
            Assert.That(player.Melds[0].Rank, Is.EqualTo(Rank.Four));
            Assert.That(player.Melds[0].IsClean, Is.True);
        }

        [Test]
        public void DeckBuilder_And_Persistence()
        {
            var pile1 = DeckBuilder.CreateDrawPile(1); // REQ#1.8.1
            Assert.That(pile1.Cards.Count, Is.EqualTo(54));
            var pile2 = DeckBuilder.CreateDrawPile(2);
            Assert.That(pile2.Cards.Count, Is.EqualTo(108));

            var game = DeckBuilder.SetupGame(new[] { "AA", "BB" }, 1); // REQ#1.8.2
            Assert.That(game.Players.All(p => p.Hand.Cards.Count == 11));
            Assert.That(game.DrawPile.Cards.Count, Is.EqualTo(54 - 22 - 22));

            var ser = new JsonGameSerializer();
            ser.Save(game, _tempFile); // REQ#1.8.3
            var loaded = ser.Load(_tempFile);
            Assert.That(loaded.Players.Count, Is.EqualTo(game.Players.Count));

            Assert.Throws<FileNotFoundException>(() => ser.Load("no_such_file.json")); // REQ#1.8.4
        }
    }
}
