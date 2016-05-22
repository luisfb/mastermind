using System;
using System.Security.Policy;
using Mastermind.Game;
using Mastermind.Game.MastermindGameSession;
using Mastermind.Game.ReturnObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class UnitTests
    {
        /*
        *** I ONLY MADE FEW TEST METHODS BECAUSE THE TIME WAS SHORT
        */
        private Game _game;
        private Response _response;
        private char[] _colors = new[] { 'R', 'B', 'G', 'Y', 'O', 'P', 'C', 'M' };

        private void StartNewGame()
        {
            _game = new Game();
            _response = _game.StartNewGame("John Snow", false);
        }

        [TestMethod]
        public void ShouldStartANewGame()
        {
            StartNewGame();

            Assert.IsTrue(_response.Colors.Count == 8);
            Assert.IsTrue(_response.Game_key.Length == 36);
            Assert.IsFalse(_response.Solved);
            Assert.IsTrue(_response.Num_guesses == 0);
            Assert.IsNull(_response.Error);
        }

        [TestMethod]
        public void ShouldCorrectlyEvaluateTheGuesses()
        {
            StartNewGame();

            var guess1 = _game.EvaluateGuess(_colors, "RBGYOPCM");
            Assert.AreEqual(guess1.Exact, 8);
            Assert.AreEqual(guess1.Near, 0);
            Assert.AreEqual(guess1.Guess, "RBGYOPCM");

            var guess2 = _game.EvaluateGuess(_colors, "RBGMMMMM");
            Assert.AreEqual(guess2.Exact, 4);
            Assert.AreEqual(guess2.Near, 0);
            Assert.AreEqual(guess2.Guess, "RBGMMMMM");

            var guess3 = _game.EvaluateGuess(_colors, "XXXXXXXX");
            Assert.AreEqual(guess3.Exact, 0);
            Assert.AreEqual(guess3.Near, 0);
            Assert.AreEqual(guess3.Guess, "XXXXXXXX");

            var guess4 = _game.EvaluateGuess(_colors, "MCPOYGBR");
            Assert.AreEqual(guess4.Exact, 0);
            Assert.AreEqual(guess4.Near, 8);
            Assert.AreEqual(guess4.Guess, "MCPOYGBR");
        }

        [TestMethod]
        [ExpectedException(typeof(GameException), "Will raise exception when validation fails")]
        public void ShouldCorrectlyValidate()
        {
            StartNewGame();
            var player = new Player("John Snow", _response.Game_key);
            //creates an expired session:
            var session = new GameSession(_colors, player, 0, false);
            Assert.IsTrue(session.IsSessionExpired());
            _game.Validate(session, player, "");
        }
    }
}
