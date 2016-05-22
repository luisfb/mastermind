using System;
using System.Collections.Generic;
using System.Linq;

namespace Mastermind.Game.MastermindGameSession
{
    internal static class GameSessionFactory
    {
        private const short GAME_MINUTES_LIMIT = 5;
        private static IList<GameSession> _gameSessions = new List<GameSession>();

        /// <summary>
        /// Tries to create a new game session in case it doesn't already exists for the current user
        /// </summary>
        /// <param name="colors">An array of char corresponding to the possible colors to use in the game</param>
        /// <param name="userName">The user/player name</param>
        /// <param name="multiplayer">Specify whether the match should be or not a multiplayer match</param>
        /// <returns>New GameSession</returns>
        public static GameSession GetSession(char[] colors, string userName, bool multiplayer)
        {
            GameSession existingSession;
            lock (_gameSessions)
            {
                existingSession = _gameSessions.FirstOrDefault(x => x.Players.Any(p => p.PlayerName == userName) && !x.IsSessionExpired());
            }
            if (existingSession != null)
                throw new GameSessionException(
                    "There is already an active session for the current user. Please inform your game key to retreive your current session.");

            var newPlayer = new Player(userName, Guid.NewGuid().ToString());
            var newSession = new GameSession(colors, newPlayer, GAME_MINUTES_LIMIT, multiplayer);
            lock (_gameSessions)
            {
                _gameSessions.Add(newSession);
            }
            return newSession;
        }

        /// <summary>
        /// Retrieves the current session for the specified key
        /// </summary>
        /// <param name="gameKey">The game key</param>
        /// <returns>Existing GameSession</returns>
        public static GameSession GetSession(string gameKey)
        {
            GameSession existingSession;
            lock (_gameSessions)
            {
                existingSession = _gameSessions.FirstOrDefault(x => x.Players.Any(p => p.GameKey == gameKey));
            }
            if(existingSession == null)
                throw new GameSessionException("No existing session with the provided key.");
            return existingSession;
        }

        /// <summary>
        /// Retrieves an existing session for a multiplayer game
        /// </summary>
        /// <param name="host">The user/player name that is waiting for a challenger</param>
        /// <param name="userName">The challenger name</param>
        /// <returns>Existing Multiplayer GameSession</returns>
        public static GameSession GetSession(string host, string userName)
        {
            GameSession existingMultiplayerSession;
            lock (_gameSessions)
            {
                existingMultiplayerSession = _gameSessions.FirstOrDefault(x => x.Players.Any(p => p.PlayerName == host) &&
                                                               x.IsMultiplayer &&
                                                               x.SessionStatus == GameSessionStatus.WaitingForChallenger);
            }
            if (existingMultiplayerSession == null)
                throw new GameSessionException("No existing multiplayer session with the provided host username.");

            var challenger = new Player(userName, Guid.NewGuid().ToString());
            existingMultiplayerSession.AddPlayerToMatch(challenger);

            return existingMultiplayerSession;
        }
    }
}
