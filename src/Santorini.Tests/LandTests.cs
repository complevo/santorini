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
        private readonly Board _board;

        public LandTests()
        {
            _faker = new Faker();
            _board = new Board();
        }

        [Fact]
        public void Land_created_with_valid_positions()
        {
            // arrange
            var posX = _faker.Random.Number(0, 4);
            var posY = _faker.Random.Number(0, 4);

            // act
            var land = new Land(_board, posX, posY);

            // assert
            land.Should().NotBeNull();
            land.X.Should().Be(posX);
            land.Y.Should().Be(posY);
            land.Pieces.Should().HaveCount(0);
            land.HasBuilder.Should().BeFalse();
            land.HasBuilding.Should().BeFalse();
            land.Board.Should().NotBeNull();
            land.Board.Should().Be(_board);
            land.MaxLevelReached.Should().BeFalse();
        }

        [Fact]
        public void Lands_with_same_positions_are_equals()
        {
            // arrange
            var posX = _faker.Random.Number(0, 4);
            var posY = _faker.Random.Number(0, 4);

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
            var posX = _faker.Random.Number(0, 4);
            var posY = _faker.Random.Number(0, 4);

            var land = new Land(_board, posX, posY);

            var building = new Building();

            // act
            var hasBuildingBeforeAct = land.HasBuilding;
            var success = land.TryPutPiece(building);

            // assert
            success.Should().BeTrue();
            hasBuildingBeforeAct.Should().BeFalse();
            land.HasBuilder.Should().BeFalse();
            land.HasBuilding.Should().BeTrue();
            land.Building.Equals(building).Should().BeTrue();
            land.MaxLevelReached.Should().BeFalse();
        }

        [Fact]
        public void Land_must_refuse_building_when_another_building_exists()
        {
            // arrange
            var posX = _faker.Random.Number(0, 4);
            var posY = _faker.Random.Number(0, 4);

            var land = new Land(_board, posX, posY);

            var previousBuilding = new Building();
            land.TryPutPiece(previousBuilding);

            var building = new Building();

            // act
            var hasBuildingBeforeAct = land.HasBuilding;
            var success = land.TryPutPiece(building);

            // assert
            success.Should().BeFalse();
            hasBuildingBeforeAct.Should().BeTrue();
            land.HasBuilder.Should().BeFalse();
            land.HasBuilding.Should().BeTrue();
            land.Pieces.Should().HaveCount(1);
            land.Building.Equals(previousBuilding).Should().BeTrue();
        }

        [Fact]
        public void Land_must_accept_builder_when_empty()
        {
            // arrange
            var posX = _faker.Random.Number(0, 4);
            var posY = _faker.Random.Number(0, 4);

            var land = new Land(_board, posX, posY);

            var player = new Player(_faker.Name.FirstName());
            var builder = player.Builders.First();

            // act
            var success = land.TryPutPiece(builder);

            // assert
            success.Should().BeTrue();
            land.HasBuilder.Should().BeTrue();
            land.Builder.Equals(builder).Should().BeTrue();
        }

        [Fact]
        public void Land_must_accept_builder_when_existing_buildind_upto_1_level_higher()
        {
            // arrange
            var posX = _faker.Random.Number(0, 4);
            var posY = _faker.Random.Number(0, 4);

            var originLand = new Land(_board, posX, posY);
            var player = new Player(_faker.Name.FirstName());
            var builder = player.Builders.First();
            originLand.TryPutPiece(builder);

            posX = _faker.Random.Number(0, 4);
            posY = _faker.Random.Number(0, 4);

            var destinationland = new Land(_board, posX, posY);
            destinationland.TryPutPiece(new Building());

            // act
            var hasBuildingBeforeMove = destinationland.HasBuilding;
            var landLevelBeforeMove = destinationland.LandLevel;
            var success = destinationland.TryPutPiece(builder);

            // assert
            success.Should().BeTrue();
            hasBuildingBeforeMove.Should().BeTrue();
            landLevelBeforeMove.Should().Be(1);
            destinationland.HasBuilder.Should().BeTrue();
            destinationland.Builder.Equals(builder).Should().BeTrue();
        }

        [Fact]
        public void Land_must_refuse_builder_when_existing_buildind_more_than_1_level_higher()
        {
            // arrange
            var posX = _faker.Random.Number(0, 4);
            var posY = _faker.Random.Number(0, 4);

            var originLand = new Land(_board, posX, posY);
            var player = new Player(_faker.Name.FirstName());
            var builder = player.Builders.First();
            originLand.TryPutPiece(builder);

            posX = _faker.Random.Number(0, 4);
            posY = _faker.Random.Number(0, 4);

            var destinationland = new Land(_board, posX, posY);
            var building = new Building();
            building.RaiseLevel();
            destinationland.TryPutPiece(building);

            // act
            var hasBuildingBeforeMove = destinationland.HasBuilding;
            var landLevelBeforeMove = destinationland.LandLevel;
            var success = destinationland.TryPutPiece(builder);

            // assert
            success.Should().BeFalse();
            hasBuildingBeforeMove.Should().BeTrue();
            landLevelBeforeMove.Should().Be(2);
            destinationland.HasBuilder.Should().BeFalse();
            destinationland.HasBuilder.Should().BeFalse();
        }

        [Fact]
        public void Land_must_reject_builder_when_occupied_by_another_builder()
        {
            // arrange
            var posX = _faker.Random.Number(0, 4);
            var posY = _faker.Random.Number(0, 4);

            var land = new Land(_board, posX, posY);

            var player = new Player(_faker.Name.FirstName());
            var builder1 = player.Builders.First();
            var builder2 = player.Builders.Last();

            land.TryPutPiece(builder1);

            // act
            var success = land.TryPutPiece(builder2);

            // assert
            success.Should().BeFalse();
            land.HasBuilder.Should().BeTrue();
            land.Builder.Equals(builder1).Should().BeTrue();
            land.Builder.Equals(builder2).Should().BeFalse();
        }

        [Fact]
        public void Land_should_allow_remove_existing_builder()
        {
            // arrange
            var posX = _faker.Random.Number(0, 4);
            var posY = _faker.Random.Number(0, 4);

            var land = new Land(_board, posX, posY);

            var player = new Player(_faker.Name.FirstName());
            var builder = player.Builders.First();

            land.TryPutPiece(builder);

            // act
            var hasBuilderBeforeRemove = land.HasBuilder;
            var success = land.TryRemoveBuilder(builder);

            // assert
            success.Should().BeTrue();
            hasBuilderBeforeRemove.Should().BeTrue();
            land.HasBuilder.Should().BeFalse();
        }

        [Fact]
        public void Land_should_fail_to_remove_non_existing_builder()
        {
            // arrange
            var posX = _faker.Random.Number(0, 4);
            var posY = _faker.Random.Number(0, 4);

            var land = new Land(_board, posX, posY);

            var player = new Player(_faker.Name.FirstName());
            var builder = player.Builders.First();

            // act
            var hasBuilderBeforeRemove = land.HasBuilder;
            var success = land.TryRemoveBuilder(builder);

            // assert
            success.Should().BeFalse();
            hasBuilderBeforeRemove.Should().BeFalse();
            land.HasBuilder.Should().BeFalse();
        }

        [Fact]
        public void Land_should_fail_to_remove_different_builder()
        {
            // arrange
            var posX = _faker.Random.Number(0, 4);
            var posY = _faker.Random.Number(0, 4);

            var player = new Player(_faker.Name.FirstName());
            var builder1 = player.Builders.First();
            var builder2 = player.Builders.Last();

            var land = new Land(_board, posX, posY);
            land.TryPutPiece(builder1);

            // act
            var hasBuilderBeforeRemove = land.HasBuilder;
            var success = land.TryRemoveBuilder(builder2);

            // assert
            success.Should().BeFalse();
            hasBuilderBeforeRemove.Should().BeTrue();
            land.HasBuilder.Should().BeTrue();
            land.Builder.Equals(builder1).Should().BeTrue();
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
            var board = default(Board);
            var posX = _faker.Random.Number(0, 4);
            var posY = _faker.Random.Number(0, 4);

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
            var equalsAB = landA.Equals(landB);
            var equalsBC = landB.Equals(landC);
            var equalsCD = landC.Equals(landD);
            var equalsDE = landD.Equals(landE);

            // assert
            equalsAB.Should().BeTrue();
            equalsBC.Should().BeFalse();
            equalsCD.Should().BeFalse();
            equalsDE.Should().BeFalse();
        }
    }
}
