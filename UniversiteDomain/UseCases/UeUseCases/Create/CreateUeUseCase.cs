using UniversiteDomain.Adapters.AdapterFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.ParcoursExceptions;
using UniversiteDomain.Exceptions.UeExceptions;

namespace UniversiteDomain.UseCases.UeUseCases.Create;

public class CreateUeUseCase(IRepositoryFactory repositoryFactory)
{
    public async Task<Ue> ExecuteAsync(string numeroUe, string titre)
    {
        var ue = new Ue{NumeroUe = numeroUe, Intitule = titre};
        return await ExecuteAsync(ue);
    }
    public async Task<Ue> ExecuteAsync(Ue ue)
    {
        await CheckBusinessRules(ue);
        Ue newUe = await repositoryFactory.UeRepository().CreateAsync(ue);
        repositoryFactory.UeRepository().SaveChangesAsync().Wait();
        return newUe;
    }

    private async Task CheckBusinessRules(Ue ue)
    {
        ArgumentNullException.ThrowIfNull(ue);
        ArgumentNullException.ThrowIfNull(ue.NumeroUe);
        ArgumentNullException.ThrowIfNull(ue.Intitule);
        ArgumentNullException.ThrowIfNull(repositoryFactory.UeRepository());

        List<Ue> existe = await repositoryFactory.UeRepository().FindByConditionAsync(e => e.NumeroUe.Equals(ue.NumeroUe));
        if (existe.Any()) throw new DuplicateNumeroUeException(ue.NumeroUe + " - existe déjà");

        if (ue.Intitule.Length < 3)
            throw new UeNameTooSmallException(ue.Intitule + " - ce nom d'UE est trop court");
    }
}