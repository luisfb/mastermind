using System;

namespace Mastermind.Game.ReturnObjects
{
    public class Result
    {
        public Result(short exact, short near, string guess)
        {
            Exact = exact;
            Near = near;
            Guess = guess;
            Time = DateTime.Now;
        }
        public short Exact { get; private set; }
        public short Near { get; private set; }
        public string Guess { get; private set; }
        public DateTime Time { get; private set; }
    }
}
