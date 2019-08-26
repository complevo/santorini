using System;
using System.Collections.Generic;
using System.Linq;

namespace Santorini.Players
{
    public class GerwinPlayer
    {
        private const string PLAYER_NAME = "Gerwin";
        private static readonly Random _random = new Random();

        private const int SEARCH_DEPTH = 4;
        private MoveCommand NextMoveCommand = null;

        public string GetPlayerName()
        {
            return PLAYER_NAME;
        }

        public Coord GetNewWorkerCoord(int workerIndex)
        {
            if (workerIndex < 0 || workerIndex > 1)
                throw new IndexOutOfRangeException("Worker Index musst be 0 or 1");
            Coord newCoord = new Coord(_random.Next(0, 4), _random.Next(0, 4));
            return newCoord;
        }

        public MoveCommand GetNextMoveCommand(Island island)
        {
            IEnumerable<MoveCommand> allCommands = GenerateAllPossibleCommands(island);
            Dictionary<MoveCommand, int> dictMoveCommandEvaluations = new Dictionary<MoveCommand, int>();
            MoveCommand currentBestCommand = null;
            int currentBestValue = -100000;
            foreach (MoveCommand command in allCommands)
            {               
                int value = AssesCommand(island, command);
                dictMoveCommandEvaluations.Add(command, value);

                if (value >= currentBestValue)
                {
                    currentBestValue = value;
                    currentBestCommand = command;
                }
            }

            return currentBestCommand;
        }

        public MoveCommand GetNextMoveCommandMiniMax(Island island)
        {
            IEnumerable<MoveCommand> allCommands = GenerateAllPossibleCommands(island);
            Dictionary<MoveCommand, int> dictMoveCommandEvaluations = new Dictionary<MoveCommand, int>();
            MoveCommand currentBestCommand = null;
            int currentBestValue = -100000;
            foreach (MoveCommand command in allCommands)
            {
                Game game = new Game();
                game.TryAddPlayer(PLAYER_NAME);
                game.TryAddPlayer("Opponent");
                game.Island = island;

                game.TryMoveWorker(command);
                int value = AssesStatus(island);
                dictMoveCommandEvaluations.Add(command, value);

                if (value >= currentBestValue)
                {
                    currentBestValue = value;
                    currentBestCommand = command;
                }
            }

            return currentBestCommand;
        }

        public int Maximize(Game game, int depth)
        {
            if (depth == 0)
                return AssesStatus(game.Island);

            int maxValue = -100000;
            //MoveCommand currentBestCommand = null;

            IEnumerable<MoveCommand> allCommands = GenerateAllPossibleCommands(game.Island, PLAYER_NAME);

            foreach (MoveCommand command in allCommands)
            {
                Coord movedFrom = game.Island.GetWorker(command.PlayerName, command.WorkerNumber).CurrentLand.Coord;
                game.TryMoveWorker(command);
                int value = Minimize(game, depth - 1);
                game.TryUndoCommand(command, movedFrom);

                if (value > maxValue)
                {
                    maxValue = value;
                    if (depth == SEARCH_DEPTH)
                    {
                        NextMoveCommand = command;
                    }
                }
            }

            return maxValue;
        }

        public int Minimize(Game game, int depth)
        {        
            if (depth == 0)
                return AssesStatus(game.Island);

            int minValue = +100000;

            IEnumerable<MoveCommand> allCommands = GenerateAllPossibleCommands(game.Island, "Opponent");

            foreach (MoveCommand command in allCommands)
            {
                Coord movedFrom = game.Island.GetWorker(command.PlayerName, command.WorkerNumber).CurrentLand.Coord;
                game.TryMoveWorker(command);
                int value = Maximize(game, depth - 1);
                game.TryUndoCommand(command, movedFrom);

                if (value < minValue)
                {
                    minValue = value;
                }
            }

            return minValue;
        }

        private IEnumerable<MoveCommand> GenerateAllPossibleCommands(Island island, string playerName = PLAYER_NAME)
        {
            List<MoveCommand> moveCommands = new List<MoveCommand>();
            for (int workerIndex = 0; workerIndex < 2; workerIndex++)
            {
                Worker worker = island.GetWorker(playerName, workerIndex);
                List<Coord> unoccupiedMoveDirections = GetPossibleNeighbours(worker.CurrentLand.Coord)
                    .Where(c =>
                        island.IsUnoccupied(c.X, c.Y) &&
                        island.Board[c.X, c.Y].LandLevel - worker.LandLevel > 2 &&
                        island.Board[c.X, c.Y].LandLevel != 4)
                    .OrderByDescending(c => island.Board[c.X, c.Y].LandLevel)
                    .ToList();
                foreach (Coord moveCoord in unoccupiedMoveDirections)
                {
                    List<Coord> unoccupiedBuildDirections = GetPossibleNeighbours(moveCoord)
                        .Where(c =>
                            (island.IsUnoccupied(c.X, c.Y) || c == moveCoord) &&
                            island.Board[c.X, c.Y].LandLevel != 4)
                        .ToList();
                    if (!unoccupiedBuildDirections.Any())
                        continue;

                    //Coord buildCoord = unoccupiedBuildDirections[_random.Next(unoccupiedBuildDirections.Count)];
                    foreach (Coord buildCoord in unoccupiedBuildDirections)
                    {
                        moveCommands.Add(new MoveCommand(playerName, worker.Number, moveCoord, buildCoord));
                    }

                }
            }
            return moveCommands;
        }

        private IEnumerable<Coord> GetPossibleNeighbours(Coord coord)
        {
            return CartesianProduct(new[] { new[] { coord.X - 1, coord.X, coord.X + 1 }, new[] { coord.Y - 1, coord.Y, coord.Y + 1 } })
                .Select(e => new Coord(e.First(), e.Last()))
                .Where(c =>
                    c != coord &&
                    Island.IsValidPosition(c.X, c.Y));
        }

        private static IEnumerable<IEnumerable<T>> CartesianProduct<T>(IEnumerable<IEnumerable<T>> sequences)
        {
            IEnumerable<IEnumerable<T>> emptyProduct = new[] { Enumerable.Empty<T>() };
            return sequences.Aggregate(emptyProduct,
                (accumulator, sequence) =>
                    from accseq in accumulator
                    from item in sequence
                    select accseq.Concat(new[] { item }));
        }

        private int AssesStatus(Island island)
        {
            int myValue = 0;
            int opponentsValue = 0;

            int value = 0;

            IEnumerable<Worker> allWorkers = island.GetAllWorkers();

            string myName = GetPlayerName();

            foreach (Worker worker in allWorkers)
            {
                int workersValue = AssesWorker(worker, island);
                if (worker.Player.Name == myName)
                {
                    myValue += workersValue;
                    value += workersValue;
                }
                else
                {
                    opponentsValue += workersValue;
                    value += (-1)*workersValue;
                }
            }
            return value;
        }

        private int AssesCommand(Island island, MoveCommand command)
        {
            int myValue = 0;
            int opponentsValue = 0;

            int value = 0;

            IEnumerable<Worker> allWorkers = island.GetAllWorkers();

            string myName = GetPlayerName();

            foreach (Worker worker in allWorkers)
            {
                int workersValue = AssesWorker(worker, island, command);
                if (worker.Player.Name == myName)
                {
                    myValue += workersValue;
                    value += workersValue;
                }
                else
                {
                    opponentsValue += workersValue;
                    value += (-1) * workersValue;
                }
            }
            return value;
        }


        private int AssesWorker(Worker worker, Island island)
        {
            int value = 0;
            if (worker.LandLevel == 3)
                return 10000;
            IEnumerable<Coord> neighbouringFields = GetPossibleNeighbours(worker.CurrentLand.Coord);

            value += 1000 * worker.CurrentLand.LandLevel;

            foreach (Coord neighbouringField in neighbouringFields)
            {
                island.TryGetLand(neighbouringField.X, neighbouringField.Y, out var land);
                value += 100 * (land.MaxLevelReached? -1 : land.LandLevel);
            }

            foreach (Coord neighbouringField in neighbouringFields)
            {
                if (worker.CanMoveTo(neighbouringField.X, neighbouringField.Y, out var land))
                {
                    if (worker.IsSteppingUp(worker.CurrentLand, land))
                    {
                        value += 10;
                    }
                    else
                    {
                        value += 1;
                    }
                }               
            }
            return value;
        }

        private int AssesWorker(Worker worker, Island island, MoveCommand command)
        {
            int value = 0;
            island.TryGetLand(command.MoveTo.X, command.MoveTo.Y, out Land newPosition);
            if (newPosition.LandLevel == 3)
                return 10000;


            IEnumerable<Coord> neighbouringFields = GetPossibleNeighbours(newPosition.Coord);

            value += 1000 * newPosition.LandLevel;

            foreach (Coord neighbouringField in neighbouringFields)
            {
                island.TryGetLand(neighbouringField.X, neighbouringField.Y, out var neighbour);
                int newLevelOfNeighbour = neighbour.LandLevel;
                if (command.BuildAt.Equals(neighbouringField))
                    newLevelOfNeighbour += 1;
                value += 100 * (newLevelOfNeighbour== 4 ? -1 : newLevelOfNeighbour);

                if (newLevelOfNeighbour == newPosition.LandLevel + 1)
                    value += 10;
                else if (newLevelOfNeighbour == newPosition.LandLevel)
                    value += 5;
                else if (newLevelOfNeighbour < newPosition.LandLevel)
                    value += 1;
            }
           
            return value;
        }
    }
}
