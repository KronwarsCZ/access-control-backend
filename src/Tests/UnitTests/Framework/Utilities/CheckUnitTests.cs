using Framework.Option;
using Framework.Utilities;
using NUnit.Framework;

namespace UnitTests.Framework.Utilities;

public sealed class CheckUnitTests
{
    [TestFixture]
    public sealed class CheckOption
    {
        [TestCase("some value")]
        [TestCase(42069)]
        public void CheckOption_Succeeds_With_Non_Empty_Option(object obj)
        {
            // Arrange
            var option = Option<object>.Some(obj);

            // Act
            var objectInsideOption = Check.Contract(option, "my error message");

            // Assert
            Assert.That(objectInsideOption, Is.EqualTo(obj));
        }
        
        public void CheckNotNull_Fails_With_Null()
        {
            // Arrange
            var emptyOption = Option<object>.None();

            Assert.Throws<ContractException>(() => Check.Contract(emptyOption, "my error message"));
        }

        [TestCase(null)]
        public void CheckNotNull_Fails_With_Null(object? obj)
        {
            // Arrange
            var emptyOption = Option<object>.Some(obj!);

            Assert.Throws<ContractException>(() => Check.Contract(emptyOption, "my error message"));
        }
    }
    
    [TestFixture]
    public sealed class CheckNotNull
    {
        [TestCase("some value")]
        [TestCase(42069)]
        public void CheckNotNull_Succeeds_With_Non_Null(object obj)
        {
            // Act
            var formattedOutput = Check.Contract(obj, "my error message");

            // Assert
            Assert.That(formattedOutput, Is.EqualTo(obj));
        }
        
        [TestCase(null)]
        public void CheckNotNull_Fails_With_Null(object? obj)
        {
            Assert.Throws<ContractException>(() => Check.Contract(obj, "my error message"));
        }
    }

    [TestFixture]
    public sealed class CheckBool
    {
        [TestCase(true)]
        public void CheckBool_True(bool obj)
        {
            // Assert
            Assert.DoesNotThrow(() => Check.Contract(obj, "my error message"));
        }
        
        [TestCase(false)]
        public void CheckBool_False(bool obj)
        {
            // Assert
            Assert.Throws<ContractException>(() => Check.Contract(obj, "my error message"));
        }
    }
}
