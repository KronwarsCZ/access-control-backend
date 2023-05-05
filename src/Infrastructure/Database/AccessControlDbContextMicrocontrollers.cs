using Core;
using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public sealed partial class AccessControlDbContext
{
    public DbSet<Person> Persons { get; set; }
}