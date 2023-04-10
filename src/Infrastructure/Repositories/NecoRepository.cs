using Application.IRepositories;
using Core;

namespace Infrastructure.Repositories;

public class NecoRepository : INecoRepository
{
    private List<Stringos> _list = new List<Stringos>();
    public List<Stringos> ContractedGetMethod()
    {
        var neco = new Stringos("sdsdsd");
        var neco2 = new Stringos("asxcxcx");
        _list.Add(neco);
        _list.Add(neco2);
        return _list;
    }
}