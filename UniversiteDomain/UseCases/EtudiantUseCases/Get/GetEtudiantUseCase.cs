using UniversiteDomain.Adapters.AdapterFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.EtudiantUseCases.Get;

public class GetEtudiantUseCase(IRepositoryFactory repositoryFactory)
{
    public async Task<Etudiant> ExecuteAsync(long id)
    {
        Etudiant et = await repositoryFactory.EtudiantRepository().FindAsync(id);
        return et;
    }
}