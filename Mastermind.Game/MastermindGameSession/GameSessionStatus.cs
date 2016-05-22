using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mastermind.Game.MastermindGameSession
{
    internal enum GameSessionStatus
    {
        Undefined = 0,
        WaitingForChallenger = 1,
        Started = 2,
        Finished = 4,
        Expired = 8
    }
}
