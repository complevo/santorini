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
            var board = new Board();

            board.Lands.Should().NotBeNull();
            board.Lands.Should().HaveCount(25);
            foreach (var land in board.Lands)
            {
                land.Should().NotBeNull();
                land.IsEmpty.Should().BeTrue();
                board.IsEmpty(land.X, land.Y).Should().BeTrue();
            }

            for (var x = 0; x < 5; x++)
                for (var y = 0; y < 5; y++)
                    board.Lands[x, y].Equals(new Land(board, x, y));
        }

        [Fact]
        public void Board_accept_upto_4_builders()
        {
            // arrange
            var board = new Board();
            var player1 = new Player(_faker.Name.FirstName());
            var player2 = new Player(_faker.Name.FirstName());

            // act
            var success = true;
            success = success && board.TryAddPiece(player1.Builders.First(), 0, 0);
            success = success && board.TryAddPiece(player1.Builders.Last(), 0, 1);
            success = success && board.TryAddPiece(player2.Builders.First(), 0, 2);
            success = success && board.TryAddPiece(player2.Builders.Last(), 0, 3);

            // assert
            success.Should().BeTrue();
            board.IsEmpty(0, 0).Should().BeFalse();
            board.IsEmpty(0, 1).Should().BeFalse();
            board.IsEmpty(0, 2).Should().BeFalse();
            board.IsEmpty(0, 3).Should().BeFalse();
        }

        [Fact]
        public void Board_can_retrieve_builder_by_playernamer_and_buildernumber()
        {
            // arrange
            var board = new Board();
            var playerName = _faker.Name.FirstName();
            var player = new Player(playerName);
            var builder1 = player.Builders.First();
            var builder2 = player.Builders.Last();

            var posX = _faker.Random.Number(0, 4);
            var posY = _faker.Random.Number(0, 4);
            board.TryAddPiece(builder1, posX, posY);

            // act
            var builderFound = board.GetBuilder(playerName, 1);
            var builderNotFound = board.GetBuilder(playerName, 2);

            // assert
            builderFound.Should().NotBeNull();
            builderFound.Number.Should().Be(1);
            builderFound.Player.Equals(player).Should().BeTrue();
            builderNotFound.Should().BeNull();
        }


        [Fact]
        public void Board_can_refuse_put_piece_invalid_coordinate()
        {
            // arrange
            var board = new Board();
            var building = new Building();

            // act
            var success = board.TryAddPiece(building, 0, 5);

            // assert
            success.Should().BeFalse();
        }

        [Fact]
        public void Board_can_find_land_from_valid_coordinate()
        {
            // arrange
            var board = new Board();
            var posX = _faker.Random.Number(0, 4);
            var posY = _faker.Random.Number(0, 4);

            // act
            var success = board.TryGetLand(posX, posY, out var land);

            // assert
            success.Should().BeTrue();
            land.Should().NotBeNull();
            land.X.Should().Be(posX);
            land.Y.Should().Be(posY);
        }

        [Fact]
        public void Board_cannot_find_land_from_invalid_coordinate()
        {
            // arrange
            var board = new Board();
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
