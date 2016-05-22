using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mastermind.Game.ReturnObjects;

namespace Mastermind.Game.MastermindGameSession
{
    internal class Player
    {
        public Player(string name, string gameKey)
        {
            PlayerName = name;
            GameKey = gameKey;
            PastResults = new List<Result>();
        }
        public void AddResult(Result result)
        {
            PastResults.Add(result);
        }
        public string PlayerName { get; private set; }
        public string GameKey { get; private set; }
        public IList<Result> PastResults { get; private set; }

        public Result LastResult
        {
            get { return PastResults?.Last(); } 
        }

        public short TotalGuesses { get; set; }
        public bool Solved { get; set; }
    }
}
