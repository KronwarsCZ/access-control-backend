using FluentAssertions;
using Framework.Option;

namespace UnitTests.Framework;

public sealed class OptionTypeUnitTests
{
    [TestFixture]
    public sealed class MappingTests
    {
        [Test]
        public void Option_GetOrNull_Val()
        {
            // Arrange
            var myString = Option<string>.Some("someValue");
            
            // Act
            var res = myString.GetOrNull();
            
            // Assert
            res.Should().Be("someValue");
        }

        [Test]
        public void Option_GetOrNull_NonVal()
        {
            // Arrange
            var myString = Option<string>.None();
            
            // Act
            var res = myString.GetOrNull();
            
            // Assert
            res.Should().Be(null);
        }
        
        [Test]
        public void Option_Map_Val()
        {
            // Arrange
            var myString = Option<string>.Some("someValue");
            
            // Act
            var mappedVal = myString.Map(s => s + "Mapped").GetOrNull();
            
            // Assert
            mappedVal.Should().Be("someValueMapped");
        }

        [Test]
        public void Option_Map_NonVal()
        {
            // Arrange
            var emptyOption = Option<string>.None();
            
            // Act
            var mappedVal = emptyOption.Map(s => s + "Mapped").GetOrNull();
            
            // Assert
            mappedVal.Should().Be(null);
        }
        
        [Test]
        public void Option_GetOrElse_NonVal()
        {
            // Arrange
            var emptyOption = Option<string>.None();
            
            // Act
            var mappedVal = emptyOption.Map(s => s + "Mapped").GetOrElse("fallbackValue");
            
            // Assert
            mappedVal.Should().Be("fallbackValue");
        }
        
        [Test]
        public void Option_GetOrElse_Val()
        {
            // Arrange
            var myString = Option<string>.Some("someValue");
            
            // Act
            var mappedVal = myString.Map(s => s + "Mapped").GetOrElse("fallbackValue");
            
            // Assert
            mappedVal.Should().Be("someValueMapped");
        }
    }

    [TestFixture]
    public sealed class HasValueTests
    {
        [Test]
        public void Option_HasValue_Val()
        {
            // Arrange
            var myString = Option<string>.Some("someValue");

            // Act
            var res = myString.HasValue();

            // Assert
            Assert.IsTrue(res);
        }
        
        [Test]
        public void Option_HasValue_Empty()
        {
            // Arrange
            var emptyOption = Option<string>.None();

            // Act
            var res = emptyOption.HasValue();

            // Assert
            Assert.IsFalse(res);
        }
    }
}