using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Bogus;
using FluentAssertions;
using Xunit;

namespace Santorini.Tests
{
    [ExcludeFromCodeCoverage]
    public class CoordTests
    {
        private readonly Faker _faker;

        public CoordTests()
        {
            _faker = new Faker();
        }

        [Fact]
        public void Create_valid_coord()
        {
            // arrange
            var posX = NewPos();
            var posY = NewPos();

            // act
            var coord = new Coord(posX, posY);

            // assert
            coord.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Coord_can_be_compared()
        {
            // arrange
            var coord1 = new Coord(0, 0);
            var coord2 = new Coord(0, 1);
            var coord3 = new Coord(1, 0);
            var coord4 = new Coord(1, 1);
            var coord5 = new Coord(1, 1);
            var coord6 = default(Coord);
            var coord7 = default(Coord);
            var coord8 = default(object);
            var anything = DateTime.UtcNow;

            // act
            var eq12_1 = coord1.Equals(coord2);
            var eq12_2 = coord1 == coord2;
            var eq23_1 = coord2.Equals(coord3);
            var eq23_2 = coord2 == coord3;
            var eq34_1 = coord3.Equals(coord4);
            var eq34_2 = coord3 == coord4;
            var eq45_0 = coord4.Equals((object)coord5);
            var eq45_1 = coord4.Equals(coord5);
            var eq45_2 = coord4 == coord5;            
            var eq56_1 = coord5.Equals((object)coord6);
            var eq56_2 = coord5 == coord6;
            var eq56_3 = coord6 == coord5;
            var eq67_1 = coord6 == coord7;
            var eq58_1 = coord5.Equals(coord8);
            var eq58_2 = coord5.Equals(anything);

            // assert
            eq12_1.Should().BeFalse();
            eq12_2.Should().BeFalse();
            eq23_1.Should().BeFalse();
            eq23_2.Should().BeFalse();
            eq34_1.Should().BeFalse();
            eq34_2.Should().BeFalse();
            eq45_0.Should().BeTrue();
            eq45_1.Should().BeTrue();
            eq45_2.Should().BeTrue();
            eq56_1.Should().BeFalse();
            eq56_2.Should().BeFalse();
            eq56_3.Should().BeFalse();
            eq67_1.Should().BeTrue();
            eq58_1.Should().BeFalse();
            eq58_2.Should().BeFalse();

            coord4.GetHashCode().Should().Be(coord5.GetHashCode());
        }

        private int NewPos()
            => _faker.Random.Number(0, 4);
    }
}
