using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mastermind.Game.MastermindGameSession
{
    internal class GameSession
    {
        private readonly char[] _colors;
        private readonly short _gameMinutesLimit;
        public GameSession(char[] colors, Player player, short gameMinutesLimit, bool multiPlayer)
        {
            _colors = ShuffleColors(colors);
            Players = new List<Player> {player};
            _gameMinutesLimit = gameMinutesLimit;
            IsMultiplayer = multiPlayer;
            if(multiPlayer)
                SessionStatus = GameSessionStatus.WaitingForChallenger;
            else
                StartSession();
        }

        public char[] GetColors()
        {
            return _colors;
        }

        public void StartSession()
        {
            if (SessionStatus == GameSessionStatus.Started || SessionStatus == GameSessionStatus.Finished)
                return;
            StarTime = DateTime.Now;
            SessionStatus = GameSessionStatus.Started;
        }

        public bool IsSessionExpired()
        {
            if (SessionStatus == GameSessionStatus.Finished || SessionStatus == GameSessionStatus.Expired)
                return true;
            if (SessionStatus == GameSessionStatus.Started)
            {
                if ((DateTime.Now - StarTime).Minutes >= _gameMinutesLimit)
                {
                    SessionStatus = GameSessionStatus.Expired;
                    return true;
                }
            }
            return false;
        }

        public void AddPlayerToMatch(Player player)
        {
            if (SessionStatus != GameSessionStatus.WaitingForChallenger)
                return;
            Players.Add(player);
            StartSession();
        }

        public Player GetPlayer(string gameKey)
        {
            return Players.FirstOrDefault(x => x.GameKey == gameKey);
        }

        public void SetWinner(Player player)
        {
            Winner = player;
            SessionStatus = GameSessionStatus.Finished;
        }

        private char[] ShuffleColors(char[] colors)
        {
            var rnd = new Random();
            return colors.OrderBy(x => rnd.Next()).ToArray();
        }

        public bool IsMultiplayer { get; private set; }
        public DateTime StarTime { get; private set; }
        public IList<Player> Players { get; private set; }
        public GameSessionStatus SessionStatus { get; private set; }
        public Player Winner { get;  set; }
    }
}
