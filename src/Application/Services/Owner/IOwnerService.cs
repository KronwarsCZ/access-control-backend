namespace Application.Services.Owner;

public interface IOwnerService : IBasicService<Core.Entities.Owner>
{
    Core.Entities.Owner Create(string name);
}
