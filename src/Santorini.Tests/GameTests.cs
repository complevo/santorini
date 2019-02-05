using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Bogus;
using FluentAssertions;
using Xunit;

namespace Santorini.Tests
{
    [ExcludeFromCodeCoverage]
    public class GameTests
    {
        private readonly Faker _faker;

        public GameTests()
        {
            _faker = new Faker();
        }

        [Fact]
        public void Create_valid_game()
        {
            // arrange, act
            var game = new Game();

            // assert
            game.Should().NotBeNull();
            game.Island.Should().NotBeNull();
            game.Players.Should().HaveCount(0);
            game.MovesHistory.Should().HaveCount(0);
        }

        [Fact]
        public void game_can_accept_2_players_registration()
        {
            // arrange
            var game = new Game();
            var player1Name = _faker.Name.FirstName();
            var player2Name = _faker.Name.FirstName();
            var player3Name = _faker.Name.FirstName();

            // act
            var success1 = game.TryAddPlayer(player1Name);
            var success2 = game.TryAddPlayer(player2Name);
            var success3 = game.TryAddPlayer(player3Name);
            var success4 = game.TryAddPlayer(player1Name);

            // assert
            success1.Should().BeTrue();
            success2.Should().BeTrue();
            success3.Should().BeFalse();
            success4.Should().BeFalse();
            game.Players.Should().HaveCount(2);
            game.Players.First().Name.Should().Be(player1Name);
            game.Players.Last().Name.Should().Be(player2Name);
        }

        [Fact]
        public void game_should_refuse_players_with_same_name()
        {
            // arrange
            var game = new Game();
            var playerName = _faker.Name.FirstName();

            // act
            var success1 = game.TryAddPlayer(playerName);
            var success2 = game.TryAddPlayer(playerName);

            // assert
            success1.Should().BeTrue();
            success2.Should().BeFalse();
            game.Players.Should().HaveCount(1);
            game.Players.First().Name.Should().Be(playerName);
        }

        [Fact]
        public void game_should_refuse_movement_while_all_players_present()
        {
            // arrange
            var game = new Game();
            var coord = GetEmptyCoord(game);
            var playerName = _faker.Name.FirstName();
            game.TryAddPlayer(playerName);

            // act
            var success = game.TryAddWorker(playerName, 1, coord.X, coord.Y);

            // assert
            success.Should().BeFalse();
        }

        [Fact]
        public void game_should_refuse_movement_wrong_player_name()
        {
            // arrange
            var game = new Game();
            var player1Name = _faker.Name.FirstName();
            var player2Name = _faker.Name.FirstName();

            game.TryAddPlayer(player1Name);
            game.TryAddPlayer(player2Name);

            var coord = GetEmptyCoord(game);
            game.TryAddWorker(player1Name, 1, coord.X, coord.Y);
            coord = GetEmptyCoord(game);
            game.TryAddWorker(player1Name, 2, coord.X, coord.Y);
            coord = GetEmptyCoord(game);
            game.TryAddWorker(player2Name, 1, coord.X, coord.Y);
            coord = GetEmptyCoord(game);
            game.TryAddWorker(player2Name, 2, coord.X, coord.Y);

            var unknownName = _faker.Name.FirstName();            
            var moveCmd = new MoveCommand(unknownName, 1, GetEmptyCoord(game), GetEmptyCoord(game));

            // act
            var success = game.TryMoveWorker(moveCmd);

            // assert
            success.Should().BeFalse();
        }

        [Fact]
        public void Cannot_modify_registered_players()
        {
            // arrange
            var game = new Game();
            var player1Name = _faker.Name.FirstName();
            var player2Name = _faker.Name.FirstName();
            game.TryAddPlayer(player1Name);
            game.TryAddPlayer(player2Name);

            // act
            var player = game.Players.First();            
            player = null;

            // assert
            game.Players.Should().HaveCount(2);
            game.Players.First().Name.Should().Be(player1Name);
            game.Players.Last().Name.Should().Be(player2Name);
        }

        [Fact]
        public void Players_might_add_its_Workers()
        {
            // arrange
            var game = new Game();
            var player1Name = _faker.Name.FirstName();
            var player2Name = _faker.Name.FirstName();
            game.TryAddPlayer(player1Name);
            game.TryAddPlayer(player2Name);

            // act                        
            var p1b1Coord = GetEmptyCoord(game);
            var p1Success1 = game.TryAddWorker(player1Name, 1, p1b1Coord.X, p1b1Coord.Y);
            var p1b2Coord = GetEmptyCoord(game);
            var p1Success2 = game.TryAddWorker(player1Name, 2, p1b2Coord.X, p1b2Coord.Y);

            var p2b1Coord = GetEmptyCoord(game);
            var p2Success1 = game.TryAddWorker(player2Name, 1, p2b1Coord.X, p2b1Coord.Y);
            var p2b2Coord = GetEmptyCoord(game);
            var p2Success2 = game.TryAddWorker(player2Name, 2, p2b2Coord.X, p2b2Coord.Y);

            // assert
            p1Success1.Should().BeTrue();
            p1Success2.Should().BeTrue();
            p2Success1.Should().BeTrue();
            p2Success2.Should().BeTrue();

            var p1b1Land = game.Island.Board[p1b1Coord.X, p1b1Coord.Y];
            var p1b2Land = game.Island.Board[p1b2Coord.X, p1b2Coord.Y];
            var p2b1Land = game.Island.Board[p2b1Coord.X, p2b1Coord.Y];
            var p2b2Land = game.Island.Board[p2b2Coord.X, p2b2Coord.Y];

            var p1Worker1 = p1b1Land.Worker;
            var p1Worker2 = p1b2Land.Worker;
            var p2Worker1 = p2b1Land.Worker;
            var p2Worker2 = p2b2Land.Worker;

            p1b1Land.HasWorker.Should().BeTrue();
            p1b2Land.HasWorker.Should().BeTrue();
            p2b1Land.HasWorker.Should().BeTrue();
            p2b2Land.HasWorker.Should().BeTrue();

            p1Worker1.Number.Should().Be(1);
            p1Worker1.Player.Name.Should().Be(player1Name);
            p1Worker2.Number.Should().Be(2);
            p1Worker2.Player.Name.Should().Be(player1Name);
            p2Worker1.Number.Should().Be(1);
            p2Worker1.Player.Name.Should().Be(player2Name);
            p2Worker2.Number.Should().Be(2);
            p2Worker2.Player.Name.Should().Be(player2Name);
        }

        [Fact]
        public void Player_request_move_command()
        {
            // arrange
            var game = new Game();
            var player1Name = _faker.Name.FirstName();
            var player2Name = _faker.Name.FirstName();
            game.TryAddPlayer(player1Name);
            game.TryAddPlayer(player2Name);

            var p1b1Coord = new Coord(0, 0);
            game.TryAddWorker(player1Name, 1, p1b1Coord.X, p1b1Coord.Y);
            var p1b2Coord = new Coord(1, 1);
            game.TryAddWorker(player1Name, 2, p1b2Coord.X, p1b2Coord.Y);
            var p2b1Coord = new Coord(4, 4);
            game.TryAddWorker(player2Name, 1, p2b1Coord.X, p2b1Coord.Y);
            var p2b2Coord = new Coord(3, 3);
            game.TryAddWorker(player2Name, 2, p2b2Coord.X, p2b2Coord.Y);

            var moveTo = new Coord(0, 1);
            var buildAt = new Coord(0, 0);
            var moveCmd = new MoveCommand(player1Name, 1, moveTo, buildAt);

            // act
            var success = game.TryMoveWorker(moveCmd);

            // assert
            success.Should().BeTrue();
            game.Island.Board[0, 0].IsUnoccupied.Should().BeTrue();
            game.Island.Board[0, 0].HasTower.Should().BeTrue();
            game.Island.Board[0, 1].IsUnoccupied.Should().BeFalse();
            game.Island.Board[0, 1].HasWorker.Should().BeTrue();
            game.Island.Board[0, 1].Worker.Player.Name.Should().Be(player1Name);
            game.Island.Board[0, 1].Worker.Number.Should().Be(1);
        }

        [Fact]
        public void game_should_refuse_invalid_move_command_with_wrong_workerNumber()
        {
            // arrange
            var game = new Game();
            var moveCmd = new MoveCommand(
                playerName: _faker.Name.FirstName(),
                workerNumber: 0,
                moveTo: new Coord(0, 0),
                buildAt: new Coord(0, 0));

            // act 
            var success = game.TryMoveWorker(moveCmd);

            // assert
            game.Should().NotBeNull();
            moveCmd.IsValid.Should().BeFalse();
            success.Should().BeFalse();
        }

        [Fact]
        public void game_should_refuse_move_command_with_wrong_playerName()
        {
            // arrange
            var game = new Game();
            var moveCmd = new MoveCommand(
                playerName: _faker.Name.FirstName(),
                workerNumber: 1,
                moveTo: new Coord(0, 0),
                buildAt: new Coord(0, 1));

            // act 
            var success = game.TryMoveWorker(moveCmd);

            // assert
            game.Should().NotBeNull();
            moveCmd.IsValid.Should().BeTrue();
            success.Should().BeFalse();
        }

        [Fact]
        public void game_should_refuse_move_command_to_occupied_land()
        {
            // arrange
            var game = new Game();
            var player1Name = _faker.Name.FirstName();
            var player2Name = _faker.Name.FirstName();
            game.TryAddPlayer(player1Name);
            game.TryAddPlayer(player2Name);

            var p1b1Coord = new Coord(x: 0, y: 0);
            var successP1B1 = game.TryAddWorker(player1Name, 1, p1b1Coord.X, p1b1Coord.Y);
            var p1b2Coord = new Coord(x: 1, y: 1);
            var successP1B2 = game.TryAddWorker(player1Name, 2, p1b2Coord.X, p1b2Coord.Y);
            var p2b1Coord = new Coord(x: 4, y: 4);
            var successP2B1 = game.TryAddWorker(player2Name, 1, p2b1Coord.X, p2b1Coord.Y);
            var p2b2Coord = new Coord(x: 3, y: 3);
            var successP2B2 = game.TryAddWorker(player2Name, 2, p2b2Coord.X, p2b2Coord.Y);

            var moveTo = new Coord(0, 1);
            var buildAt = new Coord(0, 0);
            var moveCmd = new MoveCommand(player1Name, 1, moveTo, buildAt);

            // act
            var success = game.TryMoveWorker(moveCmd);

            // assert
            successP1B1.Should().BeTrue();
            successP1B2.Should().BeTrue();
            successP2B1.Should().BeTrue();
            successP2B2.Should().BeTrue();
            success.Should().BeTrue();            
            game.Island.Board[0, 0].IsUnoccupied.Should().BeTrue();
            game.Island.Board[0, 0].HasTower.Should().BeTrue();
            game.Island.Board[0, 1].IsUnoccupied.Should().BeFalse();
            game.Island.Board[0, 1].HasWorker.Should().BeTrue();
            game.Island.Board[0, 1].Worker.Player.Name.Should().Be(player1Name);
            game.Island.Board[0, 1].Worker.Number.Should().Be(1);
        }

        [Fact]
        public void game_is_over_when_worker_at_3rd_level()
        {
            // arrange
            var game = new Game();
            var player1Name = _faker.Name.FirstName();
            var player2Name = _faker.Name.FirstName();
            game.TryAddPlayer(player1Name);
            game.TryAddPlayer(player2Name);

            var coord = new Coord(x: 0, y: 0);
            game.TryAddWorker(player1Name, 1, coord.X, coord.Y);
            coord = new Coord(x: 1, y: 1);
            game.TryAddWorker(player1Name, 2, coord.X, coord.Y);
            coord = new Coord(x: 4, y: 4);
            game.TryAddWorker(player2Name, 1, coord.X, coord.Y);
            coord = new Coord(x: 3, y: 3);
            game.TryAddWorker(player2Name, 2, coord.X, coord.Y);

            for (var i = 0; i < 3; i++)
            {
                // build level
                var moveTo = new Coord(0, 1);
                var buildAt = new Coord(0, 0);
                var moveCmd = new MoveCommand(player1Name, 1, moveTo, buildAt);
                game.TryMoveWorker(moveCmd);

                if (i >= 2) break;

                // build level
                moveTo = new Coord(0, 0);
                buildAt = new Coord(0, 1);
                moveCmd = new MoveCommand(player1Name, 1, moveTo, buildAt);
                game.TryMoveWorker(moveCmd);
            }

            // act
            var winMoveCmd = new MoveCommand(player1Name, 1, new Coord(0, 0), new Coord(0, 1));
            var success = game.TryMoveWorker(winMoveCmd);

            // assert
            success.Should().BeTrue();
            game.GameIsOver.Should().BeTrue();
            game.Winner.Name.Should().Be(player1Name);
        }

        private Coord GetEmptyCoord(Game game)
        {
            int x = 0, y = 0;
            while (!game.Island.Board[x, y].IsUnoccupied)
            {
                x = _faker.Random.Number(0, 4);
                y = _faker.Random.Number(0, 4);
            }
            return new Coord(x, y);
        }
    }
}
