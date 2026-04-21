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
            var owner = await GetOwnerAsync();
            var KeyName = "mujKey";
            var key = Key.Create(owner, KeyName, "AABB0011");
            await keyService.AddAsync(key.ToEnumerable(), new CancellationToken());
            await keyService.SaveChangesAsync(new CancellationToken());
            // Act
            var fetchedKey = await keyService.GetByIdAsync(key.Id, new CancellationToken());
            
            // Assert
            Assert.NotNull(fetchedKey);
            Assert.Equal(key.KeyUid, fetchedKey.KeyUid);
            Assert.Equal(KeyName, fetchedKey.Name);
        }

        [Fact]
        public async Task KeyService_GetAllAsync_MultipleKeys_Query_Succeeds()
        {
            // Arrange
            var owner = await GetOwnerAsync();
            var KeyName1 = "mujKey1";
            var key1 = Key.Create(owner, KeyName1, "AABB0001");
            var KeyName2 = "mujKey2";
            var key2 = Key.Create(owner, KeyName2, "AABB0002");
            var KeyName3 = "mujKey3";
            var key3 = Key.Create(owner, KeyName3, "AABB0003");

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
            var owner = await GetOwnerAsync();
            var KeyName1 = "mujKey1";
            var key1 = Key.Create(owner, KeyName1, "AABB0001");
            var KeyName2 = "mujKey2";
            var key2 = Key.Create(owner, KeyName2, "AABB0002");
            var KeyName3 = "mujKey3";
            var key3 = Key.Create(owner, KeyName3, "AABB0003");

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
        public async Task KeyService_EditValidityAsync_UpdatesFields()
        {
            // Arrange
            var owner = await GetOwnerAsync();
            var key = Key.Create(owner, "validity key", "AABB00A0");
            await keyService.AddAsync(key.ToEnumerable(), new CancellationToken());
            await keyService.SaveChangesAsync(new CancellationToken());

            var newStart = DateTime.UtcNow - TimeSpan.FromMinutes(1);
            var newEnd = DateTime.UtcNow + TimeSpan.FromHours(2);

            // Act
            await keyService.EditValidityAsync(key, newStart, newEnd, new CancellationToken());
            await keyService.SaveChangesAsync(new CancellationToken());

            var fetchedKey = await keyService.GetByIdAsync(key.Id, new CancellationToken());

            // Assert
            Assert.NotNull(fetchedKey);
            Assert.Equal(newStart, fetchedKey.ValidityStartUtc);
            Assert.Equal(newEnd, fetchedKey.ValidityEndUtc);
        }

        [Fact]
        public async Task KeyService_Delete_EmptyDB_Query_NotThrow()
        {
            // Arrange
            var owner = await GetOwnerAsync();
            var KeyName = "mujKey";
            var key = Key.Create(owner, KeyName, "AABB0099");
            var keyEnum = new [] { key };
            
            // Act
            var fetchedKey = await keyService.GetByIdAsync(key.Id, new CancellationToken());

            // Assert
            Assert.Null(fetchedKey);
        }
    }
}
