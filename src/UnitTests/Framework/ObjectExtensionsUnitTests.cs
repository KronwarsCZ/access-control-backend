using FluentAssertions;
using Framework.Extensions;

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
            Assert.IsNull(await nullReference.MapRefAsync(_ => Task.FromResult("mapped value")));
        }
    }

    [TestFixture]
    public sealed class NullTests
    {
        [Test]
        public void IsNull_Null()
        {
            var nullReference = (string)null;
            Assert.True(nullReference.IsNull());
        }
        
        [Test]
        public void IsNull_NonNull()
        {
            var validReference = "not a null";
            Assert.False(validReference.IsNull());
        }
        
        [Test]
        public void IsNotNull_Null()
        {
            var nullReference = (string)null;
            Assert.False(nullReference.IsNotNull());
        }
        
        [Test]
        public void IsNotNull_NonNull()
        {
            var validReference = "not a null";
            Assert.True(validReference.IsNotNull());
        }
    }
}
