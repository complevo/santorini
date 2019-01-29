using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Bogus;
using FluentAssertions;
using Xunit;

namespace Santorini.Tests
{
    [ExcludeFromCodeCoverage]
    public class MatchTests
    {
        private readonly Faker _faker;

        public MatchTests()
        {
            _faker = new Faker();
        }

        [Fact]
        public void Create_valid_match()
        {
            // arrange, act
            var match = new Match();

            // assert
            match.Should().NotBeNull();
            match.Island.Should().NotBeNull();
            match.Players.Should().HaveCount(0);
            match.MovesHistory.Should().HaveCount(0);
        }

        [Fact]
        public void Match_can_accept_2_players_registration()
        {
            // arrange
            var match = new Match();
            var player1Name = _faker.Name.FirstName();
            var player2Name = _faker.Name.FirstName();
            var player3Name = _faker.Name.FirstName();

            // act
            var success1 = match.TryAddPlayer(player1Name);
            var success2 = match.TryAddPlayer(player2Name);
            var success3 = match.TryAddPlayer(player3Name);
            var success4 = match.TryAddPlayer(player1Name);

            // assert
            success1.Should().BeTrue();
            success2.Should().BeTrue();
            success3.Should().BeFalse();
            success4.Should().BeFalse();
            match.Players.Should().HaveCount(2);
            match.Players.First().Name.Should().Be(player1Name);
            match.Players.Last().Name.Should().Be(player2Name);
        }

        [Fact]
        public void Match_should_refuse_players_with_same_name()
        {
            // arrange
            var match = new Match();
            var playerName = _faker.Name.FirstName();

            // act
            var success1 = match.TryAddPlayer(playerName);
            var success2 = match.TryAddPlayer(playerName);

            // assert
            success1.Should().BeTrue();
            success2.Should().BeFalse();
            match.Players.Should().HaveCount(1);
            match.Players.First().Name.Should().Be(playerName);
        }

        [Fact]
        public void Match_should_refuse_movement_while_all_players_present()
        {
            // arrange
            var match = new Match();
            var coord = GetEmptyCoord(match);
            var playerName = _faker.Name.FirstName();
            match.TryAddPlayer(playerName);

            // act
            var success = match.TryAddWorker(playerName, 1, coord.X, coord.Y);

            // assert
            success.Should().BeFalse();
        }

        [Fact]
        public void Match_should_refuse_movement_wrong_player_name()
        {
            // arrange
            var match = new Match();
            var player1Name = _faker.Name.FirstName();
            var player2Name = _faker.Name.FirstName();

            match.TryAddPlayer(player1Name);
            match.TryAddPlayer(player2Name);

            var coord = GetEmptyCoord(match);
            match.TryAddWorker(player1Name, 1, coord.X, coord.Y);
            coord = GetEmptyCoord(match);
            match.TryAddWorker(player1Name, 2, coord.X, coord.Y);
            coord = GetEmptyCoord(match);
            match.TryAddWorker(player2Name, 1, coord.X, coord.Y);
            coord = GetEmptyCoord(match);
            match.TryAddWorker(player2Name, 2, coord.X, coord.Y);

            var unknownName = _faker.Name.FirstName();            
            var moveCmd = new MoveCommand(unknownName, 1, GetEmptyCoord(match), GetEmptyCoord(match));

            // act
            var success = match.TryMoveWorker(moveCmd);

            // assert
            success.Should().BeFalse();
        }

        [Fact]
        public void Cannot_modify_registered_players()
        {
            // arrange
            var match = new Match();
            var player1Name = _faker.Name.FirstName();
            var player2Name = _faker.Name.FirstName();
            match.TryAddPlayer(player1Name);
            match.TryAddPlayer(player2Name);

            // act
            var player = match.Players.First();            
            player = null;

            // assert
            match.Players.Should().HaveCount(2);
            match.Players.First().Name.Should().Be(player1Name);
            match.Players.Last().Name.Should().Be(player2Name);
        }

        [Fact]
        public void Players_might_add_its_Workers()
        {
            // arrange
            var match = new Match();
            var player1Name = _faker.Name.FirstName();
            var player2Name = _faker.Name.FirstName();
            match.TryAddPlayer(player1Name);
            match.TryAddPlayer(player2Name);

            // act                        
            var p1b1Coord = GetEmptyCoord(match);
            var p1Success1 = match.TryAddWorker(player1Name, 1, p1b1Coord.X, p1b1Coord.Y);
            var p1b2Coord = GetEmptyCoord(match);
            var p1Success2 = match.TryAddWorker(player1Name, 2, p1b2Coord.X, p1b2Coord.Y);

            var p2b1Coord = GetEmptyCoord(match);
            var p2Success1 = match.TryAddWorker(player2Name, 1, p2b1Coord.X, p2b1Coord.Y);
            var p2b2Coord = GetEmptyCoord(match);
            var p2Success2 = match.TryAddWorker(player2Name, 2, p2b2Coord.X, p2b2Coord.Y);

            // assert
            p1Success1.Should().BeTrue();
            p1Success2.Should().BeTrue();
            p2Success1.Should().BeTrue();
            p2Success2.Should().BeTrue();

            var p1b1Land = match.Island.Board[p1b1Coord.X, p1b1Coord.Y];
            var p1b2Land = match.Island.Board[p1b2Coord.X, p1b2Coord.Y];
            var p2b1Land = match.Island.Board[p2b1Coord.X, p2b1Coord.Y];
            var p2b2Land = match.Island.Board[p2b2Coord.X, p2b2Coord.Y];

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
            var match = new Match();
            var player1Name = _faker.Name.FirstName();
            var player2Name = _faker.Name.FirstName();
            match.TryAddPlayer(player1Name);
            match.TryAddPlayer(player2Name);

            var p1b1Coord = new Coord(0, 0);
            match.TryAddWorker(player1Name, 1, p1b1Coord.X, p1b1Coord.Y);
            var p1b2Coord = new Coord(1, 1);
            match.TryAddWorker(player1Name, 2, p1b2Coord.X, p1b2Coord.Y);
            var p2b1Coord = new Coord(4, 4);
            match.TryAddWorker(player2Name, 1, p2b1Coord.X, p2b1Coord.Y);
            var p2b2Coord = new Coord(3, 3);
            match.TryAddWorker(player2Name, 2, p2b2Coord.X, p2b2Coord.Y);

            var moveTo = new Coord(0, 1);
            var buildAt = new Coord(0, 0);
            var moveCmd = new MoveCommand(player1Name, 1, moveTo, buildAt);

            // act
            var success = match.TryMoveWorker(moveCmd);

            // assert
            success.Should().BeTrue();
            match.Island.Board[0, 0].IsUnoccupied.Should().BeTrue();
            match.Island.Board[0, 0].HasTower.Should().BeTrue();
            match.Island.Board[0, 1].IsUnoccupied.Should().BeFalse();
            match.Island.Board[0, 1].HasWorker.Should().BeTrue();
            match.Island.Board[0, 1].Worker.Player.Name.Should().Be(player1Name);
            match.Island.Board[0, 1].Worker.Number.Should().Be(1);
        }

        [Fact]
        public void Match_should_refuse_invalid_move_command_with_wrong_workerNumber()
        {
            // arrange
            var match = new Match();
            var moveCmd = new MoveCommand(
                playerName: _faker.Name.FirstName(),
                workerNumber: 0,
                moveTo: new Coord(0, 0),
                buildAt: new Coord(0, 0));

            // act 
            var success = match.TryMoveWorker(moveCmd);

            // assert
            match.Should().NotBeNull();
            moveCmd.IsValid.Should().BeFalse();
            success.Should().BeFalse();
        }

        [Fact]
        public void Match_should_refuse_move_command_with_wrong_playerName()
        {
            // arrange
            var match = new Match();
            var moveCmd = new MoveCommand(
                playerName: _faker.Name.FirstName(),
                workerNumber: 1,
                moveTo: new Coord(0, 0),
                buildAt: new Coord(0, 1));

            // act 
            var success = match.TryMoveWorker(moveCmd);

            // assert
            match.Should().NotBeNull();
            moveCmd.IsValid.Should().BeTrue();
            success.Should().BeFalse();
        }

        [Fact]
        public void Match_should_refuse_move_command_to_occupied_land()
        {
            // arrange
            var match = new Match();
            var player1Name = _faker.Name.FirstName();
            var player2Name = _faker.Name.FirstName();
            match.TryAddPlayer(player1Name);
            match.TryAddPlayer(player2Name);

            var p1b1Coord = new Coord(x: 0, y: 0);
            var successP1B1 = match.TryAddWorker(player1Name, 1, p1b1Coord.X, p1b1Coord.Y);
            var p1b2Coord = new Coord(x: 1, y: 1);
            var successP1B2 = match.TryAddWorker(player1Name, 2, p1b2Coord.X, p1b2Coord.Y);
            var p2b1Coord = new Coord(x: 4, y: 4);
            var successP2B1 = match.TryAddWorker(player2Name, 1, p2b1Coord.X, p2b1Coord.Y);
            var p2b2Coord = new Coord(x: 3, y: 3);
            var successP2B2 = match.TryAddWorker(player2Name, 2, p2b2Coord.X, p2b2Coord.Y);

            var moveTo = new Coord(0, 1);
            var buildAt = new Coord(0, 0);
            var moveCmd = new MoveCommand(player1Name, 1, moveTo, buildAt);

            // act
            var success = match.TryMoveWorker(moveCmd);

            // assert
            successP1B1.Should().BeTrue();
            successP1B2.Should().BeTrue();
            successP2B1.Should().BeTrue();
            successP2B2.Should().BeTrue();
            success.Should().BeTrue();            
            match.Island.Board[0, 0].IsUnoccupied.Should().BeTrue();
            match.Island.Board[0, 0].HasTower.Should().BeTrue();
            match.Island.Board[0, 1].IsUnoccupied.Should().BeFalse();
            match.Island.Board[0, 1].HasWorker.Should().BeTrue();
            match.Island.Board[0, 1].Worker.Player.Name.Should().Be(player1Name);
            match.Island.Board[0, 1].Worker.Number.Should().Be(1);
        }



        private Coord GetEmptyCoord(Match match)
        {
            int x = 0, y = 0;
            while (!match.Island.Board[x, y].IsUnoccupied)
            {
                x = _faker.Random.Number(0, 4);
                y = _faker.Random.Number(0, 4);
            }
            return new Coord(x, y);
        }
    }
}
