using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Santorini.Players.Host.Controllers
{
    [Route("commands")]
    [ApiController]
    public class CommandController : ControllerBase
    {
        private readonly RandomPlayer _player;

        public CommandController(RandomPlayer player)
        {
            _player = player;
        }

        [HttpPost("add-workers")]
        public ActionResult<PlaceWorkersCommand> AddWorker([FromBody]Game game)
        {
            while (true)
            {
                var placeWorkerCmd = new PlaceWorkersCommand
                {
                    WorkerOne = _player.GetNewWorkerCoord(0),
                    WorkerTwo = _player.GetNewWorkerCoord(1)
                };

                if (placeWorkerCmd.IsValid)
                    return placeWorkerCmd;
            }
        }

        [HttpPost("move")]
        public ActionResult<MoveCommand> Get([FromBody]Game game)
        {
            while (true)
            {
                var moveCmd = _player.GetNextMoveCommand(game.Island);

                if (moveCmd.IsValid)
                    return moveCmd;
            }
        }
        
        [HttpPost("report")]
        public async Task<IActionResult> Post([FromBody] Game game)
        {
            var report = JsonConvert.SerializeObject(game);
            await System.IO.File.AppendAllTextAsync($"./reports/game-{DateTime.UtcNow.ToString("yyMMddHHmmss")}.log", report);

            return Ok();
        }
    }
}
