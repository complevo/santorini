using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Bogus;
using FluentAssertions;
using Xunit;

namespace Santorini.Tests
{
    [ExcludeFromCodeCoverage]
    public class PieceTests
    {
        private readonly Faker _faker;

        public PieceTests()
        {
            _faker = new Faker();
        }

        [Fact]
        public void Piece_must_always_have_unique_Id()
        {
            var player = new Player(_faker.Name.FirstName());

            var piece1 = player.Builders.First() as Piece;
            var piece2 = player.Builders.Last() as Piece;

            piece1.Id.Should().NotBeEmpty();
            piece2.Id.Should().NotBeEmpty();
            piece2.Id.Should().NotBe(piece1.Id);
        }
    }
}
