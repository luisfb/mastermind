using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mastermind.Game.ReturnObjects;

namespace Mastermind.Controllers
{
    public class GameController : Controller
    {
        // POST: new_game
        [HttpPost]
        public JsonResult NewGame(string user, string hostPlayer = "", bool multiplayer = false)
        {
            var game = new Game.Game();
            Response response;
            response = string.IsNullOrEmpty(hostPlayer) ? game.StartNewGame(user, multiplayer) : game.JoinAMultiplayerMatch(hostPlayer, user);

            var returnObject = new
            {
                colors = response.Colors,
                code_length = response.Code_length,
                game_key = response.Game_key,
                num_guesses = response.Num_guesses,
                past_results = response.Past_results,
                solved = response.Solved,
                error = response.Error,
                multiplayer = response.Multiplayer
            };

            return Json(returnObject, JsonRequestBehavior.DenyGet);
        }

        // POST: guess
        [HttpPost]
        public JsonResult Guess(string game_key, string code)
        {
            var game = new Game.Game();
            var response = game.TryANewGuess(game_key, code);
            var returnObject = new
            {
                colors = response.Colors,
                code_length = response.Code_length,
                game_key = response.Game_key,
                num_guesses = response.Num_guesses,
                result = new
                {
                    exact = response.Result?.Exact,
                    near = response.Result?.Near
                },
                past_results = response.Past_results?.Select(x => new { exact = x.Exact, near = x.Near} ).ToList(),
                opponent_past_results = response.Opponent_Past_results?.Select(x => new { exact = x.Exact, near = x.Near }).ToList(),
                solved = response.Solved,
                error = response.Error,
                multiplayer = response.Multiplayer,
                winner = response.Winner
            };

            return Json(returnObject, JsonRequestBehavior.DenyGet);
        }
        

      
    }
}
