using Framework.Identification;

namespace Core.Entities;

public class Key : Entity
{
    public required string Pass { get; init; }
    
    public required string Name { get; set; }

    public static Key Create(string name, string pass)
    {
        return new Key
        {
            Id = SequentialGuid.Create(),
            Name = name,
            Pass = pass,
        };
    }
}