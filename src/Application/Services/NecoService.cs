using Application.IRepositories;
using Core;

namespace Application.Services;

public class NecoService : INecoService
{
    private readonly INecoRepository _necoPrivateRepository;
    
    public NecoService(INecoRepository necoRepository)
    {
        _necoPrivateRepository = necoRepository;
    }

    public List<Stringos> ContractedProcessMethod()
    {
        var aa = _necoPrivateRepository.ContractedGetMethod();
        return aa;
    }
}