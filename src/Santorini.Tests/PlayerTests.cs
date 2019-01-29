using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Bogus;
using FluentAssertions;
using Xunit;

namespace Santorini.Tests
{
    [ExcludeFromCodeCoverage]
    public class PlayerTests
    {
        private readonly Faker _faker;

        public PlayerTests()
        {
            _faker = new Faker();
        }

        [Fact]
        public void Player_must_have_a_name()
        {
            // arrange
            var playerName = string.Empty;

            // act
            Action act = () => new Player(playerName);

            // assert
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Player_start_with_2_workers()
        {
            // arrange
            var playerName = _faker.Name.FirstName();

            // arrange & act
            var player = new Player(playerName);

            // assert
            player.Should().NotBeNull();
            player.Name.Should().Be(playerName);
            player.Workers.Should().HaveCount(2);
            player.Workers.First().Number.Should().Be(1);
            player.Workers.Last().Number.Should().Be(2);
        }
    }
}
