using System;
using System.Collections.Generic;
using System.Linq;
using Mastermind.Game.MastermindGameSession;

namespace Mastermind.Game.ReturnObjects
{
    public sealed class Response
    {
        internal Response(IEnumerable<char> possibleColors, string gameKey, bool multiplayer)
        {
            Colors = new List<char>(possibleColors);
            Game_key = gameKey;
            Past_results = new List<Result>();
            Multiplayer = multiplayer;
        }
        internal Response(IEnumerable<char> possibleColors, Player player, GameSession session)
        {
            Colors = new List<char>(possibleColors);
            Game_key = player.GameKey;
            Num_guesses = player.TotalGuesses;
            Past_results = player.PastResults.Take(player.PastResults.Count - 1).ToList();
            Result = player.LastResult;
            Solved = player.Solved;
            Multiplayer = session.IsMultiplayer;
            if (session.Winner != null)
            {
                Winner = session.Winner.PlayerName;
                if (session.IsMultiplayer)
                    Opponent_Past_results = session.Players.First(x => x.GameKey != player.GameKey).PastResults;
            }
        }
        internal Response(Exception exception)
        {
            Error = exception.Message;
        }
        internal Response(string errorMessage)
        {
            Error = errorMessage;
        }

        public IList<char> Colors { get; set; }
        public short Code_length
        {
            get { return (short) (Colors?.Count ?? 0); } 
        }
        public string Game_key { get; private set; }
        public short Num_guesses { get; private set; }
        public IList<Result> Past_results { get; private set; }
        public IList<Result> Opponent_Past_results { get; private set; }
        public Result Result { get; private set; }
        public bool Solved { get; private set; }
        public string Error { get; private set; }
        public string Winner { get; private set; }
        public bool Multiplayer { get; private set; }
    }
}
