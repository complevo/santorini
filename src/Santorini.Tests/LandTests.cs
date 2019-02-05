using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Bogus;
using FluentAssertions;
using Xunit;

namespace Santorini.Tests
{
    [ExcludeFromCodeCoverage]
    public class LandTests
    {
        private readonly Faker _faker;
        private readonly Island _board;

        public LandTests()
        {
            _faker = new Faker();
            _board = new Island();
        }

        [Fact]
        public void Land_created_with_valid_positions()
        {
            // arrange
            var posX = NewPos();
            var posY = NewPos();

            // act
            var land = new Land(_board, posX, posY);

            // assert
            land.Should().NotBeNull();
            land.Coord.X.Should().Be(posX);
            land.Coord.Y.Should().Be(posY);
            land.Pieces.Should().HaveCount(0);
            land.HasWorker.Should().BeFalse();
            land.HasTower.Should().BeFalse();
            land.Island.Should().NotBeNull();
            land.Island.Should().Be(_board);
            land.MaxLevelReached.Should().BeFalse();
        }

        [Fact]
        public void Lands_with_same_positions_are_equals()
        {
            // arrange
            var posX = NewPos();
            var posY = NewPos();

            var land1 = new Land(_board, posX, posY);
            var land2 = new Land(_board, posX, posY);

            // act
            var equals = land1.Equals(land2);

            //assert
            equals.Should().BeTrue();
        }

        [Fact]
        public void Land_must_accept_building_when_empty()
        {
            // arrange
            var posX = NewPos();
            var posY = NewPos();

            var land = new Land(_board, posX, posY);

            var building = new Tower();

            // act
            var hasBuildingBeforeAct = land.HasTower;
            var success = land.TryPutPiece(building);

            // assert
            success.Should().BeTrue();
            hasBuildingBeforeAct.Should().BeFalse();
            land.HasWorker.Should().BeFalse();
            land.HasTower.Should().BeTrue();
            land.Tower.Equals(building).Should().BeTrue();
            land.MaxLevelReached.Should().BeFalse();
        }

        [Fact]
        public void Land_must_refuse_building_when_another_building_exists()
        {
            // arrange
            var posX = NewPos();
            var posY = NewPos();

            var land = new Land(_board, posX, posY);

            var previousBuilding = new Tower();
            land.TryPutPiece(previousBuilding);

            var building = new Tower();

            // act
            var hasBuildingBeforeAct = land.HasTower;
            var success = land.TryPutPiece(building);

            // assert
            success.Should().BeFalse();
            hasBuildingBeforeAct.Should().BeTrue();
            land.HasWorker.Should().BeFalse();
            land.HasTower.Should().BeTrue();
            land.Pieces.Should().HaveCount(1);
            land.Tower.Equals(previousBuilding).Should().BeTrue();
        }

        [Fact]
        public void Land_must_accept_worker_when_empty()
        {
            // arrange
            var posX = NewPos();
            var posY = NewPos();

            var land = new Land(_board, posX, posY);

            var player = new Player(_faker.Name.FirstName());
            var worker = player.Workers.First();

            // act
            var success = land.TryPutPiece(worker);

            // assert
            success.Should().BeTrue();
            land.HasWorker.Should().BeTrue();
            land.Worker.Equals(worker).Should().BeTrue();
            land.IsUnoccupied.Should().BeFalse();
        }     

        [Fact]
        public void Land_must_accept_worker_when_existing_buildind_upto_1_level_higher()
        {
            // arrange
            var posX = NewPos();
            var posY = NewPos();

            var originLand = new Land(_board, posX, posY);
            var player = new Player(_faker.Name.FirstName());
            var worker = player.Workers.First();
            originLand.TryPutPiece(worker);

            posX = NewPos();
            posY = NewPos();

            var destinationland = new Land(_board, posX, posY);
            destinationland.TryPutPiece(new Tower());

            // act
            var hasBuildingBeforeMove = destinationland.HasTower;
            var landLevelBeforeMove = destinationland.LandLevel;
            var success = destinationland.TryPutPiece(worker);

            // assert
            success.Should().BeTrue();
            hasBuildingBeforeMove.Should().BeTrue();
            landLevelBeforeMove.Should().Be(1);
            destinationland.HasWorker.Should().BeTrue();
            destinationland.Worker.Equals(worker).Should().BeTrue();
        }

        [Fact]
        public void Land_must_refuse_worker_when_existing_buildind_more_than_1_level_higher()
        {
            // arrange
            var posX = NewPos();
            var posY = NewPos();

            var originLand = new Land(_board, posX, posY);
            var player = new Player(_faker.Name.FirstName());
            var worker = player.Workers.First();
            originLand.TryPutPiece(worker);

            posX = NewPos();
            posY = NewPos();

            var destinationland = new Land(_board, posX, posY);
            var tower = new Tower();
            tower.RaiseLevel();
            destinationland.TryPutPiece(tower);

            // act
            var hasTowerBeforeMove = destinationland.HasTower;
            var landLevelBeforeMove = destinationland.LandLevel;
            var success = destinationland.TryPutPiece(worker);

            // assert
            success.Should().BeFalse();
            hasTowerBeforeMove.Should().BeTrue();
            landLevelBeforeMove.Should().Be(2);
            destinationland.HasWorker.Should().BeFalse();
            destinationland.HasWorker.Should().BeFalse();
        }

        [Fact]
        public void Land_must_reject_worker_when_occupied_by_another_worker()
        {
            // arrange
            var posX = NewPos();
            var posY = NewPos();

            var land = new Land(_board, posX, posY);

            var player = new Player(_faker.Name.FirstName());
            var worker1 = player.Workers.First();
            var worker2 = player.Workers.Last();

            land.TryPutPiece(worker1);

            // act
            var success = land.TryPutPiece(worker2);

            // assert
            success.Should().BeFalse();
            land.HasWorker.Should().BeTrue();
            land.Worker.Equals(worker1).Should().BeTrue();
            land.Worker.Equals(worker2).Should().BeFalse();
        }

        [Fact]
        public void Land_should_allow_remove_existing_worker()
        {
            // arrange
            var posX = NewPos();
            var posY = NewPos();

            var land = new Land(_board, posX, posY);

            var player = new Player(_faker.Name.FirstName());
            var worker = player.Workers.First();

            land.TryPutPiece(worker);

            // act
            var hasWorkerBeforeRemove = land.HasWorker;
            var success = land.TryRemoveWorker(worker);

            // assert
            success.Should().BeTrue();
            hasWorkerBeforeRemove.Should().BeTrue();
            land.HasWorker.Should().BeFalse();
        }

        [Fact]
        public void Land_should_fail_to_remove_non_existing_worker()
        {
            // arrange
            var posX = NewPos();
            var posY = NewPos();

            var land = new Land(_board, posX, posY);

            var player = new Player(_faker.Name.FirstName());
            var worker = player.Workers.First();

            // act
            var hasWorkerBeforeRemove = land.HasWorker;
            var success = land.TryRemoveWorker(worker);

            // assert
            success.Should().BeFalse();
            hasWorkerBeforeRemove.Should().BeFalse();
            land.HasWorker.Should().BeFalse();
        }

        [Fact]
        public void Land_should_fail_to_remove_different_worker()
        {
            // arrange
            var posX = NewPos();
            var posY = NewPos();

            var player = new Player(_faker.Name.FirstName());
            var worker1 = player.Workers.First();
            var worker2 = player.Workers.Last();

            var land = new Land(_board, posX, posY);
            land.TryPutPiece(worker1);

            // act
            var hasWorkerBeforeRemove = land.HasWorker;
            var success = land.TryRemoveWorker(worker2);

            // assert
            success.Should().BeFalse();
            hasWorkerBeforeRemove.Should().BeTrue();
            land.HasWorker.Should().BeTrue();
            land.Worker.Equals(worker1).Should().BeTrue();
        }

        [Fact]
        public void Land_throw_exception_with_invalid_positions()
        {
            // arrange
            var posX = _faker.Random.Number(5, int.MaxValue);
            var posY = _faker.Random.Number(5, int.MaxValue);

            // act
            Action act = () => new Land(_board, posX, posY);

            // assert
            act.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void Land_throw_exception_with_null_board()
        {
            // arrange
            var board = default(Island);
            var posX = NewPos();
            var posY = NewPos();

            // act
            Action act = () => new Land(board, posX, posY);

            // assert
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Land_can_be_compared()
        {
            // arrange
            var posX = _faker.Random.Number(1, 4);
            var posY = _faker.Random.Number(1, 4);

            var landA = new Land(_board, posX, posY);
            var landB = new Land(_board, posX, posY);
            var landC = new Land(_board, posX, 0);
            var landD = new Land(_board, 0, posY);
            var landE = default(Land);

            // act
            var equalsAB_1 = landA.Equals(landB);
            var equalsAB_2 = landA == landB;
            var equalsAB_3 = landA.GetHashCode() == landB.GetHashCode();
            var equalsBC_1 = landB.Equals(landC);
            var equalsBC_2 = landB == landC;
            var equalsCD_1 = landC.Equals(landD);
            var equalsCD_2 = landC == landD;
            var equalsDE_1 = landD.Equals(landE);
            var equalsDE_2 = landD == landE;
            var equalsA0_1 = landA == landE;
            var equalsA0_2 = landE == landA;
            var equals00_1 = landE == null;
            var equals00_2 = landD.Equals(DateTime.UtcNow);
            var equals00_3 = landA.Equals((object)DateTime.UtcNow);
            var equals00_4 = landA.Equals((object)landB);
            var equals00_5 = landA.Equals((object)null);

            // assert
            equalsAB_1.Should().BeTrue();
            equalsAB_2.Should().BeTrue();
            equalsAB_3.Should().BeTrue();
            equalsBC_1.Should().BeFalse();
            equalsBC_2.Should().BeFalse();
            equalsCD_1.Should().BeFalse();
            equalsCD_2.Should().BeFalse();
            equalsDE_1.Should().BeFalse();
            equalsDE_2.Should().BeFalse();
            equalsA0_1.Should().BeFalse();
            equalsA0_2.Should().BeFalse();
            equals00_1.Should().BeTrue();
            equals00_2.Should().BeFalse();
            equals00_3.Should().BeFalse();
            equals00_4.Should().BeTrue();
            equals00_5.Should().BeFalse();
        }

        private int NewPos()
            => _faker.Random.Number(0, 4);
    }
}
