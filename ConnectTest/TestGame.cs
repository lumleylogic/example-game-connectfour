using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConnectFour.Game;

namespace ConnectTest.Tests
{
    [TestClass]
    public class TestGame
    {
        private IConnectGame _game;

        [TestInitialize()]
        public void Initialise()
        {
            _game = GameManager.Instance().CreateRegularConnectFourGame();
        }

        [TestCleanup()]
        public void Cleanup()
        {
            _game = null;
        }

        [TestMethod]
        public void TestGameOver()
        {
            _game.Restart();
            _game.Stop();

            Assert.IsNull(_game.WhosInPlay, "Game should have stopped.");

            try
            {
                _game.AddDisc(1);
                Assert.Fail("Game should have prevented adding disc while not in progress.");
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex is GameOverException);
            }
        }

        [TestMethod]
        public void TestAddDiscOutOfBounds()
        {
            _game.Restart();

            try
            {
                _game.AddDisc(_game.Columns + 1);
                Assert.Fail("Game should have prevented adding disc to out of range column.");
            }
            catch(Exception ex)
            {
                Assert.IsTrue(ex is GameInvalidColumnException);
            }
            
            for(int row = 1; row <= _game.Rows; row++)
            {
                _game.AddDisc(1);
            }
            try
            {
                _game.AddDisc(1);
                Assert.Fail("Game should have prevented adding disc to a full column.");
            }
            catch(Exception ex)
            {
                Assert.IsTrue(ex is GameColumnFullException);
            }
        }

        [TestMethod]
        public void TestDiscLandedInExpectedSlot()
        {
            _game.Restart();
            var row = _game.Rows;
            var disc = (Player?)null;

            Assert.IsNull(_game.GetDisc(1, row), "Column 1, Row " + row + " should be empty.");
            _game.AddDisc(1);
            disc = _game.GetDisc(1, row);
            Assert.IsTrue(disc != null && disc.Value == Player.PlayerOne, "Colum 1, Row " + row + " should have player one disc.");

            Assert.IsNull(_game.GetDisc(2, row), "Column 2, Row " + row + " should be empty.");
            _game.AddDisc(2);
            disc = _game.GetDisc(2, row);
            Assert.IsTrue(disc != null && disc.Value == Player.PlayerTwo, "Colum 2, Row " + row + " should have player two disc.");

            row--;
            Assert.IsNull(_game.GetDisc(1, row), "Column 1, Row " + row + " should be empty.");
            _game.AddDisc(1);
            disc = _game.GetDisc(1, row);
            Assert.IsTrue(disc != null && disc.Value == Player.PlayerOne, "Colum 1, Row " + row + " should have player one disc.");
        }

        [TestMethod]
        public void TestConnectFourOnAllAxis()
        {
            var gameWinningLineSequences = new []
            {
                new Tuple<string, uint[], Player>("45 degress", new uint[] { 1, 2, 2, 3, 3, 4, 3, 4, 5, 4, 4 }, Player.PlayerOne ),
                new Tuple<String, uint[], Player>("90 degress", new uint[] { 1, 2, 3, 2, 4, 2, 5, 2 }, Player.PlayerTwo),
                new Tuple<string, uint[], Player>("135 degress", new uint[] { 1, 1, 1, 1, 2, 2, 3, 2, 5, 3, 5, 4 }, Player.PlayerTwo),
                new Tuple<String, uint[], Player>("180 degress", new uint[] { 1, 7, 2, 7, 3, 7, 4 }, Player.PlayerOne)   
            };

            foreach (var gameWinningLineSequence in gameWinningLineSequences)
            {
                _game.Restart();
                for (int disc = 0; disc < gameWinningLineSequence.Item2.Length; disc++)
                {
                    _game.AddDisc(gameWinningLineSequence.Item2[disc]);
                    if (disc == gameWinningLineSequence.Item2.Length - 1)
                        Assert.IsTrue(
                            _game.WhoIsWinner != null && _game.WhoIsWinner == gameWinningLineSequence.Item3,
                            gameWinningLineSequence.Item1 + " axis test failed, player " + gameWinningLineSequence.Item3 + " should have won."
                        );
                    else
                        Assert.IsNull(
                            _game.WhoIsWinner,
                            "Game ended prematurely on disc " + (disc + 1) + " with player " + _game.WhoIsWinner + " the winner."
                        );
                 }
            }
        }
    }
}
