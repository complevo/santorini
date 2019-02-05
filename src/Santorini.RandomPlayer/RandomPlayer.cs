using System;
using System.Collections.Generic;
using System.Linq;

namespace Santorini.RandomPlayer
{
    public class RandomPlayer
    {
        private const string PLAYER_NAME = "Randomizer";
        private static readonly Random _random = new Random();

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
            int playerId = _random.Next(2);
            return TryGenerateCommand(playerId, island) ?? TryGenerateCommand(1 - playerId, island) ?? throw new Exception("Loose");
        }

        private MoveCommand TryGenerateCommand(int workerIndex, Island island)
        {
            Worker worker = island.GetWorker(PLAYER_NAME, workerIndex);
            List<Coord> unoccupiedMoveDirections = GetPossibleNeighbours(worker.CurrentLand.Coord)
                .Where(c =>
                    island.IsUnoccupied(c.X, c.Y) &&
                    island.Board[c.X, c.Y].LandLevel - worker.LandLevel > 2 &&
                    island.Board[c.X, c.Y].LandLevel != 4)
                .ToList();
            Shuffle(unoccupiedMoveDirections);
            foreach (Coord moveCoord in unoccupiedMoveDirections)
            {
                List<Coord> unoccupiedBuildDirections = GetPossibleNeighbours(moveCoord)
                    .Where(c =>
                        (island.IsUnoccupied(c.X, c.Y) || c == moveCoord) &&
                        island.Board[c.X, c.Y].LandLevel != 4)
                    .ToList();
                if (!unoccupiedBuildDirections.Any())
                    continue;

                Coord buildCoord = unoccupiedBuildDirections[_random.Next(unoccupiedBuildDirections.Count)];

                return new MoveCommand(PLAYER_NAME, workerIndex, moveCoord, buildCoord);
            }
            return null;
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

        public static void Shuffle<T>(IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = _random.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
