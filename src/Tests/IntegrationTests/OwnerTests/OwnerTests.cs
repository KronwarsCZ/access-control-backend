using Core.Entities;
using Framework.Extensions;

namespace IntegrationTests.OwnerTests;

public class OwnerTests : AccessControlIntegrationTests
{
    [Fact]
    public async Task OwnerService_OneOwner_OnlyName_Query_Succeeds()
    {
        // Arrange
        var ownerName = "mujOwner";
        var owner = Owner.Create(ownerName);
        unitOfWork.OwnerRepository.Add(owner.ToEnumerable());
        await unitOfWork.SaveChangesAsync();
            
        // Act
        var fetchedOwner = await ownerService.GetByIdAsync(owner.Id , new CancellationToken());

        // Assert
        Assert.NotNull(fetchedOwner);
        Assert.Equal(ownerName, fetchedOwner.Name);
    }
    
    [Fact]
    public async Task OwnerService_OneOwner_Areas_Query_Succeeds()
    {
        // Arrange
        var ownerName = "mujOwner";
        var owner = Owner.Create(ownerName);
        var area = new Area() { Name = "a", Buildings = null, Owner = owner, Location = "" };
        var areas = new List<Area>() {area };
        owner.Areas = areas;
        unitOfWork.OwnerRepository.Add(owner.ToEnumerable());
        await unitOfWork.SaveChangesAsync();
            
        // Act
        var fetchedOwner = await ownerService.GetByIdAsync(owner.Id, new CancellationToken());


        // Assert
        Assert.NotNull(fetchedOwner);
        Assert.Equal(ownerName, fetchedOwner.Name);
        Assert.Equal(areas,owner.Areas);
    }
    
    [Fact]
    public async Task OwnerService_OneOwner_AreasDifferent_Query_Succeeds()
    {
        // Arrange
        var ownerName = "mujOwner";
        var owner = Owner.Create(ownerName);
        var area = new Area() { Name = "a", Buildings = null, Owner = owner, Location = "" };
        var areas = new List<Area>() {area };
        owner.Areas = areas;
        unitOfWork.OwnerRepository.Add(owner.ToEnumerable());
        await unitOfWork.SaveChangesAsync();
            
        // Act
        var fetchedOwner = await ownerService.GetByIdAsync(owner.Id, new CancellationToken());
        var area2 = new Area() { Name = "a", Buildings = null, Owner = owner, Location = "a" };
        var areas2 = new List<Area>() {area2 };

        // Assert
        Assert.NotNull( fetchedOwner);
        Assert.Equal(ownerName, fetchedOwner.Name);
        Assert.NotEqual(areas2,owner.Areas);
    }
    
    [Fact]
    public async Task OwnerService_OneOwner_AreasSameValues_Query_NotEqual_Succeeds()
    {
        // Arrange
        var ownerName = "mujOwner";
        var owner = Owner.Create(ownerName);
        var area = new Area() { Name = "a", Buildings = null, Owner = owner, Location = "" };
        var areas = new List<Area>() {area };
        owner.Areas = areas;
        unitOfWork.OwnerRepository.Add(owner.ToEnumerable());
        await unitOfWork.SaveChangesAsync();
            
        // Act
        var fetchedOwner = await ownerService.GetByIdAsync(owner.Id, new CancellationToken());

        var area3 = new Area()
        {
            Name = area.Name, UpdatedUtc = area.UpdatedUtc, CreatedUtc = area.CreatedUtc, Buildings = area.Buildings, Id = area.Id,
            Location = area.Location, Owner = area.Owner, IsDeleted = area.IsDeleted
        };
        var areas3 = new List<Area>() { area3 };
        
        // Assert
        Assert.NotNull(fetchedOwner);
        Assert.Equal(ownerName, fetchedOwner.Name);
        Assert.NotEqual(areas3,owner.Areas);
    }
    
    [Fact]
    public async Task OwnerService_OneOwner_Accounts_Query_Succeeds()
    {
        // Arrange
        var ownerName = "mujOwner";
        var owner = Owner.Create(ownerName);
        var account = new Account() { Role = AccountRole.User, Email = "martin@martin.com", Owner = owner, PasswordHash = "sfgsfsdfsdfsdf", TwoFactorAuthenticationEnabled = true};
        var accounts = new List<Account>() {account };
        owner.Accounts = accounts;
        unitOfWork.OwnerRepository.Add(owner.ToEnumerable());
        await unitOfWork.SaveChangesAsync();
            
        // Act
        var fetchedOwner = await ownerService.GetByIdAsync(owner.Id, new CancellationToken());

        // Assert
        Assert.NotNull(fetchedOwner);
        Assert.Equal(ownerName, fetchedOwner.Name);
        Assert.Equal(accounts,owner.Accounts);
    }
    
    [Fact]
    public async Task OwnerService_OneOwner_Devices_Query_Succeeds()
    {
        // Arrange
        var ownerName = "mujOwner";
        var owner = Owner.Create(ownerName);
        var device = Device.Create(owner,"xd");
        var devices = new List<Device>() {device };
        owner.Devices = devices;
        unitOfWork.OwnerRepository.Add(owner.ToEnumerable());
        await unitOfWork.SaveChangesAsync();
            
        // Act
        var fetchedOwner = await ownerService.GetByIdAsync(owner.Id, new CancellationToken());

        // Assert
        Assert.NotNull(fetchedOwner);
        Assert.Equal(ownerName, fetchedOwner.Name);
        Assert.Equal(devices,owner.Devices);
    }
    
    
    [Fact]
    public async Task OwnerService_MultipleOwners_GetByID_Query_Succeeds()
    {
        // Arrange
        var ownerName1 = "mujOwner";
        var owner1 = Owner.Create(ownerName1);
        var ownerName2 = "mujOwner2";
        var owner2 = Owner.Create(ownerName2);
        var ownerName3 = "mujOwner3";
        var owner3 = Owner.Create(ownerName3);

        unitOfWork.OwnerRepository.Add(owner1.ToEnumerable());
        unitOfWork.OwnerRepository.Add(owner2.ToEnumerable());
        unitOfWork.OwnerRepository.Add(owner3.ToEnumerable());
        await unitOfWork.SaveChangesAsync();
            
        // Act
        var id = owner2.Id;
        var fetchedOwner = await ownerService.GetByIdAsync(id, new CancellationToken());

        // Assert
        Assert.NotNull(fetchedOwner);
        Assert.Equal(fetchedOwner.Id, id);
        Assert.Equal(fetchedOwner.Name, owner2.Name);
    }
    
    [Fact]
    public async Task OwnerService_OneOwner_RegisterNewOwner_GetByID_Query_Succeeds()
    {
        // Arrange
        var ownerName = "mujOwner";
        var owner = Owner.Create(ownerName);
        
        await ownerService.RegisterNewOwnerAsync(owner, new CancellationToken());
        await unitOfWork.SaveChangesAsync();
            
        // Act
        var id = owner.Id;
        var fetchedOwner = await ownerService.GetByIdAsync(id, new CancellationToken());

        // Assert
        Assert.NotNull(fetchedOwner);
        Assert.Equal(fetchedOwner.Id, id);
        Assert.Equal(fetchedOwner.Name, owner.Name);
    }
}
