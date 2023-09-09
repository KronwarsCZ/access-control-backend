using Core.Entities;
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
            var key = Key.Create(KeyName, "XD");
            await keyService.AddAsync(key.ToEnumerable(), new CancellationToken());
            
            // Act
            var fetchedKeys = await keyService.GetAllAsync(new CancellationToken());
            var fetchedKey = fetchedKeys.First();
            
            // Assert
            Assert.Single(fetchedKeys);
            Assert.Equal(key.Pass, fetchedKey.Pass);
            Assert.Equal(KeyName, fetchedKey.Name);
        }

        [Fact]
        public async Task KeyService_GetAllAsync_MultipleKeys_Query_Succeeds()
        {
            // Arrange
            var KeyName1 = "mujKey1";
            var key1 = Key.Create(KeyName1, "11");
            var KeyName2 = "mujKey2";
            var key2 = Key.Create(KeyName2, "22");
            var KeyName3 = "mujKey3";
            var key3 = Key.Create(KeyName3, "33");

            var keysEnum = new[] { key1, key2, key3 };
            await keyService.AddAsync(keysEnum, new CancellationToken());

            //Act
            var fetchedKeys = await keyService.GetAllAsync(new CancellationToken());
            
            // Assert
            var fetchedKey1 = fetchedKeys.First();
            var fetchedKey2 = fetchedKeys.Skip(1).First();
            var fetchedKey3 = fetchedKeys.Skip(2).First();

            Assert.Equal(3, fetchedKeys.Count());
            
            Assert.Equal(key1.Pass, fetchedKey1.Pass);
            Assert.Equal(KeyName1, fetchedKey1.Name);
            
            Assert.Equal(key2.Pass, fetchedKey2.Pass);
            Assert.Equal(KeyName2, fetchedKey2.Name);
            
            Assert.Equal(key3.Pass, fetchedKey3.Pass);
            Assert.Equal(KeyName3, fetchedKey3.Name);
        }

        // LEOTODO: pouzit getbyid v testu
        [Fact]
        public async Task KeyService_Delete_SingleKey_FromMultiple_Query_Succeeds()
        {
            // Arrange
            var KeyName1 = "mujKey1";
            var key1 = Key.Create(KeyName1, "11");
            var KeyName2 = "mujKey2";
            var key2 = Key.Create(KeyName2, "22");
            var KeyName3 = "mujKey3";
            var key3 = Key.Create(KeyName3, "33");

            var keysEnum = new[] { key1, key2, key3 };
            await keyService.AddAsync(keysEnum, new CancellationToken());

            //Act
            await keyService.DeleteAsync(key2, new CancellationToken());
            var fetchedKeys = await keyService.GetAllAsync(new CancellationToken());

            // Assert
            var fetchedKey1 = fetchedKeys.First();
            var fetchedKey2 = fetchedKeys.Skip(1).First();

            Assert.Equal(2, fetchedKeys.Count());
            
            Assert.Equal(key1.Pass, fetchedKey1.Pass);
            Assert.Equal(KeyName1, fetchedKey1.Name);
            
            Assert.Equal(key3.Pass, fetchedKey2.Pass);
            Assert.Equal(KeyName3, fetchedKey2.Name);  
        }
        
        // LEOTODO: tady prosim pridat getbyid a pouzit to i v testu 
        [Fact]
        public async Task KeyService_Delete_EmptyDB_Query_NotThrow()
        {
            // Arrange
            var KeyName = "mujKey";
            var key = Key.Create(KeyName, "XD");
            var keyEnum = new [] { key };
            
            // Act
            var fetchedKeys = await keyService.GetAllAsync(new CancellationToken());

            // Assert
            Assert.Empty(fetchedKeys);
        }
    }
}
