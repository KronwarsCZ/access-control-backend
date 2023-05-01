using Core;
using Core.Entities;

namespace Application.IRepositories;

public interface INecoRepository
{
    List<Person> ContractedGetMethod();
}