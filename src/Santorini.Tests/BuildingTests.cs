using System.Diagnostics.CodeAnalysis;
using Bogus;
using FluentAssertions;
using Xunit;

namespace Santorini.Tests
{
    [ExcludeFromCodeCoverage]
    public class BuildingTests
    {
        private readonly Faker _faker;

        public BuildingTests()
        {
            _faker = new Faker();
        }

        [Fact]
        public void Building_must_always_initiate_level_1()
        {
            var building = new Building();

            building.Id.Should().NotBeEmpty();
            building.Level.Should().Be(1);
        }

        [Fact]
        public void Building_must_raise_only_1_step()
        {
            var building = new Building();
            var levelBeforeRaise = building.Level;
            var levelAfterRaise = building.RaiseLevel();

            levelBeforeRaise.Should().Be(1);
            levelAfterRaise.Should().Be(2);
            levelAfterRaise.Should().Be(building.Level);
        }
    }
}
