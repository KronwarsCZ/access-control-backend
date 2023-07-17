using System;
using System.ComponentModel.DataAnnotations;

namespace Core.Entities;

public class Device : Entity
{
    public string Owner { get; init; }

    public List<Role> Roles { get; set; }

    public List<Key> Keys { get; set; }
}
