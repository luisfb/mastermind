using System;

namespace Mastermind.Game.MastermindGameSession
{
    public class GameSessionException : Exception
    {
        public GameSessionException(string message) : base(message)
        {}
    }
}
