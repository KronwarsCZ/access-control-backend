using Core.Entities;
namespace IntegrationTests.RolesTests;

public class RolesTests : AccessControlIntegrationTests
{
    [Fact]
    public async Task RoleService_AddRole_GetAll_Query_Succeeds()
    {
        // Arrange
        var deviceName = "mujdevice";
        var owner = data.SampleOwner;
        var device = Device.Create(owner, deviceName);

        var role = Role.Create("role");
        var roles = new Role[] { role };

        // Act
        await roleService.AddAsync(roles, new CancellationToken());
        await unitOfWork.SaveChangesAsync();

        var fetchedRoles = await roleService.GetAllAsync(new CancellationToken());
        var fetchedRole = fetchedRoles.First();

        // Assert
        Assert.Equal(role.Id, fetchedRole.Id);
        Assert.Equal(role.Name, role.Name);
    }
    
    [Fact]
    public async Task RoleService_GetById_Query_Succeeds()
    {
        // Arrange
        var deviceName = "mujdevice";
        var owner = data.SampleOwner;
        var device = Device.Create(owner, deviceName);

        var role = Role.Create("role");
        var roles = new Role[] { role };

        // Act
        await roleService.AddAsync(roles, new CancellationToken());
        await unitOfWork.SaveChangesAsync();

        var fetchedRole = await roleService.GetByIdAsync(role.Id,new CancellationToken());

        // Assert
        Assert.Equal(role.Id, fetchedRole.Id);
        Assert.Equal(role.Name, role.Name);
    }
}
