using Application.IRepositories;
using Core;
using Infrastructure.Database;

namespace Infrastructure.Repositories;

public class NecoRepository : INecoRepository
{
    private List<Person> _list = new List<Person>();
    public List<Person> ContractedGetMethod()
    {
        return _list;
    }
}