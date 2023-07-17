using System;
using System.ComponentModel.DataAnnotations;

namespace Core.Entities;

public class Device : Entity
{
    public required Owner Owner { get; init; }

    public required List<Role> Roles { get; set; }

    public required List<Key> Keys { get; set; }
}
