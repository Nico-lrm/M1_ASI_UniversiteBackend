using UniversiteDomain.Adapters;
using UniversiteDomain.Adapters.AdapterFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.ParcoursExceptions;

namespace UniversiteDomain.UseCases.ParcoursUseCases.Create;

public class CreateParcoursUseCase(IRepositoryFactory repositoryFactory)
{
    public async Task<Parcours> ExecuteAsync(string nom, int annee)
    {
        var parcours = new Parcours{NomParcours = nom, Annee = annee};
        return await ExecuteAsync(parcours);
    }
    public async Task<Parcours> ExecuteAsync(Parcours parcours)
    {
        await CheckBusinessRules(parcours);
        Parcours pc = await repositoryFactory.ParcoursRepository().CreateAsync(parcours);
        repositoryFactory.ParcoursRepository().SaveChangesAsync().Wait();
        return pc;
    }

    private async Task CheckBusinessRules(Parcours parcours)
    {
        ArgumentNullException.ThrowIfNull(parcours);
        ArgumentNullException.ThrowIfNull(parcours.NomParcours);
        ArgumentOutOfRangeException.ThrowIfLessThan(parcours.Annee, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(parcours.Annee, 2);
        ArgumentNullException.ThrowIfNull(repositoryFactory.ParcoursRepository());
        
        // On recherche un étudiant avec le même numéro étudiant
        List<Parcours> existe = await repositoryFactory.ParcoursRepository().FindByConditionAsync(e=>e.NomParcours.Equals(parcours.NomParcours));

        // Si un parcours avec le même nom existe déjà, on lève une exception personnalisée
        if (existe is {Count:>0}) throw new DuplicateNomException(parcours.NomParcours+ " - ce parcours existe déjà");

        if (parcours.NomParcours.Length < 2)
            throw new ParcoursNameTooSmallException(parcours.NomParcours + " - ce nom de parcours est trop court");
    }
}