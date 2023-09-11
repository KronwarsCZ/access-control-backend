using Core.Entities.Keys;
using Framework.Extensions;

namespace IntegrationTests.KeysTests
{
    public class KeysTests : AccessControlIntegrationTests
    {
        [Fact]
        public async Task KeyService_GetAllAsync_SingleKey_Query_Succeeds()
        {
            // Arrange
            var KeyName = "mujKey";
            var key = Key.Create(KeyName);
            await keyService.AddAsync(key.ToEnumerable(), new CancellationToken());
            await keyService.SaveChangesAsync(new CancellationToken());
            // Act
            var fetchedKey = await keyService.GetByIdAsync(key.Id, new CancellationToken());
            
            // Assert
            Assert.NotNull(fetchedKey);
            Assert.Equal(key.Pass, fetchedKey.Pass);
            Assert.Equal(KeyName, fetchedKey.Name);
        }

        [Fact]
        public async Task KeyService_GetAllAsync_MultipleKeys_Query_Succeeds()
        {
            // Arrange
            var KeyName1 = "mujKey1";
            var key1 = Key.Create(KeyName1);
            var KeyName2 = "mujKey2";
            var key2 = Key.Create(KeyName2);
            var KeyName3 = "mujKey3";
            var key3 = Key.Create(KeyName3);

            var keysEnum = new[] { key1, key2, key3 };
            await keyService.AddAsync(keysEnum, new CancellationToken());
            await keyService.SaveChangesAsync(new CancellationToken());

            //Act
            var fetchedKey = await keyService.GetByIdAsync(key1.Id, new CancellationToken());
            
            // Assert
            
            Assert.Equal(key1.Id, fetchedKey.Id);
        }

        // LEOTODO: pouzit getbyid v testu
        [Fact]
        public async Task KeyService_Delete_SingleKey_FromMultiple_Query_Succeeds()
        {
            // Arrange
            var KeyName1 = "mujKey1";
            var key1 = Key.Create(KeyName1);
            var KeyName2 = "mujKey2";
            var key2 = Key.Create(KeyName2);
            var KeyName3 = "mujKey3";
            var key3 = Key.Create(KeyName3);

            var keysEnum = new[] { key1, key2, key3 };
            await keyService.AddAsync(keysEnum, new CancellationToken());
            await keyService.SaveChangesAsync(new CancellationToken());
            //Act
            await keyService.DeleteAsync(key2, new CancellationToken());
            await keyService.SaveChangesAsync(new CancellationToken());

            var fetchedKey = await keyService.GetByIdAsync(key1.Id, new CancellationToken());

            // Assert

            Assert.NotNull(fetchedKey);
            Assert.Equal(key1.Id, fetchedKey.Id);
        }
        
        [Fact]
        public async Task KeyService_Delete_EmptyDB_Query_NotThrow()
        {
            // Arrange
            var KeyName = "mujKey";
            var key = Key.Create(KeyName);
            var keyEnum = new [] { key };
            
            // Act
            var fetchedKey = await keyService.GetByIdAsync(key.Id, new CancellationToken());

            // Assert
            Assert.Null(fetchedKey);
        }
    }
}
