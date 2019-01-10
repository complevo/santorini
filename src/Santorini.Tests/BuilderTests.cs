using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Bogus;
using FluentAssertions;
using Xunit;

namespace Santorini.Tests
{
    [ExcludeFromCodeCoverage]
    public class BuilderTests
    {
        private readonly Faker _faker;

        public BuilderTests()
        {
            _faker = new Faker();
        }

        [Fact]
        public void Builder_must_have_number_greater_than_zero()
        {
            // arrange
            var player = new Player(_faker.Name.FirstName());

            // act
            Action act = () => new Builder(player, 0);

            // assert
            act.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void Builder_must_have_player_as_owner()
        {
            // arrange
            var player = default(Player);

            // act
            Action act = () => new Builder(player, 0);

            // assert
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Builders_must_have_numbers_1_and_2()
        {
            var player = new Player(_faker.Name.FirstName());

            var builder1 = player.Builders.First();
            var builder2 = player.Builders.Last();

            player.Builders.Should().HaveCount(2);
            builder1.Number.Should().Be(1);
            builder2.Number.Should().Be(2);
        }

        [Fact]
        public void Builders_must_always_belongs_to_a_player()
        {
            var player = new Player(_faker.Name.FirstName());

            var builder1 = player.Builders.First();
            var builder2 = player.Builders.Last();

            builder1.Player.Should().NotBeNull();
            builder1.Player.Name.Should().Be(player.Name);
            builder2.Player.Should().NotBeNull();
            builder2.Player.Name.Should().Be(player.Name);
        }

        [Fact]
        public void Builders_must_be_different()
        {
            var player = new Player(_faker.Name.FirstName());

            var builder1 = player.Builders.First();
            var builder2 = player.Builders.Last();

            builder1.Equals(builder2).Should().BeFalse();
        }

        [Fact]
        public void Builders_can_be_compared()
        {
            var player1 = new Player(_faker.Name.FirstName());
            var player2 = new Player(_faker.Name.FirstName());

            var builderA = new Builder(player1, 1);
            var builderB = new Builder(player1, 1);
            var builderC = new Builder(player1, 2);
            var builderD = new Builder(player2, 2);
            var builderE = default(Builder);

            builderA.Equals(builderB).Should().BeTrue();
            builderB.Equals(builderC).Should().BeFalse();
            builderC.Equals(builderD).Should().BeFalse();
            builderD.Equals(builderE).Should().BeFalse();
        }

        [Fact]
        public void Builder_can_move_to_next_empty_land()
        {
            // arrange
            var board = new Board();
            var player = new Player(_faker.Name.FirstName());
            var builder = player.Builders.First();
            board.TryAddPiece(builder, 0, 0);

            // act
            var success = builder.TryMoveTo(0, 1);

            // arrange
            success.Should().BeTrue();
            board.IsEmpty(0, 0).Should().BeTrue();
            board.IsEmpty(0, 1).Should().BeFalse();
        }

        [Fact]
        public void Builder_can_cannot_move_not_placed_builder()
        {
            // arrange
            var board = new Board();
            var player = new Player(_faker.Name.FirstName());
            var builder1 = player.Builders.First();
            var builder2 = player.Builders.Last();

            board.TryAddPiece(builder1, 0, 0);

            // act
            var success1 = builder1.TryMoveTo(0, 1);
            var success2 = builder2.TryMoveTo(1, 0);

            // assert
            success1.Should().BeTrue();
            success2.Should().BeFalse();
        }

        [Fact]
        public void Refuse_move_builder_to_invalid_coordinate()
        {
            // arrange
            var board = new Board();
            var player = new Player(_faker.Name.FirstName());
            var builder = player.Builders.First();

            board.TryAddPiece(builder, 0, 0);

            // act
            var success = builder.TryMoveTo(-1, 5);

            // assert
            success.Should().BeFalse();
        }


        [Fact]
        public void Refuse_move_builder_2_lands_away()
        {
            // arrange
            var board = new Board();
            var player = new Player(_faker.Name.FirstName());
            var builder = player.Builders.First();

            board.TryAddPiece(builder, 0, 0);

            // act
            var success1 = builder.TryMoveTo(0, 2);
            var success2 = builder.TryMoveTo(2, 0);

            // assert
            success1.Should().BeFalse();
            success2.Should().BeFalse();
        }

        [Fact]
        public void Refuse_move_builder_to_occupied_land()
        {
            // arrange
            var board = new Board();
            var player = new Player(_faker.Name.FirstName());
            var builder = player.Builders.First();

            board.TryAddPiece(builder, 0, 0);

            // act
            var success1 = builder.TryMoveTo(0, 2);
            var success2 = builder.TryMoveTo(2, 0);

            // assert
            success1.Should().BeFalse();
            success2.Should().BeFalse();
        }

        [Fact]
        public void Refuse_move_builder_to_a_land_2_steps_higher()
        {
            // arrange
            var board = new Board();
            var player = new Player(_faker.Name.FirstName());
            var builder1 = player.Builders.First();
            var builder2 = player.Builders.Last();

            board.TryAddPiece(builder1, 0, 0);
            board.TryAddPiece(builder2, 4, 4);

            var building1 = new Building();

            var building2 = new Building();
            building2.RaiseLevel();

            board.TryAddPiece(building1, 0, 1);
            board.TryAddPiece(building2, 4, 3);

            // act
            var success1 = builder1.TryMoveTo(0, 1);
            var success2 = builder1.TryMoveTo(4, 3);

            // assert
            success1.Should().BeTrue();
            success2.Should().BeFalse();
        }

        [Fact]
        public void Builder_can_build_to_next_empty_land()
        {
            // arrange
            var board = new Board();
            var player = new Player(_faker.Name.FirstName());
            var builder = player.Builders.First();
            board.TryAddPiece(builder, 0, 0);

            // act
            var success = builder.TryBuildAt(0, 1);

            // arrange
            success.Should().BeTrue();
            board.IsEmpty(0, 0).Should().BeFalse();
            board.Lands[0, 0].HasBuilder.Should().BeTrue();
            board.Lands[0, 1].HasBuilder.Should().BeFalse();
            board.Lands[0, 0].HasBuilding.Should().BeFalse();
            board.Lands[0, 1].HasBuilding.Should().BeTrue();
        }

        [Fact]
        public void Builder_cannot_build_2_lands_away()
        {
            // arrange
            var board = new Board();
            var player = new Player(_faker.Name.FirstName());
            var builder1 = player.Builders.First();
            var builder2 = player.Builders.First();

            board.TryAddPiece(builder1, 0, 0);
            board.TryAddPiece(builder2, 4, 4);

            // act
            var success1 = builder1.TryBuildAt(0, 2);
            var success2 = builder2.TryBuildAt(2, 4);

            // assert
            success1.Should().BeFalse();
            success2.Should().BeFalse();
        }

        [Fact]
        public void Builder_cannot_build_occupied_land()
        {
            // arrange
            var board = new Board();
            var player = new Player(_faker.Name.FirstName());
            var builder1 = player.Builders.First();
            var builder2 = player.Builders.Last();

            board.TryAddPiece(builder1, 0, 0);
            board.TryAddPiece(builder2, 0, 1);

            // act
            var success = builder1.TryBuildAt(0, 1);

            // assert
            success.Should().BeFalse();
        }

        [Fact]
        public void Builder_cannot_build_own_land()
        {
            // arrange
            var board = new Board();
            var player = new Player(_faker.Name.FirstName());
            var builder = player.Builders.First();

            board.TryAddPiece(builder, 0, 0);

            // act
            var success = builder.TryBuildAt(0, 0);

            // assert
            success.Should().BeFalse();
        }

        [Fact]
        public void Builder_cannot_build_land_maxwith_maxlevel_reached()
        {
            // arrange
            var board = new Board();
            var player = new Player(_faker.Name.FirstName());
            var builder = player.Builders.First();
            var building = new Building();
            while (!building.MaxLevelReached) building.RaiseLevel();

            board.TryAddPiece(builder, 0, 0);
            board.TryAddPiece(building, 0, 1);

            // act
            var success = builder.TryBuildAt(0, 1);

            // assert
            success.Should().BeFalse();
        }

        [Fact]
        public void Builder_can_raise_building_level_by_1()
        {
            // arrange
            var board = new Board();
            var player = new Player(_faker.Name.FirstName());
            var builder = player.Builders.First();
            var building = new Building();
            building.RaiseLevel();

            board.TryAddPiece(builder, 0, 0);
            board.TryAddPiece(building, 0, 1);

            // act
            var buildingLevelBeforeBuild = building.Level;
            var success = builder.TryBuildAt(0, 1);

            // assert
            success.Should().BeTrue();
            buildingLevelBeforeBuild.Should().Be(2);
            building.Level.Should().Be(3);
        }


        [Fact]
        public void Builder_cannot_raise_building_at_maxlevel()
        {
            // arrange
            var board = new Board();
            var player = new Player(_faker.Name.FirstName());
            var builder = player.Builders.First();
            var building = new Building();
            while (!building.MaxLevelReached) building.RaiseLevel();

            board.TryAddPiece(builder, 0, 0);
            board.TryAddPiece(building, 0, 1);

            // act
            var buildingLevelBeforeBuild = building.Level;
            var success = builder.TryBuildAt(0, 1);

            // assert
            success.Should().BeFalse();
            buildingLevelBeforeBuild.Should().Be(4);
            building.Level.Should().Be(4);
        }

        [Fact]
        public void Builder_cannot_build_at_invalid_coordinate()
        {
            // arrange
            var board = new Board();
            var player = new Player(_faker.Name.FirstName());
            var builder = player.Builders.First();

            board.TryAddPiece(builder, 0, 0);

            // act
            var success = builder.TryBuildAt(5, -1);

            // assert
            success.Should().BeFalse();
        }
    }
}
