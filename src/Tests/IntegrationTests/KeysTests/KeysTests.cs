using Core.Entities;
using Framework.Extensions;
using Microsoft.EntityFrameworkCore;

namespace IntegrationTests.KeysTests
{
    public class KeysTests : AccessControlIntegrationTests
    {
        [Fact]
        public async Task KeyService_GetById_SingleKey_Query_Succeeds()
        {
            // Arrange
            var KeyName = "mujKey";
            var key = Key.Create(KeyName, "XD");
            await keyService.AddAsync(key.ToEnumerable(), new CancellationToken());
            
            // Act
            var fetchedKey = await keyService.GetByIdAsync(key.Id, new CancellationToken());

            // Assert
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
            var fetchedKey1 = await keyService.GetByIdAsync(key1.Id,new CancellationToken());
            var fetchedKey2 = await keyService.GetByIdAsync(key2.Id, new CancellationToken());
            var fetchedKey3 = await keyService.GetByIdAsync(key3.Id,new CancellationToken());
            
            // Assert
            Assert.Equal(key1.Pass, fetchedKey1.Pass);
            Assert.Equal(KeyName1, fetchedKey1.Name);
            
            Assert.Equal(key2.Pass, fetchedKey2.Pass);
            Assert.Equal(KeyName2, fetchedKey2.Name);
            
            Assert.Equal(key3.Pass, fetchedKey3.Pass);
            Assert.Equal(KeyName3, fetchedKey3.Name);
        }
        
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
            await keyService.DeleteAsync(key2, new CancellationToken());

            //Act
            var fetchedKey1 = await keyService.GetByIdAsync(key1.Id,new CancellationToken());
            var fetchedKey2 = await keyService.GetByIdAsync(key2.Id, new CancellationToken());
            var fetchedKey3 = await keyService.GetByIdAsync(key3.Id,new CancellationToken());
            
            // Assert
            Assert.Equal(key1.Pass, fetchedKey1.Pass);
            Assert.Equal(KeyName1, fetchedKey1.Name);
            
            Assert.Null(fetchedKey2);
            
            Assert.Equal(key3.Pass, fetchedKey3.Pass);
            Assert.Equal(KeyName3, fetchedKey3.Name);  
        }
        
        [Fact]
        public async Task KeyService_Delete_EmptyDB_Query_NotThrow()
        {
            // Arrange
            var KeyName = "mujKey";
            var key = Key.Create(KeyName, "XD");

            // Act

            // Assert
            await Assert.ThrowsAsync<DbUpdateConcurrencyException>(async () => await keyService.DeleteAsync(key, new CancellationToken()));

        }
    }
}
