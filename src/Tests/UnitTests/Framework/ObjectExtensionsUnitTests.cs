using FluentAssertions;
using Framework.Extensions;
using NUnit.Framework;

namespace UnitTests.Framework;

public sealed class ObjectExtensionsUnitTests
{
    [TestFixture]
    public sealed class MappingTests
    {

        [Test]
        public void MapRef_NonNull()
        {
            var nonNullReference = "not a null";
            ("some value").Should().Be(nonNullReference.MapRef(_ => { return "some value"; }));
        }

        [Test]
        public void MapRef_Null()
        {
            var nullReference = (string)null;
            ((object)null).Should().Be(nullReference.MapRef(_ => "some mapped value"));
        }
        
        [Test]
        public async Task MapRefAsync_NonNull_Async()
        {
            var nonNullReference = "non null";
            ("mapped value").Should().Be(await nonNullReference.MapRefAsync(_ => Task.FromResult("mapped value"))); 
        }

        [Test]
        public async Task MapRefAsync_Null_Async()
        {
            var nullReference = (string)null;
            Assert.That(await nullReference.MapRefAsync(_ => Task.FromResult("mapped value")), Is.Null);
        }
    }

    [TestFixture]
    public sealed class NullTestsReferenceType
    {
        [Test]
        public void IsNull_Null()
        {
            var nullReference = (string)null;
            Assert.That(nullReference.IsNull(), Is.True);
        }
        
        [Test]
        public void IsNull_NonNull()
        {
            var validReference = "not a null";
            Assert.That(validReference.IsNull(), Is.False);
        }
        
        [Test]
        public void IsNotNull_Null()
        {
            var nullReference = (string)null;
            Assert.That(nullReference.IsNotNull(), Is.False);
        }
        
        [Test]
        public void IsNotNull_NonNull()
        {
            var validReference = "not a null";
            Assert.That(validReference.IsNotNull(), Is.True);
        }
    }
    
    [TestFixture]
    public sealed class NullTestsValueType
    {
        [Test]
        public void IsNull_Null()
        {
            var nullReference = (int?)null;
            Assert.That(nullReference.IsNull(), Is.True);
        }
        
        [Test]
        public void IsNull_NonNull()
        {
            int? validReference = 69;
            Assert.That(validReference.IsNull(), Is.False);
        }
        
        [Test]
        public void IsNotNull_Null()
        {
            var nullReference = (int?)null;
            Assert.That(nullReference.IsNotNull(), Is.False);
        }
        
        [Test]
        public void IsNotNull_NonNull()
        {
            int? validReference = 420;
            Assert.That(validReference.IsNotNull(), Is.True);
        }
    }
    
    [TestFixture]
    public sealed class ToEnumerable
    {
        [Test]
        public void ToEnumerable_Reference()
        {
            // Arrange
            var myString = "someValue";
            
            // Act
            var res = myString.ToEnumerable();
            
            // Assert
            Assert.That(res.IsNotNull(), Is.True);
            Assert.That(res.Count(), Is.EqualTo(1));
        }
        
        [Test]
        public void ToEnumerable_Struct()
        {
            // Arrange
            int myNumber = 5;
            
            // Act
            var res = myNumber.ToEnumerable();
            
            // Assert
            Assert.That(res.IsNotNull(), Is.True);
            Assert.That(res.Count(), Is.EqualTo(1));
        }
        
                
        [Test]
        public void ToEnumerable_StructNullable()
        {
            // Arrange
            int? myNumber = 5;
            
            // Act
            var res = myNumber.ToEnumerable();
            
            // Assert
            Assert.That(res.IsNotNull(), Is.True);
            Assert.That(res.Count(), Is.EqualTo(1));
        }
        
        [Test]
        public void ToEnumerable_Null()
        {
            // Arrange
            var mynull = (string)null;
            
            // Act
            var res = mynull.ToEnumerable();
            
            // Assert
            Assert.That(res.IsNotNull(), Is.True);
            Assert.That(res.Count(), Is.EqualTo(1));
        }
    }
}
