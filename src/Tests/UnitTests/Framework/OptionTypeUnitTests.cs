using FluentAssertions;
using Framework.Extensions;
using Framework.Option;
using NUnit.Framework;

namespace UnitTests.Framework;

public sealed class OptionTypeUnitTests
{

    [Test]
    public void ToOption()
    {
        // Arrange
        var myString = "someValue";
            
        // Act
        var res = myString.ToOption();
            
        // Assert
        res.HasValue().Should().Be(true);
        res.GetOrNull().Should().Be(myString);
    }

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
            Assert.That(res, Is.True);
        }
        
        [Test]
        public void Option_HasValue_NonVal()
        {
            // Arrange
            var myString = Option<string>.None();

            // Act
            var res = myString.HasValue();

            // Assert
            Assert.That(res, Is.False);
        }
    }
    
    [TestFixture]
    public sealed class MatchTests
    {
        [Test]
        public void Option_Match_Returning_Value()
        {
            // Arrange
            var option = Option<string>.Some("some value");

            // Act
            var res = option.Match(a => { return "value";}, () => { return "empty";});

            // Assert
            res.Should().Be("value");
        }

        [Test]
        public void Option_Match_Returning_Empty()
        {
            // Arrange
            var emptyOption = Option<string>.None();

            // Act
            var res = emptyOption.Match(a => { return "value";}, () => { return "empty";});

            // Assert
            res.Should().Be("empty");
        }
        
        [Test]
        public void Option_Match_Actions_Value()
        {
            // Arrange
            var option = Option<string>.Some("some value");
            var res = "";

            // Act
            option.Match(a => { res = "value";}, () => { res = "empty";});

            // Assert
            res.Should().Be("value");
        }

        [Test]
        public void Option_Match_Actions_Empty()
        {
            // Arrange
            var emptyOption = Option<string>.None();
            var res = "";

            // Act
            emptyOption.Match(a => { res = "value";}, () => { res = "empty";});

            // Assert
            res.Should().Be("empty");
        }
    }
}