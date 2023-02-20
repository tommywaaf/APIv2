using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace YourNamespace
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleController : ControllerBase
    {
        private readonly IMongoDatabase _db;

        public ScheduleController(IMongoDatabase db)
        {
            _db = db;
        }

        [HttpGet("next")]
        public ActionResult<GameDetails> GetNextGame()
        {
            var gamesCollection = _db.GetCollection<Game>("games");
            var botsCollection = _db.GetCollection<Bot>("bots");

            var game = gamesCollection.Find(g => g.Winner == null).SortBy(g => g.GameId).FirstOrDefault();
            if (game == null)
            {
                return NotFound(new { message = "No active games found." });
            }

            var team1 = botsCollection.Find(b => b.BotId == game.Team1).FirstOrDefault();
            var team2 = botsCollection.Find(b => b.BotId == game.Team2).FirstOrDefault();

            var gameDetails = new GameDetails
            {
                GameId = game.GameId,
                Team1 = new BotDetails
                {
                    Name = team1.Name,
                    BattleCapability = team1.BattleCapability,
                    Wins = team1.Wins,
                    Losses = team1.Losses,
                    BotId = team1.BotId,
                    ImageId = team1.ImageId.ToString()
                },
                Team2 = new BotDetails
                {
                    Name = team2.Name,
                    BattleCapability = team2.BattleCapability,
                    Wins = team2.Wins,
                    Losses = team2.Losses,
                    BotId = team2.BotId,
                    ImageId = team2.ImageId.ToString()
                }
            };

            return Ok(gameDetails);
        }
    }
    public class Game
    {
        public int gameId { get; set; }
        public string team1 { get; set; }
        public string team2 { get; set; }
        public string winner { get; set; }
        public string resulttext { get; set; }
    }

    public class Bot
    {
        public string botId { get; set; }
        public string name { get; set; }
        public int battleCapability { get; set; }
        public int wins { get; set; }
        public int losses { get; set; }
        public string imageId { get; set; }
    }

    public class GameDetails
    {
        public int gameId { get; set; }
        public BotDetails team1 { get; set; }
        public BotDetails team2 { get; set; }
    }

    public class BotDetails
    {
        public string name { get; set; }
        public int battleCapability { get; set; }
        public int wins { get; set; }
        public int losses { get; set; }
        public string botId { get; set; }
        public string imageId { get; set; }
    }
