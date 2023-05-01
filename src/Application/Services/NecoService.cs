using Application.IRepositories;
using Azure.Identity;
using Core;
using Core.Entities;

namespace Application.Services;

public class NecoService : INecoService
{
    private readonly INecoRepository _necoPrivateRepository;
    
    public NecoService(INecoRepository necoRepository)
    {
        _necoPrivateRepository = necoRepository;
    }

    public List<Person> ContractedProcessMethod()
    {
        var aa = _necoPrivateRepository.ContractedGetMethod();
        return aa;
    }
}