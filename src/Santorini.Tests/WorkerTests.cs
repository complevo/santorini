using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Bogus;
using FluentAssertions;
using Xunit;

namespace Santorini.Tests
{
    [ExcludeFromCodeCoverage]
    public class WorkerTests
    {
        private readonly Faker _faker;

        public WorkerTests()
        {
            _faker = new Faker();
        }

        [Fact]
        public void Worker_must_have_number_greater_than_zero()
        {
            // arrange
            var player = new Player(_faker.Name.FirstName());

            // act
            Action act = () => new Worker(player, 0);

            // assert
            act.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void Worker_must_have_player_as_owner()
        {
            // arrange
            var player = default(Player);

            // act
            Action act = () => new Worker(player, 0);

            // assert
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Workers_must_have_numbers_1_and_2()
        {
            var player = new Player(_faker.Name.FirstName());

            var worker1 = player.Workers.First();
            var worker2 = player.Workers.Last();

            player.Workers.Should().HaveCount(2);
            worker1.Number.Should().Be(1);
            worker2.Number.Should().Be(2);
        }

        [Fact]
        public void Workers_must_always_belongs_to_a_player()
        {
            var player = new Player(_faker.Name.FirstName());

            var worker1 = player.Workers.First();
            var worker2 = player.Workers.Last();

            worker1.Player.Should().NotBeNull();
            worker1.Player.Name.Should().Be(player.Name);
            worker2.Player.Should().NotBeNull();
            worker2.Player.Name.Should().Be(player.Name);
        }

        [Fact]
        public void Workers_must_be_different()
        {
            var player = new Player(_faker.Name.FirstName());

            var worker1 = player.Workers.First();
            var worker2 = player.Workers.Last();

            worker1.Equals(worker2).Should().BeFalse();
        }

        [Fact]
        public void Workers_can_be_compared()
        {
            var player1 = new Player(_faker.Name.FirstName());
            var player2 = new Player(_faker.Name.FirstName());

            var workerA = new Worker(player1, 1);
            var workerB = new Worker(player1, 1);
            var workerC = new Worker(player1, 2);
            var workerD = new Worker(player2, 2);
            var workerE = default(Worker);

            workerA.Equals(workerB).Should().BeTrue();
            workerB.Equals(workerC).Should().BeFalse();
            workerC.Equals(workerD).Should().BeFalse();
            workerD.Equals(workerE).Should().BeFalse();
        }

        [Fact]
        public void Worker_can_move_to_next_empty_land()
        {
            // arrange
            var board = new Island();
            var player = new Player(_faker.Name.FirstName());
            var worker = player.Workers.First();
            board.TryAddPiece(worker, 0, 0);

            // act
            var success = worker.TryMoveTo(0, 1);

            // arrange
            success.Should().BeTrue();
            board.IsUnoccupied(0, 0).Should().BeTrue();
            board.IsUnoccupied(0, 1).Should().BeFalse();
        }

        [Fact]
        public void Worker_can_cannot_move_not_placed_worker()
        {
            // arrange
            var board = new Island();
            var player = new Player(_faker.Name.FirstName());
            var worker1 = player.Workers.First();
            var worker2 = player.Workers.Last();

            board.TryAddPiece(worker1, 0, 0);

            // act
            var success1 = worker1.TryMoveTo(0, 1);
            var success2 = worker2.TryMoveTo(1, 0);

            // assert
            success1.Should().BeTrue();
            success2.Should().BeFalse();
        }

        [Fact]
        public void Refuse_move_worker_to_invalid_coordinate()
        {
            // arrange
            var board = new Island();
            var player = new Player(_faker.Name.FirstName());
            var worker = player.Workers.First();

            board.TryAddPiece(worker, 0, 0);

            // act
            var success = worker.TryMoveTo(-1, 5);

            // assert
            success.Should().BeFalse();
        }


        [Fact]
        public void Refuse_move_worker_2_lands_away()
        {
            // arrange
            var board = new Island();
            var player = new Player(_faker.Name.FirstName());
            var worker = player.Workers.First();

            board.TryAddPiece(worker, 0, 0);

            // act
            var success1 = worker.TryMoveTo(0, 2);
            var success2 = worker.TryMoveTo(2, 0);

            // assert
            success1.Should().BeFalse();
            success2.Should().BeFalse();
        }

        [Fact]
        public void Refuse_move_worker_to_occupied_land()
        {
            // arrange
            var board = new Island();
            var player = new Player(_faker.Name.FirstName());
            var worker = player.Workers.First();

            board.TryAddPiece(worker, 0, 0);

            // act
            var success1 = worker.TryMoveTo(0, 2);
            var success2 = worker.TryMoveTo(2, 0);

            // assert
            success1.Should().BeFalse();
            success2.Should().BeFalse();
        }

        [Fact]
        public void Refuse_move_worker_to_a_land_2_steps_higher()
        {
            // arrange
            var board = new Island();
            var player = new Player(_faker.Name.FirstName());
            var worker1 = player.Workers.First();
            var worker2 = player.Workers.Last();

            board.TryAddPiece(worker1, 0, 0);
            board.TryAddPiece(worker2, 4, 4);

            var building1 = new Tower();

            var building2 = new Tower();
            building2.RaiseLevel();

            board.TryAddPiece(building1, 0, 1);
            board.TryAddPiece(building2, 4, 3);

            // act
            var success1 = worker1.TryMoveTo(0, 1);
            var success2 = worker1.TryMoveTo(4, 3);

            // assert
            success1.Should().BeTrue();
            success2.Should().BeFalse();
        }

        [Fact]
        public void Worker_can_build_to_next_empty_land()
        {
            // arrange
            var board = new Island();
            var player = new Player(_faker.Name.FirstName());
            var worker = player.Workers.First();
            board.TryAddPiece(worker, 0, 0);

            // act
            var success = worker.TryBuildAt(0, 1);

            // arrange
            success.Should().BeTrue();
            board.IsUnoccupied(0, 0).Should().BeFalse();
            board.Board[0, 0].HasWorker.Should().BeTrue();
            board.Board[0, 1].HasWorker.Should().BeFalse();
            board.Board[0, 0].HasTower.Should().BeFalse();
            board.Board[0, 1].HasTower.Should().BeTrue();
        }

        [Fact]
        public void Worker_cannot_build_2_lands_away()
        {
            // arrange
            var board = new Island();
            var player = new Player(_faker.Name.FirstName());
            var worker1 = player.Workers.First();
            var worker2 = player.Workers.First();

            board.TryAddPiece(worker1, 0, 0);
            board.TryAddPiece(worker2, 4, 4);

            // act
            var success1 = worker1.TryBuildAt(0, 2);
            var success2 = worker2.TryBuildAt(2, 4);

            // assert
            success1.Should().BeFalse();
            success2.Should().BeFalse();
        }

        [Fact]
        public void Worker_cannot_build_occupied_land()
        {
            // arrange
            var board = new Island();
            var player = new Player(_faker.Name.FirstName());
            var worker1 = player.Workers.First();
            var worker2 = player.Workers.Last();

            board.TryAddPiece(worker1, 0, 0);
            board.TryAddPiece(worker2, 0, 1);

            // act
            var success = worker1.TryBuildAt(0, 1);

            // assert
            success.Should().BeFalse();
        }

        [Fact]
        public void Worker_cannot_build_own_land()
        {
            // arrange
            var board = new Island();
            var player = new Player(_faker.Name.FirstName());
            var worker = player.Workers.First();

            board.TryAddPiece(worker, 0, 0);

            // act
            var success = worker.TryBuildAt(0, 0);

            // assert
            success.Should().BeFalse();
        }

        [Fact]
        public void Worker_cannot_build_land_maxwith_maxlevel_reached()
        {
            // arrange
            var board = new Island();
            var player = new Player(_faker.Name.FirstName());
            var worker = player.Workers.First();
            var building = new Tower();
            while (!building.IsComplete) building.RaiseLevel();

            board.TryAddPiece(worker, 0, 0);
            board.TryAddPiece(building, 0, 1);

            // act
            var success = worker.TryBuildAt(0, 1);

            // assert
            success.Should().BeFalse();
        }

        [Fact]
        public void Worker_can_raise_building_level_by_1()
        {
            // arrange
            var board = new Island();
            var player = new Player(_faker.Name.FirstName());
            var worker = player.Workers.First();
            var building = new Tower();
            building.RaiseLevel();

            board.TryAddPiece(worker, 0, 0);
            board.TryAddPiece(building, 0, 1);

            // act
            var buildingLevelBeforeBuild = building.Level;
            var success = worker.TryBuildAt(0, 1);

            // assert
            success.Should().BeTrue();
            buildingLevelBeforeBuild.Should().Be(2);
            building.Level.Should().Be(3);
        }


        [Fact]
        public void Worker_cannot_raise_building_at_maxlevel()
        {
            // arrange
            var board = new Island();
            var player = new Player(_faker.Name.FirstName());
            var worker = player.Workers.First();
            var building = new Tower();
            while (!building.IsComplete) building.RaiseLevel();

            board.TryAddPiece(worker, 0, 0);
            board.TryAddPiece(building, 0, 1);

            // act
            var buildingLevelBeforeBuild = building.Level;
            var success = worker.TryBuildAt(0, 1);

            // assert
            success.Should().BeFalse();
            buildingLevelBeforeBuild.Should().Be(4);
            building.Level.Should().Be(4);
        }

        [Fact]
        public void Worker_cannot_build_at_invalid_coordinate()
        {
            // arrange
            var board = new Island();
            var player = new Player(_faker.Name.FirstName());
            var worker = player.Workers.First();

            board.TryAddPiece(worker, 0, 0);

            // act
            var success = worker.TryBuildAt(5, -1);

            // assert
            success.Should().BeFalse();
        }
    }
}
