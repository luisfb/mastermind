using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Mastermind.Game.MastermindGameSession;
using Mastermind.Game.ReturnObjects;

namespace Mastermind.Game
{
    public sealed class Game
    {
        /// <summary>
        /// R = Red, B = Blue, G = Green, Y = Yellow, O = Orange, P = Purple, C = Cyan, M = Magenta
        /// </summary>
        private static readonly char[] _colors = new[] {'R', 'B', 'G', 'Y', 'O', 'P', 'C', 'M'};
        
        public Response StartNewGame(string userName, bool multiplayer)
        {
            if (string.IsNullOrEmpty(userName))
                return new Response("Please provide a valid player name.");
            Response response;
            try
            {
                var session = GameSessionFactory.GetSession(_colors, userName, multiplayer);
                response = new Response(_colors, session.Players[0].GameKey, multiplayer);
            }
            catch (GameSessionException ex)
            {
                response = new Response(ex);
            }
            return response;
        }

        public Response JoinAMultiplayerMatch(string hostPlayer, string userName)
        {
            if (string.IsNullOrEmpty(hostPlayer) || string.IsNullOrEmpty(userName))
                return new Response("Please provide a valid host player and your player name.");
            Response response;
            try
            {
                var session = GameSessionFactory.GetSession(hostPlayer, userName);
                response = new Response(_colors, session.Players[1].GameKey, true);
            }
            catch (GameSessionException ex)
            {
                response = new Response(ex);
            }
            return response;
        }

        public Response TryANewGuess(string gameKey, string guess)
        {
            if(string.IsNullOrEmpty(gameKey) || string.IsNullOrEmpty(guess))
                return new Response("Please provide a valid game key and a colors guess.");
            guess = guess.ToUpper();
            GameSession session = GameSessionFactory.GetSession(gameKey);
            Player player = session.GetPlayer(gameKey);
            Response response;
            try
            {
                Validate(session, player, guess);
            }
            catch (GameException ex)
            {
                response = new Response("Unable to evaluate your guess: " + ex.Message);
                return response;
            }
            Player opponent = session.Players.FirstOrDefault(x => x.GameKey != player.GameKey);
            if (opponent != null)
            {
                //avoid a second asynchronous call
                if (player.TotalGuesses > opponent.TotalGuesses)
                    return new Response("Please wait until your opponent submit his guess.");
            }
            Result guessEvaluation = EvaluateGuess(session.GetColors(), guess);
            player.AddResult(guessEvaluation);
            player.TotalGuesses += 1;
            player.Solved = guessEvaluation.Exact == session.GetColors().Length;

            if (opponent != null)
            {
                if (player.TotalGuesses != opponent.TotalGuesses)
                {
                    //wait until the opponent submit his guess
                    WaitUntilBothPlayersHaveGuessed(session, player, opponent);
                }
            }

            if (player.Solved)
                session.SetWinner(player);
                
            response = new Response(_colors, player, session);
            return response;
        }

        private void WaitUntilBothPlayersHaveGuessed(GameSession session, Player playerOne, Player playerTwo)
        {
            var task = new Task(() =>
            {
                while (playerOne.TotalGuesses != playerTwo.TotalGuesses)
                {
                    if(session.IsSessionExpired())
                        break;
                    Thread.Sleep(1000);
                }
            });
            task.RunSynchronously();
        }

        internal void Validate(GameSession session, Player player, string guess)
        {
            if (session.SessionStatus == GameSessionStatus.WaitingForChallenger)
                throw new GameException("The game session is still waiting for a challenger.");
            
            if (session.IsSessionExpired())
            {
                if (session.SessionStatus == GameSessionStatus.Finished)
                    throw new GameException("This game session is already finished.");
                if (session.SessionStatus == GameSessionStatus.Expired)
                    throw new GameException("This game session is expired!");
            }
            if (guess.Length != session.GetColors().Length)
                throw new GameException("Please provide a guess with " + session.GetColors().Length.ToString() + " colors.");
        }

        internal Result EvaluateGuess(char[] correctSequence, string guess)
        {
            var guessSequence = guess.ToCharArray();
            short exactGuesses = 0;
            short nearGuesses = 0;
            for (var i = 0; i < correctSequence.Length; i++)
            {
                //Firstly, identifies if the current char is in the correct place
                if (correctSequence[i] == guessSequence[i])
                {
                    exactGuesses += 1;
                }
                //If not, tries to find if the guessed sequence contains the current char at any position
                else if (guessSequence.Contains(correctSequence[i]))
                {
                    nearGuesses += 1;
                }
            }
            return new Result(exactGuesses, nearGuesses, guess);
        }
    }
}
