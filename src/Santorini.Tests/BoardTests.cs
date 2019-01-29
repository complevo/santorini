using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Bogus;
using FluentAssertions;
using Xunit;

namespace Santorini.Tests
{
    [ExcludeFromCodeCoverage]
    public class BoardTests
    {
        public readonly Faker _faker;

        public BoardTests()
        {
            _faker = new Faker();
        }

        [Fact]
        public void Board_must_start_with_25_empty_lands()
        {
            var board = new Island();

            board.Board.Should().NotBeNull();
            board.Board.Should().HaveCount(25);
            foreach (var land in board.Board)
            {
                land.Should().NotBeNull();
                land.IsUnoccupied.Should().BeTrue();
                board.IsUnoccupied(land.Coord.X, land.Coord.Y).Should().BeTrue();
            }

            for (var x = 0; x < 5; x++)
                for (var y = 0; y < 5; y++)
                    board.Board[x, y].Equals(new Land(board, x, y));
        }

        [Fact]
        public void Board_accept_upto_4_workers()
        {
            // arrange
            var board = new Island();
            var player1 = new Player(_faker.Name.FirstName());
            var player2 = new Player(_faker.Name.FirstName());

            // act
            var success = true;
            success = success && board.TryAddPiece(player1.Workers.First(), 0, 0);
            success = success && board.TryAddPiece(player1.Workers.Last(), 0, 1);
            success = success && board.TryAddPiece(player2.Workers.First(), 0, 2);
            success = success && board.TryAddPiece(player2.Workers.Last(), 0, 3);

            // assert
            success.Should().BeTrue();
            board.IsUnoccupied(0, 0).Should().BeFalse();
            board.IsUnoccupied(0, 1).Should().BeFalse();
            board.IsUnoccupied(0, 2).Should().BeFalse();
            board.IsUnoccupied(0, 3).Should().BeFalse();
        }

        [Fact]
        public void Board_can_retrieve_worker_by_playernamer_and_workernumber()
        {
            // arrange
            var board = new Island();
            var playerName = _faker.Name.FirstName();
            var player = new Player(playerName);
            var worker1 = player.Workers.First();            

            var posX = _faker.Random.Number(0, 4);
            var posY = _faker.Random.Number(0, 4);
            board.TryAddPiece(worker1, posX, posY);

            // act
            var workerFound = board.GetWorker(playerName, 1);
            var workerNotFound = board.GetWorker(playerName, 2);

            // assert
            workerFound.Should().NotBeNull();
            workerFound.Number.Should().Be(1);
            workerFound.Player.Equals(player).Should().BeTrue();
            workerNotFound.Should().BeNull();
        }


        [Fact]
        public void Board_can_refuse_put_piece_invalid_coordinate()
        {
            // arrange
            var board = new Island();
            var building = new Tower();

            // act
            var success = board.TryAddPiece(building, 0, 5);

            // assert
            success.Should().BeFalse();
        }

        [Fact]
        public void Board_can_find_land_from_valid_coordinate()
        {
            // arrange
            var board = new Island();
            var posX = _faker.Random.Number(0, 4);
            var posY = _faker.Random.Number(0, 4);

            // act
            var success = board.TryGetLand(posX, posY, out var land);

            // assert
            success.Should().BeTrue();
            land.Should().NotBeNull();
            land.Coord.X.Should().Be(posX);
            land.Coord.Y.Should().Be(posY);
        }

        [Fact]
        public void Board_cannot_find_land_from_invalid_coordinate()
        {
            // arrange
            var board = new Island();
            var posX = _faker.Random.Number(int.MinValue, -1);
            var posY = _faker.Random.Number(5, int.MaxValue);

            // act
            var success = board.TryGetLand(posX, posY, out var land);

            // assert
            success.Should().BeFalse();
            land.Should().BeNull();
        }
    }
}
