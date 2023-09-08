using Core.Entities;
namespace IntegrationTests.RolesTests;

public class RolesTests : AccessControlIntegrationTests
{
    [Fact]
    public async Task RoleService_Add_GetById_Query_Succeeds()
    {
        // Arrange
        var role = Role.Create("role");
        var roles = new Role[] { role };

        // Act
        await roleService.AddAsync(roles, new CancellationToken());

        var fetchedRole = await roleService.GetByIdAsync(role.Id,new CancellationToken());

        // Assert
        Assert.Equal(role.Id, fetchedRole.Id);
        Assert.Equal(role.Name, role.Name);
    }
}