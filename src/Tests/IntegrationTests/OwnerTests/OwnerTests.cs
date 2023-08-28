using AccessControl.Web.BlazorUI.Pages;
using Core.Entities;

namespace IntegrationTests.OwnerTests;

public class OwnerTests : AccessControlIntegrationTests
{
    [Fact]
    public async Task OwnerService_OneOwner_OnlyName_Query_Succeeds()
    {
        // Arrange
        var ownerName = "mujOwner";
        var owner = Owner.Create(ownerName);
        unitOfWork.OwnerCommandRepository.Add(owner);
        await unitOfWork.SaveChangesAsync();
            
        // Act
        var fetchedOwners = await ownerService.GetAllAsync(new CancellationToken());
        var fetchedOwner = fetchedOwners.First();
            
        // Assert
        Assert.Equal(1, fetchedOwners.Count());
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
        unitOfWork.OwnerCommandRepository.Add(owner);
        await unitOfWork.SaveChangesAsync();
            
        // Act
        var fetchedOwners = await ownerService.GetAllAsync(new CancellationToken());
        var fetchedOwner = fetchedOwners.First();

        // Assert
        Assert.Equal(1, fetchedOwners.Count());
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
        unitOfWork.OwnerCommandRepository.Add(owner);
        await unitOfWork.SaveChangesAsync();
            
        // Act
        var fetchedOwners = await ownerService.GetAllAsync(new CancellationToken());
        var fetchedOwner = fetchedOwners.First();
        var area2 = new Area() { Name = "a", Buildings = null, Owner = owner, Location = "a" };
        var areas2 = new List<Area>() {area2 };

        // Assert
        Assert.Equal(1, fetchedOwners.Count());
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
        unitOfWork.OwnerCommandRepository.Add(owner);
        await unitOfWork.SaveChangesAsync();
            
        // Act
        var fetchedOwners = await ownerService.GetAllAsync(new CancellationToken());
        var fetchedOwner = fetchedOwners.First();

        var area3 = new Area()
        {
            Name = area.Name, UpdatedUtc = area.UpdatedUtc, CreatedUtc = area.CreatedUtc, Buildings = area.Buildings, Id = area.Id,
            Location = area.Location, Owner = area.Owner, IsDeleted = area.IsDeleted
        };
        var areas3 = new List<Area>() { area3 };
        
        // Assert
        Assert.Equal(1, fetchedOwners.Count());
        Assert.Equal(ownerName, fetchedOwner.Name);
        Assert.NotEqual(areas3,owner.Areas);
    }
    
    [Fact]
    public async Task OwnerService_OneOwner_Accounts_Query_Succeeds()
    {
        // Arrange
        var ownerName = "mujOwner";
        var owner = Owner.Create(ownerName);
        var account = new Account() { Role = AccountRole.User, Email = null, Owner = owner, PasswordHash = "", TwoFactorAuthenticationEnabled = true};
        var accounts = new List<Account>() {account };
        owner.Accounts = accounts;
        unitOfWork.OwnerCommandRepository.Add(owner);
        await unitOfWork.SaveChangesAsync();
            
        // Act
        var fetchedOwners = await ownerService.GetAllAsync(new CancellationToken());
        var fetchedOwner = fetchedOwners.First();
            
        // Assert
        Assert.Equal(1, fetchedOwners.Count());
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
        unitOfWork.OwnerCommandRepository.Add(owner);
        await unitOfWork.SaveChangesAsync();
            
        // Act
        var fetchedOwners = await ownerService.GetAllAsync(new CancellationToken());
        var fetchedOwner = fetchedOwners.First();
            
        // Assert
        Assert.Equal(1, fetchedOwners.Count());
        Assert.Equal(ownerName, fetchedOwner.Name);
        Assert.Equal(devices,owner.Devices);
    }
}
