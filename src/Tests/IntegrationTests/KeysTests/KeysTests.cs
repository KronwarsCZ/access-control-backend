using Core.Entities;

namespace IntegrationTests.KeysTests
{
    public class KeysTests : AccessControlIntegrationTests
    {
        [Test]
        public async Task KeyService_GetAllAsync_SingleKey_Query_Succeeds()
        {
            // Arrange
            var KeyName = "mujKey";
            var key = Key.Create(KeyName, "XD");
            var keyEnum = new [] { key };
            unitOfWork.KeyCommandRepository.Add(keyEnum);
            await unitOfWork.SaveChangesAsync();
            
            // Act
            var fetchedKeys = await keyService.GetAllAsync(new CancellationToken());
            var fetchedKey = fetchedKeys.First();
            
            // Assert
            Assert.AreEqual(1, fetchedKeys.Count());
            Assert.AreEqual(key.Pass, fetchedKey.Pass);
            Assert.AreEqual(KeyName, fetchedKey.Name);
        }

        [Test]
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
            unitOfWork.KeyCommandRepository.Add(keysEnum);
            await unitOfWork.SaveChangesAsync();

            //Act
            var fetchedKeys = await keyService.GetAllAsync(new CancellationToken());
            
            // Assert
            var fetchedKey1 = fetchedKeys.First();
            var fetchedKey2 = fetchedKeys.Skip(1).First();
            var fetchedKey3 = fetchedKeys.Skip(2).First();

            Assert.AreEqual(3, fetchedKeys.Count());
            
            Assert.AreEqual(key1.Pass, fetchedKey1.Pass);
            Assert.AreEqual(KeyName1, fetchedKey1.Name);
            
            Assert.AreEqual(key2.Pass, fetchedKey2.Pass);
            Assert.AreEqual(KeyName2, fetchedKey2.Name);
            
            Assert.AreEqual(key3.Pass, fetchedKey3.Pass);
            Assert.AreEqual(KeyName3, fetchedKey3.Name);
        }

        // The instance of entity type 'KeyEntity' cannot be tracked because another instance with the same key value for {'Id'} is already being tracked.
        /*
        [Test]
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
            unitOfWork.KeyCommandRepository.Add(keysEnum);
            await unitOfWork.SaveChangesAsync();

            var keyDeleteEnum = new[] { key2 };
            unitOfWork.KeyCommandRepository.Delete(keyDeleteEnum);
            await unitOfWork.SaveChangesAsync();

            //Act
            var fetchedKeys = await keyService.GetAllAsync(new CancellationToken());

            // Assert
            var fetchedKey1 = fetchedKeys.First();
            var fetchedKey2 = fetchedKeys.Skip(1).First();

            Assert.AreEqual(2, fetchedKeys.Count());
            
            Assert.AreEqual(key1.Pass, fetchedKey1.Pass);
            Assert.AreEqual(KeyName1, fetchedKey1.Name);
            
            Assert.AreEqual(key3.Pass, fetchedKey2.Pass);
            Assert.AreEqual(KeyName3, fetchedKey2.Name);  
        }
        */
    }
}
